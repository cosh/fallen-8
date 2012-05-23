// 
// RTree.cs
//  
// Author:
//       Andriy Kupershmidt <kuper133@googlemail.com>
// 
// Copyright (c) 2011 Henning Rauch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using Fallen8.API.Index.Spatial.Implementation.SpatialContainer;
using System.Collections.ObjectModel;
using Fallen8.API.Model;
using Framework.Serialization;
using Fallen8.API.Helper;
using Fallen8.API.Error;


namespace Fallen8.API.Index.Spatial.Implementation.RTree
{
    /// <summary>
    /// Spatial index implementation as r-tree
    /// </summary>
    public sealed class RTree : AThreadSafeElement,ISpatialIndex
    {

        #region public member
        /// <summary>
        /// Metric for R-Tree space
        /// </summary>
        public IMetric Metric { get; private set; }
        /// <summary>
        /// minimal value of count of kinder in the container
        /// </summary>
        public int MinCountOfNode { get; private set; }
        /// <summary>
        /// maximal value of count of kinder in the container
        /// </summary>
        public int MaxCountOfNode { get; private set; }
        /// <summary>
        /// a space for the r-tree
        /// </summary>
        public List<IDimension> Space { get; private set; }
        #endregion

        #region private member
        private List<Boolean> _levelForOverflowStrategy;
        private int _countOfR;
        private int _countOfReInsert;
        private Dictionary<int, IRTreeDataContainer> _mapOfContainers;
        private ARTreeContainer _root;
        #endregion

        #region constructor
        public RTree()
        {
        }
        #endregion

        #region private function
        #region distance between two spatial containers
        private float Distance(ISpatialContainer container1, ISpatialContainer container2)
        {
            if (container1 is APointContainer && container2 is APointContainer)
            {
                return Metric.Distance((APointContainer)container1, (APointContainer)container2);
            }

            var points = FindNeighborPoints(container1.LowerPoint, container1.UpperPoint, container2.LowerPoint, container2.UpperPoint);
            return Metric.Distance(points.Item1, points.Item2);
        }
        #endregion

        #region Find neighbor points
        private Tuple<IMBP, IMBP> FindNeighborPoints(float[] lower1, float[] upper1, float[] lower2, float[] upper2)
        {
            var point1 = new float[this._countOfR];
            var point2 = new float[this._countOfR];


            for (int i = 0; i < this._countOfR; i++)
            {
                if (lower2[i] >= lower1[i])
                {
                    if (upper1[i] >= lower2[i])
                    {
                        point1[i] = 0;
                        point2[i] = 0;
                    }
                    else
                    {
                        point1[i] = upper1[i];
                        point2[i] = lower2[i];
                    }
                }
                else
                {
                    if (upper2[i] >= lower1[i])
                    {
                        point1[i] = 0;
                        point2[i] = 0;
                    }
                    else
                    {
                        point1[i] = lower1[i];
                        point2[i] = upper2[i];
                    }
                }

            }

            return Tuple.Create((IMBP)new RTreePoint(point1), (IMBP)new RTreePoint(point2));
        }
        #endregion

        #region methodes for spatial search

        #region universal searching
        private ReadOnlyCollection<AGraphElement> Searching(IRTreeDataContainer element, Predicate<AGraphElement> predicate, Func<ASpatialContainer, IRTreeDataContainer, bool> spatialPredicate, Func<IRTreeDataContainer, IRTreeDataContainer, bool> dataContainerPredicate)
        {
            var stack = new Stack<ARTreeContainer>();
            var currentResult = new List<AGraphElement>();
            if (spatialPredicate(_root, element))
                stack.Push(_root);
            while (stack.Count > 0)
            {
                var currentContainer = stack.Pop();

                if (!currentContainer.IsLeaf)
                {
                    foreach (ARTreeContainer value in ((RTreeNode)currentContainer).Children)
                    {
                        if (spatialPredicate(value, element))
                            stack.Push(value);
                    }
                }
                else
                {
                    foreach (IRTreeDataContainer value in ((RTreeLeaf)currentContainer).Data)
                    {
                        if (dataContainerPredicate(value, element))
                        {

                            currentResult.Add(value.GraphElement);
                        }

                    }
                }
            }

            if (predicate != null)
            {
                return currentResult.FindAll(predicate).AsReadOnly();
            }
            
            return currentResult.AsReadOnly();
        }
        #endregion
        
        #region EqualsSearch
        private ReadOnlyCollection<AGraphElement> EqualSearch(IRTreeDataContainer element)
        {
            var stack = new Stack<ARTreeContainer>();
            var currentResult = new List<AGraphElement>();
            if (_root.Inclusion(element))
                stack.Push(_root);
            while (stack.Count > 0)
            {
                var currentContainer = stack.Pop();

                if (!currentContainer.IsLeaf)
                {
                    foreach (ARTreeContainer value in ((RTreeNode)currentContainer).Children)
                    {
                        if (value.Inclusion(element))
                            stack.Push(value);
                    }
                }
                else
                {
                    var container = (RTreeLeaf) currentContainer;

                    for (var i = 0; i < container.Data.Count; i++)
                    {
                        var value = container.Data[i];
                        if (value.EqualTo(element))
                        {
                            currentResult.Add(value.GraphElement);
                        }
                    }
                }
            }
            
            return currentResult.AsReadOnly();
        }
        #endregion

        #region OverlapSearching
        #region Search with Distance
        #region create search container
        /// <summary>
        /// create new search container
        /// </summary>
        /// <param name="element">
        /// container of data with element of graph
        /// </param>
        /// <param name="distance">
        /// distance
        /// </param>
        /// <returns>
        /// container for searching
        /// </returns>
        private SpatialDataContainer CreateSearchContainer(IRTreeDataContainer element, float distance)
        {
            return this.CreateSearchContainer(new MBR(element.LowerPoint, element.UpperPoint), distance);
        }

