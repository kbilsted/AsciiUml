using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsciiUml.Geo;

namespace AsciiUml.UI {
	public class Canvass {
		readonly int?[,] occupants = new int?[40, 80];

		readonly List<char[]> lines = new List<char[]>();

		public Canvass() {
			for (int i = 0; i < 39; i++) {
				lines.Add(new char[80]);
			}
		}

		public Tuple<int, int> GetSize() {
			return Tuple.Create(lines.Count, lines.First().Length);
		}

		public int? GetOccupants(Coord pos) {
			return occupants[pos.Y, pos.X];
		}

		public char GetCell(Coord pos) {
			return lines[pos.Y][pos.X];
		}

		public bool IsCellFree(Coord pos) {
			int x = pos.X, y = pos.Y;
			if (y > lines.Count)
				return false; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			var line = lines[y];
			if (x > line.Length)
				return false; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			//Console.WriteLine($"{x},{y}::{(int)line[x]}");
			return line[x] != '*'; // TODO change semantics so we know what occupies the cell..ie. box/label/..
		}

		public void Paint(Coord pos, char c, int objectId) {
			Paint(pos.X, pos.Y, c, objectId);
		}

		public void Paint(int x, int y, char c, int objectId) {
			if (y > lines.Count || y < 0)
				return; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			var line = lines[y];
			if (x > line.Length || x < 0)
				return; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			lines[y][x] = c;
			var isCursor = objectId == -1;
			if (!isCursor)
				occupants[y, x] = objectId;
		}

		public static void PaintString(Canvass c, string s, int x, int y, int objectId) {
			for (int i = 0; i < s.Length; i++)
				c.Paint(new Coord(x + i, y), s[i], objectId);
		}

		public static string NilToSpace(char[] c) {
			return new string(c.Select(x => x == 0 ? ' ' : x).ToArray());
		}

		public override string ToString() {
			var sb = new StringBuilder();
			lines.Each(x => sb.AppendLine(NilToSpace(x)));
			return sb.ToString();
		}

		public string TrimEndToString() {
			var asLines = lines.Select(x => NilToSpace(x)).ToList();
			var lastLineWithContent = asLines.FindLastIndex(x => x.Trim().Any());

			var sb = new StringBuilder();
			asLines
				.Where((s, line) => line <= lastLineWithContent)
				.Each(x => sb.AppendLine(x.TrimEnd(' ')));

			return sb.ToString().Trim();
		}
	}
}