using System;
using System.Collections.Generic;
using System.Text;
using AsciiUml.Geo;

namespace AsciiUml.UI.GuiLib
{
	class TextBox : GuiComponent
	{
		public string Value { get; set; }
		private int cursor = 0;

		public TextBox(GuiComponent parent, int width) : base(parent){
			Dimensions = new GuiDimensions(new Size(width), new Size(1));
		}

		public override bool HandleKey(ConsoleKeyInfo key) {
			if (key.Key == ConsoleKey.LeftArrow) {
				cursor = Math.Max(0, cursor - 1);
				return true;
			}
			if (key.Key == ConsoleKey.RightArrow)
			{
				cursor = Math.Min(Dimensions.Width.Pixels, Math.Min(Value.Length, cursor + 1));
				return true;
			}
			Value += key.KeyChar;
			cursor++;
			return true;
		}

		public override Canvass Paint() {
			var c = new Canvass();
			c.RawPaintString(Value, 0, 0, BackGround, Foreground);

			if (IsFocused) {
				var pos = Parent.GetInnerCanvasTopLeft();
				Console.SetCursorPosition(pos.X+cursor,pos.Y);
			}

			return c;
		}

		public override Coord GetInnerCanvasTopLeft() {
			return new Coord(0,0);
		}
	}
}
