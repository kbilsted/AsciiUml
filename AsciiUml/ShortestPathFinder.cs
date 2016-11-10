using System;
using System.Collections.Generic;
using System.Linq;

namespace AsciiUml
{
	static class ShortestPathFinder
	{
		const int WeightOfTurn = 1;
		const int StepLength = 1;

		class UnhandledField
		{
			public readonly Coord Position;
			public readonly List<Coord> Path;
			public readonly int Distance;

			public UnhandledField(Coord position, List<Coord> path, int distance)
			{
				Position = position;
				Path = path;
				Distance = distance;
			}

			public override string ToString()
			{
				return $"{Position}::{Path.Count}";
			}
		}

		class Solution
		{
			public readonly List<Coord> Path;
			public readonly int Distance;

			public Solution(List<Coord> path, int distance)
			{
				Path = path;
				Distance = PenalizeTurns(path);
			}

			int PenalizeTurns(List<Coord> rute)
			{
				int distance = rute.Count;

				for (int i = 0; i < rute.Count - 3; i++)
					if (IsTurn(rute[i], rute[i + 1], rute[i + 2]))
						distance = distance + WeightOfTurn;
				return distance;
			}

			bool IsTurn(Coord a, Coord b, Coord c)
			{
				if (a.X == b.X && b.X == c.X)
					return false;
				if (a.Y == b.Y && b.Y == c.Y)
					return false;
				return true;
			}
		}

		public static List<Coord> Calculate(Coord from, Coord to, Canvass c)
		{
			var solutions = new Solution[c.Lines.Count, c.Lines.First().Length];

			var unHandled = new Stack<UnhandledField>();
			unHandled.Push(new UnhandledField(from, new List<Coord>(), 0));

			while (unHandled.Count > 0)
			{
				var current = unHandled.Pop();

				var pathForPosition = new List<Coord>(current.Path.Count + 1);
				pathForPosition.AddRange(current.Path);
				pathForPosition.Add(current.Position);

				var solution = new Solution(pathForPosition, current.Distance);

				var ifNoOrWeakerSolution = solutions[current.Position.Y, current.Position.X] == null
										   || solution.Distance < solutions[current.Position.Y, current.Position.X].Distance;
				if (ifNoOrWeakerSolution)
				{
					solutions[current.Position.Y, current.Position.X] = solution;

					var currentBestSolutionAtDestination = solutions[to.Y, to.X] == null
						? int.MaxValue
						: solutions[to.Y, to.X].Distance;

					var neighbours = CalculateNSEW(current.Position);

					var potentials = neighbours
						.Where(x => x == to || c.IsCellFree(x.X, x.Y))
						.Select(x =>
								new
								{
									Neighbour = x, EstimatedDist = PaintServiceCore.ManhattenDistance(x, to) + (x.IsStraighLineBetweenPoints(to) ? 0 : WeightOfTurn)
								})
						.Where(x => current.Distance + x.EstimatedDist + StepLength < currentBestSolutionAtDestination)
						.OrderByDescending(x => x.EstimatedDist)
						.Select(x => new UnhandledField(x.Neighbour, pathForPosition, current.Distance + StepLength));

					potentials.Each(x => unHandled.Push(x));
				}
			}

			var shortestPath = solutions[to.Y, to.X];
			return shortestPath == null ? new List<Coord>() : shortestPath.Path;
		}

		private static Coord[] CalculateNSEW(Coord coord)
		{
			List<Coord> result = new List<Coord>(4);
			if (coord.X > 0)
				result.Add(new Coord(coord.X - 1, coord.Y));
			if (coord.Y > 0)
				result.Add(new Coord(coord.X, coord.Y - 1));
			if (coord.X < 80)
				result.Add(new Coord(coord.X + 1, coord.Y));
			if (coord.Y < 40)
				result.Add(new Coord(coord.X, coord.Y + 1));

			return result.ToArray();
		}
	}
}