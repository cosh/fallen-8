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
using System.Text;
using Fallen8.API.Index.Spatial.Implementation.SpatialContainer;
using Fallen8.API.Model;
using System.Collections.ObjectModel;
using Framework.Serialization;

namespace Fallen8.API.Index.Spatial.Implementation.RTree
{
    /// <summary>
    /// Spatial index implementation as r-tree
    /// </summary>
    public class RTree : ISpatialIndex
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
        public IEnumerable<IDimension> Space { get; private set; }
        #endregion
        #region private member
        private int countOfR = 0;
        private Dictionary<int, ARTreeContainer> mapOfContainers;
        #endregion
        #region constructor
        public RTree(IMetric metric, int minCountOfChilder, int maxCountofChildren, IEnumerable<IDimension> space)
        {
            Space = new List<IDimension>(space);

            foreach (IDimension value in space)
            {
                countOfR += value.CountOfR;
            }

            this.mapOfContainers = new Dictionary<int, ARTreeContainer>();
            Metric = metric;
            MinCountOfNode = minCountOfChilder;
            MaxCountOfNode = maxCountofChildren;
        }
        #endregion
        #region private function
        #region distance between two spatial containers
        private double Distance(ISpatialContainer container1, ISpatialContainer container2)
        {
            throw new NotImplementedException();
           /* var lower1 = container1.LowerPoint.GetEnumerator();
            var upper1 = container1.UpperPoint.GetEnumerator();
            var lower2 = container2.LowerPoint.GetEnumerator();
            var upper2 = container2.UpperPoint.GetEnumerator();

            var countOfDimension = container1.LowerPoint.Count();

            if (countOfDimension == 0
                && countOfDimension == container1.UpperPoint.Count()
                && countOfDimension == container2.LowerPoint.Count()
                && countOfDimension == container2.UpperPoint.Count())
                throw new Exception("The points are in different space or space not exist ");


            var pointStart = new List<double>(this.countOfR);
            var pointEnd = new List<double>(this.countOfR);

            do
            {

                if (!(lower1.Current > lower2.Current &&
                                  lower1.Current < upper2.Current ||
                                  (lower1.Current < lower2.Current &&
                                  upper1.Current > lower2.Current)))
                {
                    if (Math.Abs(lower1.Current - upper2.Current) >
                        Math.Abs(upper1.Current - lower2.Current))
                    {
                        pointStart.Add(upper1.Current);
                        pointEnd.Add(lower2.Current);
                    }
                    else
                    {
                        pointStart.Add(lower1.Current);
                        pointEnd.Add(upper2.Current);
                    }
                }
            }
            while (
                lower1.MoveNext() &&
                lower2.MoveNext() &&
                upper1.MoveNext() &&
                upper2.MoveNext());


            if (pointStart.Count == 0)
                return 0.0;
            else
                return this.Metric.Distance(new RTreePoint(pointStart), new RTreePoint(pointEnd));
            */
        }
        #endregion
        #endregion


        public bool TryGetValues(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat = null)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValues(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat = null)
        {
            throw new NotImplementedException();
        }

        public bool Insert(AGraphElement element, IGeometry geometry)
        {
            throw new NotImplementedException();
        }

        public double Distance(IGeometry geometry1, IGeometry geometry2)
        {
            throw new NotImplementedException();
        }

        public double Distance(AGraphElement graphElement1, AGraphElement graphElement2)
        {
            throw new NotImplementedException();
        }

        public bool SearchRegion(out ReadOnlyCollection<AGraphElement> result, IMBR minimalBoundedRechtangle, Predicate<AGraphElement> predicat = null)
        {
            throw new NotImplementedException();
        }

        public bool Overlap(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool Overlap(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool Enclosure(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool Enclosure(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool Containment(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool Containment(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool GetAllNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry graphElement, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, AGraphElement graphElement, int countOfNextNeighbors, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool GetNextNeighbors(out ReadOnlyCollection<AGraphElement> result, IGeometry geometry, int countOfNextNeighbors, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public int CountOfKeys()
        {
            throw new NotImplementedException();
        }

        public int CountOfValues()
        {
            return this.mapOfContainers.Count;
        }

        public void AddOrUpdate(IComparable key, AGraphElement graphElement)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveKey(IComparable key)
        {
            throw new NotImplementedException();
        }

        public void RemoveValue(AGraphElement graphElement)
        {
            throw new NotImplementedException();
        }

        public void Wipe()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComparable> GetKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<IComparable, ReadOnlyCollection<AGraphElement>>> GetKeyValues()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(out ReadOnlyCollection<AGraphElement> result, IComparable key)
        {
            throw new NotImplementedException();
        }

        public string PluginName
        {
            get { return "SpatialIndex"; }
        }

        public Type PluginCategory
        {
            get { return typeof(IIndex); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string Manufacturer
        {
            get { return "Andriy Kupershmidt"; }
        }

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
          /*  IMetric currentMetric = parameter["IMetric"] as IMetric;
            int minCountOfChilder = (int)parameter["MinCount"];
            int maxCountofChildren = (int) parameter["MaxCount"];
            IEnumerable<IDimension> space = parameter["Space"] as IEnumerable<IDimension>;
*/
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Save(SerializationWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Open(SerializationReader reader, Fallen8 fallen8)
        {
            throw new NotImplementedException();
        }


        public bool SearchDistance(out ReadOnlyCollection<AGraphElement> result, double distance, AGraphElement graphElement, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }

        public bool SerchDistance(out ReadOnlyCollection<AGraphElement> result, double distance, IGeometry geometry, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }


        public void AddOrUpdate(IGeometry key, AGraphElement graphElement)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveKey(IGeometry key)
        {
            throw new NotImplementedException();
        }


        public bool SearchPoint(out ReadOnlyCollection<AGraphElement> result, IPoint point, Predicate<AGraphElement> predicat)
        {
            throw new NotImplementedException();
        }
    }
}
