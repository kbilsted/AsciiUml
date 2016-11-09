using System;
using System.Collections.Generic;
using System.Linq;
using AsciiUml;
using LanguageExt;
using NUnit.Framework;
using static LanguageExt.Prelude;
using static AsciiUmlTests.Test;

namespace AsciiUmlTests {

	public class SlopedLineTests {
		static Label labelX = new Label(1000, 0, 0, "x", LabelDirection.LeftToRight);

		public class SinglePoint_DragTests {
			[Test]
			public void Test2_0() {
				var res = PaintOneLine(labelX, GetLine2_0());
				Assert.AreEqual(@"x -", res);
			}

			[Test]
			public void Test2_0_DragLeftAt_2_0() {
				var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(1, 0));
				var res = PaintOneLine(labelX, line);
				Assert.AreEqual(@"x--", res);
			}

			[Test]
			public void Test2_0_DragRightAt_2_0() {
				var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(3, 0));
				var res = PaintOneLine(labelX, line);
				Assert.AreEqual(@"x --", res);
			}

			[Test]
			public void Test2_0_DragRightDragLeft_Should_result_in_size_1()
			{
				var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(3, 0)).Drag(new Coord(3,0), new Coord(2,0) );
				var res = PaintOneLine(labelX, line);
				Assert.AreEqual(@"x -", res);
			}

			[Test]
			public void Test2_0_DragDownAt_2_0() {
				var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(2, 1));
				var res = PaintOneLine(labelX, line);
				Assert.AreEqual(@"x |
  |", res);
			}

			[Test]
			public void Test2_1_DragUpAt_2_1() {
				var line = GetLine(new Coord(2, 1), new Coord(2, 1)).Drag(new Coord(2, 1), new Coord(2, 0));
				var res = PaintOneLine(labelX, line);
				Assert.AreEqual(@"x |
  |", res);
			}
		}

		public class Line_DragTests {
			[Test]
			public void Test1_4() {
				var res = PaintOneLine(labelX, GetLine10_40());
				Assert.AreEqual(@"x----", res);
			}

			[Test]
			public void Drag_outside_line_has_no_effect() {
				var res = PaintOneLine(labelX, GetLine10_40().Drag(new Coord(2,2), new Coord(2,3)));
				Assert.AreEqual(@"x----", res);
			}

			[Test]
			public void DragLeft_on_left_will_extend_line() {
				var line14 = GetLine10_40().Drag(new Coord(1, 0), new Coord(0, 0));
				var res = PaintOneLine(line14);
				Assert.AreEqual(@"-----", res);
			}

			[Test]
			public void DragRight_on_left_will_subtract_line() {
				var line14 = GetLine10_40().Drag(new Coord(1, 0), new Coord(2, 0));
				var res = PaintOneLine(labelX, line14);
				Assert.AreEqual(@"x ---", res);
			}

			[Test]
			public void DragLeft_on_secondleft_will_not_extend_line() {
				var line14 = GetLine10_40().Drag(new Coord(2, 0), new Coord(3, 0));
				var res = PaintOneLine(labelX, line14);
				Assert.AreEqual(@"x----", res);
			}

			[Test]
			public void DragRight_on_secondleft_will_not_extend_line() {
				var line14 = GetLine10_40().Drag(new Coord(2, 0), new Coord(1, 0));
				var res = PaintOneLine(labelX, line14);
				Assert.AreEqual(@"x----", res);
			}

			[Test]
			public void DragLeft_on_rightend_will_subtract_line() {
				var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(3, 0));
				var res = PaintOneLine(labelX, line14);
				Assert.AreEqual(@"x---", res);
			}

			[Test]
			public void DragRight_on_rightend_will_extend_line() {
				var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(5, 0));
				var res = PaintOneLine(labelX, line14);
				Assert.AreEqual(@"x-----", res);
			}
		}

		// todo drag lines up/down

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

		private static SlopedLine GetLine10_40() {
			return GetLine(new Coord(1, 0), new Coord(4, 0));
		}
	}
}