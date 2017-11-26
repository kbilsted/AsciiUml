using System;
using System.Linq;
using AsciiConsoleUi;
using AsciiUml;
using AsciiUml.Geo;
using NUnit.Framework;
using static AsciiUmlTests.Test;

namespace AsciiUmlTests {
	public class BoxTest {
		[Test]
		public void FrameCoords() {
			var res = new Box(new Coord(3,3), "").GetFrameCoords();

			Assert.AreEqual(new[] {new Coord(3, 3)}, res);
		}

		[Test]
		public void GetFrameParts1_1() {
			Assert.Throws<ArgumentException>(() => new Box(new Coord(1, 1), 1, 1));
		}

		[Test]
		public void GetFrameParts2_2() {
			var res = new Box(new Coord(1,1), 2, 2).GetFrameParts();
			Assert.AreEqual(new[] {
					Tuple.Create(new Coord(1, 1), BoxFramePart.NWCorner),
					Tuple.Create(new Coord(2, 1), BoxFramePart.NECorner),
					Tuple.Create(new Coord(1, 2), BoxFramePart.SWCorner),
					Tuple.Create(new Coord(2, 2), BoxFramePart.SECorner),
				},
				res);
		}

		[Test]
		public void GetFrameParts2_3() {
			var res = new Box(new Coord(1,1), 2,3 ).GetFrameParts();
			Assert.AreEqual(new[] {
					Tuple.Create(new Coord(1, 1), BoxFramePart.NWCorner),
					Tuple.Create(new Coord(2, 1), BoxFramePart.NECorner),
					Tuple.Create(new Coord(1, 3), BoxFramePart.SWCorner),
					Tuple.Create(new Coord(2, 3), BoxFramePart.SECorner),
					Tuple.Create(new Coord(1, 2), BoxFramePart.Vertical),
					Tuple.Create(new Coord(2, 2), BoxFramePart.Vertical),
				},
				res);
		}

		[Test]
		public void GetFrameParts3_3() {
			var res = new Box(new Coord(1,1), 3, 3).GetFrameParts();
			Assert.AreEqual(new[] {
					Tuple.Create(new Coord(1, 1), BoxFramePart.NWCorner),
					Tuple.Create(new Coord(3, 1), BoxFramePart.NECorner),
					Tuple.Create(new Coord(1, 3), BoxFramePart.SWCorner),
					Tuple.Create(new Coord(3, 3), BoxFramePart.SECorner),
					Tuple.Create(new Coord(2, 1), BoxFramePart.Horizontal),
					Tuple.Create(new Coord(2, 3), BoxFramePart.Horizontal),
					Tuple.Create(new Coord(1, 2), BoxFramePart.Vertical),
					Tuple.Create(new Coord(3, 2), BoxFramePart.Vertical),
				},
				res);
		}

		[Test]
		public void PaintBox() {
			var res = Paint(new Box(new Coord(0,0), 4, 3));

			Test.AssertString(
				@"
****
*  *
****", res);
		}

		[Test]
		public void PaintBoxChangeStyleNext()
		{
			var box = new Box(new Coord(0, 0), 4, 3);
			box.ChangeStyle(StyleChangeKind.Next);
			var res = Paint(box);

			Test.AssertString(
				@"
....
.  .
....", res);
		}

		[Test]
		public void PaintBoxChangeStyleNextAndPrev()
		{
			var box = new Box(new Coord(0, 0), 4, 3);
			box.ChangeStyle(StyleChangeKind.Next);
			box.ChangeStyle(StyleChangeKind.Previous);
			var res = Paint(box);

			Test.AssertString(
				@"
****
*  *
****", res);
		}

		[Test]
		public void PaintTextBox() {
			var res = Paint(new Box(new Coord(0,0), "Foo"));

			Test.AssertString(
				@"
*******
* Foo *
*******", res);
		}

		[Test]
		public void PaintMultiTextBox() {
			var res = Paint(new Box(new Coord(0,0), "Foo\nbazooka"));

			Test.AssertString(
				@"
***********
* Foo     *
* bazooka *
***********", res);
		}


		[Test]
		public void PaintMultiTextBoxex() {
			var res = Paint(new Box(new Coord(0,0), "Foo\nbazooka"), new Box(new Coord(14,1), "Bar.."));

			Test.AssertString(
				@"
***********
* Foo     *   *********
* bazooka *   * Bar.. *
***********   *********", res);
		}
	}
}