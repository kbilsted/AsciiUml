using System;
using System.Collections.Generic;
using System.Linq;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.Geo {
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

	public interface IConnectable {
		Coord[] GetFrameCoords();
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

		public Cursor(Coord c) {
			Pos = c;
		}

		public int Id {
			get { return -1; }
		}

		public Cursor Move(Coord delta) {
			return new Cursor(Pos.Move(delta));
		}
		public override string ToString() {
			return $"{X},{Y}";
		}
	}

	public enum LineDirection {
		North = 1,
		South = 2,
		East = 4,
		West = 8,
	}

	public static class LineDirections {
		public static LineDirection GetDirectionFromBend(LineDirection direction, EndpointKind kind, int dragx, int dragy) {
			switch (kind) {
				case EndpointKind.From:
					switch (direction) {
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
					switch (direction) {
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

		public static LineDirection GetDirectionDragginFromPart(LineDirection direction, int dragx, int dragy) {
			switch (direction)
			{
				case LineDirection.North:
				case LineDirection.South:
					if (dragx > 0)
						return LineDirection.West;
					return dragx < 0 ? LineDirection.East : direction;
				case LineDirection.East:
				case LineDirection.West:
					if (dragy > 0)
						return LineDirection.North;
					return dragy < 0 ? LineDirection.South : direction;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}

		}

		public static LineDirection GetDirectionFromBend2(LineDirection direction, EndpointKind kind, int dragx, int dragy) {
			var resultingDirection = GetDirectionDragginFromPart(direction, dragx, dragy);
			return kind == EndpointKind.From ? resultingDirection : Vector.GetOppositeDirection(resultingDirection);
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

	public class Label : IPaintable<Label>, ISelectable, IConnectable
	{
		public int Id { get; }
		public int X => Pos.X;
		public int Y => Pos.Y;
		public string Text { get; }
		public Coord Pos { get; }
		public LabelDirection Direction { get; }

		public Label(string text) : this(new Coord(0, 0), text) {}

		public Label(Coord pos, string text) {
			Id = PaintAbles.Id++;
			Text = text;
			Pos = pos;
		}

		public Label(int id, Coord pos, string text, LabelDirection direction) {
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

		public Coord[] GetFrameCoords() {
			var strings = Text.Split('\n');
			return RectangleHelper.GetFrameCoords(Pos.X, Pos.Y, strings.Length, strings.Max(x => x.Length));
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

	public static class RectangleHelper {
		private static readonly Dictionary<string, Coord[]> CacheFrames = new Dictionary<string, Coord[]>();

		public static Tuple<Coord, BoxFramePart>[] GetFrameParts(int x, int y, int h, int w) {
			if (h == 1 && w == 1)
				return new[] {Tuple.Create(new Coord(x, y), BoxFramePart.NWCorner)};

			var coords = new List<Tuple<Coord, BoxFramePart>>(2*h*w) {
				Tuple.Create(new Coord(x, y), BoxFramePart.NWCorner),
				Tuple.Create(new Coord(x + w - 1, y), BoxFramePart.NECorner),
				Tuple.Create(new Coord(x, y + h - 1), BoxFramePart.SWCorner),
				Tuple.Create(new Coord(x + w - 1, y + h - 1), BoxFramePart.SECorner)
			};

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


		public static Coord[] GetFrameCoords(int x, int y, int h, int w) {
			Coord[] res;
			var key = "" + x + y + h + w;
			if (!CacheFrames.TryGetValue(key, out res)) {
				CacheFrames[key] = res = GetFrameParts(x, y, h, w).Select(c => c.Item1).ToArray();
			}
			return res;
		}
	}
}