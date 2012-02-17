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
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        /// <param name="fallen8">Fallen-8</param>
        public static void Load (string pathToSavePoint, ref int currentIdOfFallen8, ref List<AGraphElement> graphElementsOfFallen8, ref IFallen8IndexFactory indexFactoryOfFallen8, Fallen8 fallen8)
        {
            //if there is no savepoint file... do nothing
            if (!File.Exists(pathToSavePoint))
            {
                return;
            }

            using (var file = File.Open(pathToSavePoint, FileMode.Open, FileAccess.Read))
            {
                var reader = new SerializationReader(file);

                //get the maximum id
                currentIdOfFallen8 = reader.ReadOptimizedInt32();

                var numberOfGraphElements = currentIdOfFallen8 + 1;
                
                #region graph elements

                //initialize the list of graph elements
                var graphElements = new AGraphElement[numberOfGraphElements];
                var graphElementStreams = new List<String>();
                var numberOfGraphElemementStreams = reader.ReadOptimizedInt32();
                for (var i = 0; i < numberOfGraphElemementStreams; i++)
                {
                    graphElementStreams.Add(reader.ReadOptimizedString());
                }

                LoadGraphElements(graphElements, graphElementStreams);
                graphElementsOfFallen8 = new List<AGraphElement>(graphElements);//we'll see how much memory this action takes

                #endregion

                #region indexe

                var indexStreams = new List<String>();
                var numberOfIndexStreams = reader.ReadOptimizedInt32();
                for (var i = 0; i < numberOfIndexStreams; i++)
                {
                    indexStreams.Add(reader.ReadOptimizedString());
                }
                var newIndexFactory = new Fallen8IndexFactory();
                LoadIndices(fallen8, newIndexFactory, indexStreams);
                indexFactoryOfFallen8 = newIndexFactory;
                #endregion
            }
        }

        /// <summary>
        /// Save the specified graphElements, indices and pathToSavePoint.
        /// </summary>
        /// <param name="currentId">The current graph element id</param>
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
                path = path + DateTime.Now.ToBinary().ToString(CultureInfo.InvariantCulture);
            }

            using (var file = File.Create(path, Constants.BufferSize, FileOptions.SequentialScan))
            {
                var writer = new SerializationWriter(file, true);

                //create some futures to save as much as possible in parallel
                const TaskCreationOptions options = TaskCreationOptions.LongRunning;
                var f = new TaskFactory(CancellationToken.None, options, TaskContinuationOptions.None, TaskScheduler.Default);

                //the maximum id
                writer.WriteOptimized(currentId);

                #region graph elements

                var graphElementPartitions = CreatePartitions(graphElements.Count, savePartitions);
                var graphElementSaver = (Task<String>[])Array.CreateInstance(typeof(Task<String>), graphElementPartitions.Count);

                for (int i = 0; i < graphElementPartitions.Count; i++)
                {
                    var partition = graphElementPartitions[i];
                    graphElementSaver[i] = f.StartNew(() => SaveBunch(partition, graphElements, path));
                }

                #endregion

                #region indices

                var indexSaver = (Task<String>[])Array.CreateInstance(typeof(Task<String>), indices.Count);

                var counter = 0;
                foreach (var aIndex in indices)
                {
                    var indexName = aIndex.Key;
                    var index = aIndex.Value;

                    indexSaver[counter] = f.StartNew(() => SaveIndex(indexName, index, path));
                    counter++;
                }

                #endregion

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

                writer.UpdateHeader();
                writer.Flush();
                file.Flush();
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
            var indexFileName = path + "_index_" + indexName;

            using (var indexFile = File.Create(indexFileName, Constants.BufferSize, FileOptions.SequentialScan))
            {
                var indexWriter = new SerializationWriter(indexFile);

                indexWriter.WriteOptimized(indexName);
                indexWriter.WriteOptimized(index.PluginName);
                index.Save(indexWriter);

                indexWriter.UpdateHeader();
                indexWriter.Flush();
                indexFile.Flush();
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
        /// <param name="edgeTodoOnVertex">The edges that have to be added to this vertex</param>
        private static List<EdgeSneakPeak> LoadAGraphElementBunch (
            string graphElementBunchPath, 
            AGraphElement[] graphElementsOfFallen8,
            ConcurrentDictionary<Int32, List<EdgeOnVertexToDo>> edgeTodoOnVertex)
        {
            //if there is no savepoint file... do nothing
            if (!File.Exists(graphElementBunchPath))
            {
                return null;
            }

            var result = new List<EdgeSneakPeak>();

            using (var file = File.Open(graphElementBunchPath, FileMode.Open, FileAccess.Read))
            {
                var reader = new SerializationReader(file);
                var minimumId = reader.ReadOptimizedInt32();
                var maximumId = reader.ReadOptimizedInt32();
                var countOfElements = maximumId - minimumId;

                for (var i = 0; i < countOfElements; i++)
                {
                    var kind = reader.ReadOptimizedInt32();
                    switch (kind)
                    {

                        case SerializedEdge:
                            //edge
                            LoadEdge(reader, graphElementsOfFallen8, result);
                            break;

                        case SerializedVertex:
                            //vertex
                            LoadVertex(reader, graphElementsOfFallen8, edgeTodoOnVertex);
                            break;

                        case SerializedNull:
                            //null --> do nothing
                            break;
                    }
                }
            }

            return result;
        }
  
        
        private static void LoadIndices (Fallen8 fallen8, Fallen8IndexFactory indexFactory, List<String> indexStreams)
        {
            //create some futures to load as much as possible in parallel
            const TaskCreationOptions options = TaskCreationOptions.LongRunning;
            var f = new TaskFactory(CancellationToken.None, options, TaskContinuationOptions.None, TaskScheduler.Default);
            var tasks = (Task[])Array.CreateInstance(typeof(Task), indexStreams.Count);
            
            //load the indices
            for (var i = 0; i < indexStreams.Count; i++)
            {
                var indexStreamLocation = indexStreams[i];
                tasks[i] = f.StartNew(() => LoadAnIndex(indexStreamLocation, fallen8, indexFactory));
            }
        }

        private static void LoadAnIndex (string indexLocaion, Fallen8 fallen8, Fallen8IndexFactory indexFactory)
        {
            //if there is no savepoint file... do nothing
            if (!File.Exists(indexLocaion))
            {
                return;
            }

            using (var file = File.Open(indexLocaion, FileMode.Open, FileAccess.Read))
            {
                var reader = new SerializationReader(file);

                var indexName = reader.ReadOptimizedString();
                var indexPluginName = reader.ReadOptimizedString();

                indexFactory.OpenIndex(indexName, indexPluginName, reader, fallen8);
            }
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
            var partitionFileName = pathToSavePoint + "_graphElements_" + range.Item1 + "_to_" + range.Item2;

            using (var partitionFile = File.Create(partitionFileName, Constants.BufferSize, FileOptions.SequentialScan))
            {
                var partitionWriter = new SerializationWriter(partitionFile);

                partitionWriter.WriteOptimized(range.Item1);
                partitionWriter.WriteOptimized(range.Item2);

                for (var i = range.Item1; i < range.Item2; i++)
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

                partitionWriter.UpdateHeader();
                partitionWriter.Flush();
                partitionFile.Flush();
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
        private static void LoadGraphElements(AGraphElement[] graphElements, List<String> graphElementStreams)
        {
            //create some futures to load as much as possible in parallel
            const TaskCreationOptions options = TaskCreationOptions.LongRunning;
            var f = new TaskFactory(CancellationToken.None, options, TaskContinuationOptions.None, TaskScheduler.Default);
            var tasks = (Task<List<EdgeSneakPeak>>[])Array.CreateInstance(typeof(Task<List<EdgeSneakPeak>>), graphElementStreams.Count);
            var edgeTodo = new ConcurrentDictionary<Int32, List<EdgeOnVertexToDo>>();

            //create the major part of the graph elements
            for (var i = 0; i < graphElementStreams.Count; i++)
            {
                var streamLocation = graphElementStreams[i];
                tasks[i] = f.StartNew(() => LoadAGraphElementBunch(streamLocation, graphElements, edgeTodo));
            }

            var continuationTask = f.ContinueWhenAll<List<EdgeSneakPeak>>(tasks,
                finishedTasks => Parallel.ForEach(finishedTasks, 
                    aFinishedTask =>
                        {
                            foreach (var aSneakPeak in aFinishedTask.Result)
                            {
                                graphElements[aSneakPeak.Id] = new EdgeModel(
                                    aSneakPeak.Id,
                                    aSneakPeak.CreationDate,
                                    aSneakPeak.ModificationDate,
                                    (VertexModel)graphElements[aSneakPeak.TargetVertexId],
                                    (VertexModel)graphElements[aSneakPeak.SourceVertexId],
                                    aSneakPeak.Properties);
                            }
                        })).ContinueWith(
                        task =>
                            {
                                //add the remaining edges to vertices
                                Parallel.ForEach(edgeTodo, aKV =>
                                {
                                    var edge = (EdgeModel)graphElements[aKV.Key];
                                    foreach (var aTodo in aKV.Value)
                                    {
                                        var interestingVertex = (VertexModel)graphElements[aTodo.VertexId];
                                        if (aTodo.IsIncomingEdge)
                                        {
                                            interestingVertex.AddIncomingEdge(aTodo.EdgePropertyId, edge);
                                        }
                                        else
                                        {
                                            interestingVertex.AddOutEdge(aTodo.EdgePropertyId, edge);
                                        }
                                    }
                                });
                            });

            continuationTask.Wait();
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
        private static List<Tuple<Int32, Int32>> CreatePartitions(int totalCount, int savePartitions)
        {
            var result = new List<Tuple<Int32, Int32>>();

            if (totalCount < savePartitions)
            {
                for (var i = 0; i < totalCount; i++)
                {
                    result.Add(new Tuple<Int32, Int32>(i, i + 1));
                }

                return result;
            }

            var size = (totalCount/savePartitions);

            for (var i = 0; i < savePartitions; i++)
            {
                result.Add(new Tuple<int, int>(i * size, (i * size) + size));
            }

            result[savePartitions - 1] = new Tuple<int, int>(result[savePartitions - 1].Item1, totalCount);

            return result;
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
            writer.Write(graphElement.CreationDate);
            writer.Write(graphElement.ModificationDate);

            var properties = graphElement.GetAllProperties();
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
            var creationDate = reader.ReadUInt32();
            var modificationDate = reader.ReadUInt32();
            
            #region properties
            
            var propertyCount = reader.ReadOptimizedInt32();
            PropertyContainer[] properties = null;

            if (propertyCount > 0) 
            {
                properties = new PropertyContainer[propertyCount];
                for (var i = 0; i < propertyCount; i++) 
                {
                    var propertyIdentifier = reader.ReadOptimizedUInt16();
                    var propertyValue = reader.ReadObject();
                    
                    properties[i] = new PropertyContainer { PropertyId = propertyIdentifier, Value = propertyValue };   
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
                for (var i = 0; i < outEdgeCount; i++) 
                {
                    var outEdgePropertyId = reader.ReadOptimizedUInt16();
                    var outEdgePropertyCount = reader.ReadOptimizedInt32();
                    var outEdges = new List<EdgeModel>();
                    for (var j = 0; j < outEdgePropertyCount; j++) 
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
                for (var i = 0; i < incEdgeCount; i++) 
                {
                    var incEdgePropertyId = reader.ReadOptimizedUInt16();
                    var incEdgePropertyCount = reader.ReadOptimizedInt32();
                    var incEdges = new List<EdgeModel>();
                    for (var j = 0; j < incEdgePropertyCount; j++) 
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
        /// Loads the edge.
        /// </summary>
        /// <param name='reader'>
        /// Reader.
        /// </param>
        /// <param name='graphElements'>
        /// Graph elements.
        /// </param>
        /// <param name='sneakPeaks'>
        /// Sneak peaks.
        /// </param>
        private static void LoadEdge (SerializationReader reader, AGraphElement[] graphElements, List<EdgeSneakPeak> sneakPeaks)
        {
            var id = reader.ReadOptimizedInt32();
            var creationDate = reader.ReadUInt32();
            var modificationDate = reader.ReadUInt32();
            
            #region properties
            
            PropertyContainer[] properties = null;
            var propertyCount = reader.ReadOptimizedInt32();
            
            if (propertyCount > 0) 
            {
                properties = new PropertyContainer[propertyCount];
                for (var i = 0; i < propertyCount; i++) 
                {
                    var propertyIdentifier = reader.ReadOptimizedUInt16();
                    var propertyValue = reader.ReadObject();
                    
                    properties[i] = new PropertyContainer { PropertyId = propertyIdentifier, Value = propertyValue };   
                }
            }
            
            #endregion
            
            var sourceVertexId = reader.ReadOptimizedInt32();
            var targetVertexId = reader.ReadOptimizedInt32();
            
            var sourceVertex = graphElements[sourceVertexId];
            var targetVertex = graphElements[targetVertexId];
            
            if (sourceVertex != null && targetVertex != null) 
            {
                graphElements[id] = new EdgeModel(id, creationDate, modificationDate, (VertexModel)targetVertex, (VertexModel)sourceVertex, properties);
            }
            else 
            {
                sneakPeaks.Add(new EdgeSneakPeak { CreationDate = creationDate, Id = id, ModificationDate = modificationDate, Properties = properties, SourceVertexId = sourceVertexId, TargetVertexId = targetVertexId});
            }
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

