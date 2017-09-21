namespace AsciiUml.UI {
	public static class TooSlowConsoleWriter {
		//public static void Write(Canvass canvass, int? selected) {
		//	bool cursorUsingStdColors = true;
		//	Screen.SetConsoleStandardColor();
		//	canvass.Lines.Each((row, y) => {
		//		row.Each((obj, x) => {
		//			if (obj == 0)
		//				Console.Write(' ');
		//			else {
		//				if (canvass.Occupants[y, x].HasValue && canvass.Occupants[y, x] == selected) {
		//					if (cursorUsingStdColors) {
		//						Screen.SetConsoleSelectColor();
		//						cursorUsingStdColors = false;
		//					}
		//				}

		//				if (canvass.Occupants[y, x].HasValue && canvass.Occupants[y, x] != selected) {
		//					if (!cursorUsingStdColors) {
		//						Screen.SetConsoleStandardColor();
		//						cursorUsingStdColors = true;
		//					}
		//				}

		//				Console.Write(obj);
		//			}
		//		});
		//		Console.WriteLine();
		//	});
		//}
	}
}