        /// <summary>
        /// create new search container
        /// </summary>
        /// <param name="mbr">
        /// minimal bounded rechtangle
        /// </param>
        /// <param name="distance">
        /// distance
        /// </param>
        /// <returns>
        /// container for searching
        /// </returns>
        private SpatialDataContainer CreateSearchContainer(IMBR mbr, float distance)
        {

            var lower = new float[this._countOfR];
            var upper = new float[this._countOfR];

            var transformationOfDistance = Metric.TransformationOfDistance(distance, mbr);

            for (int i = 0; i < this._countOfR; i++)
            {
                lower[i] = mbr.LowerPoint[i] - transformationOfDistance[i];
                upper[i] = mbr.UpperPoint[i] + transformationOfDistance[i];
            }

            return new SpatialDataContainer(new MBR(lower, upper));
        }
        #endregion
        #region search
        /// <summary>
        /// searching of distance
        /// </summary>
        /// <param name="searchContainer">
        /// container for searching
        /// </param>
        /// <param name="predicate">
        /// predicate
        /// </param>
        /// <returns>
        /// list of result
        /// </returns>
        private ReadOnlyCollection<AGraphElement> OverlapSearch(IRTreeDataContainer searchContainer, Predicate<AGraphElement> predicate = null)
        {
            var stack = new Stack<ARTreeContainer>();
            var currentResult = new List<AGraphElement>();
            if (_root.Intersection(searchContainer))
                stack.Push(_root);
            while (stack.Count > 0)
            {
                var currentContainer = stack.Pop();

                if (!currentContainer.IsLeaf)
                {
                    foreach (ARTreeContainer value in ((RTreeNode)currentContainer).Children)
                    {
                        if (value.Intersection(searchContainer))
                            stack.Push(value);
                    }
                }
                else
                {
                    var container = (RTreeLeaf) currentContainer;

                    for (var i = 0; i < container.Data.Count; i++)
                    {
                        if (container.Data[i].Intersection(searchContainer))
                        {
                            currentResult.Add(container.Data[i].GraphElement);
                        }
                    }
                }
            }

            if (predicate != null)
            {
                return currentResult.FindAll(predicate).AsReadOnly();
            }
            
            return currentResult.AsReadOnly();
        }
        #endregion
        #endregion
        #endregion
        #endregion

        #region methodes for r*-tree

        #region Local reorganisation by removing

        private void LocalReorganisationByRemoving(ARTreeContainer rTree, int level)
        {
            if (rTree.Parent != null)
            {
                ((RTreeNode)rTree.Parent).Children.Remove(rTree);
                RecalculationByRemoving(rTree);
                var leafLevel = this._levelForOverflowStrategy.Count - 1;
                if (((RTreeNode)rTree.Parent).Children.Count < this.MinCountOfNode)
                    LocalReorganisationByRemoving(rTree.Parent, level - 1);
                
                if (rTree is RTreeLeaf)
                {
                    foreach (IRTreeDataContainer value in ((RTreeLeaf)rTree).Data)
                    {
                        var newLeafLeavel = this._levelForOverflowStrategy.Count - 1;
                        Insert(value, level - (leafLevel - newLeafLeavel));
                    }

                }
                else
                {
                    foreach (ARTreeContainer value in ((RTreeNode)rTree).Children)
                    {
                        var newLeafLeavel = this._levelForOverflowStrategy.Count - 1;
                        Insert(value, level - (leafLevel - newLeafLeavel));
                    }

                }


                rTree.Dispose();
            }
            else
            {
                if (rTree is RTreeLeaf)
                {
                    if (((RTreeLeaf)rTree).Data.Count == 0)
                        for (int i = 0; i < this._countOfR; i++)
                        {
                            rTree.UpperPoint[i] = float.NegativeInfinity;
                            rTree.LowerPoint[i] = float.PositiveInfinity;
                        }
                }
                else
                {
                    if (((RTreeNode)rTree).Children.Count == 1)
                    {
                        _root = ((RTreeNode)rTree).Children[0];
                        _root.Parent = null;
                        rTree.Dispose();
                        this._levelForOverflowStrategy.RemoveAt(this._levelForOverflowStrategy.Count - 1);
                    }
                }

            }

        }

        #endregion

        #region Insert of the data
        private void InsertData(IRTreeDataContainer container)
        {

            //  for (int i = 1; i < this.levelForOverflowStrategy.Count; i++)
            //      levelForOverflowStrategy[i] = true;
            this.Insert(container, this._levelForOverflowStrategy.Count - 1);
            //      for (int i = 1; i < this.levelForOverflowStrategy.Count; i++)
            //          levelForOverflowStrategy[i] = false;

        }
        #endregion

        #region Insert
        private void Insert(IRTreeContainer container, int level)
        {
            if (level == this._levelForOverflowStrategy.Count - 1)
            {
                var chooseSubTree = (RTreeLeaf)this.ChooseSubTree(container, level);
                container.Parent = chooseSubTree;
                chooseSubTree.Data.Add((IRTreeDataContainer)container);
                Recalculation(container);
                if (chooseSubTree.Data.Count > this.MaxCountOfNode)
                {
                    this.OverflowTreatment(level, chooseSubTree);
                }
            }
            else
            {
                var chooseSubTree = (RTreeNode)this.ChooseSubTree(container, level);
                container.Parent = chooseSubTree;
                chooseSubTree.Children.Add((ARTreeContainer)container);
                Recalculation(container);

                if (chooseSubTree.Children.Count > this.MaxCountOfNode)
                {
                    this.OverflowTreatment(level, chooseSubTree);
                }
            }

        }
        #endregion

        #region Recalculations of containers
        //recalculation
        private void Recalculation(IRTreeContainer newContainer)
        {
            var recalculatePath = newContainer;
            while (recalculatePath.Parent != null)
            {

                if (recalculatePath.Parent.Inclusion(newContainer))
                    break;

                var recalculateMBR = FindMBR(recalculatePath.Parent, newContainer);
                recalculatePath.Parent.Area = GetArea(recalculateMBR);
                recalculatePath.Parent.LowerPoint = recalculateMBR.LowerPoint;
                recalculatePath.Parent.UpperPoint = recalculateMBR.UpperPoint;
                recalculatePath = recalculatePath.Parent;
            }
        }
        private void RecalculationByRemoving(IRTreeContainer removeContainer)
        {
            var recalculatePath = removeContainer;

            while (recalculatePath.Parent != null)
            {
                if (recalculatePath.Parent.IsLeaf)
                {
                    var newMBR = FindMBR(((RTreeLeaf)recalculatePath.Parent).Data);
                    recalculatePath.Parent.LowerPoint = newMBR.LowerPoint;
                    recalculatePath.Parent.UpperPoint = newMBR.UpperPoint;
                    recalculatePath.Parent.Area = GetArea(newMBR);
                }
                else
                {
                    var newMBR = FindMBR(((RTreeNode)recalculatePath.Parent).Children);
                    recalculatePath.Parent.LowerPoint = newMBR.LowerPoint;
                    recalculatePath.Parent.UpperPoint = newMBR.UpperPoint;
                    recalculatePath.Parent.Area = GetArea(newMBR);

                }

                recalculatePath = recalculatePath.Parent;

                if (recalculatePath.Parent == null || recalculatePath.Parent.Inclusion(removeContainer))
                    break;


            }
        }
        private void RecalculationByRemoving(IEnumerable<IRTreeContainer> removeContainers)
        {
            var removeMBR = FindMBR(removeContainers);
            var recalculatePath = removeContainers.First();

            while (recalculatePath.Parent != null)
            {
                if (recalculatePath.Parent.IsLeaf)
                {
                    var newMBR = FindMBR(((RTreeLeaf)recalculatePath.Parent).Data);
                    recalculatePath.Parent.LowerPoint = newMBR.LowerPoint;
                    recalculatePath.Parent.UpperPoint = newMBR.UpperPoint;
                    recalculatePath.Parent.Area = GetArea(newMBR);
                }
                else
                {
                    var newMBR = FindMBR(((RTreeNode)recalculatePath.Parent).Children);
                    recalculatePath.Parent.LowerPoint = newMBR.LowerPoint;
                    recalculatePath.Parent.UpperPoint = newMBR.UpperPoint;
                    recalculatePath.Parent.Area = GetArea(newMBR);

                }
                recalculatePath = recalculatePath.Parent;

                if (recalculatePath.Parent == null || recalculatePath.Parent.Inclusion(removeMBR))
                    break;


            }
        }
        #endregion
        
