using System;

namespace AsciiConsoleUi.CompositeComponents {
	public class MultilineInputForm {
		private readonly TitledWindow titled;
		private readonly TextArea textArea;
		public Action<string> OnSubmit = text => { };
		public Action OnCancel = () => { };

		public MultilineInputForm(GuiComponent parent, string title, string explanation, string content, Coord position) {
			titled = new TitledWindow(parent, title) {Position = position};

			new TextLabel(titled, explanation, new Coord(0, 0));
			var textHeight = 10;
			textArea = new TextArea(titled, 12, textHeight, content, new Coord(0, 1)) {OnUserEscape = titled.RemoveMeAndChildren};
			var ok = new Button(titled, "Ok", () => Submit(),
				new Coord(2, textArea.RelativePositionToParent.Y + textArea.Dimensions.Height.Pixels + 1)) {
				BackGround = ConsoleColor.DarkGray,
				Foreground = ConsoleColor.Green
			};
			var border = new TextLabel(titled, "", new Coord(0, ok.RelativePositionToParent.Y + 1));
		}

		void Submit() {
			titled.RemoveMeAndChildren();
			OnSubmit(textArea.Value);
		}

		public void Focus() {
			textArea.Focus();
		}
	}
}