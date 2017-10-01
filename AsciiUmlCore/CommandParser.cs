using System;
using System.Linq;
using System.Text;
using AsciiUml.Geo;
using LanguageExt;
using LanguageExt.SomeHelp;

namespace AsciiUml {
	public static class CommandParser {
		public static Option<int> ReadInt(int[] onlyAllowedValues, string text)
		{
		    Range<int> range = onlyAllowedValues.MinMax(x => x);

            var input = TryReadLineWithCancel(text);
			return input.Match(x => {
				int result;
				if (!int.TryParse(x, out result))
					return ReadInt(onlyAllowedValues, "\nNot a number. Try again: ");

				if (result < range.Min || result > range.Max)
					return ReadInt(onlyAllowedValues, $"\nNot in range {range}: ");

                if(onlyAllowedValues.All(y => y != result))
                    return ReadInt(onlyAllowedValues, $"\nNot a valid id. ");

                return result.ToSome();
			}, () => Option<int>.None);
		}

		public static Option<string> TryReadLineWithCancel(string explanation) {
			Console.WriteLine("ESC to abort input. RETURN to finish input. SHIFT+RETURN for multiline input");
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