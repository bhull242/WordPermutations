using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordPermutations
{
	/// <summary>
	/// A list of <see cref="string"/>s with the same starting <see cref="char"/>, which is also the key for the list.
	/// </summary>
	public class SubList : IList<string>, IReadOnlyList<string>, ICollection<string>, IReadOnlyCollection<string>,
		IList, ICollection, IEnumerable<string>, IEnumerable, IGrouping<char, string>
	{
		private IList<string> list;

		/// <summary>
		/// The first <see cref="char"/> of every <see cref="string"/> in this list, or the null character if this contains 
		/// strings with multiple different starting characters.
		/// </summary>
		public char Key { get; }
		public int Count => list.Count;
		public bool IsReadOnly => false;
		bool IList.IsFixedSize => false;
		bool IList.IsReadOnly => false;
		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot => this;
		static IComparer<string> comparer = Descrambler.comparer;

		public string this[int index] {
			get => list[index];
			set {
				if (string.IsNullOrEmpty(value) || (Key != value[0] && Key != '\0') || list.Contains(value))
					return;
				list[index] = value;
			}
		}
		object IList.this[int index]
		{
			get => this[index];
			set => this[index] = value is string s ? s : value.ToString();
		}

		public SubList(char key = '\0')
		{
			list = new List<string>();
			Key = key;
		}
		
		protected (bool found, int index) Search(string value)
		{
			if (string.IsNullOrEmpty(value) || (Key == '\0' && Key == value[0]))
				return (false, -1);
			int min = 0, max = Count-1, count = max + 1;
			(bool found, int index) = (false, -1);
			int prev = 0;
			while (!found && min <= max) {
				index = (min + max) / 2;
				int comp = comparer.Compare(value, list[index]);
				if (comp == 0)
					found = true;
				else if (comp < 0) {
					max = index - 1;
					prev = max;
				}
				else {
					min = index + 1;
					prev = min;
				}
			}
			if (!found)
				if (prev < 0)
					index = 0;
				else if (prev > count)
					index = count;
				else
					index = prev;
			return (found, index);
		}
		public void Add(string item)
		{
			(bool found, int index) = Search(item);
			if (found)
				return;
			list.Insert(index, item);
		}
		public void Clear() => list.Clear();
		public bool Contains(string item) => Search(item).found;
		public void CopyTo(string[] array, int arrayIndex)
		{
			if (array is null)
				throw new ArgumentNullException(nameof(array));
			int count = Count, length = array.Length;
			for (int i = 0, j = arrayIndex; i < count && j < length; i++, j++)
				array[j] = list[i];
		}
		public int IndexOf(string item)
		{
			(bool found, int index) = Search(item);
			if (!found)
				return -1;
			return index;
		}
		public void Insert(int index, string item) => throw new NotImplementedException();
		public bool Remove(string item)
		{
			(bool found, int index) = Search(item);
			if (found)
				list.RemoveAt(index);
			return found;
		}
		public void RemoveAt(int index) => list.RemoveAt(index);
		int IList.Add(object value)
		{
			if (value is string s)
				if (string.IsNullOrEmpty(s) || (Key != s[0] && Key != '\0') || list.Contains(s))
					return -1;
				else {
					list.Add(s);
					return list.IndexOf(s);
				}
			else
				return ((IList)this).Add(value.ToString());
		}
		bool IList.Contains(object value) => value is string s ? Contains(s) : Contains(value.ToString());
		int IList.IndexOf(object value) => value is string s ? IndexOf(s) : IndexOf(value.ToString());
		void IList.Insert(int index, object value) => throw new NotImplementedException();
		void IList.Remove(object value)
		{
			if (value is string s)
				Remove(s);
			else
				Remove(value.ToString());
		}
		public IEnumerator<string> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

		public void Deconstruct(out char key, out IList<string> list)
		{
			key = Key;
			list = this.list;
		}
	}
}
