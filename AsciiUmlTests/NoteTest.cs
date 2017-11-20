using System;
using System.Linq;
using AsciiConsoleUi;
using AsciiUml;
using AsciiUml.Geo;
using NUnit.Framework;
using static AsciiUmlTests.Test;

namespace AsciiUmlTests {
	public class NoteTest {
		[Test]
		public void PaintDefaultNote() {
			var res = Paint(new Note(new Coord(0,0),""));

			Assert.AreEqual(@"
+~~+\
|  |_\
|    |
+~~~~+", res);
		}

		[Test]
		public void Paint1LineNote() {
			var res = Paint(new Note(new Coord(0,0), "Hello world"));

			Assert.AreEqual(
				@"
+~~~~~~~~~~~~~+\
|             |_\
|Hello world    |
+~~~~~~~~~~~~~~~+", res);
		}


		[Test]
		public void PaintMultiLineNote() {
			var res = Paint(new Note(new Coord(0,0), "Hello\nWorld\nAnd stuff.."));

			Assert.AreEqual(
				@"
+~~~~~~~~~~~~~+\
|             |_\
|Hello          |
|World          |
|And stuff..    |
+~~~~~~~~~~~~~~~+", res);
		}
	}
}