using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;

namespace AsciiUml {
	static class PaintAbles {
		public static int Id { get; set; }
	}

	public interface IPaintable<out T> {
		int Id { get; }
		T Move(int x, int y);
	}

	public interface ISelectable {
		int Id { get; }
		int X { get; }
		int Y { get; }
	}

	public enum Direction {
		North,
		South,
		East,
		West
	}

	public enum LabelDirection {
		LeftToRight,
		TopDown
	}


	public class Cursor : IPaintable<Cursor> {
		public int X { get; }
		public int Y { get; }

		public Cursor(int x, int y) {
			X = x;
			Y = y;
		}

		public int Id {
			get { return -1; }
		}

		public Cursor Move(int x, int y) {
			return new Cursor(Math.Max(0, X + x), Math.Max(0, Y + y)); // todo bug.. kan bevæge sig over max for canvas
		}
	}


	public class LineSegment {
		public int Id { get; }
		public Coord From, To;
		public SegmentKind Kind;
		public SlopedLine Origin;

		public LineSegment(SlopedLine l) {
			Id = PaintAbles.Id++;
			Origin = l;
		}

		public LineSegment(int id, SlopedLine l, Coord from, Coord to, SegmentKind kind) {
			Id = id;
			Origin = l;
			From = from;
			To = to;
			Kind = kind;
		}

		public LineSegment Move(int x, int y) {
			return new LineSegment(Id, Origin, From.Move(x, y), To.Move(x, y), Kind);
		}

		public LineSegment ExtendEndpoint(int x, int y, EndpointKind kind) {
			Coord newTo, newFrom;
			var direction = Kind;

			switch (kind) {
				case EndpointKind.From:
					newFrom = new Coord(From.X + x, From.Y + y);
					newTo = To;
					break;
				case EndpointKind.To:
					newFrom = From;
					newTo = new Coord(To.X + x, To.Y + y);
					direction = From == To && y != 0 ? SegmentKind.Vertical : SegmentKind.Horizontal;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
			}
			return new LineSegment(Id, Origin, newFrom, newTo, direction);
		}
	}

	public enum SegmentKind {
		Horizontal,
		Vertical
	}

	public enum EndpointKind {
		From,
		To
	}

	public class SlopedLine : IPaintable<SlopedLine> {
		public List<LineSegment> Segments = new List<LineSegment>();
		public int Id { get; }

		public SlopedLine() {
			Id = PaintAbles.Id++;
		}

		public SlopedLine(int id, List<LineSegment> segments) {
			Id = id;
			Segments = segments;
		}

		public SlopedLine Move(int x, int y) {
			throw new NotImplementedException();
		}

		public SlopedLine AutoRoute() {
			return null; // return a new shortest path from start to end
		}

		public SlopedLine Drag(Coord dragFrom, Coord dragTo) {
			return DragAnArrowLinePiece(dragFrom, dragTo).MatchUnsafe(x => x, () => this);
		}

		public Option<SlopedLine> DragAnArrowLinePiece(Coord dragFrom, Coord dragTo) {
			var endpoints = MatchEndpoint(dragFrom).ToList();
			if (endpoints.Any()) {
				//if (endpoints.Count == 1)
				{
					var newList = Segments.ToList();
					var match = endpoints.First();
					var deltaX = dragTo.X - dragFrom.X;
					var deltaY = dragTo.Y - dragFrom.Y;

					for (int i = match.Item1; i < newList.Count; i++)
						newList[i] = newList[i].ExtendEndpoint(deltaX, deltaY, match.Item2);
					return new SlopedLine(Id, newList);
				}
			}

			int pos = -1;
			var matchedSegment = Segments.FirstOrDefault(s => IsPointPartOfLine(s.From, s.To, dragFrom), p => pos = p);
			var noLinesAreHit = matchedSegment == null;
			if (noLinesAreHit)
				return null;

			var isMoveWithinLine = IsPointPartOfLine(matchedSegment.From, matchedSegment.To, dragTo);
			if (isMoveWithinLine)
				return null;
			return null;
		}


		private IEnumerable<Tuple<int, EndpointKind>> MatchEndpoint(Coord dragFrom) {
			for (int i = 0; i < Segments.Count; i++) {
				var segment = Segments[i];

				if (segment.From == segment.To) {
					if (segment.To == dragFrom) {
						yield return Tuple.Create(i, EndpointKind.To);
					}
				}
				else {
					if (segment.From == dragFrom)
						yield return Tuple.Create(i, EndpointKind.From);
					if (segment.To == dragFrom && segment.From != segment.To)
						yield return Tuple.Create(i, EndpointKind.To);
				}
			}
		}

		private bool IsPointPartOfLine(Coord lineFrom, Coord lineTo, Coord point) {
			if (lineFrom.X <= point.X && point.X <= lineTo.X
			    && lineFrom.Y <= point.Y && point.Y <= lineTo.Y)
				return true;
			return false;
		}
	}


	public class Line : IPaintable<Line> {
		public int Id { get; }

		public int FromId { get; set; }
		public Direction? RequiredFromPosition { get; set; }
		public int? RequiredFromOffset { get; set; }

		public int ToId { get; set; }
		public Direction? RequiredToPosition { get; set; }
		public int? RequiredToOffset { get; set; }

		public Line() {
			Id = PaintAbles.Id++;
		}

