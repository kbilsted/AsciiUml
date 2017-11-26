using System;
using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public static class Vector {
		public static readonly Coord DeltaNorth = new Coord(0, -1);
		public static readonly Coord DeltaSouth = new Coord(0, 1);
		public static readonly Coord DeltaEast = new Coord(1, 0);
		public static readonly Coord DeltaWest = new Coord(-1, 0);

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
			if (from == to)
				return LineDirection.East;
			if (from.X < to.X)
				return LineDirection.East;
			if (from.X > to.X)
				return LineDirection.West;
			return from.Y < to.Y ? LineDirection.South : LineDirection.North;
		}

		public static bool IsOrthogonal(LineDirection d1, LineDirection d2) {
			return d1 == LineDirection.East || d1 == LineDirection.West
				? d2 == LineDirection.North || d2 == LineDirection.South
				: d2 == LineDirection.East || d2 == LineDirection.West;
		}

		public static LineDirection GetOppositeDirection(LineDirection direction) {
			switch (direction) {
				case LineDirection.North:
					return LineDirection.South;
				case LineDirection.South:
					return LineDirection.North;
				case LineDirection.East:
					return LineDirection.West;
				case LineDirection.West:
					return LineDirection.East;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}
	}
}