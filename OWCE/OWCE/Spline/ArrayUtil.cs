namespace OWCE.Spline
{
	using System;
	using System.Text;

	/// <summary>
	/// Utility methods for arrays.
	/// </summary>
	public static class ArrayUtil
	{
		/// <summary>
		/// Create a string to display the array values.
		/// </summary>
		/// <param name="array">The array</param>
		/// <param name="format">Optional. A string to use to format each value. Must contain the colon, so something like ':0.000'</param>
		public static string ToString<T>(T[] array, string format = "")
		{
			var s = new StringBuilder();
			string formatString = "{0" + format + "}";

			for (int i = 0; i < array.Length; i++)
			{
				if (i < array.Length - 1)
				{
					s.AppendFormat(formatString + ", ", array[i]);
				}
				else
				{
					s.AppendFormat(formatString, array[i]);
				}
			}

			return s.ToString();
		}
	}
}
