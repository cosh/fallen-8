// 
// RTree.cs
//  
// Author:
//       Andriy Kupershmidt <kuper133@googlemail.com>
// 
// Copyright (c) 2011-2015 Henning Rauch
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

#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Framework.Serialization;
using NoSQL.GraphDB.Error;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Index.Spatial.Implementation.SpatialContainer;
using NoSQL.GraphDB.Model;

#endregion

namespace NoSQL.GraphDB.Index.Spatial.Implementation.RTree
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
        private IMetric Metric { get; set; }
        /// <summary>
        /// minimal value of count of kinder in the container
        /// </summary>
        private int MinCountOfNode { get; set; }
        /// <summary>
        /// maximal value of count of kinder in the container
        /// </summary>
        private int MaxCountOfNode { get; set; }
        /// <summary>
        /// a space for the r-tree
        /// </summary>
        private List<IDimension> Space { get; set; }

        #endregion

        #region private member
        private List<Boolean> _levelForOverflowStrategy;
        private int _countOfR;
        private int _countOfReInsert;
        private Dictionary<int, IRTreeDataContainer> _mapOfContainers;
        private ARTreeContainer _root;

        private delegate bool SpatialFilter(ISpatialContainer spatialContainer1, ISpatialContainer spatialContainer2);
		
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
            var point1 = new float[_countOfR];
            var point2 = new float[_countOfR];


            for (int i = 0; i < _countOfR; i++)
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
        private ReadOnlyCollection<AGraphElement> Searching(IRTreeDataContainer element, SpatialFilter spatialPredicate, SpatialFilter dataContainerPredicate)
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
                    foreach (var value in ((RTreeNode)currentContainer).Children)
                    {
                        if (spatialPredicate(value, element))
                            stack.Push(value);
                    }
                }
                else
                {
                    foreach (var value in ((RTreeLeaf)currentContainer).Data)
                    {
                        if (dataContainerPredicate(value, element))
                        {

                            currentResult.Add(value.GraphElement);
                        }

                    }
                }
            }

            return currentResult.AsReadOnly();
        }
        #endregion
        
		#region helper methods

        private static bool Inclusion(ISpatialContainer x, ISpatialContainer y)
		{
			return x.Inclusion(y);
		}


        private static bool Adjacency(ISpatialContainer x, ISpatialContainer y)
		{

			return x.Adjacency(y);
		}

        private static bool ReInclusion(ISpatialContainer x, ISpatialContainer y)
        {
			return y.Inclusion(x);
		}

		

        private static bool Intersection(ISpatialContainer x, ISpatialContainer y)
		{
			return x.Intersection(y);
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
                    foreach (var value in ((RTreeNode)currentContainer).Children)
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
            return CreateSearchContainer(new MBR(element.LowerPoint, element.UpperPoint), distance);
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

            var lower = new float[_countOfR];
            var upper = new float[_countOfR];

            var transformationOfDistance = Metric.TransformationOfDistance(distance, mbr);

            for (int i = 0; i < _countOfR; i++)
            {
                lower[i] = mbr.Lower[i] - transformationOfDistance[i];
                upper[i] = mbr.Upper[i] + transformationOfDistance[i];
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
        /// <returns>
        /// list of result
        /// </returns>
        private ReadOnlyCollection<AGraphElement> OverlapSearch(IRTreeDataContainer searchContainer)
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
                    foreach (var value in ((RTreeNode)currentContainer).Children)
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
                var leafLevel = _levelForOverflowStrategy.Count - 1;
                if (((RTreeNode)rTree.Parent).Children.Count < MinCountOfNode)
                    LocalReorganisationByRemoving(rTree.Parent, level - 1);
                
                if (rTree is RTreeLeaf)
                {
                    foreach (var value in ((RTreeLeaf)rTree).Data)
                    {
                        var newLeafLeavel = _levelForOverflowStrategy.Count - 1;
                        Insert(value, level - (leafLevel - newLeafLeavel));
                    }

                }
                else
                {
                    foreach (var value in ((RTreeNode)rTree).Children)
                    {
                        var newLeafLeavel = _levelForOverflowStrategy.Count - 1;
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
                        for (int i = 0; i < _countOfR; i++)
                        {
                            rTree.Upper[i] = float.NegativeInfinity;
                            rTree.Lower[i] = float.PositiveInfinity;
                        }
                }
                else
                {
                    if (((RTreeNode)rTree).Children.Count == 1)
                    {
                        _root = ((RTreeNode)rTree).Children[0];
                        _root.Parent = null;
                        rTree.Dispose();
                        _levelForOverflowStrategy.RemoveAt(_levelForOverflowStrategy.Count - 1);
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
            Insert(container, _levelForOverflowStrategy.Count - 1);
            //      for (int i = 1; i < this.levelForOverflowStrategy.Count; i++)
            //          levelForOverflowStrategy[i] = false;

        }
        #endregion

        #region Insert
        private void Insert(IRTreeContainer container, int level)
        {
            if (level == _levelForOverflowStrategy.Count - 1)
            {
                var chooseSubTree = (RTreeLeaf)ChooseSubTree(container, level);
                container.Parent = chooseSubTree;
                chooseSubTree.Data.Add((IRTreeDataContainer)container);
                Recalculation(container);
                if (chooseSubTree.Data.Count > MaxCountOfNode)
                {
                    OverflowTreatment(level, chooseSubTree);
                }
            }
            else
            {
                var chooseSubTree = (RTreeNode)ChooseSubTree(container, level);
                container.Parent = chooseSubTree;
                chooseSubTree.Children.Add((ARTreeContainer)container);
                Recalculation(container);

                if (chooseSubTree.Children.Count > MaxCountOfNode)
                {
                    OverflowTreatment(level, chooseSubTree);
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
                recalculatePath.Parent.Lower = recalculateMBR.Lower;
                recalculatePath.Parent.Upper = recalculateMBR.Upper;
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
                    recalculatePath.Parent.Lower = newMBR.Lower;
                    recalculatePath.Parent.Upper = newMBR.Upper;
                    recalculatePath.Parent.Area = GetArea(newMBR);
                }
                else
                {
                    var newMBR = FindMBR(((RTreeNode)recalculatePath.Parent).Children);
                    recalculatePath.Parent.Lower = newMBR.Lower;
                    recalculatePath.Parent.Upper = newMBR.Upper;
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
                    recalculatePath.Parent.Lower = newMBR.Lower;
                    recalculatePath.Parent.Upper = newMBR.Upper;
                    recalculatePath.Parent.Area = GetArea(newMBR);
                }
                else
                {
                    var newMBR = FindMBR(((RTreeNode)recalculatePath.Parent).Children);
                    recalculatePath.Parent.Lower = newMBR.Lower;
                    recalculatePath.Parent.Upper = newMBR.Upper;
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
            if (level != 0 && _levelForOverflowStrategy[level])
            {
                _levelForOverflowStrategy[level] = false;
                ReInsert(container, level);

            }
            else
            {
                Split(container, level);
            }
        }
        #endregion

        #region ReInsert
        private void ReInsert(ARTreeContainer container, int level)
        {
            var center = FindCenterOfContainer(container);
            var distance = new List<Tuple<float, IRTreeContainer>>(MaxCountOfNode + 1);
            var dataContainer = container.IsLeaf 
                                     ? new List<ISpatialContainer>(((RTreeLeaf) container).Data) 
                                     : new List<ISpatialContainer>(((RTreeNode) container).Children);

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

            var reinsertData = new List<Tuple<float, IRTreeContainer>>(_countOfReInsert);
            reinsertData.AddRange(distance.GetRange(0, _countOfReInsert));
            if (container.IsLeaf)
                ((RTreeLeaf) container).Data.RemoveAll(_ => reinsertData.Exists(y => y.Item2 == _));
            else
                ((RTreeNode) container).Children.RemoveAll(_ => reinsertData.Exists(y => y.Item2 == _));

            RecalculationByRemoving(reinsertData.Select(_ => _.Item2));
            var leafLevel = _levelForOverflowStrategy.Count - 1;
            for (int i = 0; i < _countOfReInsert; i++)
            {
                reinsertData[i].Item2.Parent = null;
                var newLeafLeavel = _levelForOverflowStrategy.Count - 1;
                Insert(reinsertData[i].Item2, level - (leafLevel - newLeafLeavel));
            }


        }

        #endregion

        #region Find center of container
        private float[] FindCenterOfContainer(ASpatialContainer container)
        {
            var center = new float[_countOfR];
            for (int i = 0; i < _countOfR; i++)
            {
                center[i] = ((container.Lower[i] + container.Upper[i]) / 2);
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
                var lower = _root.Lower;
                var upper = _root.Upper;
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
                    if (((RTreeNode)container1.Parent).Children.Count > MaxCountOfNode)
                        OverflowTreatment(level - 1, container1.Parent);
                }
                else
                {
                    if (((RTreeLeaf)container2.Parent).Data.Count > MaxCountOfNode)
                        OverflowTreatment(level - 1, container2.Parent);
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
            var currentContainers = container.IsLeaf == false 
                ? new List<IRTreeContainer>(((RTreeNode)container).Children) 
                : new List<IRTreeContainer>(((RTreeLeaf)container).Data);

            result = currentContainers;

            var marginValue = new float[_countOfR];

            for (int i = 0; i < _countOfR; i++)
            {

                currentContainers.Sort((x, y) => x.LowerPoint[i].CompareTo(y.LowerPoint[i]));
                currentContainers.Sort((x, y) => x.UpperPoint[i].CompareTo(y.UpperPoint[i]));

                var firstSeq = new List<IRTreeContainer>();
                var secondSeq = new List<IRTreeContainer>();

                firstSeq.AddRange(currentContainers.GetRange(0, MinCountOfNode));
                secondSeq.AddRange(currentContainers.GetRange(MinCountOfNode, currentContainers.Count - MinCountOfNode));

                var position = 0;
                marginValue[i] = FindMarginValue(firstSeq, i) +
                                       FindMarginValue(secondSeq, i);
                while (position <= currentContainers.Count - 2 * MinCountOfNode)
                {
                    firstSeq.Add(secondSeq.First());
                    secondSeq.RemoveAt(0);
                    marginValue[i] += FindMarginValue(firstSeq, i) +
                                       FindMarginValue(secondSeq, i);
                    position++;
                }
            }

            var splitAxis = 0;
            var sum = marginValue[0];

            for (int i = 1; i < _countOfR; i++)
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

            foreach (var value in containers)
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


            firstSeqTest.AddRange(containers.GetRange(0, MinCountOfNode - 1));
            secondSeqTest.AddRange(containers.GetRange(MinCountOfNode - 1, containers.Count - MinCountOfNode + 1));

            var position = 0;

            float minOverlapValue = float.PositiveInfinity;
            IMBR mbrParent1 = null;
            IMBR mbrParent2 = null;
            float area1 = float.PositiveInfinity;
            float area2 = float.PositiveInfinity;
            var sequenzPosition = 0;

            while (position <= containers.Count - 2 * MinCountOfNode)
            {
                firstSeqTest.Add(secondSeqTest.First());
                secondSeqTest.RemoveAt(0);
                var mbr1 = FindMBR(firstSeqTest);
                var mbr2 = FindMBR(secondSeqTest);
                var currentOverlap = FindOverlapValue(mbr1, mbr2);
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
            parent1.Lower = mbrParent1.Lower;
            parent1.Upper = mbrParent1.Upper;

            parent2.Area = area2;
            parent2.Lower = mbrParent2.Lower;
            parent2.Upper = mbrParent2.Upper;

            if (!parent1.IsLeaf)
            {
                for (int counter = 0; counter < containers.Count; counter++)
                {
                    if (counter <= MinCountOfNode - 1 + sequenzPosition)
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
                    if (counter <= MinCountOfNode - 1 + sequenzPosition)
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
            var lower = new float[_countOfR];
            var upper = new float[_countOfR];

            for (int dimension = 0; dimension < _countOfR; dimension++)
            {
                var currentMin = float.PositiveInfinity;
                var currentMax = float.NegativeInfinity;
                foreach (var value in containers)
                {
                    if (value is ASpatialContainer)
                    {
                        var temporalMin = ((ASpatialContainer)value).Lower[dimension];
                        var temporalMax = ((ASpatialContainer)value).Upper[dimension];
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
            var lower = new float[_countOfR];
            var upper = new float[_countOfR];

            for (int dimension = 0; dimension < _countOfR; dimension++)
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
            var value = new float[_countOfR];
            var lowerPoint1 = mbr1.Lower;
            var upperPoint1 = mbr1.Upper;
            var lowerPoint2 = mbr2.Lower;
            var upperPoint2 = mbr2.Upper;
            var overlapValue = 1.0f;

            for (int i = 0; i < _countOfR; i++)
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

            foreach (var value in list)
            {
                var oldOverlap = 0.0f;
                var newOverlap = 0.0f;
                var newMBR = FindMBR(value, container);
                foreach (var value2 in list)
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
            foreach (var value in list)
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
            for (int i = 0; i < mbr.Lower.Length; i++)
            {
                currentArea *= mbr.Upper[i] - mbr.Lower[i];

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
            return iPoint.PointToSpaceR().Length == _countOfR;
        }

        #endregion
        #region Test of Dimension
        private Boolean DimensionTest(List<IDimension> dimensions)
        {
            if (dimensions == null)
            {
                return false;
            }

            if (dimensions.Count != Space.Count || dimensions.Count == 0)
                return false;

            for (var i = 0; i < dimensions.Count; i++)
            {
                if (dimensions[i].ObjectType !=
                    Space[i].ObjectType ||
                    dimensions[i].CountOfR !=
                    Space[i].CountOfR)
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
            if (mbr.Lower.Length != mbr.Upper.Length && mbr.Lower.Length != _countOfR)
            {
                return false;
            }
            for (int i = 0; i < mbr.Lower.Length; i++)
                if (mbr.Lower[i] > mbr.Upper[i])
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
                var count = _mapOfContainers.Count;

                FinishReadResource();

                return count;
            }
            throw new CollisionException(this);
        }

        public void RemoveValue(AGraphElement graphElement)
        {
            if (WriteResource())
            {
                IRTreeDataContainer dataContainer;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out dataContainer))
                {
                    ((RTreeLeaf)dataContainer.Parent).Data.Remove(dataContainer);
                    RecalculationByRemoving(dataContainer);
                    _mapOfContainers.Remove(graphElement.Id);
                    if (((RTreeLeaf)dataContainer.Parent).Data.Count < MinCountOfNode)
                    {

                        LocalReorganisationByRemoving(dataContainer.Parent, _levelForOverflowStrategy.Count - 1);
                    }
                }

                FinishWriteResource();

                return;
            }
            throw new CollisionException(this);
        }

        public void Wipe()
        {
            _mapOfContainers.Clear();
            var lower = new float[_countOfR];
            var upper = new float[_countOfR];
            for (int i = 0; i < _countOfR; i++)
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

        public IEnumerable<KeyValuePair<object, ReadOnlyCollection<AGraphElement>>> GetKeyValues()
        {
            return Enumerable.Empty<KeyValuePair<object, ReadOnlyCollection<AGraphElement>>>();
        }

        #endregion

        #region ISpatialIndex Implementation

        public bool TryRemoveKey(Object keyObject)
        {
            IGeometry key;
            if (!IndexHelper.CheckObject(out key, keyObject))
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

                    var removeObjects = Searching(searchContainer, Inclusion, Inclusion);
                    FinishReadResource();
                    foreach (var value in removeObjects)
                    {
                        RemoveValue(value);
                    }

                    return removeObjects.Count > 0;
                }
                return false;
            }
            throw new CollisionException(this);
        }

        #region TryGetValue
        public bool TryGetValue(out ReadOnlyCollection<AGraphElement> result, Object geometryObject)
        {
            IGeometry geometry;
            if (!IndexHelper.CheckObject(out geometry, geometryObject))
            {
                result = null;
                return false;
            }

            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer element;
                if (DimensionTest(geometry.Dimensions) && TestOfGeometry(geometry))
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
            throw new CollisionException(this);
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
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        public bool SearchDistance(out ReadOnlyCollection<AGraphElement> result, float distance, IGeometry geometry)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                SpatialDataContainer searchContainer;
                if (DimensionTest(geometry.Dimensions) && TestOfGeometry(geometry))
                {
                    searchContainer = CreateSearchContainer(geometry.GeometryToMBR(), distance);
                    result = OverlapSearch(searchContainer);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
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
        /// <returns>
        /// <c>true</c> if something was found; otherwise, <c>false</c>.
        /// </returns>
        public bool SearchDistance(out ReadOnlyCollection<AGraphElement> result, float distance, AGraphElement graphElement)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly(); IRTreeDataContainer element;
                SpatialDataContainer searchContainer;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out element))
                {
                    searchContainer = CreateSearchContainer(element, distance);
                    result = OverlapSearch(searchContainer);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }


        public void AddOrUpdate(Object keyObject, AGraphElement graphElement)
        {
            IGeometry key;
            if (!IndexHelper.CheckObject(out key, keyObject))
            {
                return;
            }

            if (WriteResource())
            {
                if (DimensionTest(key.Dimensions) && TestOfGeometry(key))
                {
                    if (!_mapOfContainers.ContainsKey(graphElement.Id))
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
                            currentContainer = new SpatialDataContainer(mbr.Lower, mbr.Upper)
                                                   {GraphElement = graphElement};
                        }

                        InsertData(currentContainer);
                        _mapOfContainers.Add(graphElement.Id, currentContainer);

                    }
                    else
                    {
                        RemoveValue(graphElement);
                        AddOrUpdate(key, graphElement);
                    }
                    
                }
                FinishWriteResource();
                return;
            }
            throw new CollisionException(this);
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
                var points = FindNeighborPoints(mbr1.Lower, mbr1.Upper, mbr2.Lower, mbr2.Upper);
                    
                FinishReadResource();
                return Metric.Distance(points.Item1, points.Item2);
            }
            throw new CollisionException(this);
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
            throw new CollisionException(this);

        }

        public bool SearchRegion(out ReadOnlyCollection<AGraphElement> result, IMBR minimalBoundedRechtangle)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfMBR(minimalBoundedRechtangle))
                {
                    var searchRegion = new SpatialDataContainer(minimalBoundedRechtangle);
                    result = OverlapSearch(searchRegion);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool Overlap(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry)
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

                    result = OverlapSearch(searchContainer);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool Overlap(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = OverlapSearch(output);
                }

                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool Enclosure(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = Searching(output, Inclusion, Inclusion);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool Enclosure(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry)
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


                    result = Searching(searchContainer, Inclusion, Inclusion);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool Containment(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry)
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


                    result = Searching(searchContainer, Intersection, ReInclusion);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool Containment(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = Searching(output, Intersection, ReInclusion);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    result = Searching(output, Intersection, Adjacency);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry)
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


                    result = Searching(searchContainer, Intersection, Adjacency);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }

        public bool GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, int countOfNextNeighbors)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                IRTreeDataContainer output;
                if (_mapOfContainers.TryGetValue(graphElement.Id, out output))
                {
                    var containers = new LinkedList<Tuple<float, IRTreeDataContainer>>();
                    var elements = ((RTreeLeaf)output.Parent).Data;
                    var parent = (RTreeLeaf)output.Parent;
                    var maxElement = float.PositiveInfinity;
                    foreach (var value in elements)
                    {
                        var dist = Distance(output, value);
                        if ((containers.Count < countOfNextNeighbors || dist < maxElement) && value != output)
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
                    var stack = new Stack<ARTreeContainer>();
                    if (Distance(_root, output) < maxElement || containers.Count < countOfNextNeighbors)
                        stack.Push(_root);
                    while (stack.Count > 0)
                    {
                        var currentContainer = stack.Pop();

                        if (!currentContainer.IsLeaf)
                        {
                            foreach (var value2 in ((RTreeNode)currentContainer).Children)
                            {
                                if (Distance(value2, output) < maxElement || containers.Count < countOfNextNeighbors)
                                    stack.Push(value2);
                            }
                        }
                        else
                        {
                            if (currentContainer != parent)
                                foreach (var value2 in ((RTreeLeaf)currentContainer).Data)
                                {
                                    var dist = Distance(value2, output);

                                    if ((containers.Count < countOfNextNeighbors || dist < maxElement) && value2 != output)
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
            throw new CollisionException(this);
        }

        public bool GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, int countOfNextNeighbors)
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


                    var containers = new LinkedList<Tuple<float, IRTreeDataContainer>>();
                    var maxElement = float.PositiveInfinity;

                    var stack = new Stack<ARTreeContainer>();
                    if (Distance(_root, output) < maxElement || containers.Count < countOfNextNeighbors)
                        stack.Push(_root);
                    while (stack.Count > 0)
                    {
                        var currentContainer = stack.Pop();

                        if (!currentContainer.IsLeaf)
                        {
                            foreach (var value2 in ((RTreeNode)currentContainer).Children)
                            {
                                if (Distance(value2, output) < maxElement || containers.Count < countOfNextNeighbors)
                                    stack.Push(value2);
                            }
                        }
                        else
                        {
                            foreach (var value2 in ((RTreeLeaf)currentContainer).Data)
                            {
                                var dist = Distance(value2, output);

                                if ((containers.Count < countOfNextNeighbors || dist < maxElement) && value2 != output)
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
            throw new CollisionException(this);
        }
        

        public bool SearchPoint(out ReadOnlyCollection<AGraphElement> result, IPoint point)
        {
            if (ReadResource())
            {
                result = new List<AGraphElement>().AsReadOnly();
                if (TestOfGeometry(point) && DimensionTest(point.Dimensions))
                {
                    var searchContainer = new PointDataContainer(point.PointToSpaceR());
                    result = Searching(searchContainer, Intersection, Intersection);
                }
                FinishReadResource();
                return result.Count > 0;
            }
            throw new CollisionException(this);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            _mapOfContainers.Clear();
            Metric = null;
            _root.Dispose();
            _root = null;
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

            foreach (var value in Space)
            {
                _countOfR += value.CountOfR;
            }

            _mapOfContainers = new Dictionary<int, IRTreeDataContainer>();
            
            
            if (MinCountOfNode*2 > MaxCountOfNode+1)
                throw new Exception("with this parametrs MinCount and MaxCount is split method inposible");


            _countOfReInsert = (MaxCountOfNode - MinCountOfNode) / 3;
            if (_countOfReInsert < 1) _countOfReInsert = 1;


            //set of root
            var lower = new float[_countOfR];

            var upper = new float[_countOfR];

            for (int i = 0; i < _countOfR; i++)
            {
                lower[i] = float.PositiveInfinity;
                upper[i] = float.NegativeInfinity;
            }
            _root = new RTreeLeaf(lower, upper);

            _levelForOverflowStrategy = new List<bool> {true};
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

        public void Load(SerializationReader reader, Fallen8 fallen8)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
