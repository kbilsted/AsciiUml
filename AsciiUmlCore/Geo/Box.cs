using System;
using System.Linq;
using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public class Box : IPaintable<Box>, ISelectable, IResizeable<Box>, IConnectable, IHasTextProperty {
		public int Id { get; }
		public int X => Pos.X;
		public int W { get; private set; }
		public int Y => Pos.Y;
		public int H { get; private set; }
		public Coord Pos { get; }

	    private string text;
		public string Text { get => text; set => SetText(value); }

		public Box(Coord pos) : this(PaintAbles.Id++, pos) {
		}

		public Box(Coord pos, string text) : this(PaintAbles.Id++, pos) {
			SetText(text);
		}

		public Box(int id, Coord pos) {
			Id = id;
			H = 1;
			W = 1;
			Pos = pos;
		}

		public Box(int id, Coord pos, int w, int h, string text) {
			Id = id;
			W = w;
			H = h;
			Text = text;
			Pos = pos;
			Check();
		}

		public Box(Coord pos, int w, int h) : this(PaintAbles.Id++, pos, w, h, null) {
		}

		public Box SetText(string text) {
			var rows = text.Split('\n');

			var requiredWidth = rows.Select(x => x.Length).Max() + 4;
			W = W < requiredWidth ? requiredWidth : W;

			var requiredHeight = 2 + rows.Length;
			H = H < requiredHeight ? requiredHeight : H;

		    this.text = text;

		    return this;
		}

		public Box Move(Coord delta) {
			return new Box(Id, Pos.Move(delta), W, H, Text);
		}

		public IPaintable<object> Resize(Coord delta) {
			return Resize(delta.X, delta.Y);
		}

		public Box Resize(int width, int height) {
			return new Box(Id, Pos, W + width, H + height, Text);
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
	}
}