        #region OverflowTreatment
        private void OverflowTreatment(int level, ARTreeContainer container)
        {
            if (level != 0 && this._levelForOverflowStrategy[level])
            {
                _levelForOverflowStrategy[level] = false;
                this.ReInsert(container, level);

            }
            else
            {
                this.Split(container, level);
            }
        }
        #endregion

        #region ReInsert
        private void ReInsert(ARTreeContainer container, int level)
        {
            var center = FindCenterOfContainer(container);
            var distance = new List<Tuple<float, IRTreeContainer>>(this.MaxCountOfNode + 1);
            List<ISpatialContainer> dataContainer;
            if (container.IsLeaf)
            {
                dataContainer = new List<ISpatialContainer>(((RTreeLeaf) container).Data);
            }
            else
            {
                dataContainer = new List<ISpatialContainer>(((RTreeNode) container).Children);
            }

            foreach (IRTreeContainer value in dataContainer)
            {
                if (value is APointContainer)
                    distance.Add(Tuple.Create(
                        Metric.Distance(new RTreePoint(((APointContainer) value).Coordinates),
                                        new RTreePoint(center)), value));
                else
                    distance.Add(Tuple.Create(
                        Metric.Distance(new RTreePoint(FindCenterOfContainer((ASpatialContainer) value)),
                                        new RTreePoint(center)), value));
            }
            distance.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            distance.Reverse();

            var reinsertData = new List<Tuple<float, IRTreeContainer>>(this._countOfReInsert);
            reinsertData.AddRange(distance.GetRange(0, this._countOfReInsert));
            if (container.IsLeaf)
                ((RTreeLeaf) container).Data.RemoveAll(_ => reinsertData.Exists(y => y.Item2 == _));
            else
                ((RTreeNode) container).Children.RemoveAll(_ => reinsertData.Exists(y => y.Item2 == _));

            this.RecalculationByRemoving(reinsertData.Select(_ => _.Item2));
            var leafLevel = this._levelForOverflowStrategy.Count - 1;
            for (int i = 0; i < this._countOfReInsert; i++)
            {
                reinsertData[i].Item2.Parent = null;
                var newLeafLeavel = this._levelForOverflowStrategy.Count - 1;
                this.Insert(reinsertData[i].Item2, level - (leafLevel - newLeafLeavel));
            }


        }

        #endregion

        #region Find center of container
        private float[] FindCenterOfContainer(ASpatialContainer container)
        {
            var center = new float[this._countOfR];
            for (int i = 0; i < this._countOfR; i++)
            {
                center[i] = ((container.LowerPoint[i] + container.UpperPoint[i]) / 2);
            }
            return center;
        }
        #endregion

        #region Split
        private void Split(ARTreeContainer container, int level)
        {

            List<IRTreeContainer> result;
            var axis = ChooseSplitAxis(out result, container);
            ARTreeContainer container1;
            ARTreeContainer container2;

            if (!container.IsLeaf)
            {
                container1 = new RTreeNode();
                container2 = new RTreeNode();
            }
            else
            {
                container1 = new RTreeLeaf();
                container2 = new RTreeLeaf();

            }
            ChooseSplitIndex(result, axis, container1, container2);

            if (container.Parent == null)
            {
                var lower = _root.LowerPoint;
                var upper = _root.UpperPoint;
                var area = _root.Area;
                _root.Dispose();
                _root = null;
                _root = new RTreeNode(lower, upper) {Area = area};
                container1.Parent = _root;
                container2.Parent = _root;
                ((RTreeNode)_root).Children.Add(container1);
                ((RTreeNode)_root).Children.Add(container2);
                _levelForOverflowStrategy.Add(true);
            }
            else
            {
                container1.Parent = container.Parent;
                container2.Parent = container.Parent;

                ((RTreeNode)container.Parent).Children.Remove(container);
                ((RTreeNode)container.Parent).Children.Add(container1);
                ((RTreeNode)container.Parent).Children.Add(container2);

                container.Dispose();
                if (!container1.Parent.IsLeaf)
                {
                    if (((RTreeNode)container1.Parent).Children.Count > this.MaxCountOfNode)
                        this.OverflowTreatment(level - 1, container1.Parent);
                }
                else
                {
                    if (((RTreeLeaf)container2.Parent).Data.Count > this.MaxCountOfNode)
                        this.OverflowTreatment(level - 1, container2.Parent);
                }
            }

        }
        #endregion

        #region ChooseSplitAxis

        /// <summary>
        /// find best split axis
        /// </summary>
        /// <param name="result">The Result </param>
        /// <param name="container">
        /// container with data
        /// </param>
        /// <returns>
        /// number of split axis
        /// </returns>
        private int ChooseSplitAxis(out List<IRTreeContainer> result, ARTreeContainer container)
        {
            List<IRTreeContainer> currentContainers;
            if (container.IsLeaf == false)
            {
                currentContainers = new List<IRTreeContainer>(((RTreeNode)container).Children);
            }
            else
            {
                currentContainers = new List<IRTreeContainer>(((RTreeLeaf)container).Data);
            }

            result = currentContainers;

            var marginValue = new float[this._countOfR];

            for (int i = 0; i < this._countOfR; i++)
            {

                currentContainers.Sort((x, y) => x.LowerPoint[i].CompareTo(y.LowerPoint[i]));
                currentContainers.Sort((x, y) => x.UpperPoint[i].CompareTo(y.UpperPoint[i]));

                var firstSeq = new List<IRTreeContainer>();
                var secondSeq = new List<IRTreeContainer>();

                firstSeq.AddRange(currentContainers.GetRange(0, this.MinCountOfNode));
                secondSeq.AddRange(currentContainers.GetRange(this.MinCountOfNode, currentContainers.Count - this.MinCountOfNode));

                var position = 0;
                marginValue[i] = this.FindMarginValue(firstSeq, i) +
                                       this.FindMarginValue(secondSeq, i);
                while (position <= currentContainers.Count - 2 * this.MinCountOfNode)
                {
                    firstSeq.Add(secondSeq.First());
                    secondSeq.RemoveAt(0);
                    marginValue[i] += this.FindMarginValue(firstSeq, i) +
                                       this.FindMarginValue(secondSeq, i);
                    position++;
                }
            }

            var splitAxis = 0;
            var sum = marginValue[0];

            for (int i = 1; i < this._countOfR; i++)
            {
                if (sum < marginValue[i])
                {
                    sum = marginValue[i];
                    splitAxis = i;

                }
            }
            return splitAxis;
        }
        #endregion

