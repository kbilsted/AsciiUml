using System;
using AsciiConsoleUi;
using AsciiUml.Geo;
using NUnit.Framework;
using static AsciiUmlTests.Test;

namespace AsciiUmlTests {
	[Ignore("not using the vector version")]
	class SlopedLineVectorizedTests
	{
		static Label labelX = new Label(1000,new Coord(0, 0), "x", LabelDirection.LeftToRight);

		[Ignore("not using the vector version")]
        class SinglePoint_DragTests {
            [Test]
            public void PaintLine() {
                var res = PaintOneLine(labelX, GetLine2_0());
                Test.AssertString(@"x -", res);
            }

            [Test]
            public void PaintLine_horizontal() {
                var line = new SlopedLineVectorized();
                line.Segments.Add(new LineSegment(line, new Coord(0, 0), new Coord(0, 1), SegmentType.Line));

                var res = PaintOneLine(line);
	            Test.AssertString(
                    @"|
|", res);
            }

            [Test]
            public void Test2_0_DragLeftAt_2_0() {
                var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(1, 0));
                var res = PaintOneLine(labelX, line);
	            Test.AssertString(@"x--", res);
            }

            [Test]
            public void Test2_0_DragRightAt_2_0() {
                var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(3, 0));
                var res = PaintOneLine(labelX, line);
	            Test.AssertString(@"x --", res);
            }

            [Test]
            public void Test2_0_DragRightDragLeft_Should_result_in_size_1() {
                var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(3, 0));
                var res = PaintOneLine(labelX, line);
	            Test.AssertString(@"x --", res);

                line = line.Drag(new Coord(3, 0), new Coord(2, 0));
                res = PaintOneLine(labelX, line);
                Test.AssertString(@"x -", res);
            }

