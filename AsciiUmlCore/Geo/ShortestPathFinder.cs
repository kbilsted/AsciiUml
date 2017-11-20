using System.Collections.Generic;
using System.Linq;
using AsciiConsoleUi;
using AsciiUml.UI;
using Priority_Queue;

namespace AsciiUml.Geo {
	static class ShortestPathFinder {
		const int WeightOfTurn = 1;
		const int StepLength = 1;

		class UnhandledField {
			public readonly Coord Position;
			public readonly List<Coord> Path;
			public readonly int Distance;

			public UnhandledField(Coord position, List<Coord> path, int distance) {
				Position = position;
				Path = path;
				Distance = distance;
			}

			public override string ToString() {
				return $"{Position}::{Path.Count}";
			}
		}

		class Solution {
			public readonly List<Coord> Path;
			public readonly int Distance;

			public Solution(List<Coord> path, int distance) {
				Path = path;
				Distance = PenalizeTurns(path);
			}

			int PenalizeTurns(List<Coord> rute) {
				int distance = rute.Count;

				for (int i = 0; i < rute.Count - 3; i++)
					if (IsTurn(rute[i], rute[i + 1], rute[i + 2]))
						distance = distance + WeightOfTurn;
				return distance;
			}

			bool IsTurn(Coord a, Coord b, Coord c) {
				if (a.X == b.X && b.X == c.X)
					return false;
				if (a.Y == b.Y && b.Y == c.Y)
					return false;
				return true;
			}
		}

		public static List<Coord> Calculate(Coord @from, Coord to, Canvass c, List<IPaintable<object>> model) {
			var size = c.GetSize();
			var solutions = new Solution[size.Item1, size.Item2];
			var unHandled = new SimplePriorityQueue<UnhandledField>();
			unHandled.Enqueue(new UnhandledField(@from, new List<Coord>(), 0), 0);

			while (unHandled.Count > 0) {
				var current = unHandled.Dequeue();

				var pathForPosition = new List<Coord>(current.Path.Count + 1);
				pathForPosition.AddRange(current.Path);
				pathForPosition.Add(current.Position);

				var solution = new Solution(pathForPosition, current.Distance);

				var ifNoOrWeakerSolution =
					solution.Distance < (solutions[current.Position.Y, current.Position.X]?.Distance ?? int.MaxValue);
				if (ifNoOrWeakerSolution) {
					solutions[current.Position.Y, current.Position.X] = solution;

					var currentBestSolutionAtDestination = solutions[to.Y, to.X]?.Distance ?? int.MaxValue;
					var neighbours = CalculateNSEW(current.Position);
					var potentials = neighbours
						.Where(x => x == to || IsCellFree(c, x, model))
						.Select(x =>
							new {
								Neighbour = x,
								EstimatedDistance =
								PaintServiceCore.ManhattenDistance(x, to) + (Vector.IsStraightLine(x, to) ? 0 : WeightOfTurn),
								Unhandled = new UnhandledField(x, pathForPosition, current.Distance + StepLength)
							})
						.Where(x => current.Distance + x.EstimatedDistance + StepLength < currentBestSolutionAtDestination);

					potentials.Each(x => unHandled.Enqueue(x.Unhandled, x.EstimatedDistance));
				}
			}

			var shortestPath = solutions[to.Y, to.X];
			return shortestPath == null ? new List<Coord>() : shortestPath.Path;
		}

		private static bool IsCellFree(Canvass c, Coord pos, List<IPaintable<object>> model)
		{
			int x = pos.X, y = pos.Y;

			if (x < 0 || y < 0)
				return false;

			if (y >= c.Catode.Length)
				return false; //throw new ArgumentException($"y=${y} is too large. Max ${Lines.Count}");

			if (x >= c.Catode[0].Length)
				return false; //throw new ArgumentException($"x=${x} is too large. Max ${line.Length}");

			//Console.WriteLine($"{x},{y}::{(int)line[x]}");
			var cell = c.Catode[y][x];
			if (cell == null)
				return true;
			var elem = model.First(z => z.Id == c.Occupants[y, x]);
			return elem is Line || elem is SlopedLine || elem is SlopedLine2;
		}


		private static List<Coord> CalculateNSEW(Coord coord) {
			List<Coord> result = new List<Coord>(4);
			if (coord.X > 0)
				result.Add(new Coord(coord.X - 1, coord.Y));
			if (coord.Y > 0)
				result.Add(new Coord(coord.X, coord.Y - 1));
			if (coord.X < State.MaxX)
				result.Add(new Coord(coord.X + 1, coord.Y));
			if (coord.Y < State.MaxY)
				result.Add(new Coord(coord.X, coord.Y + 1));

			return result;
		}
	}
}