        #region find margin value
        private float FindMarginValue(List<Tuple<float, float>> containers)
        {
            return containers.Max(_ => _.Item2) - containers.Min(_ => _.Item1);
        }

        private float FindMarginValue(List<IRTreeContainer> containers, int dimension)
        {
            var min = float.PositiveInfinity;
            var max = float.NegativeInfinity;

            foreach (IRTreeContainer value in containers)
            {

                if (value.LowerPoint[dimension] < min)
                    min = value.LowerPoint[dimension];
                if (value.UpperPoint[dimension] > max)
                    max = value.UpperPoint[dimension];
            }
            return max - min;
        }
        #endregion

        #region ChooseSplitIndex
        private void ChooseSplitIndex(List<IRTreeContainer> containers, int axis, ARTreeContainer parent1, ARTreeContainer parent2)
        {

            containers.Sort((x, y) => x.LowerPoint[axis].CompareTo(y.LowerPoint[axis]));
            containers.Sort((x, y) => x.UpperPoint[axis].CompareTo(y.UpperPoint[axis]));

            var firstSeqTest = new List<IRTreeContainer>();
            var secondSeqTest = new List<IRTreeContainer>();


            firstSeqTest.AddRange(containers.GetRange(0, this.MinCountOfNode - 1));
            secondSeqTest.AddRange(containers.GetRange(this.MinCountOfNode - 1, containers.Count - this.MinCountOfNode + 1));

            var position = 0;

            float minOverlapValue = float.PositiveInfinity;
            IMBR mbrParent1 = null;
            IMBR mbrParent2 = null;
            float area1 = float.PositiveInfinity;
            float area2 = float.PositiveInfinity;
            var sequenzPosition = 0;

            while (position <= containers.Count - 2 * this.MinCountOfNode)
            {
                firstSeqTest.Add(secondSeqTest.First());
                secondSeqTest.RemoveAt(0);
                var mbr1 = this.FindMBR(firstSeqTest);
                var mbr2 = this.FindMBR(secondSeqTest);
                var currentOverlap = this.FindOverlapValue(mbr1, mbr2);
                if (minOverlapValue > currentOverlap)
                {
                    mbrParent1 = mbr1;
                    mbrParent2 = mbr2;
                    sequenzPosition = position;
                    minOverlapValue = currentOverlap;

                }
                else
                {
                    if (minOverlapValue == currentOverlap)
                    {
                        area1 = GetArea(mbrParent1);
                        area2 = GetArea(mbrParent2);
                        var currentArea1 = GetArea(mbr1);
                        var currentArea2 = GetArea(mbr2);

                        if (area1 + area2 > currentArea1 + currentArea2)
                        {
                            mbrParent1 = mbr1;
                            mbrParent2 = mbr2;
                            sequenzPosition = position;
                            minOverlapValue = currentOverlap;
                            area1 = currentArea1;
                            area2 = currentArea2;
                        }
                    }

                }
                position++;

            }

            if (float.IsPositiveInfinity(area1))
            {
                area1 = GetArea(mbrParent1);
                area2 = GetArea(mbrParent2);
            }

            parent1.Area = area1;
            parent1.LowerPoint = mbrParent1.LowerPoint;
            parent1.UpperPoint = mbrParent1.UpperPoint;

            parent2.Area = area2;
            parent2.LowerPoint = mbrParent2.LowerPoint;
            parent2.UpperPoint = mbrParent2.UpperPoint;

            if (!parent1.IsLeaf)
            {
                for (int counter = 0; counter < containers.Count; counter++)
                {
                    if (counter <= this.MinCountOfNode - 1 + sequenzPosition)
                    {
                        containers[counter].Parent = parent1;
                        ((RTreeNode)parent1).Children.Add((ARTreeContainer)containers[counter]);
                    }
                    else
                    {
                        containers[counter].Parent = parent2;
                        ((RTreeNode)parent2).Children.Add((ARTreeContainer)containers[counter]);
                    }
                }
            }
            else
            {
                for (int counter = 0; counter < containers.Count; counter++)
                {
                    if (counter <= this.MinCountOfNode - 1 + sequenzPosition)
                    {
                        containers[counter].Parent = parent1;
                        ((RTreeLeaf)parent1).Data.Add((IRTreeDataContainer)containers[counter]);

                    }
                    else
                    {
                        containers[counter].Parent = parent2;
                        ((RTreeLeaf)parent2).Data.Add((IRTreeDataContainer)containers[counter]);

                    }
                }
            }
        }
        #endregion

        #region find MBR
        private IMBR FindMBR(IEnumerable<ISpatialContainer> containers)
        {
            var lower = new float[this._countOfR];
            var upper = new float[this._countOfR];

            for (int dimension = 0; dimension < this._countOfR; dimension++)
            {
                var currentMin = float.PositiveInfinity;
                var currentMax = float.NegativeInfinity;
                foreach (ISpatialContainer value in containers)
                {
                    if (value is ASpatialContainer)
                    {
                        var temporalMin = ((ASpatialContainer)value).LowerPoint[dimension];
                        var temporalMax = ((ASpatialContainer)value).UpperPoint[dimension];
                        if (currentMin > temporalMin)
                            currentMin = temporalMin;
                        if (currentMax < temporalMax)
                            currentMax = temporalMax;

                    }
                    else
                    {
                        var temporal = ((APointContainer)value).Coordinates[dimension];
                        if (currentMax < temporal)
                            currentMax = temporal;
                        if (temporal < currentMin)
                            currentMin = temporal;
                    }
                }
                lower[dimension] = currentMin;
                upper[dimension] = currentMax;

            }

            return new MBR(lower, upper);
        }
        private IMBR FindMBR(ISpatialContainer container1, ISpatialContainer container2)
        {
            var lower = new float[this._countOfR];
            var upper = new float[this._countOfR];

            for (int dimension = 0; dimension < this._countOfR; dimension++)
            {

                lower[dimension] = Math.Min(container1.LowerPoint[dimension], container2.LowerPoint[dimension]);
                upper[dimension] = Math.Max(container1.UpperPoint[dimension], container2.UpperPoint[dimension]);

            }
            return new MBR(lower, upper);

        }
        #endregion

