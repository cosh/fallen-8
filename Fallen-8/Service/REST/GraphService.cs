// 
//  GraphService.cs
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

#region Usings

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using Microsoft.CSharp;
using NoSQL.GraphDB.Algorithms.Path;
using NoSQL.GraphDB.Helper;
using NoSQL.GraphDB.Index;
using NoSQL.GraphDB.Index.Fulltext;
using NoSQL.GraphDB.Index.Spatial;
using NoSQL.GraphDB.Log;
using NoSQL.GraphDB.Model;
using NoSQL.GraphDB.Service.REST.Result;
using NoSQL.GraphDB.Service.REST.Specification;
using System.Text;

#endregion

namespace NoSQL.GraphDB.Service.REST
{
    /// <summary>
    ///   Fallen-8 REST service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public sealed class GraphService : IGraphService, IDisposable
    {
        #region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly Fallen8 _fallen8;

        #region Code generation stuff

        private readonly CSharpCodeProvider _codeProvider;
        private readonly CompilerParameters _compilerParameters;

        private const String PathDelegateClassName = "NoSQL.GraphDB.Algorithms.Path.PathDelegateProvider";
        private readonly String _pathDelegateClassPrefix = CodeGeneration.Prefix;
        private readonly String _pathDelegateClassSuffix = CodeGeneration.Suffix;
        private const String VertexFilterMethodName = "VertexFilter";
        private const String EdgeFilterMethodName = "EdgeFilter";
        private const String EdgePropertyFilterMethodName = "EdgePropertyFilter";
        private const String VertexCostMethodName = "VertexCost";
        private const String EdgeCostMethodName = "EdgeCost";

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the GraphService class.
        /// </summary>
        /// <param name='fallen8'> Fallen-8. </param>
        public GraphService(Fallen8 fallen8)
        {
            _fallen8 = fallen8;

            _compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/optimize /target:library",
            };

            var curAss = Assembly.GetAssembly(fallen8.GetType());
            _compilerParameters.ReferencedAssemblies.Add("System.dll");
            _compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
            _compilerParameters.ReferencedAssemblies.Add("System.dll");
            _compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            _compilerParameters.ReferencedAssemblies.Add(curAss.Location);

            _codeProvider = new CSharpCodeProvider(new Dictionary<string, string>
                                                      {
                                                          { "CompilerVersion", "v4.0" }
                                                      });
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //do nothing atm
        }

        #endregion

        #region IGraphService implementation

        public int AddVertex(VertexSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            return _fallen8.CreateVertex(definition.CreationDate, GenerateProperties(definition.Properties)).Id;
        }

        public int AddEdge(EdgeSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            return
                _fallen8.CreateEdge(definition.SourceVertex, definition.EdgePropertyId, definition.TargetVertex,
                                    definition.CreationDate, GenerateProperties(definition.Properties)).Id;
        }

        /// <summary>
        ///   Gets the graph element properties.
        /// </summary>
        /// <returns> The graph element properties. </returns>
        /// <param name='graphElementIdentifier'> Vertex identifier. </param>
        public PropertiesREST GetAllGraphelementProperties(string graphElementIdentifier)
        {
            AGraphElement vertex;
            if (_fallen8.TryGetGraphElement(out vertex, Convert.ToInt32(graphElementIdentifier)))
            {
                return new PropertiesREST
                {
                    Id = vertex.Id,
                    CreationDate = DateHelper.GetDateTimeFromUnixTimeStamp(vertex.CreationDate),
                    ModificationDate = DateHelper.GetDateTimeFromUnixTimeStamp(vertex.CreationDate + vertex.ModificationDate),
                    Properties = vertex.GetAllProperties().ToDictionary(key => key.PropertyId,
                                                               value => value.Value.ToString())
                };
            }

            return null;
        }

        public int GetSourceVertexForEdge(string edgeIdentifier)
        {
            EdgeModel edge;
            if (_fallen8.TryGetEdge(out edge, Convert.ToInt32(edgeIdentifier)))
            {
                return edge.SourceVertex.Id;
            }

            throw new WebException(String.Format("Could not find edge with id {0}.", edgeIdentifier));
        }

