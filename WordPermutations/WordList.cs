using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordPermutations
{
	/// <summary>
	/// An indexed and <see cref="char"/>-keyed list of <see cref="string"/>s separated into <see cref="SubList"/>s 
	/// according to the first character of each <see cref="string"/>.
	/// </summary>
	public class WordList : IReadOnlyList<SubList>, IReadOnlyDictionary<char, SubList>, IReadOnlyCollection<SubList>, 
		ICollection<string>, IEnumerable<SubList>, IEnumerable
	{
		private IList<SubList> list;
		/// <summary>
		/// List of valid starting characters to separate the lists by in order.
		/// </summary>
		private string alphabet;

		public SubList this[int index] => list[index];
		public SubList this[char key] => this[Search(key)];

		/// <summary>
		/// The number of <see cref="SubList"/>s in this list.
		/// </summary>
		public int Count => alphabet.Length + 1;
		private int num = 0;
		/// <summary>
		/// The number of <see cref="string"/>s in this list.
		/// </summary>
		int ICollection<string>.Count => num;
		/// <summary>
		/// The number of <see cref="string"/>s in this list.
		/// </summary>
		public int NumWords => num;
		bool ICollection<string>.IsReadOnly => false;
		public IEnumerable<char> Keys {
			get {
				foreach (var li in list)
					yield return li.Key;
			}
		}
		public IEnumerable<SubList> Values
		{
			get {
				foreach (var li in list)
					yield return li;
			}
		}

		public WordList(string alphabet)
		{
			this.alphabet = string.Concat(alphabet.OrderBy(c => c).Distinct());
			list = new List<SubList>();
			foreach (char c in alphabet)
				list.Add(new SubList(c));
			list.Add(new SubList('\0'));
		}

		/// <summary>
		/// Initializes a new <see cref="WordList"/> with the given list of <see cref="string"/>s separated by the 
		/// characters given by <paramref name="alphabet"/>.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="alphabet"></param>
		public WordList(IEnumerable<string> list, string alphabet) : this(alphabet)
		{
			foreach (string s in list) {
				int index = Search(s);
				if (index == -1)
					continue;
				this.list[index].Add(s);
			}
			foreach (var li in this.list)
				num += li.Count;
		}

		/// <summary>
		/// Searches for the list with the given key.
		/// </summary>
		/// <param name="key">The char to search for.</param>
		/// <returns>The index of the <see cref="SubList"/> with the given <paramref name="key"/> as its 
		/// <see cref="SubList.Key"/>. If no such list exists, returns the last list.</returns>
		protected int Search(char key)
		{
			int index = -1, min = 0, count = alphabet.Length, max = count - 1;
			while (min <= max) {
				index = (min + max) / 2;
				if (key == list[index].Key)
					return index;
				else if (key < list[index].Key)
					max = index - 1;
				else
					min = index + 1;
			}
			return count;
		}

		/// <summary>
		/// Searches for the list that would contain the given string.
		/// </summary>
		/// <param name="item">The string to be searched for.</param>
		/// <returns>The index of the <see cref="SubList"/> that would contain <paramref name="item"/>.</returns>
		protected int Search(string item)
		{
			if (string.IsNullOrEmpty(item))
				return -1;
			char key = item[0];
			return Search(key);
		}

		public void Add(string item)
		{
			int index = Search(item);
			if (index == -1)
				return;
			list[index].Add(item);
			num++;
		}
		void ICollection<string>.Clear()
		{
			foreach (var li in list)
				li.Clear();
			num = 0;
		}
		public bool Contains(string item)
		{
			int index = Search(item);
			if (index == -1)
				return false;
			return list[index].Contains(item);
		}
		void ICollection<string>.CopyTo(string[] array, int arrayIndex)
		{
			if (array is null)
				throw new ArgumentNullException(nameof(array));
			for (int i = arrayIndex, j = 0; i < array.Length && j < Count; i++, j++) {
				for (int k = 0; i < array.Length && k < list[j].Count; i++, k++)
					array[i] = list[j][k];
			}
		}

		public IEnumerator<SubList> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		IEnumerator<KeyValuePair<char,SubList>> IEnumerable<KeyValuePair<char,SubList>>.GetEnumerator()
		{
			foreach (var li in list)
				yield return new KeyValuePair<char, SubList>(li.Key, li);
		}
		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			foreach (var li in list)
				foreach (var s in li)
					yield return s;
		}
		public bool Remove(string item)
		{
			int index = Search(item);
			if (index < 0)
				return false;
			bool found = list[index].Remove(item);
			if (found)
				num--;
			return found;
		}

		public bool ContainsKey(char key) => key == '\0' || alphabet.Contains(key);
		public bool TryGetValue(char key, out SubList value)
		{
			if (ContainsKey(key)) {
				value = this[key];
				return true;
			}
			value = this[alphabet.Length];
			return false;
		}
	}
}
