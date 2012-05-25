// 
// DateHelper.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012 Henning Rauch
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

namespace Fallen8.API.Helper
{
    /// <summary>
    ///   Constants.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        ///   The basic DateTime: 01.01.1970
        /// </summary>
        private static DateTime _nineTeenSeventy = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        ///   Convertes the DateTime format to an Unix-TimeStamp
        /// </summary>
        /// <param name="date"> The DateTime </param>
        /// <returns> UInt32 representation </returns>
        public static UInt32 ConvertDateTime(DateTime date)
        {
            return (Convert.ToUInt32((date - _nineTeenSeventy).TotalSeconds));
        }

        /// <summary>
        ///   Returns the modification date as a delta from the creation date representation
        /// </summary>
        /// <param name="creationDate"> The creation date representation </param>
        /// <returns> The modification date delta </returns>
        public static UInt32 GetModificationDate(UInt32 creationDate)
        {
            return ConvertDateTime(DateTime.Now) - creationDate;
        }

        /// <summary>
        ///   Get a DateTime
        /// </summary>
        /// <param name="secondsFromNineTeenSeventy"> The seconds from 1970 </param>
        /// <returns> The DateTime </returns>
        public static DateTime GetDateTimeFromUnixTimeStamp(uint secondsFromNineTeenSeventy)
        {
            return _nineTeenSeventy.AddSeconds(secondsFromNineTeenSeventy);
        }
    }
}