        #region find overlap value
        private float FindOverlapValue(IMBR mbr1, IMBR mbr2)
        {
            float[] value = new float[this._countOfR];
            var lowerPoint1 = mbr1.LowerPoint;
            var upperPoint1 = mbr1.UpperPoint;
            var lowerPoint2 = mbr2.LowerPoint;
            var upperPoint2 = mbr2.UpperPoint;
            var overlapValue = 1.0f;

            for (int i = 0; i < this._countOfR; i++)
            {

                var lower1 = lowerPoint1[i];
                var upper1 = upperPoint1[i];
                var lower2 = lowerPoint2[i];
                var upper2 = upperPoint2[i];
                if (lower2 >= lower1)
                {
                    if (upper1 > lower2)
                    {
                        if (upper1 > upper2)
                            value[i] = upper2 - lower1;
                        else
                            value[i] = upper1 - lower2;
                    }
                    else
                        return 0;
                }
                else
                {
                    if (upper2 > lower1)
                    {
                        if (upper2 > upper1)
                            value[i] = upper1 - lower1;
                        else
                            value[i] = upper2 - lower1;
                    }
                    else
                        return 0;
                }

                overlapValue *= value[i];
            }

            return overlapValue;
        }
        #endregion

        #region Choose subtree
        private ARTreeContainer ChooseSubTree(IRTreeContainer container, int level)
        {

            if (_root.IsLeaf) return _root;
            var currentNode = (RTreeNode)_root;
            var counterOfLevel = 0;
            while (counterOfLevel < level)
            {
                if (counterOfLevel + 1 == level)
                {
                    return GetLeavesLevelByOverlap(currentNode.Children, container);
                }
                
                currentNode = (RTreeNode)GetNodeByAreaValue(currentNode.Children, container);
                counterOfLevel++;
            }
            return currentNode;
        }

        private ARTreeContainer GetLeavesLevelByOverlap(List<ARTreeContainer> list, IRTreeContainer container)
        {
            ARTreeContainer output = null;
            IMBR currentOutputMBR = null;
            float minEnlagargmentOfOverlap = float.PositiveInfinity;

            foreach (ARTreeContainer value in list)
            {
                var oldOverlap = 0.0f;
                var newOverlap = 0.0f;
                var newMBR = FindMBR(value, container);
                foreach (ARTreeContainer value2 in list)
                {
                    if (value != value2)
                    {
                        oldOverlap += FindOverlapValue(value, value2);
                        newOverlap += FindOverlapValue(newMBR, value2);
                    }
                }

                var enlargagment = newOverlap - oldOverlap;
                if (enlargagment < minEnlagargmentOfOverlap)
                {
                    output = value;
                    minEnlagargmentOfOverlap = enlargagment;
                    currentOutputMBR = newMBR;
                }
                else
                {

                    if (enlargagment == minEnlagargmentOfOverlap)
                    {
                        var oldCurrentArea = value.Area;
                        var oldMinArea = output.Area;
                        var enlargementCurrentArea = GetArea(newMBR) - oldCurrentArea;
                        var enlargementMinArea = GetArea(currentOutputMBR) - oldMinArea;

                        if ((enlargementMinArea > enlargementCurrentArea) || (enlargementMinArea == enlargementCurrentArea && oldMinArea > oldCurrentArea))
                        {
                            output = value;
                            currentOutputMBR = newMBR;
                        }
                    }
                }

            }
            return output;

        }

        private ARTreeContainer GetNodeByAreaValue(List<ARTreeContainer> list, IRTreeContainer container)
        {
            float minAreaValue = float.PositiveInfinity;
            float minOldAreaValue = float.PositiveInfinity;
            ARTreeContainer areaRechtangle = null;
            foreach (ARTreeContainer value in list)
            {
                var newMBR = FindMBR(value, container);
                var oldArea = value.Area;
                var enlargementArea = GetArea(newMBR) - oldArea;
                if (minAreaValue > enlargementArea)
                {
                    areaRechtangle = value;
                    minAreaValue = enlargementArea;
                    minOldAreaValue = oldArea;
                }
                else
                {
                    if (minAreaValue == enlargementArea && oldArea < minOldAreaValue)
                    {
                        areaRechtangle = value;
                        minOldAreaValue = oldArea;
                    }
                }
            }

            return areaRechtangle;
        }
        #region GetArea for minimal bounded rechtangle
        private float GetArea(IMBR mbr)
        {
            var currentArea = 1.0f;
            for (int i = 0; i < mbr.LowerPoint.Length; i++)
            {
                currentArea *= mbr.UpperPoint[i] - mbr.LowerPoint[i];

            }

            return currentArea;
        }
        #endregion
        #endregion

        #region Tests
        #region Test of geometry
        private bool TestOfGeometry(IGeometry key)
        {
            var iPoint = key as IPoint;
            return iPoint != null ? TestOfPoint(iPoint) : TestOfMBR(key.GeometryToMBR());
        }

        #endregion
        #region Test of point
        private bool TestOfPoint(IPoint iPoint)
        {
            return iPoint.PointToSpaceR().Length == this._countOfR;
        }

