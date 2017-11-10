using System;

namespace AsciiConsoleUi.CompositeComponents {
	public class Popup {
		public Popup(GuiComponent parent, string message) {
			var titled = new TitledWindow(parent, "About...") {
				BackGround = ConsoleColor.DarkBlue,
				Foreground = ConsoleColor.White
			};

			var label = new TextLabel(titled, message, new Coord(4, 1));

			titled.Dimensions = label.GetSize();
			titled.Dimensions.Height.Pixels += 5;
			titled.Dimensions.Width.Pixels += 6;
			titled.Position = new Coord((State.MaxX - titled.Dimensions.Width.Pixels) / 2,
				((State.MaxY - titled.Dimensions.Height.Pixels) / 2) - 1);

			label.AdjustWhenParentsReposition();

			var screenCenter = new Coord(titled.Dimensions.Width.Pixels / 2, label.Height + 2);
			var ok = new Button(titled, "OK", () => { titled.RemoveMeAndChildren(); }, screenCenter) {
				BackGround = ConsoleColor.DarkGray,
				Foreground = ConsoleColor.Green
			};
			ok.Focus();
		}
	}
}