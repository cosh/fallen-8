// 
// Constants.cs
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
    public static class Constants
    {
        /// <summary>
        ///   The size of the file buffer when reading or writing Fallen-8 from a file stream.
        /// </summary>
        public const int BufferSize = 104857600;
        
        /// <summary>
        /// The minimum id
        /// </summary>
        public const int MinId = Int32.MinValue;

        /// <summary>
        /// The version separator in save files
        /// </summary>
        public const char VersionSeparator = '#';

        /// <summary>
        /// Graph element files contain this string
        /// </summary>
        public const string GraphElementsSaveString = "_graphElements_";

        /// <summary>
        /// Index files contain this string
        /// </summary>
        public const string IndexSaveString = "_index_";

        /// <summary>
        /// Service files contain this string
        /// </summary>
        public const string ServiceSaveString = "_service_";
    }
}