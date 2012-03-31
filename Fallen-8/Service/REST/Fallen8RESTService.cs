// 
//  Fallen8RESTService.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012 Henning Rauch
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using Fallen8.API.Plugin;
using Fallen8.API.Index;
using Fallen8.API.Algorithms.Path;
using Fallen8.API.Helper;
using System.IO;
using System.ServiceModel.Web;
using Fallen8.API.Service.REST.Ressource;
using System.Text;

namespace Fallen8.API.Service.REST
{
    /// <summary>
    /// Fallen-8 REST service.
    /// </summary>
    public sealed class Fallen8RESTService : IFallen8RESTService, IDisposable
    {
        #region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly Fallen8 _fallen8;
        
        /// <summary>
        /// The ressources.
        /// </summary>
        private Dictionary<String, MemoryStream> _ressources;

        /// <summary>
        /// The html befor the code injection
        /// </summary>
        private String _frontEndPre;

        /// <summary>
        /// The html after the code injection
        /// </summary>
        private String _frontEndPost;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the Fallen8RESTService class.
        /// </summary>
        /// <param name='fallen8'>
        /// Fallen-8.
        /// </param>
        public Fallen8RESTService(Fallen8 fallen8)
        {
            _fallen8 = fallen8;
            LoadFrontend();
        }

        #endregion
        
        #region IDisposable Members

        public void Dispose()
        {
            //do nothing atm
        }

        #endregion

        #region IFallen8RESTService implementation
        
        public int AddVertex (VertexSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion
            
            return _fallen8.CreateVertex(definition.CreationDate, GenerateProperties(definition.Properties)).Id;
        }

        public int AddEdge (EdgeSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
        
            #endregion

            return _fallen8.CreateEdge(definition.SourceVertex, definition.EdgePropertyId, definition.TargetVertex, definition.CreationDate, GenerateProperties(definition.Properties)).Id;
        }

        public Fallen8RESTProperties GetAllVertexProperties (string vertexIdentifier)
        {
            return GetGraphElementProperties (vertexIdentifier);
        }

        public Fallen8RESTProperties GetAllEdgeProperties (string edgeIdentifier)
        {
            return GetGraphElementProperties (edgeIdentifier);
        }

