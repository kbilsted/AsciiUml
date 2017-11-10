using System;
using System.Linq;

namespace AsciiConsoleUi {
	public class TextLabel : GuiComponent {
		private string[] splittedText;
		private string text;
		public int Height { get; private set; }

		public string Text {
			get => text;
			set {
				text = value;
				splittedText = text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
				var maxLineLength = splittedText.Any() ? splittedText.Max(x => x.Length) : 0;
				Height = Math.Max(1, splittedText.Length);
				Dimensions = new GuiDimensions(new Size(maxLineLength), new Size(Height));
			}
		}

		public TextLabel(GuiComponent parent, string text, Coord position) : base(parent, position) {
			Text = text;
			IsFocusable = false;
		}

		public override bool HandleKey(ConsoleKeyInfo key) {
			return Parent.HandleKey(key);
		}

		public override Canvass Paint() {
			var c = new Canvass();
			for (int i = 0; i < splittedText.Length; i++) {
				c.RawPaintString(splittedText[i], 0, i, BackGround, Foreground);
			}
			return c;
		}

		public override Coord GetInnerCanvasTopLeft() {
			return new Coord(Text.Length, 1);
		}
	}
}