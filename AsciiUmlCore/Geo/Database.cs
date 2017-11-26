using System;
using System.Collections.Generic;
using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public class Database : IPaintable<Database>, ISelectable {
		public const int Width = 9;
		public const int Height = 8;


		private readonly string definition = @"  _____
 -     -
|-_____-|
|       |
|       |
 -_____-";


		public Database(Coord pos) {
			Pos = pos;
			Id = PaintAbles.GlobalId++;
		}

		private Database(int id, Coord pos) {
			Pos = pos;
			Id = id;
		}

		public int Id { get; }

		public Database Move(Coord delta) {
			return new Database(Id, Pos.Move(delta));
		}

		public Coord Pos { get; }

		public IEnumerable<Tuple<Coord, char, int>> Paint() {
			var pos = Pos;
			foreach (var c in definition) {
				if (c == '\r')
					continue;
				if (c == '\n') {
					pos = new Coord(Pos.X, pos.Y + 1);
					continue;
				}
				yield return Tuple.Create(pos, c, Id);
				pos = pos.Move(new Coord(1, 0));
			}
		}
	}
}