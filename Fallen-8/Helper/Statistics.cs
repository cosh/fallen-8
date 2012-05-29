// 
// Statistics.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace NoSQL.GraphDB.Helper
{
    /// <summary>
    /// Class for statistic calculations
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Calculates the average
        /// </summary>
        /// <param name="numbers">Double-values</param>
        /// <returns>Average</returns>
        public static double Average(List<double> numbers)
        {
            if (numbers != null) return numbers.Sum()/numbers.Count();

            throw new ArgumentException("Numbers are null or 0", "numbers");

        }

        /// <summary>
        /// Calculates the standard deviation
        /// </summary>
        /// <param name="numbers">Double-values</param>
        /// <returns>Standard deviation</returns>
        public static double StandardDeviation(List<double> numbers)
        {
            if (numbers != null)
            {
                var average = Average(numbers);

                return Math.Sqrt(numbers.Sum(_ => _ * _) / numbers.Count() - (average * average));
            }

            throw new ArgumentException("Numbers are null or 0", "numbers");
        }

        /// <summary>
        /// Calculates the median
        /// </summary>
        /// <param name="numbers">Double-values</param>
        /// <returns>Median</returns>
        public static double Median(List<double> numbers)
        {
            if (numbers != null && numbers.Count > 0)
            {
                if (numbers.Count() % 2 == 0)
                {
                    return numbers.OrderBy(_ => _).Skip(numbers.Count() / 2 - 1).Take(2).Sum() / 2;
                }
                return numbers.OrderBy(_ => _).ElementAt(Convert.ToInt32(Math.Floor((Convert.ToDouble(numbers.Count()) / 2))));
            }

            throw new ArgumentException("Numbers are null or 0", "numbers");
        }
    }
}