		public Line Move(int x, int y) {
			throw new NotImplementedException();
		}
	}

	public class Label : IPaintable<Label>, ISelectable {
		public int Id { get; }
		public int X { get; set; }
		public int Y { get; set; }
		public string Text { get; set; }
		public LabelDirection Direction { get; set; }

		public Label() {
			Id = PaintAbles.Id++;
		}

		public Label(int id, int x, int y, string text, LabelDirection direction) {
			Id = id;
			X = x;
			Y = y;
			Text = text;
			Direction = direction;
		}

		public Label Move(int x, int y) {
			return new Label(Id, X + x, Y + y, Text, Direction);
		}

		public Label Rotate() {
			return new Label(Id, X, Y, Text, (LabelDirection) ((1 + (int) Direction)%2));
		}
	}

	public interface IResizeable<out T> {
		T Resize(int width, int height);
	}

	public enum BoxFramePart {
		NWCorner,
		Horizontal,
		Vertical,
		NECorner,
		SWCorner,
		SECorner
	}

	public class Box : IPaintable<Box>, ISelectable, IResizeable<Box> {
		public int X { get; set; }
		public int W { get; set; }
		public int Y { get; set; }
		public int H { get; set; }

		private string text;

		public string Text {
			get { return text; }
			set {
				text = value;
				var rows = value.Split('\n');

				var requiredWidth = rows.Select(x => x.Length).Max() + 4;
				if (W < requiredWidth)
					W = requiredWidth;

				var requiredHeight = 2 + rows.Length;
				if (H < requiredHeight)
					H = requiredHeight;
			}
		}

		public Box() {
			Id = PaintAbles.Id++;
			H = 1;
			W = 1;
		}

		private Box(int x, int w, int y, int h) {
			Id = -3;
			X = x;
			W = w;
			Y = y;
			H = h;
		}

		public Box(int id, int x, int w, int y, int h, string text) {
			Id = id;
			X = x;
			W = w;
			Y = y;
			H = h;
			Text = text;

			Check();
		}

		public int Id { get; set; }

		public Box Move(int x, int y) {
			return new Box(Id, X + x, W, Y + y, H, Text);
		}

		public Box Resize(int width, int height) {
			return new Box(Id, X, W + width, Y, H + height, Text);
		}

		public void Check() {
			if (H < 3)
				throw new ArgumentException("Height must be at least 3");
			if (W < 2)
				throw new ArgumentException("Width must be at least 2");
		}

		static Dictionary<string, Coord[]> CacheFrames = new Dictionary<string, Coord[]>();

		public static Coord[] GetFrameCoords(int x, int y, int h, int w) {
			Coord[] res;
			var key = "" + x + y + h + w;
			if (!CacheFrames.TryGetValue(key, out res))
				CacheFrames[key] = res = CalcFrameCoords(x, y, h, w);
			return res;
		}

		public static Tuple<Coord, BoxFramePart>[] GetFrameParts(int x, int y, int h, int w) {
			if (h == 1 && w == 1)
				throw new ArgumentException("box too small to be painted");

			List<Tuple<Coord, BoxFramePart>> coords = new List<Tuple<Coord, BoxFramePart>>();

			coords.Add(Tuple.Create(new Coord(x, y), BoxFramePart.NWCorner));
			coords.Add(Tuple.Create(new Coord(x + w - 1, y), BoxFramePart.NECorner));
			coords.Add(Tuple.Create(new Coord(x, y + h - 1), BoxFramePart.SWCorner));
			coords.Add(Tuple.Create(new Coord(x + w - 1, y + h - 1), BoxFramePart.SECorner));

			// top + bottom 
			for (int i = 1; i < w - 1; i++) {
				coords.Add(Tuple.Create(new Coord(x + i, y), BoxFramePart.Horizontal));
				coords.Add(Tuple.Create(new Coord(x + i, y + h - 1), BoxFramePart.Horizontal));
			}

			// frame
			for (int i = 1; i < h - 1; i++) {
				coords.Add(Tuple.Create(new Coord(x, y + i), BoxFramePart.Vertical));
				coords.Add(Tuple.Create(new Coord(x + w - 1, y + i), BoxFramePart.Vertical));
			}

			return coords.Where(c => !c.Item1.IsAnyNegative()).ToArray();
		}

		public static Coord[] CalcFrameCoords(int x, int y, int h, int w) {
			if (h == 1 && w == 1) {
				var coord = new Coord(x, y);
				if (coord.IsAnyNegative())
					return new Coord[0];
				return new[] {coord};
			}

			List<Coord> coords = new List<Coord>();

			// top + bottom 
			for (int i = 0; i < w; i++) {
				coords.Add(new Coord(x + i, y));
				coords.Add(new Coord(x + i, y + h - 1));
			}

			// frame
			for (int i = 1; i < h - 1; i++) {
				coords.Add(new Coord(x, y + i));
				coords.Add(new Coord(x + w - 1, y + i));
			}

			return coords.Where(c => !c.IsAnyNegative()).ToArray();
		}

		public Coord[] GetFrameCoords() {
			return Box.GetFrameCoords(X, Y, H, W);
		}

		public Tuple<Coord, BoxFramePart>[] GetFrameParts() {
			return Box.GetFrameParts(X, Y, H, W);
		}
	}
}