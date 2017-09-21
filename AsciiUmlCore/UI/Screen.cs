using System;

namespace AsciiUml.UI {
	static class Screen {
		public static void SetConsoleGetInputColors() {
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.Green;
		}

		public static void SetConsoleStandardColor() {
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void SetConsoleSelectColor() {
			Console.BackgroundColor = ConsoleColor.DarkYellow;
			Console.ForegroundColor = ConsoleColor.Yellow;
		}
	}
}