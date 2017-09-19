using System;
using System.Collections.Generic;

namespace AsciiUml {
	public class Coord : IEquatable<Coord> {
		public readonly int X, Y;

		public Coord(int x, int y) {
			X = x;
			Y = y;
		}

		public Coord Move(Coord c) {
			return new Coord(X + c.X, Y + c.Y);
		}

		public override string ToString() {
			return $"x:{X}, y:{Y}";
		}

		public static bool operator ==(Coord a, Coord b) {
			return a.Equals(b);
		}

		public static bool operator !=(Coord a, Coord b) {
			return !(a == b);
		}

		public override bool Equals(object obj) {
			return Equals(obj as Coord);
		}

		public bool Equals(Coord other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return X == other.X && Y == other.Y;
		}

		public override int GetHashCode() {
			unchecked {
				return (X*397) ^ Y;
			}
		}

		public bool IsAnyNegative() {
			return X < 0 || Y < 0;
		}
	}
}