        public List<ushort> GetAllAvailableOutEdgesOnVertex (string vertexIdentifier)
        {
            VertexModel vertex;
            return _fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)) ? vertex.GetOutgoingEdgeIds() : null;
        }

        public List<ushort> GetAllAvailableIncEdgesOnVertex (string vertexIdentifier)
        {
            VertexModel vertex;
            return _fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)) ? vertex.GetIncomingEdgeIds() : null;
        }

        public List<int> GetOutgoingEdges (string vertexIdentifier, string edgePropertyIdentifier)
        {
            VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier))) 
            {
                ReadOnlyCollection<EdgeModel> edges;
                if (vertex.TryGetOutEdge(out edges, Convert.ToInt32(edgePropertyIdentifier))) 
                {
                    return edges.Select(_ => _.Id).ToList();
                }
            }
            return null;
        }

        public List<int> GetIncomingEdges (string vertexIdentifier, string edgePropertyIdentifier)
        {
            VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier))) 
            {
                ReadOnlyCollection<EdgeModel> edges;
                if (vertex.TryGetInEdges(out edges, Convert.ToInt32(edgePropertyIdentifier))) 
                {
                    return edges.Select(_ => _.Id).ToList();
                }
            }
            return null;
        }

        public void Trim ()
        {
            _fallen8.Trim();
        }

        public Fallen8Status Status ()
        {
            var currentProcess = Process.GetCurrentProcess();
            var totalBytesOfMemoryUsed = currentProcess.WorkingSet64;
            
            var freeMem = new PerformanceCounter("Memory", "Available Bytes");
            var freeBytesOfMemory = Convert.ToInt64(freeMem.NextValue());
            
            var vertexCount = _fallen8.GetVertices().Count;
            var edgeCount = _fallen8.GetEdges().Count;
            
            IEnumerable<String> availableIndices;
            Fallen8PluginFactory.TryGetAvailablePlugins<IIndex>(out availableIndices);
            
            IEnumerable<String> availablePathAlgos;
            Fallen8PluginFactory.TryGetAvailablePlugins<IShortestPathAlgorithm>(out availablePathAlgos);
            
            IEnumerable<String> availableServices;
            Fallen8PluginFactory.TryGetAvailablePlugins<IFallen8Service>(out availableServices);
            
            return new Fallen8Status
            {
                AvailableIndexPlugins = new List<String>(availableIndices),
                AvailablePathPlugins = new List<String>(availablePathAlgos),
                AvailableServicePlugins = new List<String>(availableServices),
                EdgeCount = edgeCount,
                VertexCount = vertexCount,
                UsedMemory = totalBytesOfMemoryUsed,
                FreeMemory = freeBytesOfMemory
            };
        }
        
        public Stream GetFrontend()
        {
            if (WebOperationContext.Current != null)
            {
                var baseUri = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri;

                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

                var sb = new StringBuilder();

                sb.Append(_frontEndPre);
                sb.Append(Environment.NewLine);
                sb.AppendLine("var baseUri = \"" + baseUri.ToString() + "\";" + Environment.NewLine);
                sb.Append(_frontEndPost);

                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }

            return new MemoryStream(Encoding.UTF8.GetBytes("Sorry, no frontend available."));
        }

        public void ReloadFrontend()
        {
            LoadFrontend();
        }

        public Stream GetFrontendRessources(String ressourceName)
        {
            MemoryStream ressourceStream;
            if (_ressources.TryGetValue(ressourceName, out ressourceStream)) 
            {
                var result = new MemoryStream();
                var buffer = new byte[32768];
                int read;
                while ((read = ressourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Write(buffer, 0, read);
                }
                ressourceStream.Position = 0;
                result.Position = 0;

                if (WebOperationContext.Current != null)
                {
                    var extension = ressourceName.Split('.').Last();

                    switch (extension)
                    {
                        case "html":
                        case "htm":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                            break;
                        case "png":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
                            break;
                        case "css":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "text/css";
                            break;
                        case "gif":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "image/gif";
                            break;
                        case "ico":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "image/ico";
                            break;
                        case "swf":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-shockwave-flash";
                            break;
                        case "js":
                            WebOperationContext.Current.OutgoingResponse.ContentType = "text/javascript";
                            break;
                        default:
                            throw new ApplicationException(String.Format("File type {0} not supported", extension));
                    }
                }

                return result;
            }
            
            return null;
        }

        public IEnumerable<int> GraphScan(String propertyIdString, ScanSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            var propertyId = Convert.ToUInt16(propertyIdString);

            var value = (IComparable)Convert.ChangeType(definition.Literal.Value,
                                           Type.GetType(definition.Literal.FullQualifiedTypeName, true, true));

            List<AGraphElement> graphElements;
            return _fallen8.GraphScan(out graphElements, propertyId, value, definition.Operator) ? CreateResult(graphElements, definition.ResultType) : Enumerable.Empty<Int32>();
        }

        public IEnumerable<int> IndexScan(string indexId, ScanSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            var value = (IComparable)Convert.ChangeType(definition.Literal.Value,
                                          Type.GetType(definition.Literal.FullQualifiedTypeName, true, true));

            ReadOnlyCollection<AGraphElement> graphElements;
            return _fallen8.IndexScan(out graphElements, indexId, value, definition.Operator) ? CreateResult(graphElements, definition.ResultType) : Enumerable.Empty<Int32>();
        }

        public IEnumerable<int> RangeIndexScan(string indexId, RangeScanSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            var left = (IComparable)Convert.ChangeType(definition.LeftLimit,
                                          Type.GetType(definition.FullQualifiedTypeName, true, true));

            var right = (IComparable)Convert.ChangeType(definition.RightLimit,
                                          Type.GetType(definition.FullQualifiedTypeName, true, true));

            ReadOnlyCollection<AGraphElement> graphElements;
            return _fallen8.RangeIndexScan(out graphElements, indexId, left, right, definition.IncludeLeft, definition.IncludeRight) ? CreateResult(graphElements, definition.ResultType) : Enumerable.Empty<Int32>();            
        }

        #endregion
        
        #region private helper

        /// <summary>
        /// Creats the result
        /// </summary>
        /// <param name="graphElements">The graph elements</param>
        /// <param name="resultTypeSpecification">The result specification</param>
        /// <returns></returns>
        private static IEnumerable<int> CreateResult(IEnumerable<AGraphElement> graphElements, ResultTypeSpecification resultTypeSpecification)
        {
            switch (resultTypeSpecification)
            {
                case ResultTypeSpecification.Vertices:
                    return graphElements.OfType<VertexModel>().Select(_ => _.Id);

                case ResultTypeSpecification.Edges:
                    return graphElements.OfType<EdgeModel>().Select(_ => _.Id);

                case ResultTypeSpecification.Both:
                    return graphElements.Select(_ => _.Id);

                default:
                    throw new ArgumentOutOfRangeException("resultTypeSpecification");
            }
        }

        /// <summary>
        /// Load the frontend
        /// </summary>
        private void LoadFrontend()
        {
            if (_ressources != null)
            {
                foreach (var memoryStream in _ressources)
                {
                    memoryStream.Value.Dispose();
                }
            }

            _ressources = FindRessources();
            _frontEndPre = Frontend.Pre;
            _frontEndPost = Frontend.Post;
        }

        /// <summary>
        /// Find all ressources
        /// </summary>
        /// <returns>Ressources</returns>
        private static Dictionary<string, MemoryStream> FindRessources()
        {
            var ressourceDirectory = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "Service" +
                                     System.IO.Path.DirectorySeparatorChar + "REST" +
                                     System.IO.Path.DirectorySeparatorChar + "Ressource" +
                                     System.IO.Path.DirectorySeparatorChar;

            return Directory.EnumerateFiles(ressourceDirectory)
                .ToDictionary(
                    key => key.Split(System.IO.Path.DirectorySeparatorChar).Last(),
                    CreateMemoryStreamFromFile);
        }

        /// <summary>
        /// Creates a memory stream from a file
        /// </summary>
        /// <param name="value">The path of the file</param>
        /// <returns>MemoryStream</returns>
        private static MemoryStream CreateMemoryStreamFromFile(string value)
        {
            MemoryStream result;

            using (var file = File.OpenRead(value))
            {
                var reader = new BinaryReader(file);
                result = new MemoryStream(reader.ReadBytes((Int32)file.Length));
            }

            return result;
        }

        /// <summary>
        /// Generates the properties.
        /// </summary>
        /// <returns>
        /// The properties.
        /// </returns>
        /// <param name='propertySpecification'>
        /// Property specification.
        /// </param>
        private static PropertyContainer[] GenerateProperties (Dictionary<UInt16, PropertySpecification> propertySpecification)
        {
            PropertyContainer[] properties = null;
            
            if (propertySpecification != null)
            {
                var propCounter = 0;
                properties = new PropertyContainer[propertySpecification.Count];
                
                foreach (var aPropertyDefinition in propertySpecification)
                {
                    properties[propCounter] = new PropertyContainer 
                    { 
                        PropertyId = aPropertyDefinition.Key, 
                        Value = Convert.ChangeType(aPropertyDefinition.Value.Property, Type.GetType(aPropertyDefinition.Value.TypeName, true, true)) 
                    };
                    propCounter++;
                }
            }
        
            return properties;
        }
        
        /// <summary>
        /// Gets the graph element properties.
        /// </summary>
        /// <returns>
        /// The graph element properties.
        /// </returns>
        /// <param name='vertexIdentifier'>
        /// Vertex identifier.
        /// </param>
        private Fallen8RESTProperties GetGraphElementProperties (string vertexIdentifier)
        {
            AGraphElement vertex;
            if (_fallen8.TryGetGraphElement(out vertex, Convert.ToInt32(vertexIdentifier))) 
            {
                return new Fallen8RESTProperties 
                {
                    Id = vertex.Id,
                    CreationDate = Constants.GetDateTimeFromUnixTimeStamp(vertex.CreationDate),
                    ModificationDate = Constants.GetDateTimeFromUnixTimeStamp(vertex.CreationDate + vertex.ModificationDate),
                    Properties = vertex.GetAllProperties().ToDictionary(key => key.PropertyId, value => value.Value.ToString())
                };
            }
            
            return null;
        }
        
        #endregion
    }
}

