using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsciiUml;
using LanguageExt;
using NUnit.Framework;
using static LanguageExt.Prelude;
using static ClassLibrary1.Test;

namespace ClassLibrary1 {
	public class SlopedLineTests {
		Label labelX = new Label(1000, 0, 0, "x", LabelDirection.LeftToRight);

		[Test]
		public void PaintSlopedLine_2_0() {
			var res = PaintOneLine(labelX, GetLine2_0());
			Assert.AreEqual(@"x -", res);
		}


		[Test]
		public void PaintSlopedLine_2_0_DragLeftAt_2_0() {
			var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(1, 0));
			var res = PaintOneLine(labelX, line);
			Assert.AreEqual(@"x--", res);
		}

		[Test]
		public void PaintSlopedLine_2_0_DragRightAt_2_0() {
			var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(3, 0));
			var res = PaintOneLine(labelX, line);
			Assert.AreEqual(@"x --", res);
		}


		[Test]
		public void PaintSlopedLine_1_4() {
			var res = Paint(labelX, GetLine1_4());

			Assert.AreEqual(
				@"
x
 ---", res);
		}


		[Test]
		public void PaintSlopedLine_1_4_drag_11_to_01_will_extend_start_of_segments() {
			var line14 = GetLine1_4().DragAnArrowLinePiece(new Coord(1, 1), new Coord(0, 1)).MatchUnsafe(x => x, () => null);
			;
			var res = Paint(labelX, line14);

			Assert.AreEqual(
				@"
x
----", res);
		}


		[Test]
		public void PaintSlopedLine_1_4_drag_11_to_21_move_inside_line_along_the_line_Then_no_action() {
			var act = GetLine1_4().DragAnArrowLinePiece(new Coord(1, 1), new Coord(2, 1));
			Assert.AreEqual(Option<SlopedLine>.None, act);

			act = GetLine1_4().DragAnArrowLinePiece(new Coord(2, 1), new Coord(3, 1));
			Assert.AreEqual(Option<SlopedLine>.None, act);

			act = GetLine1_4().DragAnArrowLinePiece(new Coord(3, 1), new Coord(4, 1));
			Assert.AreEqual(Option<SlopedLine>.None, act);
		}

		[Test]
		public void PaintSlopedLine_1_4_drag_reverse_11_to_21_move_inside_line_along_the_line_Then_no_action() {
			var act = GetLine1_4().DragAnArrowLinePiece(new Coord(2, 1), new Coord(1, 1));
			Assert.AreEqual(Option<SlopedLine>.None, act);

			act = GetLine1_4().DragAnArrowLinePiece(new Coord(3, 1), new Coord(2, 1));
			Assert.AreEqual(Option<SlopedLine>.None, act);

			act = GetLine1_4().DragAnArrowLinePiece(new Coord(4, 1), new Coord(3, 1));
			Assert.AreEqual(Option<SlopedLine>.None, act);
		}

		[Test]
		public void PaintSlopedLine_1_4_drag_41_to_51_extend_end_point() {
			var line14 = GetLine1_4().DragAnArrowLinePiece(new Coord(4, 1), new Coord(5, 1)).MatchUnsafe(x => x, () => null);
			var res = Paint(labelX, line14);

			Assert.AreEqual(
				@"
x
 ----", res);
		}

		private static SlopedLine GetLine(Coord from, Coord to) {
			SlopedLine l1 = new SlopedLine();
			l1.Segments = new List<LineSegment>() {
				new LineSegment(l1) {From = from, To = to}
			};
			return l1;
		}

		private static SlopedLine GetLine2_0() {
			return GetLine(new Coord(2, 0), new Coord(2, 0));
		}

		private static SlopedLine GetLine1_4() {
			return GetLine(new Coord(1, 1), new Coord(4, 1));
		}
	}
}