        public int GetTargetVertexForEdge(string edgeIdentifier)
        {
            EdgeModel edge;
            if (_fallen8.TryGetEdge(out edge, Convert.ToInt32(edgeIdentifier)))
            {
                return edge.TargetVertex.Id;
            }

            throw new WebException(String.Format("Could not find edge with id {0}.", edgeIdentifier));
        }

        public List<ushort> GetAllAvailableOutEdgesOnVertex(string vertexIdentifier)
        {
            VertexModel vertex;
            return _fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier))
                       ? vertex.GetOutgoingEdgeIds()
                       : null;
        }

        public List<ushort> GetAllAvailableIncEdgesOnVertex(string vertexIdentifier)
        {
            VertexModel vertex;
            return _fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier))
                       ? vertex.GetIncomingEdgeIds()
                       : null;
        }

        public List<int> GetOutgoingEdges(string vertexIdentifier, string edgePropertyIdentifier)
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

        public List<int> GetIncomingEdges(string vertexIdentifier, string edgePropertyIdentifier)
        {
            VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)))
            {
                ReadOnlyCollection<EdgeModel> edges;
                if (vertex.TryGetInEdge(out edges, Convert.ToInt32(edgePropertyIdentifier)))
                {
                    return edges.Select(_ => _.Id).ToList();
                }
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

            var value = (IComparable) Convert.ChangeType(definition.Literal.Value,
                                                         Type.GetType(definition.Literal.FullQualifiedTypeName, true,
                                                                      true));

            List<AGraphElement> graphElements;
            return _fallen8.GraphScan(out graphElements, propertyId, value, definition.Operator)
                       ? CreateResult(graphElements, definition.ResultType)
                       : Enumerable.Empty<Int32>();
        }

        public IEnumerable<int> IndexScan(IndexScanSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            var value = (IComparable) Convert.ChangeType(definition.Literal.Value,
                                                         Type.GetType(definition.Literal.FullQualifiedTypeName, true,
                                                                      true));

            ReadOnlyCollection<AGraphElement> graphElements;
            return _fallen8.IndexScan(out graphElements, definition.IndexId, value, definition.Operator)
                       ? CreateResult(graphElements, definition.ResultType)
                       : Enumerable.Empty<Int32>();
        }

        public IEnumerable<int> RangeIndexScan(RangeIndexScanSpecification definition)
        {
            #region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

            var left = (IComparable) Convert.ChangeType(definition.LeftLimit,
                                                        Type.GetType(definition.FullQualifiedTypeName, true, true));

            var right = (IComparable) Convert.ChangeType(definition.RightLimit,
                                                         Type.GetType(definition.FullQualifiedTypeName, true, true));

            ReadOnlyCollection<AGraphElement> graphElements;
            return _fallen8.RangeIndexScan(out graphElements, definition.IndexId, left, right, definition.IncludeLeft,
                                           definition.IncludeRight)
                       ? CreateResult(graphElements, definition.ResultType)
                       : Enumerable.Empty<Int32>();
        }

		public FulltextSearchResultREST FulltextIndexScan (FulltextIndexScanSpecification definition)
		{
			#region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

			FulltextSearchResult result;
            return _fallen8.FulltextIndexScan(out result, definition.IndexId, definition.RequestString)
                       ? new FulltextSearchResultREST(result)
                       : null;
		}

		public IEnumerable<int> SpatialIndexScanSearchDistance (SearchDistanceSpecification definition)
		{
			#region initial checks

            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            #endregion

			AGraphElement graphElement;
			if (_fallen8.TryGetGraphElement(out graphElement, definition.GraphElementId)) 
			{
				IIndex idx;
				if (_fallen8.IndexFactory.TryGetIndex(out idx, definition.IndexId)) 
				{
					var spatialIndex = idx as ISpatialIndex;
					if (spatialIndex != null) 
					{
						ReadOnlyCollection<AGraphElement> result;
						return spatialIndex.SearchDistance(out result, definition.Distance, graphElement)
							? result.Select(_ => _.Id)
							: null;
					}
                    Logger.LogError(string.Format("The index with id {0} is no spatial index.", definition.IndexId));
					return null;
				}
                Logger.LogError(string.Format("Could not find index {0}.", definition.IndexId));
				return null;
			}
			Logger.LogError(string.Format("Could not find graph element {0}.", definition.GraphElementId));
			return null;
		}

		public bool TryAddProperty (string graphElementIdString, string propertyIdString, PropertySpecification definition)
		{
			var graphElementId = Convert.ToInt32(graphElementIdString);
			var propertyId = Convert.ToUInt16(propertyIdString);

			var property = Convert.ChangeType(
				definition.Property, 
				Type.GetType(definition.FullQualifiedTypeName, true, true));

			return _fallen8.TryAddProperty(graphElementId, propertyId, property);
		}

		public bool TryRemoveProperty (string graphElementIdString, string propertyIdString)
		{
			var graphElementId = Convert.ToInt32(graphElementIdString);
			var propertyId = Convert.ToUInt16(propertyIdString);

			return _fallen8.TryRemoveProperty(graphElementId, propertyId);
		}

		public bool TryRemoveGraphElement (string graphElementIdString)
		{
			var graphElementId = Convert.ToInt32(graphElementIdString);

			return _fallen8.TryRemoveGraphElement(graphElementId);
		}
		
		public uint GetInDegree (string vertexIdentifier)
		{
			VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)))
            {
				return vertex.GetInDegree();
            }
            return 0;
		}

		public uint GetOutDegree (string vertexIdentifier)
		{
			VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)))
            {
				return vertex.GetOutDegree();
            }
            return 0;
		}

		public uint GetInEdgeDegree (string vertexIdentifier, string edgePropertyIdentifier)
		{
			VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)))
            {
                ReadOnlyCollection<EdgeModel> edges;
                if (vertex.TryGetInEdge(out edges, Convert.ToInt32(edgePropertyIdentifier)))
                {
                    return Convert.ToUInt32(edges.Count);
                }
            }
            return 0;
		}

		public uint GetOutEdgeDegree (string vertexIdentifier, string edgePropertyIdentifier)
		{
			VertexModel vertex;
            if (_fallen8.TryGetVertex(out vertex, Convert.ToInt32(vertexIdentifier)))
            {
                ReadOnlyCollection<EdgeModel> edges;
                if (vertex.TryGetOutEdge(out edges, Convert.ToInt32(edgePropertyIdentifier)))
                {
                    return Convert.ToUInt32(edges.Count);
                }
            }
            return 0;
		}

        public List<PathREST> GetPaths(string from, string to, PathSpecification definition)
        {
            if (definition != null)
            {
                var fromId = Convert.ToInt32(from);
                var toId = Convert.ToInt32(to);

                var results = _codeProvider.CompileAssemblyFromSource(_compilerParameters, new[] { CreateSource(definition) });

                if (results.Errors.HasErrors)
                {
                    throw new Exception(CreateErrorMessage(results.Errors));
                }

                var type = results.CompiledAssembly.GetType(PathDelegateClassName);

                var edgeCostDelegate = CreateEdgeCostDelegate(definition.Cost, type);
                var vertexCostDelegate = CreateVertexCostDelegate(definition.Cost, type);

                var edgePropertyFilterDelegate = CreateEdgePropertyFilterDelegate(definition.Filter, type);
                var vertexFilterDelegate = CreateVertexFilterDelegate(definition.Filter, type);
                var edgeFilterDelegate = CreateEdgeFilterDelegate(definition.Filter, type);
                

                List<Path> paths;
                if (_fallen8.CalculateShortestPath(
                    out paths,
                    definition.PathAlgorithmName,
                    fromId,
                    toId,
                    definition.MaxDepth,
                    definition.MaxPathWeight,
                    definition.MaxResults,
                    edgePropertyFilterDelegate,
                    vertexFilterDelegate,
                    edgeFilterDelegate,
                    edgeCostDelegate,
                    vertexCostDelegate))
                {
                    if (paths != null)
                    {
                        return new List<PathREST>(paths.Select(aPath => new PathREST(aPath)));
                    }
                }
            }
            return null;
        }

		public List<PathREST> GetPathsByVertex (string from, string to, PathSpecification definition)
		{
			return GetPaths(from, to, definition);
		}

		public bool CreateIndex (PluginSpecification definition)
		{
			IIndex result;
			return _fallen8.IndexFactory.TryCreateIndex(out result, definition.UniqueId, definition.PluginType, ServiceHelper.CreatePluginOptions(definition.PluginOptions));
		}

		public bool AddToIndex (IndexAddToSpecification definition)
		{
			IIndex idx;
			if (_fallen8.IndexFactory.TryGetIndex(out idx, definition.IndexId)) 
			{
				AGraphElement graphElement;
				if (_fallen8.TryGetGraphElement(out graphElement, definition.GraphElementId)) 
				{
                    idx.AddOrUpdate(ServiceHelper.CreateObject(definition.Key), graphElement);
					return true; 
				}

				Logger.LogError(String.Format("Could not find graph element {0}.", definition.GraphElementId));
				return false;
			}
			Logger.LogError(String.Format("Could not find index {0}.", definition.IndexId));
			return false;
		}

		public bool RemoveKeyFromIndex (IndexRemoveKeyFromIndexSpecification definition)
		{
			IIndex idx;
			if (_fallen8.IndexFactory.TryGetIndex(out idx, definition.IndexId)) 
			{
                return idx.TryRemoveKey(ServiceHelper.CreateObject(definition.Key));
			}
			Logger.LogError(String.Format("Could not find index {0}.", definition.IndexId));
			return false;
		}

		public bool RemoveGraphElementFromIndex (IndexRemoveGraphelementFromIndexSpecification definition)
		{
			IIndex idx;
			if (_fallen8.IndexFactory.TryGetIndex(out idx, definition.IndexId)) 
			{
				AGraphElement graphElement;
				if (_fallen8.TryGetGraphElement(out graphElement, definition.GraphElementId)) 
				{
					idx.RemoveValue(graphElement);
					return true; 
				}

				Logger.LogError(String.Format("Could not find graph element {0}.", definition.GraphElementId));
				return false;
			}
			Logger.LogError(String.Format("Could not find index {0}.", definition.IndexId));
			return false;
		}

		public bool DeleteIndex (IndexDeleteSpecificaton definition)
		{
			return _fallen8.IndexFactory.TryDeleteIndex(definition.IndexId);
		}

		#endregion

        #region private helper

        /// <summary>
        /// Create the source for the code generation
        /// </summary>
        /// <param name="definition">The path specification</param>
        /// <returns>The source code</returns>
        private string CreateSource(PathSpecification definition)
        {
            if (definition == null) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine(_pathDelegateClassPrefix);

            if (definition.Cost != null)
            {
                if (definition.Cost.Edge != null)
                {
                    sb.AppendLine(definition.Cost.Edge);
                }

                if (definition.Cost.Vertex != null)
                {
                    sb.AppendLine(definition.Cost.Vertex);
                }
            }

            if (definition.Filter != null)
            {
                if (definition.Filter.Edge != null)
                {
                    sb.AppendLine(definition.Filter.Edge);
                }

                if (definition.Filter.EdgeProperty != null)
                {
                    sb.AppendLine(definition.Filter.EdgeProperty);
                }

                if (definition.Filter.Vertex != null)
                {
                    sb.AppendLine(definition.Filter.Vertex);
                }
            }

            sb.AppendLine(_pathDelegateClassSuffix);

            return sb.ToString();
        }

        /// <summary>
        /// Creates an error message.
        /// </summary>
        /// <param name="compilerErrorCollection">The compiler error collection</param>
        /// <returns>The error string</returns>
        private static string CreateErrorMessage(CompilerErrorCollection compilerErrorCollection)
        {
            if (compilerErrorCollection == null) throw new ArgumentNullException("compilerErrorCollection");
            
            var sb = new StringBuilder();

            sb.AppendLine("Error building request");
            foreach (var aError in compilerErrorCollection)
            {
                sb.AppendLine(aError.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates an edge filter delegate
        /// </summary>
        /// <param name="pathFilterSpecification">Filter specification.</param>
        /// <param name="pathDelegateType">The path delegate type  </param>
        /// <returns>The delegate</returns>
        private static PathDelegates.EdgeFilter CreateEdgeFilterDelegate(PathFilterSpecification pathFilterSpecification, Type pathDelegateType)
        {
            if (pathFilterSpecification != null && !String.IsNullOrEmpty(pathFilterSpecification.Edge))
            {
                var method = pathDelegateType.GetMethod(EdgeFilterMethodName);

                return (PathDelegates.EdgeFilter)Delegate.CreateDelegate(typeof(PathDelegates.EdgeFilter), method);
            }
            return null;
        }

        /// <summary>
        /// Creates a vertex filter delegate
        /// </summary>
        /// <param name="pathFilterSpecification">Filter specification.</param>
        /// <param name="pathDelegateType">The path delegate type </param>
        /// <returns>The delegate</returns>
        private static PathDelegates.VertexFilter CreateVertexFilterDelegate(PathFilterSpecification pathFilterSpecification, Type pathDelegateType)
        {
            if (pathFilterSpecification != null && !String.IsNullOrEmpty(pathFilterSpecification.Vertex))
            {
                var method = pathDelegateType.GetMethod(VertexFilterMethodName);

                return (PathDelegates.VertexFilter)Delegate.CreateDelegate(typeof(PathDelegates.VertexFilter), method);
            }
            return null;
        }

        /// <summary>
        /// Creates an edge property filter delegate
        /// </summary>
        /// <param name="pathFilterSpecification">Filter specification.</param>
        /// <param name="pathDelegateType">The path delegate type </param>
        /// <returns>The delegate</returns>
        private static PathDelegates.EdgePropertyFilter CreateEdgePropertyFilterDelegate(PathFilterSpecification pathFilterSpecification, Type pathDelegateType)
        {
            if (pathFilterSpecification != null && !String.IsNullOrEmpty(pathFilterSpecification.EdgeProperty))
            {
                var method = pathDelegateType.GetMethod(EdgePropertyFilterMethodName);

                return (PathDelegates.EdgePropertyFilter)Delegate.CreateDelegate(typeof(PathDelegates.EdgePropertyFilter), method);
            }
            return null;
        }

        /// <summary>
        /// Creates a vertex cost delegate
        /// </summary>
        /// <param name="pathCostSpecification">Cost specificateion</param>
        /// <param name="pathDelegateType">The path delegate type </param>
        /// <returns>The delegate</returns>
        private static PathDelegates.VertexCost CreateVertexCostDelegate(PathCostSpecification pathCostSpecification, Type pathDelegateType)
        {
            if (pathCostSpecification != null && !String.IsNullOrEmpty(pathCostSpecification.Edge))
            {
                var method = pathDelegateType.GetMethod(VertexCostMethodName);

                return (PathDelegates.VertexCost)Delegate.CreateDelegate(typeof(PathDelegates.VertexCost), method);
            }
            return null;
        }

        /// <summary>
        /// Creates an edge cost delegate
        /// </summary>
        /// <param name="pathCostSpecification">Cost specificateion</param>
        /// <param name="pathDelegateType">The path delegate type</param>
        /// <returns>The delegate</returns>
        private static PathDelegates.EdgeCost CreateEdgeCostDelegate(PathCostSpecification pathCostSpecification, Type pathDelegateType)
        {
            if (pathCostSpecification != null && !String.IsNullOrEmpty(pathCostSpecification.Edge))
            {
                var method = pathDelegateType.GetMethod(EdgeCostMethodName);

                return (PathDelegates.EdgeCost)Delegate.CreateDelegate(typeof(PathDelegates.EdgeCost), method);
            }
            return null;
        }

        /// <summary>
        ///   Creats the result
        /// </summary>
        /// <param name="graphElements"> The graph elements </param>
        /// <param name="resultTypeSpecification"> The result specification </param>
        /// <returns> </returns>
        private static IEnumerable<int> CreateResult(IEnumerable<AGraphElement> graphElements,
                                                     ResultTypeSpecification resultTypeSpecification)
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
        ///   Generates the properties.
        /// </summary>
        /// <returns> The properties. </returns>
        /// <param name='propertySpecification'> Property specification. </param>
        private static PropertyContainer[] GenerateProperties(
            Dictionary<UInt16, PropertySpecification> propertySpecification)
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
                                                      Value =
                                                          Convert.ChangeType(aPropertyDefinition.Value.Property,
                                                                             Type.GetType(
                                                                                 aPropertyDefinition.Value.FullQualifiedTypeName,
                                                                                 true, true))
                                                  };
                    propCounter++;
                }
            }

            return properties;
        }

        #endregion
    }
}