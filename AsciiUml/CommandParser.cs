using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt;
using LanguageExt.SomeHelp;

namespace AsciiUml {
	//public struct May<T> {
	//	public readonly T Value;
	//	public readonly bool HasValue;

	//	public static readonly May<T> None = new May<T>(false);

	//	private May(bool secret)
	//	{
	//		HasValue = false;
	//		Value = default(T);
	//	} 

	//	public May(T value) {
	//		Value = value;
	//		HasValue = true;
	//	}

	//	public static implicit operator May<T>(T value) => ReferenceEquals(null, value) ? None : new May<T>(value);

	//	public May<T> Bind(Func<T> code) {
	//		return HasValue ? new May<T>(code()) : this;
	//	}

	//	public void Then(Func<T> someCode, Action noneCode) {
	//		if (HasValue) someCode();
	//		else 
	//			noneCode();
	//	}

	//}

	public static class Ext {
		public static T FirstOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> filter,
			Action<int> foundActionWithPosition) {
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
	}

	public class Range<T> {
		public T Min, Max;

		public Range(T min, T max) {
			Min = min;
			Max = max;
		}

		public override string ToString() {
			return $"{Min} - {Max}";
		}
	}

	public static class CommandParser {
		public static Option<int> ReadInt(Range<int> range, string text) {
			var input = TryReadLineWithCancel(text);
			return input.Match(x => {
				int result;
				if (!int.TryParse(x, out result))
					return ReadInt(range, "\nNot a number. Try again: ");

				if (result < range.Min || result > range.Max)
					return ReadInt(range, $"\nNot in range {range}: ");

				return result.ToSome();
			}, () => Option<int>.None);
		}

		public static Option<string> TryReadLineWithCancel(string explanation) {
            Console.WriteLine("ESC to abort input. RETURN to finish input. SHIFT+RETURN for muliline input");
            Console.Write(explanation);

            var sb = new StringBuilder();
            do {
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
                {
                    if (key.Modifiers == ConsoleModifiers.Shift)
                        sb.Append("\n");
                    else
                        return sb.ToString();
                }
                else if (key.Key == ConsoleKey.Escape)
					return null;
				else if (key.Key == ConsoleKey.Backspace) {
					if (sb.Length > 0) {
						sb.Remove(sb.Length - 1, 1);
						Console.Write("\b \b");
					}
				}
				else {
					Console.Write(key.KeyChar);
					sb.Append(key.KeyChar);
				}
			} while (true);
		}
	}
}