        #endregion
        #region Test of Dimension
        private Boolean DimensionTest(List<IDimension> dimensions)
        {
            if (dimensions == null)
            {
                return false;
            }

            if (dimensions.Count != this.Space.Count || dimensions.Count == 0)
                return false;

            for (var i = 0; i < dimensions.Count; i++)
            {
                if (dimensions[i].ObjectType !=
                    this.Space[i].ObjectType ||
                    dimensions[i].CountOfR !=
                    this.Space[i].CountOfR)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
        #region Test of MBR
        private bool TestOfMBR(IMBR mbr)
        {
            if (mbr.LowerPoint.Length != mbr.UpperPoint.Length && mbr.LowerPoint.Length != this._countOfR)
            {
                return false;
            }
            for (int i = 0; i < mbr.LowerPoint.Length; i++)
                if (mbr.LowerPoint[i] > mbr.UpperPoint[i])
                    return false;
            return true;

        }
        #endregion
        #endregion
        #endregion

        #endregion

        #region IIndexImplementation

        public int CountOfKeys()
        {
            return -1;
        }

        public int CountOfValues()
        {
            if (ReadResource())
            {
                var count = this._mapOfContainers.Count;

                FinishReadResource();

                return count;
            }
            throw new CollisionException();
        }

        public void RemoveValue(AGraphElement graphElement)
        {
            if (WriteResource())
            {
                IRTreeDataContainer dataContainer;
                if (this._mapOfContainers.TryGetValue(graphElement.Id, out dataContainer))
                {
                    ((RTreeLeaf)dataContainer.Parent).Data.Remove(dataContainer);
                    RecalculationByRemoving(dataContainer);
                    this._mapOfContainers.Remove(graphElement.Id);
                    if (((RTreeLeaf)dataContainer.Parent).Data.Count < this.MinCountOfNode)
                    {

                        LocalReorganisationByRemoving(dataContainer.Parent, this._levelForOverflowStrategy.Count - 1);
                    }
                }

                FinishWriteResource();

                return;
            }
            throw new CollisionException();
        }

        public void Wipe()
        {
            _mapOfContainers.Clear();
            float[] lower = new float[_countOfR];
            float[] upper = new float[_countOfR];
            for (int i = 0; i < this._countOfR; i++)
            {
                lower[i] = float.PositiveInfinity;
                upper[i] = float.NegativeInfinity;
            }
            _root = new RTreeLeaf(lower, upper);
            _levelForOverflowStrategy.Clear();
            _levelForOverflowStrategy.Add(true);
        }

        public IEnumerable<Object> GetKeys()
        {
            return Enumerable.Empty<Object>();
        }

        public IEnumerable<KeyValuePair<T, ReadOnlyCollection<AGraphElement>>> GetKeyValues<T>()
        {
            return Enumerable.Empty<KeyValuePair<T, ReadOnlyCollection<AGraphElement>>>();
        }

        #endregion

        #region ISpatialIndex Implementation

        public bool TryRemoveKey(Object keyObject)
        {
            IGeometry key;
            if (!IndexHelper.CheckObject<IGeometry>(out key, keyObject))
            {
                return false;
            }

            if (ReadResource())
            {
                if (TestOfGeometry(key) && DimensionTest(key.Dimensions))
                {
                    IRTreeDataContainer searchContainer;
                    if (key is IPoint)
                        searchContainer = new PointDataContainer(((IPoint)key).PointToSpaceR());
                    else
                        searchContainer = new SpatialDataContainer(key.GeometryToMBR());

                    var removeObjects = Searching(searchContainer, null, (x, y) => x.Inclusion(y), (x, y) => x.Inclusion(y));
                    FinishReadResource();
                    foreach (AGraphElement value in removeObjects)
                    {
                        RemoveValue(value);
                    }

                    return removeObjects.Count > 0;
                }
                return false;
            }
            throw new CollisionException();
        }

        #region TryGetValue
        public bool TryGetValue(out ReadOnlyCollection<AGraphElement> result, Object geometryObject)
        {
            IGeometry geometry;
            if (!IndexHelper.CheckObject<IGeometry>(out geometry, geometryObject))
            {
                result = null;
                return false;
            }

            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer element;
                if (this.DimensionTest(geometry.Dimensions) && this.TestOfGeometry(geometry))
                {
                    if (geometry is IPoint)
                        element = new PointDataContainer(((IPoint)geometry).PointToSpaceR());
                    else
                        element = new SpatialDataContainer(geometry.GeometryToMBR());

                    result = EqualSearch(element);
                }
                FinishReadResource();

                return result.Count > 0;
            }
            throw new CollisionException();
        }

        #endregion

        /// <summary>
        /// find all object which distance less or equal d from this geometry have
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="distance">
        /// value of distance for serching
        /// </param>
        /// <param name="geometry">
        /// geometry
        /// </param>
        /// <param name="predicate"> The predicate </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        public bool SearchDistance(out ReadOnlyCollection<AGraphElement> result, float distance, IGeometry geometry, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                SpatialDataContainer searchContainer;
                if (this.DimensionTest(geometry.Dimensions) && this.TestOfGeometry(geometry))
                {
                    searchContainer = CreateSearchContainer(geometry.GeometryToMBR(), distance);
                    result = OverlapSearch(searchContainer, predicate);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }
        
        /// <summary>
        /// find all object which distance less or equal d from this element of graph have
        /// </summary>
        /// <param name="result">
        /// result
        /// </param>
        /// <param name="distance">
        /// value of distance for serching
        /// </param>
        /// <param name="graphElement">
        /// element of graph
        /// </param>
        /// <param name="predicate">
        /// not geomtric condition, this parameter is optional
        /// </param>
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        public bool SearchDistance(out ReadOnlyCollection<AGraphElement> result, float distance, AGraphElement graphElement, Predicate<AGraphElement> predicate = null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly(); IRTreeDataContainer element;
                SpatialDataContainer searchContainer;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out element))
                {
                    searchContainer = CreateSearchContainer(element, distance);
                    result = OverlapSearch(searchContainer, predicate);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }


        public void AddOrUpdate(Object keyObject, AGraphElement graphElement)
        {
            IGeometry key;
            if (!IndexHelper.CheckObject<IGeometry>(out key, keyObject))
            {
                return;
            }

            if (WriteResource())
            {
                if (DimensionTest(key.Dimensions) && TestOfGeometry(key))
                {
                    if (!this._mapOfContainers.ContainsKey(graphElement.Id))
                    {
                        IRTreeDataContainer currentContainer;

                        if (key is IPoint)
                        {
                            var coordinations = ((IPoint)key).PointToSpaceR();
                            currentContainer = new PointDataContainer(coordinations) {GraphElement = graphElement};

                        }
                        else
                        {
                            var mbr = key.GeometryToMBR();
                            currentContainer = new SpatialDataContainer(mbr.LowerPoint, mbr.UpperPoint)
                                                   {GraphElement = graphElement};
                        }

                        this.InsertData(currentContainer);
                        _mapOfContainers.Add(graphElement.Id, currentContainer);

                    }
                    else
                    {
                        RemoveValue(graphElement);
                        this.AddOrUpdate(key, graphElement);
                    }
                    
                }
                FinishWriteResource();
                return;
            }
            throw new CollisionException();
        }



