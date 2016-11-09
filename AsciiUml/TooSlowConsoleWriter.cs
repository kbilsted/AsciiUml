using System;

namespace AsciiUml {
	public static class TooSlowConsoleWriter {
		public static void Write(Canvass canvass, int? selected) {
			bool cursorUsingStdColors = true;
			Program.SetConsoleStandardColor();
			canvass.Lines.Each((row, y) => {
				row.Each((obj, x) => {
					if (obj == 0)
						Console.Write(' ');
					else {
						if (canvass.Occupants[y, x].HasValue && canvass.Occupants[y, x] == selected) {
							if (cursorUsingStdColors) {
								Program.SetConsoleSelectColor();
								cursorUsingStdColors = false;
							}
						}

						if (canvass.Occupants[y, x].HasValue && canvass.Occupants[y, x] != selected) {
							if (!cursorUsingStdColors) {
								Program.SetConsoleStandardColor();
								cursorUsingStdColors = true;
							}
						}

						Console.Write(obj);
					}
				});
				Console.WriteLine();
			});
		}
	}
}