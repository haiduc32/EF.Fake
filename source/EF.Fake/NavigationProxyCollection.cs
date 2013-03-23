using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	class NavigationProxyCollection<T> : ICollection<T>
	{
		#region private fields

		private FakeDbSetBase _dbSet;
		private object _parentObject;

		/// <summary>
		/// Do not use dirrectly. Access the data by Data property.
		/// </summary>
		private HashSet<T> _loadedData;

		#endregion private fields

		#region properties

		//private HashSet<T> Data
		//{
		//	get
		//	{
		//		if (_loadedData == null)
		//		{
		//			LoadData();
		//		}

		//		return _loadedData;
		//	}
		//}

		#endregion properties

		#region .ctor

		public NavigationProxyCollection(FakeDbSetBase dbSet, object parentObject)
		{
			_dbSet = dbSet;
			_parentObject = parentObject;
			_loadedData = new HashSet<T>();
		}

		#endregion .ctor

		#region internal methods

		public void AddCollection(List<object> items)
		{
			foreach (T item in items)
			{
				_loadedData.Add(item);
			}
		}

		#endregion internal methods

		#region interface implementation

		//TODO: need this method now?
		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return _loadedData.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _loadedData.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion interface implementation

	}
}
