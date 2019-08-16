// By Brian Hull starting in 2019
// Used to take a user-input string of characters and find all possible permutations of all possible combinations of 
// characters in that string that form a valid word.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordPermutations
{
	public class Descrambler
	{
		/// <summary>
		/// A string containing all the valid characters.
		/// </summary>
		public static string alphabet = "abcdefghijklmnopqrstuvwxyz";

		/// <summary>
		/// The filepath for the .txt file containing a list of all valid words.
		/// </summary>
		private static string word_filename = "words_alpha.txt";

		/// <summary>
		/// Used to sort <see cref="string"/>s when compiling the results of descrambling.
		/// </summary>
		protected internal static IComparer<string> comparer = new MyComparer();

		/// <summary>
		/// Takes the characters in a given <see cref="string"/> and returns the characters in lexicographic order.
		/// </summary>
		/// <param name="s">The <see cref="string"/> to be sorted.</param>
		/// <returns>A <see cref="string"/> containing all the characters in <paramref name="s"/> in the standard 
		/// sorting order.</returns>
		public static string SortChars(string s) => string.Concat(s.OrderBy(c => c));

		/// <summary>
		/// Generates a list of permutations of a given string that contain at least <paramref name="minLength"/> 
		/// characters.
		/// </summary>
		/// <param name="s">The string of characters to permute.</param>
		/// <param name="minLength">The minimum number of characters per string. Must be greater than or equal to 1.</param>
		/// <returns>A list of all possible permutations of any number of characters from <paramref name="s"/> with 
		/// at least <paramref name="minLength"/> characters.</returns>
		public static WordList GetPermutations(string s, int minLength = 1)
		{
			if (minLength <= 0)
				minLength = 1;
			s = SortChars(s.Normalize(NormalizationForm.FormC).ToLower());
			IList<string> list = new List<string>();
			list = PermuteRec(s, list, minLength);
			if (minLength > 3)
			{
				int len = list.Count;
				for (int i = 0; i < len; i++)
					if (s.Length < minLength)
						list.RemoveAt(i--);
			}
			return new WordList(list, alphabet);
		}

		/// <summary>
		/// Generates a list of permutations of a given string that are at least <paramref name="minLength"/> 
		/// characters long.
		/// </summary>
		/// <param name="s">The string of characters to permute.</param>
		/// <param name="list">The current list of permutations.</param>
		/// <param name="minLength">The minimum number of characters per string.</param>
		/// <returns></returns>
		protected static IList<string> PermuteRec(string s, IList<string> list, int minLength)
		{
			/* First, check to see if the minimum length has been reached. (This minimum should be >0.)
			 * This allows us to find if the base case is reached, which occurs when either the minimum has been reached 
			 * and that length is between 1 and 3 (inclusive) or the length of the word is less than the minimum and is 3.
			 */
			if (s.Length == minLength)
			{
				switch (minLength)
				{
					case 1:
						list.Add(s);
						return list;
					case 2:
						list.Add(s);
						list.Add(string.Concat(s[1], s[0]));
						return list;
					case 3:
						list.Add(s);
						list.Add(string.Concat(s[0], s[2], s[1]));
						list.Add(string.Concat(s[1], s[0], s[2]));
						list.Add(string.Concat(s[1], s[2], s[0]));
						list.Add(string.Concat(s[2], s[0], s[1]));
						list.Add(string.Concat(s[2], s[1], s[0]));
						return list;
					default:
						minLength = 3;
						break;
				}
			}
			char prev = '\0';
			
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c == prev)
					continue;
				IList<string> sublist = PermuteRec(s.Remove(i, 1), list, minLength);
				foreach (string str in sublist)
				{
					list.Add(str);
					var newList = InsertChar(c, str);
					foreach (string x in newList)
						list.Add(x);
				}
				prev = c;
			}
			return list;
		}

		/// <summary>
		/// Lists all possible results from inserting a given <see cref="char"/> into a given <see cref="string"/>.
		/// </summary>
		/// <param name="c">The <see cref="char"/> to be inserted.</param>
		/// <param name="s">The <see cref="string"/> to insert <paramref name="c"/> into.</param>
		/// <returns>An <see cref="IList{T}"/> of <see cref="string"/>s containing strings resulting from inserting 
		/// <paramref name="c"/> into each possible position in <paramref name="s"/>.</returns>
		protected static IList<string> InsertChar(char c, string s)
		{
			IList<string> list = new List<string>(s.Length+1);
			string ch = c.ToString();
			for (int i = 0; i < s.Length; i++)
				list.Add(s.Insert(i, ch));
			return list;
		}

		/// <summary>
		/// Takes words from an enumerable object and compares them against the elements of a list, generating a 
		/// set containing the words in both.
		/// </summary>
		/// <param name="first">The enumerable object of <see cref="string"/>s to pull words from.</param>
		/// <param name="second">The list of <see cref="string"/>s to compare against.</param>
		/// <returns>An <see cref="ISet{T}"/> of <see cref="string"/>s in both <paramref name="first"/> 
		/// and <paramref name="second"/>.</returns>
		protected static ISet<string> Intersection(IEnumerable<string> first, WordList second)
		{
			ISet<string> set = new SortedSet<string>(comparer);
			foreach (string word in first)
				if (second.Contains(word))
					set.Add(word);
			return set;
		}

		public static void Main(string[] args)
		{
			if (!(args is null)) {
				if (args.Length >= 1)
					word_filename = args[0];
				if (args.Length >= 2)
					alphabet = args[1];
			}
			string[] a = File.ReadAllLines(word_filename, Encoding.UTF8);
			var words = new WordList(a, alphabet);

			while (true)
			{
				int minLength;	// The minimum word length to search for
				string str;		// A string of characters to permute

				// Get the minimum word length from the user
			GetMinLength:
				Console.Out.Write("Please input minimum word length: ");
				string input = Console.In.ReadLine();
				while (!int.TryParse(input, out minLength) || minLength < 1)
				{
					Console.Out.WriteLine("Not a valid length.");
					Console.Out.WriteLine();
					Console.Out.Write("Please input minimum word length: ");
					input = Console.In.ReadLine();
				}

				// Get the string of characters to permute from the user.
				while (true)
				{
					Console.Out.Write("Please input string of characters to permute: ");
					str = Console.In.ReadLine().ToLower().Replace("[ ,.']", "");
					if (str.Length < minLength)
					{
						Console.Out.WriteLine("Input string is shorter than the input minimum word length.");
						Console.Out.WriteLine();
						goto GetMinLength;
					}
					for (int i = 0; i < str.Length; i++)
						if (!alphabet.Contains(str[i]))
						{
							Console.Out.WriteLine($"Input string contains invalid character '{str[i]}'.");
							Console.Out.WriteLine();
							Console.Out.WriteLine($"Minimum word length: {minLength}");
							continue;
						}
					break;
				}

				WordList list = GetPermutations(str, minLength);
				if (list.NumWords == 0)
					Console.Out.WriteLine("No permutations found.");
				else
				{
					ISet<string> set = Intersection(list, words);
					if (set.Count == 0)
						Console.Out.WriteLine("No valid words found.");
					else
						foreach (string s in set)
							Console.Out.WriteLine(s);
				}

				Console.Out.WriteLine();
				Console.Out.Write("Continue? Y/N: ");
				if (Console.In.ReadLine().ToLower()[0] == 'n')
					break;

				Console.Out.WriteLine();
				Console.Out.WriteLine();
			}
		}
	}
}
