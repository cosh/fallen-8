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
using System.Linq;
using System.Collections.Generic;
using Fallen8.API.Model;
using Fallen8.API.Index;
using System.IO;
using Fallen8.API.Helper;
using Framework.Serialization;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Fallen8.API.Persistency
{
    /// <summary>
    /// Fallen8 persistency factory.
    /// </summary>
    public static class Fallen8PersistencyFactory
    {
        #region public methods
  
        /// <summary>
        /// Load Fallen-8 from a save point
        /// </summary>
        /// <param name='pathToSavePoint'>
        /// Path to save point.
        /// </param>
        /// <param name='currentIdOfFallen8'>
        /// Current identifier of Fallen-8.
        /// </param>
        /// <param name='graphElementsOfFallen8'>
        /// Graph elements of Fallen-8.
        /// </param>
        /// <param name='indexFactoryOfFallen8'>
        /// Index factory of Fallen-8.
        /// </param>
        public static void Load (string pathToSavePoint, ref int currentIdOfFallen8, ref List<AGraphElement> graphElementsOfFallen8, ref IFallen8IndexFactory indexFactoryOfFallen8)
        {
            //if there is no savepoint file... do nothing
            if (!File.Exists(pathToSavePoint))
            {
                return;
            }
            
            var file = File.Open(pathToSavePoint, FileMode.Open, FileAccess.Read);
            var reader = new SerializationReader(file);
            
            //get the maximum id
            currentIdOfFallen8 = reader.ReadOptimizedInt32();
            
            #region graph elements
            
            //initialize the list of graph elements
            var graphElements = new AGraphElement[currentIdOfFallen8];
            var graphElementStreams = new List<String>();
            for (int i = 0; i < reader.ReadOptimizedInt32(); i++) 
            {
                graphElementStreams.Add(reader.ReadOptimizedString());
            }
            
            LoadGraphElements(graphElements, graphElementStreams);
            graphElementsOfFallen8 = new List<AGraphElement>(graphElements);
            
            #endregion
            
            #region indexe
            
            var indexStreams = new List<String>();
            for (int i = 0; i < reader.ReadOptimizedInt32(); i++) 
            {
                indexStreams.Add(reader.ReadOptimizedString());
            }
            
            #endregion
        }
        
        /// <summary>
        /// Save the specified graphElements, indices and pathToSavePoint.
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
        /// <param name='savePartitions'>
        /// The number of save partitions for the graph elements.
        /// </param>
        public static void Save(Int32 currentId, List<AGraphElement> graphElements, IDictionary<String, IIndex> indices, String path, int savePartitions)
        {
            // Create the new, empty data file.
            if (File.Exists(path))
            {
                //the newer save gets an timestamp
                path = path + DateTime.Now.ToBinary().ToString();
            }
            
            var file = File.Create(path, Constants.BufferSize, FileOptions.SequentialScan);
            var writer = new SerializationWriter(file);
            
            //create some futures to save as much as possible in parallel
            const TaskCreationOptions options = TaskCreationOptions.LongRunning;
            var f = new TaskFactory(CancellationToken.None, options, TaskContinuationOptions.None, TaskScheduler.Default);
            Task<String>[] graphElementSaver;
            Task<String>[] indexSaver;
            
            //the maximum id
            writer.WriteOptimized(currentId);
            
            #region graph elements
            
            var graphElementPartitions = CreatePartitions(graphElements.Count, savePartitions);
            graphElementSaver = (Task<String>[])Array.CreateInstance(typeof(Task<String>), graphElementPartitions.Count);
            
            for (int i = 0; i < graphElementPartitions.Count; i++) 
            {
                graphElementSaver[i] = f.StartNew(() => SaveBunch(graphElementPartitions[i], graphElements, path));
            }
            
            #endregion
            
            #region indices
            
            indexSaver = (Task<String>[])Array.CreateInstance(typeof(Task<String>), indices.Count);
            
            var counter = 0;
            foreach(var aIndex in indices)
            {
                indexSaver[counter] = f.StartNew(() => SaveIndex(aIndex.Key, aIndex.Value, path));
                counter++;
            }
            
            #endregion
            
            Task.WaitAll(graphElementSaver);   
            Task.WaitAll(indexSaver);   
            
            writer.WriteOptimized(graphElementSaver.Length);
            foreach (var aFileStreamName in graphElementSaver) 
            {
                writer.WriteOptimized(aFileStreamName.Result);    
            }
            
            writer.WriteOptimized(indexSaver.Length);
            foreach (var aIndexFileName in indexSaver) 
            {
                writer.WriteOptimized(aIndexFileName.Result);    
            }
            
            if (writer != null) {
                writer.Flush();
                writer.Close();
            }
            
            if (file != null) {
                file.Flush();
                file.Close();
            }
        }
  
        #endregion
        
        #region private helper
        
        /// <summary>
        /// The serialized edge.
        /// </summary>
        private const Int32 SerializedEdge = 0;
        
        /// <summary>
        /// The serialized vertex.
        /// </summary>
        private const Int32 SerializedVertex = 1; 
        
        /// <summary>
        /// The serialized null.
        /// </summary>
        private const Int32 SerializedNull = 2;
        
        /// <summary>
        /// Saves the index.
        /// </summary>
        /// <returns>
        /// The filename of the persisted index.
        /// </returns>
        /// <param name='indexName'>
        /// Index name.
        /// </param>
        /// <param name='index'>
        /// Index.
        /// </param>
        /// <param name='path'>
        /// Path.
        /// </param>
        private static String SaveIndex (string indexName, IIndex index, string path)
        {
            String indexFileName = path + "_index_" + indexName;
                
            var indexFile = File.Create(indexFileName, Constants.BufferSize, FileOptions.SequentialScan);
            SerializationWriter indexWriter = new SerializationWriter(indexFile);
            
            index.Save(ref indexWriter);
            
            if (indexWriter != null) 
            {
                indexWriter.Flush();
                indexWriter.Close();
            }
            
            if (indexFile != null) 
            {
                indexFile.Flush();
                indexFile.Close();
            }
            
            return indexFileName;
        }
  
        /// <summary>
        /// Loads a graph element bunch.
        /// </summary>
        /// <returns>
        /// The edges that point to vertices that are not within this bunch.
        /// </returns>
        /// <param name='graphElementBunchPath'>
        /// Graph element bunch path.
        /// </param>
        /// <param name='graphElementsOfFallen8'>
        /// Graph elements of Fallen-8.
        /// </param>
        private static List<EdgeSneakPeak> LoadAGraphElementBunch (string graphElementBunchPath, AGraphElement[] graphElementsOfFallen8, ConcurrentDictionary<Int32, List<EdgeOnVertexToDo>> edgeTodo)
        {
            //if there is no savepoint file... do nothing
            if (!File.Exists(graphElementBunchPath))
            {
                return null;
            }
            
            var file = File.Open(graphElementBunchPath, FileMode.Open, FileAccess.Read);
            var reader = new SerializationReader(file);
            var result = new List<EdgeSneakPeak>();
            var minimumId = reader.ReadOptimizedInt32();
            var maximumId = reader.ReadOptimizedInt32() - 1;
            var countOfElements = maximumId - minimumId;
            
            for (int i = 0; i < countOfElements; i++) 
            {
                var kind = reader.ReadOptimizedInt32();
                switch (kind) {
                
                case SerializedEdge:
                    //edge
                    break;
                    
                case SerializedVertex: 
                    //vertex
                    LoadVertex(reader, graphElementsOfFallen8, edgeTodo);
                    break;
                    
                case SerializedNull:
                    //null --> do nothing
                    break;
                }    
            }
            
            return result;
        }

        /// <summary>
        /// Saves the graph element bunch.
        /// </summary>
        /// <returns>
        /// The path to the graph element bunch
        /// </returns>
        /// <param name='range'>
        /// Range.
        /// </param>
        /// <param name='graphElements'>
        /// Graph elements.
        /// </param>
        /// <param name='pathToSavePoint'>
        /// Path to save point basis.
        /// </param>
        private static String SaveBunch (Tuple<Int32, Int32> range, List<AGraphElement> graphElements, String pathToSavePoint)
        {
            String partitionFileName = pathToSavePoint + "_graphElements_" + range.Item1 + "_to_" + range.Item2;
            
            //create file for range
            var partitionFile = File.Create(partitionFileName, Constants.BufferSize, FileOptions.SequentialScan);
            SerializationWriter partitionWriter = new SerializationWriter(partitionFile);
            
            partitionWriter.WriteOptimized(range.Item1);
            partitionWriter.WriteOptimized(range.Item2);
            
            for (int i = range.Item1; i < range.Item2; i++) 
            {
                var aGraphElement = graphElements[i];
            
                //there can be nulls
                if (aGraphElement == null) 
                {
                    partitionWriter.WriteOptimized(SerializedNull);// 2 for null
                    continue;
                }
                
                //code if it is an vertex or an edge
                if (aGraphElement is VertexModel) 
                {
                    WriteVertex((VertexModel)aGraphElement, partitionWriter);
                }
                else
                {
                    WriteEdge((EdgeModel)aGraphElement, partitionWriter);
                }
            }
            
            if (partitionWriter != null) 
            {
                partitionWriter.Flush();
                partitionWriter.Close();
            }
            
            if (partitionFile != null) 
            {
                partitionFile.Flush();
                partitionFile.Close();
            }
            
            return partitionFileName;
        }
  
        /// <summary>
        /// Loads the graph elements.
        /// </summary>
        /// <param name='graphElements'>
        /// Graph elements of Fallen-8.
        /// </param>
        /// <param name='graphElementStreams'>
        /// Graph element streams.
        /// </param>
        private static void LoadGraphElements (AGraphElement[] graphElements, List<String> graphElementStreams)
        {
            //create some futures to load as much as possible in parallel
            const TaskCreationOptions options = TaskCreationOptions.LongRunning;
            var f = new TaskFactory(CancellationToken.None, options, TaskContinuationOptions.None, TaskScheduler.Default);
            Task<List<EdgeSneakPeak>>[] tasks = (Task<List<EdgeSneakPeak>>[])Array.CreateInstance(typeof(Task<List<EdgeSneakPeak>>), graphElementStreams.Count);
            ConcurrentDictionary<Int32, List<EdgeOnVertexToDo>> edgeTodo = new ConcurrentDictionary<Int32, List<EdgeOnVertexToDo>>();
            
            for (int i = 0; i < graphElementStreams.Count; i++)
            {
                tasks[i] = f.StartNew(() => LoadAGraphElementBunch(graphElementStreams[i], graphElements, edgeTodo));
            }

            Task.WaitAll(tasks);   
        }
  
        /// <summary>
        /// Creates the partitions.
        /// </summary>
        /// <returns>
        /// The partitions.
        /// </returns>
        /// <param name='totalCount'>
        /// Total count.
        /// </param>
        /// <param name='savePartitions'>
        /// Save partitions.
        /// </param>
        private static List<Tuple<Int32, Int32>> CreatePartitions (int totalCount, int savePartitions)
        {
            var result = new List<Tuple<Int32, Int32>>();
            
            if (totalCount < savePartitions) 
            {
                for (int i = 0; i < totalCount; i++) 
                {
                    result.Add(new Tuple<Int32, Int32>(i, i + 1));
                }
                
                return result;
            }
            else 
            {
                var size = totalCount / savePartitions;
                for (int i = 0; i < savePartitions; i++) 
                {
                    var minimum = i*size;
                    
                    if (minimum < totalCount) 
                    {
                        var maximum = i*size + size;
                        if (maximum > totalCount) 
                        {
                            maximum = totalCount;    
                        }
                    
                        result.Add(new Tuple<Int32, Int32>(i * size, i * size + size));      
                    }
                }
                
                return result;
            }
        }

        /// <summary>
        /// Writes A graph element.
        /// </summary>
        /// <param name='graphElement'>
        /// Graph element.
        /// </param>
        /// <param name='writer'>
        /// Writer.
        /// </param>
        private static void WriteAGraphElement (AGraphElement graphElement, SerializationWriter writer)
        {
            writer.WriteOptimized(graphElement.Id);
            writer.WriteOptimized(graphElement.CreationDate);
            writer.WriteOptimized(graphElement.ModificationDate);
            
            List<PropertyContainer> properties = new List<PropertyContainer>(graphElement.GetAllProperties());
            writer.WriteOptimized(properties.Count);
            foreach (var aProperty in properties) 
            {
                writer.WriteOptimized(aProperty.PropertyId);
                writer.WriteObject(aProperty.Value);
            }
        }
        
        /// <summary>
        /// Loads the vertex.
        /// </summary>
        /// <param name='reader'>
        /// Reader.
        /// </param>
        /// <param name='graphElements'>
        /// Graph elements.
        /// </param>
        /// <param name='edgeTodo'>
        /// Edge todo.
        /// </param>
        private static void LoadVertex (SerializationReader reader, AGraphElement[] graphElements, ConcurrentDictionary<Int32, List<EdgeOnVertexToDo>> edgeTodo)
        {
            var id = reader.ReadOptimizedInt32();
            var creationDate = reader.ReadOptimizedDateTime();
            var modificationDate = reader.ReadOptimizedDateTime();
            
            #region properties
            
            List<PropertyContainer> properties = null;
            var propertyCount = reader.ReadOptimizedInt32();
            
            if (propertyCount > 0) 
            {
                properties = new List<PropertyContainer>(propertyCount);
                for (int i = 0; i < propertyCount; i++) 
                {
                    Int32 propertyIdentifier = reader.ReadOptimizedInt32();
                    Object propertyValue = reader.ReadObject();
                    
                    properties.Add(new PropertyContainer { PropertyId = propertyIdentifier, Value = propertyValue });   
                }
            }
            
            #endregion
            
            #region edges
            
            #region outgoing edges
            
            List<OutEdgeContainer> outEdgeProperties = null;
            var outEdgeCount = reader.ReadOptimizedInt32();
            
            if (outEdgeCount > 0) 
            {
                outEdgeProperties = new List<OutEdgeContainer>(outEdgeCount);
                for (int i = 0; i < outEdgeCount; i++) 
                {
                    var outEdgePropertyId = reader.ReadOptimizedInt32();
                    var outEdgePropertyCount = reader.ReadOptimizedInt32();
                    List<EdgeModel> outEdges = new List<EdgeModel>();
                    for (int j = 0; j < outEdgePropertyCount; j++) 
                    {
                        var edgeId = reader.ReadOptimizedInt32();
                        if (graphElements[edgeId] != null) 
                        {
                            outEdges.Add((EdgeModel)graphElements[edgeId]);
                        }
                        else 
                        {
                            var aEdgeTodo = new EdgeOnVertexToDo { VertexId = id, EdgePropertyId = outEdgePropertyId, IsIncomingEdge = false};
                            
                            edgeTodo.AddOrUpdate(
                            edgeId, 
                            new List<EdgeOnVertexToDo> 
                            { 
                                aEdgeTodo
                            },
                            (interestingId, todos) =>
                            {
                                todos.Add(aEdgeTodo);
                                return todos;
                            });
                        }
                    }
                    outEdgeProperties.Add(new OutEdgeContainer { EdgePropertyId = outEdgePropertyId, EdgeProperty = outEdges});
                }
            }
            
            #endregion
            
            #region incoming edges
            
            List<IncEdgeContainer> incEdgeProperties = null;
            var incEdgeCount = reader.ReadOptimizedInt32();
            
            if (incEdgeCount > 0) 
            {
                incEdgeProperties = new List<IncEdgeContainer>(incEdgeCount);
                for (int i = 0; i < incEdgeCount; i++) 
                {
                    var incEdgePropertyId = reader.ReadOptimizedInt32();
                    var incEdgePropertyCount = reader.ReadOptimizedInt32();
                    List<EdgeModel> incEdges = new List<EdgeModel>();
                    for (int j = 0; j < incEdgePropertyCount; j++) 
                    {
                        var edgeId = reader.ReadOptimizedInt32();
                        if (graphElements[edgeId] != null) 
                        {
                            incEdges.Add((EdgeModel)graphElements[edgeId]);
                        }
                        else 
                        {
                            var aEdgeTodo = new EdgeOnVertexToDo { VertexId = id, EdgePropertyId = incEdgePropertyId, IsIncomingEdge = true};
                            
                            edgeTodo.AddOrUpdate(
                            edgeId, 
                            new List<EdgeOnVertexToDo> 
                            { 
                                aEdgeTodo
                            },
                            (interestingId, todos) =>
                            {
                                todos.Add(aEdgeTodo);
                                return todos;
                            });
                        }
                    }
                    incEdgeProperties.Add(new IncEdgeContainer { EdgePropertyId = incEdgePropertyId, IncomingEdges = incEdges});
                }
            }
            
            #endregion
            
            #endregion
            
            graphElements[id] = new VertexModel(id, creationDate, modificationDate, properties, outEdgeProperties, incEdgeProperties);
        }
        
        /// <summary>
        /// Writes the vertex.
        /// </summary>
        /// <param name='vertex'>
        /// Vertex.
        /// </param>
        /// <param name='writer'>
        /// Writer.
        /// </param>
        private static void WriteVertex (VertexModel vertex, SerializationWriter writer)
        {
            writer.WriteOptimized(SerializedVertex);
            WriteAGraphElement(vertex, writer);
            
            #region edges
            
            var outgoingEdges = vertex.GetOutgoingEdges();
            if(outgoingEdges == null)
            {
                writer.WriteOptimized(0);
            }
            else 
            {
                writer.WriteOptimized(outgoingEdges.Count);
                foreach(var aOutEdgeProperty in outgoingEdges)
                {
                    writer.WriteOptimized(aOutEdgeProperty.EdgePropertyId);
                    writer.WriteOptimized(aOutEdgeProperty.EdgeProperty.Count);
                    foreach(var aOutEdge in aOutEdgeProperty.EdgeProperty)
                    {
                        writer.WriteOptimized(aOutEdge.Id);
                    }
                }
            }
            
            var incomingEdges = vertex.GetIncomingEdges();
            if(incomingEdges == null)
            {
                writer.WriteOptimized(0);
            }
            else 
            {
                writer.WriteOptimized(incomingEdges.Count);
                foreach(var aIncEdgeProperty in incomingEdges)
                {
                    writer.WriteOptimized(aIncEdgeProperty.EdgePropertyId);
                    writer.WriteOptimized(aIncEdgeProperty.IncomingEdges.Count);
                    foreach(var aIncEdge in aIncEdgeProperty.IncomingEdges)
                    {
                        writer.WriteOptimized(aIncEdge.Id);
                    }
                }
            }
            
            #endregion
        }
  
        /// <summary>
        /// Writes the edge.
        /// </summary>
        /// <param name='edge'>
        /// Edge.
        /// </param>
        /// <param name='writer'>
        /// Writer.
        /// </param>
        private static void WriteEdge (EdgeModel edge, SerializationWriter writer)
        {
            writer.WriteOptimized(SerializedEdge);
            WriteAGraphElement(edge, writer);
            writer.WriteOptimized(edge.SourceVertex.Id);
            writer.WriteOptimized(edge.TargetVertex.Id);
        }
        
        #endregion
    }
}