            [Test]
            public void Test2_0_DragDownAt_2_0() {
                var line = GetLine2_0().Drag(new Coord(2, 0), new Coord(2, 1));
                var res = PaintOneLine(labelX, line);
                Test.AssertString(@"x |
  |", res);
            }

            [Test]
            public void Test2_1_DragUpAt_2_1() {
                var line = GetLine(new Coord(2, 1), new Coord(2, 1)).Drag(new Coord(2, 1), new Coord(2, 0));
                var res = PaintOneLine(labelX, line);
                Test.AssertString(@"x |
  |", res);
            }
        }

		[Ignore("not using the vector version")]
		public class Line_DragTests {
            [Test]
            public void Can_draw_line_10_40() {
                var res = PaintOneLine(labelX, GetLine10_40());
                Test.AssertString(@"x----", res);
            }

            [Test]
            public void Drag_outside_any_line_has_no_effect() {
                var res = PaintOneLine(labelX, GetLine10_40().Drag(new Coord(2, 2), new Coord(2, 3)));
                Test.AssertString(@"x----", res);
            }

            [Test]
            public void DragLeft_on_left_will_extend_line() {
                var line14 = GetLine10_40().Drag(new Coord(1, 0), new Coord(0, 0));
                var res = PaintOneLine(line14);
                Test.AssertString(@"-----", res);
            }

            [Test]
            public void DragRight_on_left_will_subtract_line() {
                var line14 = GetLine10_40().Drag(new Coord(1, 0), new Coord(2, 0));
                var res = PaintOneLine(labelX, line14);
                Test.AssertString(@"x ---", res);
            }

            [ Test]
            public void Drag_down_on_leftbound_will_slope_line() {
                var line14 = GetLine10_40().Drag(new Coord(1, 0), new Coord(1, 1));
                var res = PaintOneLine(labelX, line14);
                Assert.AreEqual(
                    @"x+---
 |", res);
            }

            [Test]
            public void Drag_up_on_leftbound_will_slope_line()
            {
                var line14 = GetLine11_41().Drag(new Coord(1, 1), new Coord(1, 0));
                var res = PaintOneLine(labelX, line14);
                Assert.AreEqual(
@"x|
 +---", res);
            }

            [Test]
            public void Drag_down_on_rightbound_will_slope_line()
            {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(4, 1));
                var res = PaintOneLine(labelX, line14);
                Assert.AreEqual(
                    @"x---+
    |", res);
            }

            [Test]
            public void Drag_down_on_rightbound_then_up_will_unslope_line()
            {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(4, 1));
                line14 = line14.Drag(new Coord(4,1), new Coord(4,0));
                var res = PaintOneLine(labelX, line14);
                Console.WriteLine(res);
                Test.AssertString(@"x---+", res);
            }

            [Test]
            public void Drag_down_on_rightbound_then_drag_left_will_slope_a_u_shape()
            {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(4, 1));
                line14 = line14.Drag(new Coord(4,1), new Coord(3,1));
                var res = PaintOneLine(labelX, line14);
                Assert.AreEqual(
                    @"x---+
   -+", res);
            }

            [Test]
            public void Drag_down_on_rightbound_then_drag_left_then_drag_left_will_slope_a_u_shape()
            {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(4, 1));
                line14 = line14.Drag(new Coord(4, 1), new Coord(3, 1));
                line14 = line14.Drag(new Coord(3, 1), new Coord(2, 1));
                var res = PaintOneLine(labelX, line14);
                Assert.AreEqual(
                    @"x---+
  --+", res);
            }

            [Test]
            public void Drag_down_on_rightbound_then_drag_left_then_drag_up_will_slope_a_box_shape()
            {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(4, 1));
                line14 = line14.Drag(new Coord(4, 1), new Coord(3, 1));
                line14 = line14.Drag(new Coord(3, 1), new Coord(2, 1));
                line14 = line14.Drag(new Coord(2, 1), new Coord(2, 0));
                var res = PaintOneLine(labelX, line14);
                Console.WriteLine(res);
                Assert.AreEqual(
@"x-+-+
  +-+", res);
            }



            [Test]
            public void Drag_up_on_rightbound_will_slope_line()
            {
                var line14 = GetLine11_41().Drag(new Coord(4, 1), new Coord(4, 0));
                var res = PaintOneLine(labelX, line14);
                Assert.AreEqual(
@"x   |
 ---+", res);
            }

            [Test]
            public void DragLeft_on_secondleft_will_not_extend_line() {
                var line14 = GetLine10_40().Drag(new Coord(2, 0), new Coord(3, 0));
                var res = PaintOneLine(labelX, line14);
                Test.AssertString(@"x----", res);
            }

            [Test]
            public void DragRight_on_secondleft_will_not_extend_line() {
                var line14 = GetLine10_40().Drag(new Coord(2, 0), new Coord(1, 0));
                var res = PaintOneLine(labelX, line14);
                Test.AssertString(@"x----", res);
            }

            [Test]
            public void DragLeft_on_rightend_will_subtract_line() {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(3, 0));
                var res = PaintOneLine(labelX, line14);
                Test.AssertString(@"x---", res);
            }

            [Test]
            public void DragRight_on_rightend_will_extend_line() {
                var line14 = GetLine10_40().Drag(new Coord(4, 0), new Coord(5, 0));
                var res = PaintOneLine(labelX, line14);
                Test.AssertString(@"x-----", res);
            }
        }

        // todo drag lines up/down

        private static SlopedLineVectorized GetLine(Coord from, Coord to) {
            SlopedLineVectorized l1 = new SlopedLineVectorized();
            l1.Segments.Add(new LineSegment(l1, from, to, SegmentType.Line));
            return l1;
        }

        private static SlopedLineVectorized GetLine2_0() {
            return GetLine(new Coord(2, 0), new Coord(2, 0));
        }

        private static SlopedLineVectorized GetLine10_40()
        {
            return GetLine(new Coord(1, 0), new Coord(4, 0));
        }

        private static SlopedLineVectorized GetLine11_41()
        {
            return GetLine(new Coord(1, 1), new Coord(4, 1));
        }
    }
}