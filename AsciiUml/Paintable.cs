using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt;
using LanguageExt.SomeHelp;
using AsciiUml;

namespace AsciiUml {
	static class PaintAbles {
		public static int Id { get; set; }
	}

	public interface IPaintable<out T> {
		int Id { get; }
		T Move(Coord delta);
	}

	public interface ISelectable {
		int Id { get; }
		Coord Pos { get; }
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
		public readonly Coord Pos;
		public int X => Pos.X;
		public int Y => Pos.Y;

		public Cursor(Coord c)
		{
			Pos = c;
		}

		public int Id {
			get { return -1; }
		}

		public Cursor Move(Coord delta) {
			return new Cursor(Pos.Move(delta));
		}
	}

	public enum LineDirection {
		North = 1,
		South = 2,
		East = 4,
		West = 8,
	}

	public static class Vector {
		public static bool IsDirectionOpposite(LineDirection d1, LineDirection d2) {
			return d1 == LineDirection.North && d2 == LineDirection.South
				|| d1 == LineDirection.South && d2 == LineDirection.North
				|| d1 == LineDirection.East && d2 == LineDirection.West
				|| d1 == LineDirection.West && d2 == LineDirection.East;
		}

		public static bool IsStraightLine(Coord from, Coord to) {
			return from.X == to.X || from.Y == to.Y;
		}

		public static LineDirection GetDirection(Coord from, Coord to) {
			if (@from == to)
				return LineDirection.East;
			if (@from.X < to.X)
				return LineDirection.East;
			if (@from.X > to.X)
				return LineDirection.West;
			return @from.Y < to.Y ? LineDirection.South : LineDirection.North;
		}

		public static bool IsOrthogonal(LineDirection d1, LineDirection d2)
		{
			return d1 == LineDirection.East || d1 == LineDirection.West
				? d2 == LineDirection.North || d2 == LineDirection.South
				: d2 == LineDirection.East || d2 == LineDirection.West;
		}

