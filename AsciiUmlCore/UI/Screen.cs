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

	    public static void PrintErrorAndWaitKey(string text)
	    {
	        Console.ForegroundColor = ConsoleColor.Red;
	        Console.BackgroundColor = ConsoleColor.White;
	        Console.WriteLine(text);
	        Screen.SetConsoleStandardColor();
	        Console.ReadKey();
	    }
	}

	//           _____  _____ ______ _____   _    _ __  __ _      
	//     /\   / ____|/ ____|__   _|_   _| | |  | |  \/  | |     
	//    /  \  | (___ | |      | |   | |   | |  | | \  / | |     
	//   / /\ \  \___ \| |      | |   | |   | |  | | |\/| | |     
	//  / ____ \ ____) | |____ _| |_ _| |_  | |__| | |  | | |____ 
	// /_/    \_\_____/ \_____|_____|_____|  \____/|_|  |_|______|
}