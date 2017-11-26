using System;
using AsciiConsoleUi;

namespace AsciiUml {
	internal class TopMenu : GuiComponent {
		private readonly State state;

		public TopMenu(WindowManager manager, State state) : base(manager) {
			this.state = state;
		}

		public override bool HandleKey(ConsoleKeyInfo key) {
			return false;
		}

		public override Canvass Paint() {
			var c = new Canvass();
			var menu = state.Gui.TopMenuTextOverride ?? DefaultMenu();
			Canvass.PaintString(c, menu, 0, 0, -1, ConsoleColor.DarkGreen, ConsoleColor.Green);
			return c;
		}

		private string DefaultMenu() {
			var displayFilename = state.Config.SaveFilename;
			if (displayFilename.Length > 34)
				displayFilename = displayFilename.Substring(0, 34) + "..";
			var menu = $"Selected: {state.SelectedId?.ToString() ?? "None"}. ({state.TheCurser}). '{displayFilename}'. 'h' for help";
			return menu;
		}

		public override Coord GetInnerCanvasTopLeft() {
			return new Coord(0, 1);
		}
	}
}