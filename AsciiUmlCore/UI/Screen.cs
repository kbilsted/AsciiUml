using System;

namespace AsciiUml.UI {
	internal static class Screen {
		public static void PrintErrorAndWaitKey(string text) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.BackgroundColor = ConsoleColor.White;
			Console.WriteLine(text);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.ReadKey();
		}
	}
}