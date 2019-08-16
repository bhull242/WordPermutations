using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordPermutations
{
	/// <summary>
	/// Used to sort words by length first (ascending), then by lexicographical order (ascending).
	/// </summary>
	class MyComparer : StringComparer, IComparer<string>, IEqualityComparer<string>, IComparer, IHashCodeProvider
	{
		static int Hash { get; }
		static MyComparer()
		{
			var s = typeof(MyComparer).FullName.Cast<int>();
			int i = 0;
			foreach (int x in s)
				i ^= x;
			Hash = i;
		}

		/// <summary>
		/// Compares two strings and returns an indication
		///     of their relative sort order. Words with fewer characters come first, then it is sorted lexicographically.
		/// </summary>
		/// <param name="x">A string to compare to <paramref name="y"/>.</param>
		/// <param name="y">A string to compare <paramref name="x"/> to.</param>
		/// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the
		///     following table. Value Meaning Less than zero <paramref name="x"/> precedes <paramref name="y"/> in the 
		///     sort order.-or- <paramref name="x"/> is <see langword="null"/> and <paramref name="y"/> is not 
		///     <see langword="null"/>. Zero <paramref name="x"/> is equal to <paramref name="y"/>. -or- 
		///     <paramref name="x"/> and <paramref name="y"/> are both <see langword="null"/>. 
		///     Greater than zero <paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- 
		///     <paramref name="y"/> is <see langword="null"/> and <paramref name="x"/> is not <see langword="null"/>.
		///     </returns>
		public override int Compare(string x, string y)
		{
			if (x is null)
				return y is null ? 0 : +1;
			else if (y is null)
				return -1;
			return x.Length < y.Length ? -1 : x.Length > y.Length ? +1 : x.CompareTo(y);
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than,
		///     equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, 
		/// as shown in the following table. Value Meaning Less than zero <paramref name="x"/> is less than 
		/// <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero 
		/// <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
		int IComparer.Compare(object x, object y)
		{
			if (x is string a)
				if (y is string b)
					return Compare(a, b);
				else
					return Compare(a, y?.ToString());
			else if (y is string b)
				return Compare(x?.ToString(), b);
			else
				return Compare(x?.ToString(), y?.ToString());
		}

		public override bool Equals(object obj) => obj is MyComparer;

		/// <summary>
		/// Indicates whether two strings are equal.
		/// </summary>
		/// <param name="x">A string to compare with <paramref name="y"/></param>
		/// <param name="y">A string to compare with <paramref name="x"/></param>
		/// <returns><see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> refer to the same object, or 
		/// <paramref name="x"/> and <paramref name="y"/> are equal, or <paramref name="x"/> and <paramref name="y"/> are
		/// <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(string x, string y) => x?.Equals(y) ?? y is null;

		public override int GetHashCode() => Hash;

		/// <summary>
		/// Gets the hash code for the specified string.
		/// </summary>
		/// <param name="obj">A string</param>
		/// <returns>A 32-bit signed hash code calculated from the value of the obj parameter.</returns>
		public override int GetHashCode(string obj) => obj?.GetHashCode() ?? 0;

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		int IHashCodeProvider.GetHashCode(object obj) => obj is string s ? GetHashCode(s) : GetHashCode(obj?.ToString());

	}
}
