using System;
using System.Collections.Generic;
using System.Text;

namespace StarryNet.StarryLibrary
{
	public class FuncComparer<T> : IComparer<T>
	{
		Comparison<T> comparer;
		public FuncComparer(Comparison<T> func)
		{
			comparer = func;
		}

		int IComparer<T>.Compare(T x, T y)
		{
			return comparer(x, y);
		}
	}
}
