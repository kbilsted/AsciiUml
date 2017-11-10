using System;
using System.Linq;
using System.Text;

namespace AsciiConsoleUi {
	// TODO REMOVE
	internal static class State {
		public const int MaxX = 80, MaxY = 40;
	}

	public class Canvass {
		readonly int?[,] occupants = new int?[State.MaxY, State.MaxX]; // TODO we should enable more than one object on the same pixel

		public readonly Pixel[][] Catode = new Pixel[State.MaxY][];

		public Canvass() {
			for (int i = 0; i < State.MaxY; i++) {
				Catode[i] = new Pixel[State.MaxX];
			}
		}

		public Tuple<int, int> GetSize() {
			return Tuple.Create(State.MaxY, State.MaxX);
		}

		public int? GetOccupants(Coord pos) {
			var occupant = occupants[pos.Y, pos.X];
			return occupant;
		}

		public char? GetCell(Coord pos) {
			var pixel = Catode[pos.Y][pos.X];
			return pixel?.Char;
		}

		public bool IsCellFree(Coord pos) {
			int x = pos.X, y = pos.Y;

			if (x < 0 || y < 0)
				return false;

			if (y >= Catode.Length)
				return false; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			if (x >= Catode[0].Length)
				return false; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			//Console.WriteLine($"{x},{y}::{(int)line[x]}");
			var cell = Catode[y][x];
			return cell == null || cell.Char != '*'; // TODO change semantics so we know what occupies the cell..ie. box/label/..
		}

		public void Paint(Coord pos, char c, int objectId) {
			Paint(pos.X, pos.Y, c, objectId, ConsoleColor.Black, ConsoleColor.Gray);
		}

		public void Paint(int x, int y, char c, int objectId, ConsoleColor backgroundColor, ConsoleColor foregroundColor) {
			if (y > Catode.Length || y < 0)
				return; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			if (x > Catode[0].Length || x < 0)
				return; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			Catode[y][x] = new Pixel() {Char = c, BackGroundColor = backgroundColor, ForegroundColor = foregroundColor};
			var isCursor = objectId == -1;
			if (!isCursor)
				occupants[y, x] = objectId;
		}

		public void RawPaintString(string s, int x, int y, ConsoleColor background, ConsoleColor foreground) {
			PaintString(this, s, x, y, -10, background, foreground);
		}

		public void RawPaintString(string s, Coord pos, ConsoleColor background, ConsoleColor foreground) {
			PaintString(this, s, pos.X, pos.Y, -10, background, foreground);
		}

		public static void PaintString(Canvass c, string s, int x, int y, int objectId) {
			PaintString(c, s, x, y, objectId, ConsoleColor.Black, ConsoleColor.Gray);
		}

		public static void PaintString(Canvass c, string s, int x, int y, int objectId, ConsoleColor backgroundColor,
			ConsoleColor foregroundColor) {
			for (int i = 0; i < s.Length; i++)
				c.Paint(x + i, y, s[i], objectId, backgroundColor, foregroundColor);
		}

		public static string NilToSpace(char[] c) {
			return new string(c.Select(x => x == 0 ? ' ' : x).ToArray());
		}

		public override string ToString() {
			var sb = new StringBuilder();
			Catode.Each(x => sb.AppendLine(NilToSpace(x.Select(y => y.Char).ToArray())));
			return sb.ToString();
		}

		public string TrimEndToString() {
			var asLines = Catode.Select(row => NilToSpace(row.Select(y => y == null ? ' ' : y.Char).ToArray())).ToList();
			var lastLineWithContent = asLines.FindLastIndex(x => x.Trim().Any());

			var sb = new StringBuilder();
			asLines
				.Where((s, line) => line <= lastLineWithContent)
				.Each(x => sb.AppendLine(x.TrimEnd(' ')));

			return sb.ToString().Trim();
		}
	}
}