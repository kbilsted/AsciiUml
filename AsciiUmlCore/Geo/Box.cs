using System;
using System.Linq;
using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public enum BoxStyle {
		Lines = 0,
		Stars = 1,
		Dots = 2,
		Eqls = 3,
	}

	public class Box : IPaintable<Box>, ISelectable, IResizeable<Box>, IConnectable, IHasTextProperty, IStyleChangeable {
		public int Id { get; }
		public int X => Pos.X;
		public int W { get; private set; }
		public int Y => Pos.Y;
		public int H { get; private set; }
		public Coord Pos { get; }
		public BoxStyle Style { get; private set; }

		private string text;

		public string Text {
			get => text;
			set => SetText(value);
		}

		public Box(Coord pos, string text) {
			Id = PaintAbles.GlobalId++;
			Pos = pos;
			SetText(text);
			Style = BoxStyle.Stars;
		}

		public Box(int id, Coord pos) {
			Id = id;
			H = 1;
			W = 1;
			Pos = pos;
		}

		public Box(int id, Coord pos, int w, int h, string text, BoxStyle style) {
			Id = id;
			W = w;
			H = h;
			Text = text;
			Pos = pos;
			Check();
			Style = style;
		}

		public Box(Coord pos, int w, int h) : this(PaintAbles.GlobalId++, pos, w, h, null, BoxStyle.Stars) {
		}

		public Box SetText(string text) {
			if (text == null)
				return this;

			var rows = text.Split('\n');

			var requiredWidth = rows.Select(x => x.Length).Max() + 4;
			W = W < requiredWidth ? requiredWidth : W;

			var requiredHeight = 2 + rows.Length;
			H = H < requiredHeight ? requiredHeight : H;

			this.text = text;

			return this;
		}

		public Box Move(Coord delta) {
			return new Box(Id, Pos.Move(delta), W, H, Text, Style);
		}

		public IPaintable<object> Resize(Coord delta) {
			return Resize(delta.X, delta.Y);
		}

		public Box Resize(int width, int height) {
			return new Box(Id, Pos, W + width, H + height, Text, Style);
		}

		public void Check() {
			if (H < 2) {
				throw new ArgumentException("Height must be at least 2");
			}
			if (W < 2) {
				throw new ArgumentException("Width must be at least 2");
			}
		}

		public Coord[] GetFrameCoords() {
			return RectangleHelper.GetFrameCoords(X, Y, H, W);
		}

		public Tuple<Coord, BoxFramePart>[] GetFrameParts() {
			return RectangleHelper.GetFrameParts(X, Y, H, W);
		}

		public void ChangeStyle(StyleChangeKind change) {
			Style = Extensions.NextOrPrevEnumValue(Style, change);
		}
	}
}