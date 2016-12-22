using AsciiUml;
using NUnit.Framework;
using static LanguageExt.Prelude;
using static AsciiUmlTests.Test;

namespace AsciiUmlTests {
	public class LabelTests {

		[Test]
		public void GetLabelOutline() {
			var l = new Label(new Coord(1, 1), "a");
			var outline = l.GetFrameCoords();
			Assert.AreEqual(new[] {new Coord(1,1) }, outline);
		}

		[Test]
		public void NormalLine() {
			var res = Paint(new Label("abcde"));

			Assert.AreEqual(
				@"
abcde", res);
		}

		[Test]
		public void NormalLineRotatedx1() {
			var res = Paint(new Label("abcde").Rotate());

			Assert.AreEqual(
				@"
a
b
c
d
e", res);
		}

		[Test]
		public void NormalLineRotatedx2() {
			var res = Paint(new Label("abcde").Rotate().Rotate());

			Assert.AreEqual(
				@"
abcde", res);
		}

		[Test]
		public void MultiLine() {
			var res = Paint(new Label("abcde\nfoobar"));

			Assert.AreEqual(
				@"
abcde
foobar", res);
		}

		[Test]
		public void MultiLineRotatedx1() {
			var res = Paint(new Label("abcde\nfoobar").Rotate());

			Assert.AreEqual(
				@"
af
bo
co
db
ea
 r", res);
		}

		[Test]
		public void MultiLineRotatedx2() {
			var res = Paint(new Label("abcde\nfoobar").Rotate().Rotate());

			Assert.AreEqual(
				@"
abcde
foobar", res);
		}
	}
}