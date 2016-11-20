using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsciiUml;
using NUnit.Framework;
using static LanguageExt.Prelude;
using static AsciiUmlTests.Test;

namespace AsciiUmlTests {
	public class LineTests {
		[Test]
		public void BoxOutline() {
			var res = PaintServiceCore.CalculateBoxOutline(new Box(new Coord(3,4)));

			Assert.AreEqual(new[] {
				new Coord(2, 3),
				new Coord(2, 5),
				new Coord(3, 3),
				new Coord(3, 5),
				new Coord(4, 3),
				new Coord(4, 5),
				new Coord(2, 4),
				new Coord(4, 4),
			}, res);
		}

		[Test]
		public void PaintEastArrowClose2Straight() {
			var res = Paint(
				new Box(new Coord(0,0)) {Id = 0, Text = "Foo"},
				new Box(new Coord(9,0)) {Id = 1, Text = "Bar"},
				new Line() {FromId = 0, ToId = 1});

			Assert.AreEqual(
				@"
*******->*******
* Foo *  * Bar *
*******  *******", res);
		}

		[Test]
		public void PaintEastArrowStraight() {
			var res = Paint(
				new Box(new Coord(0,0)) {Id = 0, Text = "Foo"},
				new Box(new Coord(16,0)) {Id = 1, Text = "Bar"},
				new Line() {FromId = 0, ToId = 1});
			Assert.AreEqual(
				@"
*******-------->*******
* Foo *         * Bar *
*******         *******", res);
		}

		[Test]
		public void PaintEastArrow1Down() {
			var res = Paint(
				new Box(new Coord(0,0)) {Id = 0, Text = "Foo"},
				new Box(new Coord(16,1)) {Id = 1, Text = "Bar"},
				new Line() {FromId = 0, ToId = 1});

			Assert.AreEqual(
				@"
*******
* Foo *         *******
*******-------->* Bar *
                *******", res);
		}


		[Test]
		public void PaintLineCrossingLabel() {
			var res = Paint(
				new Box(new Coord(0,0)) {Id = 0, Text = "goo\nand\nbazooka"},
				new Box(new Coord(6,10)) {Id = 1, Text = "Mango\nTango"},
				new Line() {FromId = 0, ToId = 1},
				new Label(new Coord(5,6)) { Text = "Server\nservice\noriented"}
			);
			Console.WriteLine(res);

			Test.AssertString(
				@"
***********
* goo     *
* and     *
* bazooka *
***********
      |
     Server
     service
     oriented
      v
      *********
      * Mango *
      * Tango *
      *********", res);
		}


		[Test]
		public void NySmarteKortesteVej() {
			var res = Paint(
				new Box(new Coord(0,0)) {Id = 0, Text = "a"},
				new Box(new Coord(0,4)) {Id = 1, Text = "b"},
				new Line() {FromId = 0, ToId = 1}
			);
			Console.WriteLine(res);

			Test.AssertString(
				@"
*****
* a *
*****
v
*****
* b *
*****", res);
		}

		[Test]
		public void NySmarteKortesteVej2() {
			var res = Paint(
				new Box(new Coord(0,0)) {Id = 0, Text = "a"},
				new Box(new Coord(0,5)) {Id = 1, Text = "b"},
				new Line() {FromId = 0, ToId = 1}
			);
			Console.WriteLine(res);

			Test.AssertString(
				@"
*****
* a *
*****
|
v
*****
* b *
*****", res);
		}
	}
}