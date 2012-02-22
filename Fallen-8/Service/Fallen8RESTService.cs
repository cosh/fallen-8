// 
//  Fallen8RESTServicePlugin.cs
//  
//  Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//  
//  Copyright (c) 2012 Henning Rauch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, version 3 of the License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using Framework.Serialization;

namespace Fallen8.API.Service
{
	/// <summary>
	/// Fallen-8 REST service.
	/// </summary>
	public sealed class Fallen8RESTServicePlugin : IFallen8Service
	{
		#region Constructor
		
		public Fallen8RESTServicePlugin ()
		{
		}
		
		#endregion

		#region IFallen8Service implementation
		public bool TryStop ()
		{
			throw new NotImplementedException ();
		}

		public DateTime StartTime {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsRunning {
			get {
				throw new NotImplementedException ();
			}
		}

		public IDictionary<string, string> Metadata {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region IFallen8Serializable implementation
		public void Save (SerializationWriter writer)
		{
			throw new NotImplementedException ();
		}

		public void Open (SerializationReader reader, Fallen8 fallen8)
		{
			throw new NotImplementedException ();
		}
		#endregion

		#region IFallen8Plugin implementation
		public void Initialize (Fallen8 fallen8, IDictionary<string, object> parameter)
		{
			throw new NotImplementedException ();
		}

		public string PluginName {
			get 
			{
				return "Fallen-8_REST_Service";
			}
		}

		public Type PluginCategory {
			get 
			{
				return typeof(IFallen8Service);
			}
		}

		public string Description {
			get 
			{
				return "A simple REST service that talks JSON";
			}
		}

		public string Manufacturer {
			get 
			{
				return "Henning Rauch";
			}
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

