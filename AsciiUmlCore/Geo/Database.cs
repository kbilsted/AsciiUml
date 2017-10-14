using System;
using System.Collections.Generic;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.Geo {
	public class Database : IPaintable<Database>, ISelectable {
		public int Id { get; }

		public Coord Pos { get; }


		private string definition = @"  _____
 -     -
|-_____-|
|       |
|       |
 -_____-";

		public const int Width = 9;
		public const int Height = 8;


		public Database(Coord pos)
		{
			Pos = pos;
			Id = PaintAbles.Id++;
		}
		private Database(int id, Coord pos)
		{
			Pos = pos;
			Id = id;
		}

		public IEnumerable<Tuple<Coord, char, int>> Paint() {
			Coord pos = Pos;
			foreach (var c in definition) {
				if(c=='\r')
					continue;
				if (c == '\n') {
					pos = new Coord(Pos.X, pos.Y+1);
					continue;
				}
				yield return Tuple.Create(pos, c, Id);
				pos = pos.Move(new Coord(1,0));
			}
		}

		public Database Move(Coord delta) {
			return new Database(Id, Pos.Move(delta));
		}
	}
}