        public float Distance(IGeometry geometry1, IGeometry geometry2)
        {
            if (ReadResource())
            {
                if (geometry1 is IPoint && geometry2 is IPoint)
                {
                    FinishReadResource();
                    return Metric.Distance(new RTreePoint(((IPoint)geometry1).PointToSpaceR()),
                        new RTreePoint(((IPoint)geometry2).PointToSpaceR()));
                }
                
                var mbr1 = geometry1.GeometryToMBR();
                var mbr2 = geometry2.GeometryToMBR();
                var points = FindNeighborPoints(mbr1.LowerPoint, mbr1.UpperPoint, mbr2.LowerPoint, mbr2.UpperPoint);
                    
                FinishReadResource();
                return Metric.Distance(points.Item1, points.Item2);
            }
            throw new CollisionException();
        }

        public float Distance(AGraphElement graphElement1, AGraphElement graphElement2)
        {
            if (ReadResource())
            {
                IRTreeDataContainer container1;
                IRTreeDataContainer container2;
                if (_mapOfContainers.TryGetValue(graphElement1.Id, out container1) &&
                    _mapOfContainers.TryGetValue(graphElement2.Id, out container2))
                {
                    FinishReadResource();
                    return Distance(container1, container2);
                }
                
                FinishReadResource();
                return float.NaN;
            }
            throw new CollisionException();

        }

