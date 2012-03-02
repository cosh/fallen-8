// 
//  Fallen8Pool.cs
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
using System.Collections.Concurrent;

namespace Fallen8.API.Helper
{
	/// <summary>
	/// Fallen-8 object pool.
	/// </summary>
	public class Fallen8Pool
	{
		#region Data
		
		/// <summary>
		/// The place where the Fallen-8 live.
		/// </summary>
    	private ConcurrentQueue<Fallen8> _objects;
		
		/// <summary>
		/// The max count of instances.
		/// </summary>
		private UInt32 _maxValue;
		
		/// <summary>
		/// The min count of objects.
		/// </summary>
		private UInt32 _minValue;
		
		/// <summary>
		/// The start capacity of the Fallen-8s.
		/// </summary>
		private Int32 _startCapacity;
		
		#endregion
		
		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the Fallen-8 object pool/> class.
		/// </summary>
		/// <param name='minValue'>
		/// The min count of instances.
		/// </param>
		/// <param name='maxValue'>
		/// The max count of instances.
		/// </param>
		/// <param name='startCapacity'>
		/// The start capacity of the Fallen-8s.
		/// </param>
    	public Fallen8Pool(UInt32 minValue = 1, UInt32 maxValue = 2, Int32 startCapacity = 0)
    	{
    	    _minValue = minValue;
			_maxValue = maxValue;
			_startCapacity = startCapacity;
			_objects = new ConcurrentQueue<Fallen8>();
			FillQueue();
    	}
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Tries to get a pooled Fallen-8 instance.
		/// </summary>
		/// <returns>
		/// True for success.
		/// </returns>
		/// <param name='result'>
		/// The resulting Fallen-8.
		/// </param>
    	public bool TryGetFallen8(out Fallen8 result)
    	{
    	    if (_objects.TryDequeue(out result))
			{
				if (_objects.Count < _minValue) {
					FillQueue();
				}
				
    	    	return true;
			}
				
			result = null;
    	    return false;
		}
		
		/// <summary>
		/// Recycles a Fallen-8 instance.
		/// </summary>
		/// <param name='instance'>
		/// Fallen-8 instance.
		/// </param>
    	public void RecycleFallen8(Fallen8 instance)
    	{
			instance.TabulaRasa();
				
			if (_objects.Count < _maxValue) 
			{
    	    	_objects.Enqueue(instance);
			}
		}
		
		#endregion
		
		#region private helper
		
		/// <summary>
		/// Fills the queue.
		/// </summary>
		private void FillQueue()
		{
			
		}
		
		#endregion
	}
}

