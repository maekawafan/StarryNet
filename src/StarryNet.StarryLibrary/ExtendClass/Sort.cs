using System;
using System.Collections.Generic;
using System.Text;

namespace StarryNet.StarryLibrary
{
	public class Ascending<T> where T : IComparable<T>
	{
		public static Comparison<T> Function { get { return (a, b) => a.CompareTo(b); } }
		public static ComparerClass Comparer { get { return comparer; } }
		private static ComparerClass comparer;

		static Ascending()
		{
			comparer = new ComparerClass();
		}

		public class ComparerClass : IComparer<T>
		{
			public int Compare(T a, T b)
			{
				return Function(a, b);
			}
		}
	}

	public class Descending<T> where T : IComparable<T>
	{
		public static Comparison<T> Function { get { return (a, b) => b.CompareTo(a); } }
		public static ComparerClass Comparer { get { return comparer; } }
		private static ComparerClass comparer;

		static Descending()
		{
			comparer = new ComparerClass();
		}

		public class ComparerClass : IComparer<T>
		{
			public int Compare(T a, T b)
			{
				return Function(a, b);
			}
		}
	}

	public class Ascending
	{
		public static Comparison<int> Function { get { return (a, b) => a.CompareTo(b); } }
		public static ComparerClass Comparer { get { return comparer; } }
		private static ComparerClass comparer;

		static Ascending()
		{
			comparer = new ComparerClass();
		}

		public class ComparerClass : IComparer<int>
		{
			public int Compare(int a, int b)
			{
				return Function(a, b);
			}
		}
	}

	public class Descending
	{
		public static Comparison<int> Function { get { return (a, b) => b.CompareTo(a); } }
		public static ComparerClass Comparer { get { return comparer; } }
		private static ComparerClass comparer;

		static Descending()
		{
			comparer = new ComparerClass();
		}

		public class ComparerClass : IComparer<int>
		{
			public int Compare(int a, int b)
			{
				return Function(a, b);
			}
		}
	}
}
