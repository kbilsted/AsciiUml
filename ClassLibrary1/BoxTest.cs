using System;
using System.Linq;
using AsciiUml;
using NUnit.Framework;
using static ClassLibrary1.Test;

namespace ClassLibrary1 {
	public class BoxTest {
		[Test]
		public void FrameCoords() {
			var res = new Box() {X = 3, Y = 3}.GetFrameCoords();

			Assert.AreEqual(new[] {new Coord(3, 3)}, res);
		}

		[Test]
		public void GetFrameParts1_1() {
			var res = new Box() {X = 1, Y = 1, H = 1, W = 1};
			Assert.Throws<ArgumentException>(() => res.GetFrameParts());
		}

		[Test]
		public void GetFrameParts2_2() {
			var res = new Box() {X = 1, Y = 1, H = 2, W = 2}.GetFrameParts();
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
			var res = new Box() {X = 1, Y = 1, H = 3, W = 2}.GetFrameParts();
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
			var res = new Box() {X = 1, Y = 1, H = 3, W = 3}.GetFrameParts();
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
		public void PaintDefaultBox() {
			var res = Paint(new Box());

			Assert.AreEqual(@"
*", res);
		}

		[Test]
		public void PaintBox() {
			var res = Paint(new Box() {X = 0, Y = 0, H = 3, W = 4});

			Assert.AreEqual(
				@"
****
*  *
****", res);
		}


		[Test]
		public void PaintTextBox() {
			var res = Paint(new Box() {Text = "Foo"});

			Assert.AreEqual(
				@"
*******
* Foo *
*******", res);
		}

		[Test]
		public void PaintMultiTextBox() {
			var res = Paint(new Box() {Text = "Foo\nbazooka"});

			Assert.AreEqual(
				@"
***********
* Foo     *
* bazooka *
***********", res);
		}


		[Test]
		public void PaintMultiTextBoxex() {
			var res = Paint(new Box() {Text = "Foo\nbazooka"}, new Box() {X = 14, Y = 1, Text = "Bar.."});

			Assert.AreEqual(
				@"
***********
* Foo     *   *********
* bazooka *   * Bar.. *
***********   *********", res);
		}
	}
}