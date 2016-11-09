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
			model.OfType<SlopedLine>().Each(x => PaintSlopedLine(c, x, model));
			// labels may cross lines and be printed over lines
			model.OfType<Label>().Each(x => PaintLabel(c, x));

			return c;
		}

		private static void PaintSlopedLine(Canvass canvass, SlopedLine slopedLine, List<IPaintable<object>> model) {
			foreach (var segment in slopedLine.Segments) {
				char xx;
				switch (segment.Kind) {
					case SegmentKind.Horizontal:
						xx = '-';
						break;
					case SegmentKind.Vertical:
						xx = '|';
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				var delta = Math.Abs(segment.From.X - segment.To.X);
				for (int i = 0; i <= delta; i++)
					canvass.Paint(segment.From.X + i, segment.From.Y, xx, segment.Id);

				delta = Math.Abs(segment.From.Y - segment.To.Y);
				for (int i = 0; i <= delta; i++)
					canvass.Paint(segment.From.X, segment.From.Y + i, xx, segment.Id);
			}
		}

		private static void PaintLabel(Canvass canvass, Label label) {
			var lines = label.Text.Split('\n');

			switch (label.Direction) {
				case LabelDirection.LeftToRight:
					int extraY = 0;
					foreach (var line in lines) {
						for (int i = 0; i < line.Length; i++)
							canvass.Paint(label.X + i, label.Y + extraY, line[i], label.Id);
						extraY++;
					}
					break;

				case LabelDirection.TopDown:
					int extraX = 0;
					foreach (var line in lines) {
						for (int i = 0; i < line.Length; i++)
							canvass.Paint(label.X + extraX, label.Y + i, line[i], label.Id);
						extraX++;
					}
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void PaintString(Canvass c, string s, int x, int y, int objectId) {
			for (int i = 0; i < s.Length; i++) {
				c.Paint(x + i, y, s[i], objectId);
			}
		}

		public static void PaintBox(Canvass c, Box b) {
			b.GetFrameCoords().Each(pos => c.Paint(pos.X, pos.Y, '*', b.Id));

			if (!string.IsNullOrWhiteSpace(b.Text)) {
				b.Text.Split('\n')
					.Each((text, i) => PaintString(c, text, b.X + 2, b.Y + 1 + i, b.Id));
			}
		}

		public static char CalculateDirectionLine(Coord previous, Coord point, Coord next) {
			if (previous.X == point.X)
				return point.X == next.X ? '|' : '+';

			if (previous.Y == point.Y)
				return point.Y == next.Y ? '-' : '+';

			if (previous.X < point.X && previous.Y < point.Y)
				return '\\';
			if (previous.X < point.X && previous.Y > point.Y)
				return '/';
			if (previous.X > point.X && previous.Y < point.Y)
				return '/';
			if (previous.X > point.X && previous.Y > point.Y)
				return '\\';

			throw new ArgumentException("Cannot find a direction");
		}

		public static char CalculateDirectionArrowHead(Coord previous, Coord point) {
			if (previous.X == point.X)
				return previous.Y > point.Y ? '^' : 'v';
			if (previous.Y == point.Y)
				return previous.X > point.X ? '<' : '>';
			if (previous.X < point.X && previous.Y < point.Y)
				return '>';
			if (previous.X < point.X && previous.Y > point.Y)
				return '>';
			if (previous.X > point.X && previous.Y < point.Y)
				return '<';
			if (previous.X > point.X && previous.Y > point.Y)
				return '<';
			throw new ArgumentException("Cannot find a direction");
		}

		public static void PaintLine2(Canvass c, Line l, List<IPaintable<object>> model) {
			var fromBox = (Box) model.First(x => x.Id == l.FromId);
			var toBox = (Box) model.First(x => x.Id == l.ToId);
			var smallestDist = CalcStartAndEndSmallestDist(fromBox, toBox);

			var line = ShortestPathFinder.Calculate(smallestDist.Min, smallestDist.Max, c);
			if (line.Count < 2)
				return;

			Coord coord;

			// dont draw first nor 2-last elements. First/last elements are box-frames
			int i = 1;
			for (; i < line.Count - 2; i++) {
				coord = line[i];
				var lineChar = CalculateDirectionLine(line[i - 1], coord, line[i + 1]);
				c.Paint(coord.X, coord.Y, lineChar, l.Id);
			}

			// secondlast element is the arrow head
			coord = line[i];
			c.Paint(coord.X, coord.Y, CalculateDirectionArrowHead(line[i - 1], coord), l.Id);
		}


		public static void PaintLine(Canvass c, Line l, List<IPaintable<object>> model) {
			var fromBox = (Box) model.First(x => x.Id == l.FromId);
			var toBox = (Box) model.First(x => x.Id == l.ToId);
			var smallestDist = CalcStartAndEndSmallestDist(fromBox, toBox);

			var line = ShortestPathFinder.TooOptimisticPathFromPointToPoint(smallestDist.Min, smallestDist.Max, c);
			if (line.Count < 2)
				return;

			Coord coord;

			// dont draw first nor 2-last elements. First/last elements are box-frames
			int i = 1;
			for (; i < line.Count - 2; i++) {
				coord = line[i];
				c.Paint(coord.X, coord.Y, CalculateDirectionLine(line[i - 1], coord, line[i + 1]), l.Id);
			}

			// secondlast element is the arrow head
			coord = line[i];
			c.Paint(coord.X, coord.Y, CalculateDirectionArrowHead(line[i - 1], coord), l.Id);
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

		public static double Euclid(Coord a, Coord b) {
			var dist = Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2);
			return dist;
		}

		public static int FastEuclid(Coord a, Coord b) {
			var dist = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
			return dist;
		}

		public static Range<Coord> CalcSmallestDist(Coord[] froms, Coord[] tos) {
			double smallestDist = int.MaxValue;
			Range<Coord> minDist = null;
			foreach (var pointFrom in froms) {
				foreach (var pointTo in tos) {
					var dist = FastEuclid(pointFrom, pointTo);
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