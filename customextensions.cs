using System;
using System.Collections.Generic;

namespace Constellation.Common.Core.Extensions
{
	public static class CustomExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			source.ThrowIfNull("source");
			action.ThrowIfNull("action");
			foreach (T element in source)
			{
				action(element);
			}
		}

		public static void ThrowIfNull(this object source, string  message)
		{
			if (source == null) {
				throw new ArgumentNullException(message);
			}
		}

		public static DateTime ConvertFromUnixTimestamp(this double timestamp) {
			var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}


		public static double ConvertToUnixTimestamp(this DateTime date) {
			var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			var diff = date - origin;
			return Math.Floor(diff.TotalSeconds);
		}
	}
}