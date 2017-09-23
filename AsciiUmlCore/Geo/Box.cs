using System;
using System.Data;
using System.Linq;

namespace AsciiUml.Geo {
	public class Box : IPaintable<Box>, ISelectable, IResizeable<Box>, IConnectable {
		public int Id { get; }
		public int X => Pos.X;
		public int W { get; }
		public int Y => Pos.Y;
		public int H { get; }
		public Coord Pos { get; }

		public readonly string Text;

		public Box(Coord pos) : this(PaintAbles.Id++, pos) {
		}

		public Box(Coord pos, string text) : this(PaintAbles.Id++, pos) {
			var tmp = SetText(text);
			W = tmp.W;
			H = tmp.H;
			Text = text;
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
			var w = W < requiredWidth ? requiredWidth : W;

			var requiredHeight = 2 + rows.Length;
			var h = H < requiredHeight ? requiredHeight : H;

			return new Box(Id, Pos, w, h, text);
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

		//public static Coord[] CalcFrameCoords(int x, int y, int h, int w) {
		//	if (h == 1 && w == 1) {
		//		var coord = new Coord(x, y);
		//		if (coord.IsAnyNegative()) {
		//			return new Coord[0];
		//		}
		//		return new[] {coord};
		//	}

		//	List<Coord> coords = new List<Coord>();

		//	// top + bottom 
		//	for (int i = 0; i < w; i++) {
		//		coords.Add(new Coord(x + i, y));
		//		coords.Add(new Coord(x + i, y + h - 1));
		//	}

		//	// frame
		//	for (int i = 1; i < h - 1; i++) {
		//		coords.Add(new Coord(x, y + i));
		//		coords.Add(new Coord(x + w - 1, y + i));
		//	}

		//	return coords.Where(c => !c.IsAnyNegative()).ToArray();
		//}

		public Coord[] GetFrameCoords() {
			return RectangleHelper.GetFrameCoords(X, Y, H, W);
		}

		public Tuple<Coord, BoxFramePart>[] GetFrameParts() {
			return RectangleHelper.GetFrameParts(X, Y, H, W);
		}
	}
}