using System;
using System.Collections.Generic;
using System.Linq;

namespace AsciiUml {
	public static class PaintServiceCore {
		public static Canvass Paint(State state, params IPaintable<object>[] model) {
			var canvas = PaintModel(model.Concat(state.Model).ToList());

			return canvas;
		}

		public static Canvass PaintModel(List<IPaintable<object>> model) {
			var c = new Canvass();

			model.OfType<Box>().Each(x => PaintBox(c, x));

			// lines may not cross boxes, hence drawn afterwards
			//model.OfType<Line>().Each(x => PaintLine(c, x, model));
			model.OfType<Line>().Each(x => PaintLine2(c, x, model));
			model.OfType<SlopedLine>().Each(x => PaintSlopedLine(c, x));
			model.OfType<SlopedLine2>().Each(x => PaintSlopedLine2(c, x));
			// labels may cross lines and be printed over lines
			model.OfType<Label>().Each(x => PaintLabel(c, x));

			return c;
		}

		private static void PaintSlopedLine2(Canvass canvass, SlopedLine2 slopedLine2) {
			slopedLine2.Segments.Each((segment, i) => {
				char c = GetLineChar(slopedLine2.GetDirectionOf(i), segment.Type);
				PaintLineOrCross(canvass, segment.Pos, c, slopedLine2.Id);
			});
		}

		private static char GetLineChar(LineDirection direction, SegmentType segmentType) {
			switch (segmentType) {
				case SegmentType.Line:
					switch (direction) {
						case LineDirection.East:
						case LineDirection.West:
							return '-';
						case LineDirection.North:
						case LineDirection.South:
							return '|';
						default:
							throw new ArgumentOutOfRangeException();
					}
				case SegmentType.Slope:
					return '+';
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static void PaintSlopedLine(Canvass canvass, SlopedLine slopedLine) {
			foreach (var segment in slopedLine.Segments) {
				char c = GetLineChar(segment.Direction, segment.Type);

				var delta = Math.Abs(segment.From.X - segment.To.X);
				int direction;
				direction = segment.From.X < segment.To.X ? 1 : -1;
				for (int i = 0; i <= delta; i++) {
					var newPos = new Coord(segment.From.X + (i*direction), segment.From.Y);
					PaintLineOrCross(canvass, newPos, c, segment.Id);
				}

				direction = segment.From.Y < segment.To.Y ? 1 : -1;
				delta = Math.Abs(segment.From.Y - segment.To.Y);
				for (int i = 0; i <= delta; i++) {
					var newPos = new Coord(segment.From.X, segment.From.Y + (i*direction));
					PaintLineOrCross(canvass, newPos, c, segment.Id);
				}
			}
		}

		private static void PaintLineOrCross(Canvass canvass, Coord newPos, char c, int id) {
			if ((canvass.GetCell(newPos) == '-' && c == '|') || (canvass.GetCell(newPos) == '|' && c == '-'))
				canvass.Paint(newPos, '+', id);
			else
				canvass.Paint(newPos, c, id);
		}

		private static void PaintLabel(Canvass canvass, Label label) {
			var lines = label.Text.Split('\n');

			switch (label.Direction) {
				case LabelDirection.LeftToRight:
					lines.Each((line, extraY) => Canvass.PaintString(canvass, line, label.X, label.Y + extraY, label.Id));
					break;

				case LabelDirection.TopDown:
					int extraX = 0;
					foreach (var line in lines) {
						for (int i = 0; i < line.Length; i++)
							canvass.Paint(new Coord(label.X + extraX, label.Y + i), line[i], label.Id);
						extraX++;
					}
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void PaintBox(Canvass c, Box b) {
			b.GetFrameCoords().Each(pos => c.Paint(pos, '*', b.Id));
			const int padX = 2, padY = 1; // TODO make padding configurable pr. box
			if (!string.IsNullOrWhiteSpace(b.Text)) {
				b.Text.Split('\n').Each((text, i) => Canvass.PaintString(c, text, b.X + padX, b.Y + padY + i, b.Id));
			}
		}

		public static char CalculateDirectionLine(Coord previous, Coord point, Coord next) {
			if (previous.X == point.X) {
				return point.X == next.X ? '|' : '+';
			}

			if (previous.Y == point.Y) {
				return point.Y == next.Y ? '-' : '+';
			}

			if (previous.X < point.X && previous.Y < point.Y) {
				return '\\';
			}
			if (previous.X < point.X && previous.Y > point.Y) {
				return '/';
			}
			if (previous.X > point.X && previous.Y < point.Y) {
				return '/';
			}
			if (previous.X > point.X && previous.Y > point.Y) {
				return '\\';
			}

			throw new ArgumentException("Cannot find a direction");
		}

		public static char CalculateDirectionArrowHead(Coord previous, Coord point) {
			if (previous.X == point.X) {
				return previous.Y > point.Y ? '^' : 'v';
			}
			if (previous.Y == point.Y) {
				return previous.X > point.X ? '<' : '>';
			}

			// diagonal lines
			return previous.X < point.X ? '>' : '<';
		}

		public static void PaintLine2(Canvass c, Line l, List<IPaintable<object>> model) {
			var fromBox = (Box) model.First(x => x.Id == l.FromId);
			var toBox = (Box) model.First(x => x.Id == l.ToId);
			var smallestDist = CalcStartAndEndSmallestDist(fromBox, toBox);

			var line = ShortestPathFinder.Calculate(smallestDist.Min, smallestDist.Max, c);
			if (line.Count < 2) {
				return;
			}

			Coord coord;

			// dont draw first nor 2-last elements. First/last elements are box-frames
			int i = 1;
			for (; i < line.Count - 2; i++) {
				coord = line[i];
				var lineChar = CalculateDirectionLine(line[i - 1], coord, line[i + 1]);
				c.Paint(coord, lineChar, l.Id);
			}

			// secondlast element is the arrow head
			coord = line[i];
			c.Paint(coord, CalculateDirectionArrowHead(line[i - 1], coord), l.Id);
		}

		public static Coord[] CalculateBoxOutline(Coord b) {
			return Box.GetFrameCoords(b.X - 1, b.Y - 1, 3, 3);
		}

		public static Coord[] CalculateBoxOutline(Box b) {
			return Box.GetFrameCoords(b.X - 1, b.Y - 1, b.H + 2, b.W + 2);
		}

		public static Range<Coord> CalcStartAndEndSmallestDist(Box fromBox, Box toBox) {
			var froms = fromBox.GetFrameCoords();
			var tos = toBox.GetFrameCoords();

			return CalcSmallestDist(froms, tos);
		}

		public static int ManhattenDistance(Coord a, Coord b) {
			var dist = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
			return dist;
		}

		public static Range<Coord> CalcSmallestDist(Coord[] froms, Coord[] tos) {
			double smallestDist = int.MaxValue;
			Range<Coord> minDist = null;
			foreach (var pointFrom in froms) {
				foreach (var pointTo in tos) {
					var dist = ManhattenDistance(pointFrom, pointTo);
					if (dist < smallestDist) {
						smallestDist = dist;
						minDist = new Range<Coord>(pointFrom, pointTo);
					}
				}
			}

			if (minDist == null)
				throw new ArgumentException("no minimum distance");

			return minDist;
		}
	}
}