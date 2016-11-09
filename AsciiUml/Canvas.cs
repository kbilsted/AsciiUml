using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiUml {
	public class Canvass {
		public int?[,] Occupants = new int?[40, 80];

		public List<char[]> Lines = new List<char[]>();

		public Canvass() {
			for (int i = 0; i < 39; i++) {
				Lines.Add(new char[80]);
			}
		}

		public bool IsCellFree(int x, int y) {
			if (y > Lines.Count)
				return false; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			var line = Lines[y];
			if (x > line.Length)
				return false; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			//Console.WriteLine($"{x},{y}::{(int)line[x]}");
			return line[x] != '*'; // TODO change semantics so we know what occupies the cell..ie. box/label/..
		}

		public void Paint(int x, int y, char c, int objectId) {
			if (y > Lines.Count || y < 0)
				return; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			var line = Lines[y];
			if (x > line.Length || x < 0)
				return; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			Lines[y][x] = c;
			var isCursor = objectId == -1;
			if (!isCursor)
				Occupants[y, x] = objectId;
		}

		public static string NilToSpace(char[] c) {
			return new string(c.Select(x => x == 0 ? ' ' : x).ToArray());
		}

		public override string ToString() {
			var sb = new StringBuilder();
			Lines.Each(x => sb.AppendLine(NilToSpace(x)));
			return sb.ToString();
		}

		public string TrimEndToString() {
			var asLines = Lines.Select(x => NilToSpace(x)).ToList();
			var lastLineWithContent = asLines.FindLastIndex(x => x.Trim().Any());

			var sb = new StringBuilder();
			asLines
				.Where((s, line) => line <= lastLineWithContent)
				.Each(x => sb.AppendLine(x.TrimEnd(' ')));

			return sb.ToString().Trim();
		}
	}
}