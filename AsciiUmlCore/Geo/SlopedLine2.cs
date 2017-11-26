using System;
using System.Collections.Generic;
using System.Linq;
using AsciiConsoleUi;
using LanguageExt;

namespace AsciiUml.Geo {
	/// <summary>
	///     A line implementation where each pixel is stored rather than vectors
	/// </summary>
	public class SlopedLine2 : IPaintable<SlopedLine2>, ISelectable {
		public enum LineSemantic {
			StartArrow,
			EndArrow,
			Slope,
			LinePiece
		}

		public readonly List<SlopedSegment2> Segments = new List<SlopedSegment2>();

		public SlopedLine2(Coord pos) {
			Id = PaintAbles.GlobalId++;
			Segments.Add(new SlopedSegment2(pos, SegmentType.Line));
		}

		public int Id { get; }

		public SlopedLine2 Move(Coord delta) {
			throw new NotImplementedException();
		}

		public Coord Pos => Segments.First().Pos;

		public SlopedLine2 Drag(Coord dragFrom, Coord dragTo) {
			return DragAnArrowLinePiece(dragFrom, dragTo).MatchUnsafe(x => x, () => this);
		}

		public LineDirection GetDirectionOf(int index) {
			if (index == 0) {
				if (Segments.Count > 1)
					return Vector.GetDirection(Segments[index].Pos, Segments[index + 1].Pos);
				return Vector.GetDirection(Segments[index].Pos, Segments[index].Pos);
			}
			return Vector.GetDirection(Segments[index - 1].Pos, Segments[index].Pos);
		}

		public Option<SlopedLine2> DragAnArrowLinePiece(Coord dragFrom, Coord dragTo) {
			var s = GetSemantic(dragFrom);
			switch (s) {
				case LineSemantic.StartArrow:
					if (Segments.Count == 1) {
						Segments.Add(new SlopedSegment2(dragTo, SegmentType.Line));
					}
					else {
						if (Segments[1].Pos == dragTo) {
							Segments.RemoveAt(0);
							return this;
						}

						var directionDragFrom = GetDirectionOf(0);

						Segments.Insert(0, new SlopedSegment2(dragTo, SegmentType.Line));
						var directionDragTo = GetDirectionOf(0);

						if (Vector.IsOrthogonal(directionDragFrom, directionDragTo))
							Segments[1].Type = SegmentType.Slope;
					}

					return this;

				case LineSemantic.EndArrow:
					var nthlastPos = GetLastNthLastPos(Segments, 1);
					if (nthlastPos.HasValue) {
						var isDragBackwards = Segments[nthlastPos.Value].Pos == dragTo;
						if (isDragBackwards) {
							Segments.RemoveAt(Segments.Count - 1);
							Segments[Segments.Count - 1].Type = SegmentType.Line; // ensure to convert slopes to lines
							return this;
						}
					}

					var posAtInsert = Segments.Count - 1;
					Segments.Add(new SlopedSegment2(dragTo, SegmentType.Line));
					if (Vector.IsOrthogonal(GetDirectionOf(posAtInsert), GetDirectionOf(posAtInsert + 1)))
						Segments[posAtInsert].Type = SegmentType.Slope;
					Segments[Segments.Count - 1].Type = SegmentType.Line; // ensure to convert slopes to lines
					return this;

				case LineSemantic.Slope:
					break;
				case LineSemantic.LinePiece:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return null;
		}

		private int? GetLastNthLastPos<T>(List<T> colList, int last) {
			var pos = colList.Count - 1 - last;
			if (pos < 0)
				return null;
			return pos;
		}

		private LineSemantic GetSemantic(Coord coord) {
			var pos = Segments.FindIndex(x => x.Pos == coord);
			if (pos == 0)
				return LineSemantic.StartArrow;
			if (pos == Segments.Count - 1)
				return LineSemantic.EndArrow;
			if (Segments[pos].Type == SegmentType.Slope)
				return LineSemantic.Slope;
			return LineSemantic.LinePiece;
		}

		public SlopedLineVectorized AutoRoute() {
			return null; // return a new shortest path from start to end
		}

		public SlopedLine2 Move(int x, int y) {
			throw new NotImplementedException();
		}

		public class SlopedSegment2 {
			public Coord Pos;
			public SegmentType Type;

			public SlopedSegment2(Coord pos, SegmentType type) {
				Pos = pos;
				Type = type;
			}
		}
	}
}