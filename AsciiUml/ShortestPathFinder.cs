using System;
using System.Collections.Generic;
using System.Linq;

namespace AsciiUml {
	static class ShortestPathFinder {
		class FeltTilUndersøgelse {
			public Coord Position;
			public List<Coord> Sti;
			public int Distance;

			public FeltTilUndersøgelse(Coord position, List<Coord> sti, int distance) {
				Position = position;
				Sti = sti;
				Distance = distance;
			}

			public override string ToString() {
				return Position.ToString() + "::" + Sti.Count;
			}
		}

		class Løsning {
			public List<Coord> Sti;
			public int Distance;

			public Løsning(List<Coord> sti, int distance) {
				Sti = sti;
				Distance = UdregnDistance(sti);
			}

			int UdregnDistance(List<Coord> rute) {
				int distance = rute.Count;

				for (int i = 0; i < rute.Count - 3; i++)
					if (ErDerSving(rute[i], rute[i + 1], rute[i + 2]))
						distance = distance + 1;
				return distance;
			}

			bool ErDerSving(Coord a, Coord b, Coord c) {
				if (a.X == b.X && b.X == c.X)
					return false;
				if (a.Y == b.Y && b.Y == c.Y)
					return false;
				return true;
			}
		}

		public static List<Coord> Calculate(Coord from, Coord to, Canvass c) {
			Løsning[,] løsninger = new Løsning[c.Lines.Count, c.Lines.First().Length];

			Stack<FeltTilUndersøgelse> felterDerskalUndersøges = new Stack<FeltTilUndersøgelse>();
			felterDerskalUndersøges.Push(new FeltTilUndersøgelse(from, new List<Coord>(), 0));

			while (felterDerskalUndersøges.Count > 0) {
				var felt = felterDerskalUndersøges.Pop();
				//Console.WriteLine($"undersøger {felt.Position}");

				var stiForFeltet = new List<Coord>(felt.Sti.Count + 1);
				stiForFeltet.AddRange(felt.Sti);
				stiForFeltet.Add(felt.Position);

				var løsning = new Løsning(stiForFeltet, felt.Distance);

				if (løsninger[felt.Position.Y, felt.Position.X] == null
				    || løsning.Distance < løsninger[felt.Position.Y, felt.Position.X].Distance) {
					løsninger[felt.Position.Y, felt.Position.X] = løsning;

					var nuværendeBedsteLøsning = int.MaxValue;
					if (løsninger[to.Y, to.X] != null)
						nuværendeBedsteLøsning = løsninger[to.Y, to.X].Distance;

					var naboerne = CalculateNSEW(felt.Position);

					var potentialerne = naboerne
						.Where(x => x.Equals(to) || c.IsCellFree(x.X, x.Y))
						.Select(nabo => new {nabo, EstimatedDist = PaintServiceCore.FastEuclid(nabo, to)})
						.Where(x => felt.Distance + x.EstimatedDist < nuværendeBedsteLøsning)
						.OrderByDescending(x => x.EstimatedDist)
						.Select(x => new FeltTilUndersøgelse(x.nabo, stiForFeltet, felt.Distance));

					potentialerne.Each(x => felterDerskalUndersøges.Push(x));
				}
			}

			var kortesteVej = løsninger[to.Y, to.X];
			if (kortesteVej == null)
				return new List<Coord>();
			return kortesteVej.Sti;
		}

		private static Coord[] CalculateNSEW(Coord coord) {
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

		public static List<Coord> TooOptimisticPathFromPointToPoint(Coord from, Coord to, Canvass c) {
			//Console.WriteLine($"Calc {from}-{to}");
			if (from.Equals(to))
				return new List<Coord>();

			var res = new List<Coord>();
			res.Add(from);

			var alreadyOnRoute = new HashSet<Coord>();
			alreadyOnRoute.Add(from);

			while (true) {
				if (res.Count > 50)
					Console.WriteLine("wooops");

				//Console.WriteLine("step from =" + res.Last());
				var stepPotentials = PaintServiceCore.CalculateBoxOutline(res.Last());

				if (stepPotentials.Any(x => x.Equals(to))) {
					res.Add(to);
					return res;
				}

				var filteredPotentials = stepPotentials
					.Where(x => !alreadyOnRoute.Contains(x))
					.Where(x => c.IsCellFree(x.X, x.Y))
					.ToArray();

				var smallest = PaintServiceCore.CalcSmallestDist(filteredPotentials, new[] {to}).Min;

				res.Add(smallest);
				alreadyOnRoute.Add(smallest);
			}
		}
	}
}