        public bool SearchRegion(out ReadOnlyCollection<AGraphElement> result, IMBR minimalBoundedRechtangle, Predicate<AGraphElement> predicate = null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfMBR(minimalBoundedRechtangle))
                {
                    var searchRegion = new SpatialDataContainer(minimalBoundedRechtangle);
                    result = this.OverlapSearch(searchRegion, predicate);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool Overlap(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (this.TestOfGeometry(geometry) && this.DimensionTest(geometry.Dimensions))
                {
                    IRTreeDataContainer searchContainer;
                    if (geometry is IPoint)
                        searchContainer = new PointDataContainer(((IPoint)geometry).PointToSpaceR());
                    else
                        searchContainer = new SpatialDataContainer(geometry.GeometryToMBR());

                    result = this.OverlapSearch(searchContainer, predicate);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool Overlap(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (this._mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = this.OverlapSearch(output, predicate);
                }

                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool Enclosure(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (this._mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = this.Searching(output, predicate, (x, y) => x.Inclusion(y), (x, y) => x.Inclusion(y));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool Enclosure(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfGeometry(geometry) && DimensionTest(geometry.Dimensions))
                {
                    IRTreeDataContainer searchContainer;
                    if (geometry is IPoint)
                        searchContainer = new PointDataContainer(((IPoint)geometry).PointToSpaceR());
                    else
                        searchContainer = new SpatialDataContainer(geometry.GeometryToMBR());


                    result = this.Searching(searchContainer, predicate, (x, y) => x.Inclusion(y), (x, y) => x.Inclusion(y));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool Containment(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfGeometry(geometry) && DimensionTest(geometry.Dimensions))
                {
                    IRTreeDataContainer searchContainer;
                    if (geometry is IPoint)
                        searchContainer = new PointDataContainer(((IPoint)geometry).PointToSpaceR());
                    else
                        searchContainer = new SpatialDataContainer(geometry.GeometryToMBR());


                    result = this.Searching(searchContainer, predicate, (x, y) => x.Intersection(y), (x, y) => y.Inclusion(x));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool Containment(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (this._mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = this.Searching(output, predicate, (x, y) => x.Intersection(y), (x, y) => y.Inclusion(x));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (this._mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = this.Searching(output, predicate, (x, y) => x.Intersection(y), (x, y) => x.Adjacency(x));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfGeometry(geometry) && DimensionTest(geometry.Dimensions))
                {
                    IRTreeDataContainer searchContainer;
                    if (geometry is IPoint)
                        searchContainer = new PointDataContainer(((IPoint)geometry).PointToSpaceR());
                    else
                        searchContainer = new SpatialDataContainer(geometry.GeometryToMBR());


                    result = this.Searching(searchContainer, predicate, (x, y) => x.Intersection(y), (x, y) => x.Adjacency(x));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }

        public bool GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, int countOfNextNeighbors, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (this._mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    var containers = new LinkedList<Tuple<float, IRTreeDataContainer>>();
                    var elements = ((RTreeLeaf)output.Parent).Data;
                    var parent = (RTreeLeaf)output.Parent;
                    var maxElement = float.PositiveInfinity;
                    foreach (IRTreeDataContainer value in elements)
                    {
                        var dist = Distance(output, value);
                        if ((containers.Count < countOfNextNeighbors || dist < maxElement) && value != output && (predicate == null || predicate(value.GraphElement)))
                        {
                            if (containers.Count == 0)
                            {
                                containers.AddFirst(new LinkedListNode<Tuple<float, IRTreeDataContainer>>(Tuple.Create(dist, value)));
                                maxElement = dist;
                            }
                            else
                            {
                                var end = true;
                                var start = containers.First;
                                while (end)
                                {

                                    if (dist < start.Value.Item1)
                                    {
                                        containers.AddBefore(start, Tuple.Create(dist, value));
                                        if (containers.Count > countOfNextNeighbors)
                                        {
                                            containers.RemoveLast();
                                            maxElement = containers.Last.Value.Item1;
                                        }
                                        break;
                                    }
                                    if (start == containers.Last)
                                    {
                                        end = false;
                                    }
                                    else
                                    {
                                        start = start.Next;
                                    }
                                }
                                if (!end && containers.Count < countOfNextNeighbors)
                                {
                                    maxElement = dist;
                                    containers.AddLast(Tuple.Create(dist, value));
                                }

                            }
                        }
                    }
                    Stack<ARTreeContainer> stack = new Stack<ARTreeContainer>();
                    if (Distance(_root, output) < maxElement || containers.Count < countOfNextNeighbors)
                        stack.Push(_root);
                    while (stack.Count > 0)
                    {
                        var currentContainer = stack.Pop();

                        if (!currentContainer.IsLeaf)
                        {
                            foreach (ARTreeContainer value2 in ((RTreeNode)currentContainer).Children)
                            {
                                if (Distance(value2, output) < maxElement || containers.Count < countOfNextNeighbors)
                                    stack.Push(value2);
                            }
                        }
                        else
                        {
                            if (currentContainer != parent)
                                foreach (IRTreeDataContainer value2 in ((RTreeLeaf)currentContainer).Data)
                                {
                                    var dist = Distance(value2, output);

                                    if ((containers.Count < countOfNextNeighbors || dist < maxElement) && value2 != output && (predicate == null || predicate(value2.GraphElement)))
                                    {
                                        if (containers.Count == 0)
                                        {
                                            containers.AddFirst(new LinkedListNode<Tuple<float, IRTreeDataContainer>>(Tuple.Create(dist, value2)));
                                            maxElement = dist;
                                        }
                                        else
                                        {
                                            var end = true;
                                            var start = containers.First;
                                            while (end)
                                            {

                                                if (dist < start.Value.Item1)
                                                {
                                                    containers.AddBefore(start, Tuple.Create(dist, value2));
                                                    if (containers.Count > countOfNextNeighbors)
                                                    {
                                                        containers.RemoveLast();
                                                        maxElement = containers.Last.Value.Item1;
                                                    }
                                                    break;
                                                }
                                                if (start == containers.Last)
                                                {
                                                    end = false;
                                                }
                                                else
                                                {
                                                    start = start.Next;
                                                }
                                            }
                                            if (!end && containers.Count < countOfNextNeighbors)
                                            {
                                                maxElement = dist;
                                                containers.AddLast(Tuple.Create(dist, value2));
                                            }
                                        }
                                    }
                                }
                        }

                    }
                    result = containers.Select(_ => _.Item2.GraphElement).ToList().AsReadOnly();
                }
                FinishReadResource();
                return result.Count > 0;

            }
            throw new CollisionException();
        }

        public bool GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, int countOfNextNeighbors, Predicate<AGraphElement> predicate=null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfGeometry(geometry) && DimensionTest(geometry.Dimensions))
                {
                    IRTreeDataContainer output;
                    if (geometry is IPoint)
                        output = new PointDataContainer(((IPoint)geometry).PointToSpaceR());
                    else
                        output = new SpatialDataContainer(geometry.GeometryToMBR());


                    LinkedList<Tuple<float, IRTreeDataContainer>> containers = new LinkedList<Tuple<float, IRTreeDataContainer>>();
                    var maxElement = float.PositiveInfinity;

                    Stack<ARTreeContainer> stack = new Stack<ARTreeContainer>();
                    if (Distance(_root, output) < maxElement || containers.Count < countOfNextNeighbors)
                        stack.Push(_root);
                    while (stack.Count > 0)
                    {
                        var currentContainer = stack.Pop();

                        if (!currentContainer.IsLeaf)
                        {
                            foreach (ARTreeContainer value2 in ((RTreeNode)currentContainer).Children)
                            {
                                if (Distance(value2, output) < maxElement || containers.Count < countOfNextNeighbors)
                                    stack.Push(value2);
                            }
                        }
                        else
                        {
                            foreach (IRTreeDataContainer value2 in ((RTreeLeaf)currentContainer).Data)
                            {
                                var dist = Distance(value2, output);

                                if ((containers.Count < countOfNextNeighbors || dist < maxElement) && value2 != output && (predicate == null || predicate(value2.GraphElement)))
                                {
                                    if (containers.Count == 0)
                                    {
                                        containers.AddFirst(new LinkedListNode<Tuple<float, IRTreeDataContainer>>(Tuple.Create(dist, value2)));
                                        maxElement = dist;
                                    }
                                    else
                                    {
                                        var end = true;
                                        var start = containers.First;
                                        while (end)
                                        {

                                            if (dist < start.Value.Item1)
                                            {
                                                containers.AddBefore(start, Tuple.Create(dist, value2));
                                                if (containers.Count > countOfNextNeighbors)
                                                {
                                                    containers.RemoveLast();
                                                    maxElement = containers.Last.Value.Item1;
                                                }
                                                break;
                                            }
                                            if (start == containers.Last)
                                            {
                                                end = false;
                                            }
                                            else
                                            {
                                                start = start.Next;
                                            }
                                        }
                                        if (!end && containers.Count < countOfNextNeighbors)
                                        {
                                            maxElement = dist;
                                            containers.AddLast(Tuple.Create(dist, value2));
                                        }
                                    }
                                }
                            }
                        }

                    }
                    result = containers.Select(_ => _.Item2.GraphElement).ToList().AsReadOnly();
                }
                FinishReadResource();
                return result.Count > 0;


            }
            throw new CollisionException();
        }
        

        public bool SearchPoint(out ReadOnlyCollection<AGraphElement> result, IPoint point, Predicate<AGraphElement> predicate = null)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfGeometry(point) && DimensionTest(point.Dimensions))
                {
                    var searchContainer = new PointDataContainer(point.PointToSpaceR());
                    result = Searching(searchContainer, predicate, (x, y) => x.Intersection(y), (x, y) => x.Intersection(y));
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException();
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this._mapOfContainers.Clear();
            this.Metric = null;
            this._root.Dispose();
            this._root = null;
            _levelForOverflowStrategy.Clear();
            Space = null;
        }
        #endregion

        #region IFallen8Plugin implementation

        public Type PluginCategory
        {
            get { return typeof(IIndex); }
        }

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");

            if (!parameter.ContainsKey("IMetric")||!(parameter["IMetric"] is IMetric))
                throw new Exception("IMetric is uncorrectly");
            Metric = (IMetric)parameter["IMetric"];

            if (!parameter.ContainsKey("MinCount")||!(parameter["MinCount"] is int))
                throw new Exception("MinCount is uncorrectly");
            MinCountOfNode = (int)parameter["MinCount"];

            if (!parameter.ContainsKey("MaxCount")||!(parameter["MaxCount"] is int))
                throw new Exception("Max is uncorrectly");
            MaxCountOfNode = (int)parameter["MaxCount"];

            if (!parameter.ContainsKey("Space")||!(parameter["Space"] is IEnumerable<IDimension>))
                throw new Exception("Space is uncorrectly");
            Space = new List<IDimension>(parameter["Space"] as IEnumerable<IDimension>);

            foreach (IDimension value in Space)
            {
                _countOfR += value.CountOfR;
            }

            this._mapOfContainers = new Dictionary<int, IRTreeDataContainer>();
            
            
            if (MinCountOfNode*2 > MaxCountOfNode+1)
                throw new Exception("with this parametrs MinCount and MaxCount is split method inposible");


            this._countOfReInsert = (MaxCountOfNode - MinCountOfNode) / 3;
            if (_countOfReInsert < 1) _countOfReInsert = 1;


            //set of root
            var lower = new float[this._countOfR];

            var upper = new float[this._countOfR];

            for (int i = 0; i < this._countOfR; i++)
            {
                lower[i] = float.PositiveInfinity;
                upper[i] = float.NegativeInfinity;
            }
            _root = new RTreeLeaf(lower, upper);

            this._levelForOverflowStrategy = new List<bool> {true};
        }

        public string PluginName
        {
            get { return "SpatialIndex"; }
        }


        public string Description
        {
            get { return "This is the realisation of the spatial index as r*-tree"; }
        }

        public string Manufacturer
        {
            get { return "Andriy Kupershmidt"; }
        }

        #endregion

        #region IFallen8Serializable

        public void Save(SerializationWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Open(SerializationReader reader, Fallen8 fallen8)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