		public static LineDirection GetOppositeDirection(LineDirection direction) {
			switch (direction) {
				case LineDirection.North: return LineDirection.South;
				case LineDirection.South: return LineDirection.North;
				case LineDirection.East: return LineDirection.West;
				case LineDirection.West: return LineDirection.East;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public static readonly Coord DeltaNorth = new Coord(0, -1);
		public static readonly Coord DeltaSouth = new Coord(0, 1);
		public static readonly Coord DeltaEast = new Coord(1, 0);
		public static readonly Coord DeltaWest = new Coord(-1, 0);
	}

	public static class LineDirections {
		public static LineDirection GetDirectionFromBend(LineDirection direction, EndpointKind kind, int dragx, int dragy) {
			switch (kind) {
				case EndpointKind.From:
					switch (direction)
					{
						case LineDirection.North:
						case LineDirection.South:
							if (dragx > 0)
								return LineDirection.West;
							if (dragx < 0)
								return LineDirection.East;
							return direction;
						case LineDirection.East:
						case LineDirection.West:
							if (dragy > 0)
								return LineDirection.North;
							if (dragy < 0)
								return LineDirection.South;
							return direction;
						default:
							throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
					}
				case EndpointKind.To:
					switch (direction)
					{
						case LineDirection.North:
						case LineDirection.South:
							if (dragx > 0)
								return LineDirection.East;
							if (dragx < 0)
								return LineDirection.West;
							return direction;
						case LineDirection.East:
						case LineDirection.West:
							if (dragy > 0)
								return LineDirection.South;
							if (dragy < 0)
								return LineDirection.North;
							return direction;
						default:
							throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
					}
				default:
					throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
			}
		}
	}

	public class LineSegment {
		public int Id { get; }
		public readonly Coord From, To;
		public readonly SegmentType Type;
		public readonly LineDirection Direction;
		public readonly SlopedLine Origin;

		public LineSegment(SlopedLine l, Coord from, Coord to, SegmentType type)
			: this(PaintAbles.Id++, l, from, to, type, Vector.GetDirection(from, to)) {
		}

		public LineSegment(SlopedLine l, Coord from, Coord to, SegmentType type, LineDirection direction)
			: this(PaintAbles.Id++, l, from, to, type, direction) {
		}

		public LineSegment(int id, SlopedLine l, Coord from, Coord to, SegmentType type)
			: this(id, l, from, to, type, Vector.GetDirection(from, to)) {
		}

		public LineSegment(int id, SlopedLine l, Coord from, Coord to, SegmentType type, LineDirection direction) {
			if(type==SegmentType.Slope && from!=to)
				throw new ArgumentException("Slopes can only be size 1");

			Id = id;
			Origin = l;
			From = from;
			To = to;
			Type = type;
			Direction = direction;
		}

		public LineSegment ExtendEndpoint(int x, int y, EndpointKind kind) {
			Coord newTo, newFrom;

			switch (kind) {
				case EndpointKind.From:
					newFrom = new Coord(From.X + x, From.Y + y);
					newTo = To;
					break;
				case EndpointKind.To:
					newFrom = From;
					newTo = new Coord(To.X + x, To.Y + y);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
			}
			return new LineSegment(Id, Origin, newFrom, newTo, Type);
		}

		public LineSegment Reduce(EndpointKind kind) {
			if(!IsReducable())
				throw new ArgumentException("cannot reduce line of length 1");

			switch (kind) {
				case EndpointKind.From:
					switch (Direction) {
						case LineDirection.North:
							return new LineSegment(Id, Origin, From.Move(Vector.DeltaNorth), To, Type, Direction);
						case LineDirection.South:
							return new LineSegment(Id, Origin, From.Move(Vector.DeltaSouth), To, Type, Direction);
						case LineDirection.East:
							return new LineSegment(Id, Origin, From.Move(Vector.DeltaEast), To, Type, Direction);
						case LineDirection.West:
							return new LineSegment(Id, Origin, From.Move(Vector.DeltaWest), To, Type, Direction);
						default:
							throw new ArgumentOutOfRangeException();
					}
				case EndpointKind.To:
					switch (Direction) {
						case LineDirection.North:
							return new LineSegment(Id, Origin, From, To.Move(Vector.DeltaSouth), Type, Direction);
						case LineDirection.South:
							return new LineSegment(Id, Origin, From, To.Move(Vector.DeltaNorth), Type, Direction);
						case LineDirection.East:
							return new LineSegment(Id, Origin, From, To.Move(Vector.DeltaWest), Type, Direction);
						case LineDirection.West:
							return new LineSegment(Id, Origin, From, To.Move(Vector.DeltaEast), Type, Direction);
						default:
							throw new ArgumentOutOfRangeException();
					}
				default:
					throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
			}
		}

		public bool IsReducable() {
			return From != To;
		}

		public bool SpanOneCell() {
			return From == To;
		}
	}

	public enum SegmentType {
		Line = 1,
		Slope = 2,
	}

	public enum EndpointKind {
		From = 1,
		To = 2,
	}

	public class SlopedLine2 : IPaintable<SlopedLine2> {
		public class SlopedSegment2 {
			public Coord Pos;
			public SegmentType Type;

			public SlopedSegment2(Coord pos, SegmentType type) {
				Pos = pos;
				Type = type;
			}
		}

		public enum LineSemantic
		{
			StartArrow, EndArrow,  Slope, LinePiece,
		}

		public int Id { get; }
		public readonly List<SlopedSegment2> Segments = new List<SlopedSegment2>();

		public SlopedLine2 Drag(Coord dragFrom, Coord dragTo)
		{
			return DragAnArrowLinePiece(dragFrom, dragTo).MatchUnsafe(x => x, () => this);
		}

		public LineDirection GetDirectionOf(int index)
		{
			if (index == 0) {
				if (Segments.Count > 1)
					return Vector.GetDirection(Segments[index].Pos, Segments[index + 1].Pos);
				return Vector.GetDirection(Segments[index].Pos, Segments[index].Pos);
			}
			return Vector.GetDirection(Segments[index - 1].Pos, Segments[index].Pos);
		}

		public Option<SlopedLine2> DragAnArrowLinePiece(Coord dragFrom, Coord dragTo) {
			LineSemantic s = FindOnLine(dragFrom);
			switch (s) {
				case LineSemantic.StartArrow:
					if (Segments.Count > 1 && Segments[1].Pos == dragTo) {
						Segments.RemoveAt(0);
						return this;
					}

					bool insertWhenSingularLine = Segments.Count == 1;
					var directionDragFrom = GetDirectionOf(0);

					Segments.Insert(0, new SlopedSegment2(dragTo, SegmentType.Line));
					var directionDragTo = GetDirectionOf(0);

					if (Vector.IsOrthogonal(directionDragFrom, directionDragTo) && !insertWhenSingularLine)
						Segments[1].Type = SegmentType.Slope;
					return this;

				case LineSemantic.EndArrow:
					int nthlastPos = GetLastNthLastPos(Segments, 1);
					if (Segments.Count > 1 && Segments[nthlastPos].Pos == dragTo) {
						Segments.RemoveAt(Segments.Count - 1);
						Segments[Segments.Count - 1].Type = SegmentType.Line; // ensure to convert slopes to lines
						return this;
					}

					int posAtInsert = Segments.Count - 1;
					Segments.Add(new SlopedSegment2(dragTo, SegmentType.Line));
					if (Vector.IsOrthogonal(GetDirectionOf(posAtInsert), GetDirectionOf(posAtInsert+1)))
						Segments[posAtInsert].Type = SegmentType.Slope;
					Segments[Segments.Count-1].Type = SegmentType.Line; // ensure to convert slopes to lines
					return this;

				case LineSemantic.Slope:
					break;
				case LineSemantic.LinePiece:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return null;
		}

		private int GetLastNthLastPos<T>(List<T> colList, int last)
		{
			return colList.Count - 1 - last;
		}

		private LineSemantic FindOnLine(Coord coord) {
			int pos = Segments.FindIndex(x => x.Pos == coord);
			if (pos == 0)
				return LineSemantic.StartArrow;
			if(pos == Segments.Count-1)
				return LineSemantic.EndArrow;
			if (Segments[pos].Type == SegmentType.Slope)
				return LineSemantic.Slope;
			return LineSemantic.LinePiece;
		}

		public SlopedLine AutoRoute()
		{
			return null; // return a new shortest path from start to end
		}

		public SlopedLine2 Move(int x, int y) {
			throw new NotImplementedException();
		}

		public SlopedLine2 Move(Coord delta) {
			throw new NotImplementedException();
		}
	}


	public class SlopedLine : IPaintable<SlopedLine> {
		public readonly List<LineSegment> Segments = new List<LineSegment>();
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

		public SlopedLine Move(Coord delta) {
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
					var delta = new Coord(deltaX, deltaY);

					var currentSegment = newList[match.Item1];

					if (currentSegment.Type == SegmentType.Slope) {
						newList.Insert(0, new LineSegment(this, dragTo, dragTo, SegmentType.Line));
					}
					else if (IsLineAtomic()) { //
						newList[match.Item1] = newList[match.Item1].ExtendEndpoint(deltaX, deltaY, match.Item2);
					}
					else if (IsDragDiagonalOfLine(currentSegment, dragFrom, dragTo)) {
						if(newList[match.Item1].IsReducable())
							newList[match.Item1] = currentSegment.Reduce(match.Item2);
						else 
							newList.RemoveAt(match.Item1);

						Coord slopePoint = match.Item2 == EndpointKind.From
							? currentSegment.From
							: currentSegment.To;
						newList.Insert(match.Item1, new LineSegment(this, slopePoint, slopePoint, SegmentType.Slope));

						var directionFromBend = LineDirections.GetDirectionFromBend(currentSegment.Direction, match.Item2, deltaX, deltaY);
						var newSegmentPos = match.Item2 == EndpointKind.From
							? currentSegment.From.Move(delta)
							: currentSegment.To.Move(delta);
						newList.Insert(match.Item1, new LineSegment(this, newSegmentPos, newSegmentPos, SegmentType.Line, directionFromBend));
					}
					else {
						if (currentSegment.SpanOneCell()) {
							if(Vector.IsDirectionOpposite(currentSegment.Direction, Vector.GetDirection(dragFrom, dragTo)))
								newList.RemoveAt(match.Item1);
							else
								newList[match.Item1] = newList[match.Item1].ExtendEndpoint(deltaX, deltaY, match.Item2);
						}
						else {
							for (int i = match.Item1; i < newList.Count; i++) {
								if(newList[i].Type == SegmentType.Slope)
									break;
								newList[i] = newList[i].ExtendEndpoint(deltaX, deltaY, match.Item2);
							}
						}
					}
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

		private bool IsLineAtomic() {
			return Segments.Count == 1 && Segments[0].From == Segments[0].To;
		}

		private bool IsDragDiagonalOfLine(LineSegment segment, Coord dragFrom, Coord dragTo) {
			if (segment.Direction == LineDirection.East || segment.Direction == LineDirection.West) 
				return dragFrom.Y != dragTo.Y;

			return dragFrom.X != dragTo.X;
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
					if (segment.From == dragFrom) {
						yield return Tuple.Create(i, EndpointKind.From);
					}
					if (segment.To == dragFrom && segment.From != segment.To) {
						yield return Tuple.Create(i, EndpointKind.To);
					}
				}
			}
		}

		private bool IsPointPartOfLine(Coord lineFrom, Coord lineTo, Coord point) {
			if (lineFrom.X <= point.X && point.X <= lineTo.X && lineFrom.Y <= point.Y && point.Y <= lineTo.Y) {
				return true;
			}
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

		public Line Move(Coord delta) {
			throw new NotImplementedException();
		}
	}

	public class Label : IPaintable<Label>, ISelectable {
		public int Id { get; }
		public int X => Pos.X;
		public int Y => Pos.Y;
		public string Text { get; }
		public Coord Pos { get; }
		public LabelDirection Direction { get; }

		public Label(string text) : this(new Coord(0,0), text)
		{
		}

		public Label(Coord pos, string text)
		{
			Id = PaintAbles.Id++;
			Text = text;
			Pos = pos;
		}

		public Label(int id, Coord pos , string text, LabelDirection direction)
		{
			Id = id;
			Text = text;
			Pos = pos;
			Direction = direction;
		}

		public Label Move(Coord delta) {
			return new Label(Id, Pos.Move(delta), Text, Direction);
		}

		public Label Rotate() {
			return new Label(Id, Pos, Text, (LabelDirection) ((1 + (int) Direction)%2));
		}
	}

	public interface IResizeable<out T> {
		IPaintable<object> Resize(Coord delta);
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
		public int X { get { return Pos.X; } }
		public int W { get; set; }
		public int Y { get { return Pos.Y; } }
		public int H { get; set; }
		public Coord Pos { get; }

		private string text;
		public string Text {
			get { return text; }
			set {
				text = value;
				var rows = value.Split('\n');

				var requiredWidth = rows.Select(x => x.Length).Max() + 4;
				if (W < requiredWidth) {
					W = requiredWidth;
				}

				var requiredHeight = 2 + rows.Length;
				if (H < requiredHeight) {
					H = requiredHeight;
				}
			}
		}

		public Box(Coord pos) : this(PaintAbles.Id++, pos)
		{ }

		public Box(int id, Coord pos) {
			Id = id;
			H = 1;
			W = 1;
			Pos = pos;
		}

		public Box(int id, Coord pos, int w, int h, string text)
		{
			Id = id;
			W = w;
			H = h;
			Text = text;
			Pos = pos;
			Check();
		}

		public int Id { get; }

		public Box Move(Coord delta) {
			return new Box(Id, Pos.Move(delta),  W, H, Text);
		}

		public IPaintable<object> Resize(Coord delta) {
			return Resize(delta.X, delta.Y);
		}

		public Box Resize(int width, int height) {
			return new Box(Id, Pos, W + width, H + height, Text);
		}

		public void Check() {
			if (H < 3) {
				throw new ArgumentException("Height must be at least 3");
			}
			if (W < 2) {
				throw new ArgumentException("Width must be at least 2");
			}
		}

		private static Dictionary<string, Coord[]> CacheFrames = new Dictionary<string, Coord[]>();

		public static Coord[] GetFrameCoords(int x, int y, int h, int w) {
			Coord[] res;
			var key = "" + x + y + h + w;
			if (!CacheFrames.TryGetValue(key, out res)) {
				CacheFrames[key] = res = CalcFrameCoords(x, y, h, w);
			}
			return res;
		}

		public static Tuple<Coord, BoxFramePart>[] GetFrameParts(int x, int y, int h, int w) {
			if (h == 1 && w == 1) {
				throw new ArgumentException("box too small to be painted");
			}

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
				if (coord.IsAnyNegative()) {
					return new Coord[0];
				}
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