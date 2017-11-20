using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsciiUml.Commands;
using AsciiUml.Geo;

namespace AsciiUml {
	static class Extensions {
		public static T FirstOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> filter, Action<int> foundActionWithPosition) {
			int pos = 0;

			var res = collection.FirstOrDefault(
				x => {
					bool filterRes = filter(x);
					if (filterRes) {
						foundActionWithPosition(pos);
					}
					pos++;
					return filterRes;
				});

			return res;
		}

		public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> coll, int skip) {
			return coll.Reverse().Skip(skip).Reverse();
		}

		public static void Each<T>(this IEnumerable<T> coll, Action<T> code) {
			foreach (var c in coll) {
				code(c);
			}
		}

		public static void Each<T>(this IEnumerable<T> coll, Action<T, int> code) {
			int i = 0;
			foreach (var c in coll) {
				code(c, i++);
			}
		}

		public static Range<int> MinMax<T>(this IEnumerable<T> coll, Func<T, int> selector) {
			int min = int.MaxValue, max = int.MinValue;
			foreach (var c in coll) {
				var val = selector(c);
				if (val < min)
					min = val;
				if (val > max)
					max = val;
			}
			return new Range<int>(min, max);
		}

		public static List<ICommand> Lst(params ICommand[] vals) {
			return vals.ToList();
		}

		public static T NextOrPrevEnumValue<T>(T value, StyleChangeKind change) {
			if (!typeof(T).IsEnum)
				throw new ArgumentException(String.Format("{0} is not an Enum", typeof(T).FullName));

			int changer = change == StyleChangeKind.Next ? 1 : -1;

			T[] values = (T[])Enum.GetValues(value.GetType());
			var enumCount = values.Length;

			var index = (Array.IndexOf<T>(values, value) + changer + enumCount) % enumCount;
			return values[index];
		}

		public static string Repeat(this string s, int count) {
			if(count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			var sb = new StringBuilder();
			for (int i = 0; i < count; i++) {
				sb.Append(s);
			}
			return sb.ToString();
	}

	}
}