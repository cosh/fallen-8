// 
// Fallen8PersistencyFactory.cs
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
using System.Collections.Generic;
using Fallen8.API.Model;
using Fallen8.API.Index;
using System.IO;
using Fallen8.API.Helper;
using Framework.Serialization;

namespace Fallen8.API.Persistency
{
    /// <summary>
    /// Fallen8 persistency factory.
    /// </summary>
    public static class Fallen8PersistencyFactory
    {
        /// <summary>
        /// Save the specified graphElements, indices and path.
        /// </summary>
        /// <param name='graphElements'>
        /// Graph elements.
        /// </param>
        /// <param name='indices'>
        /// Indices.
        /// </param>
        /// <param name='path'>
        /// Path.
        /// </param>
        public static void Save(Int32 currentId, List<AGraphElement> graphElements, IDictionary<String, IIndex> indices, String path)
        {
            // Create the new, empty data file.
            if (File.Exists(path))
            {
                File.Move(path, path + "OLD");
            }
            
            var file = File.Create(path, Constants.BufferSize, FileOptions.SequentialScan);
            SerializationWriter writer = null;
            
           
            writer = new SerializationWriter(file);
            
            #region write vertices and edges
            
            
            #endregion
            
            #region write indices
            
            
            #endregion
     
            if (writer != null) {
                writer.Flush();
                writer.Close();
            }
            
            if (file != null) {
                file.Flush();
                file.Close();
            }
        }
    }
}

