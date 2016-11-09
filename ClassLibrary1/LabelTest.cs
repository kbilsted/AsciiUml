using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsciiUml;
using NUnit.Framework;
using static LanguageExt.Prelude;
using static ClassLibrary1.Test;

namespace ClassLibrary1 {
	public class LabelTests {
		[Test]
		public void NormalLine() {
			var res = Paint(new Label() {Text = "abcde"});

			Assert.AreEqual(
				@"
abcde", res);
		}

		[Test]
		public void NormalLineRotatedx1() {
			var res = Paint(new Label() {Text = "abcde"}.Rotate());

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
			var res = Paint(new Label() {Text = "abcde"}.Rotate().Rotate());

			Assert.AreEqual(
				@"
abcde", res);
		}

		[Test]
		public void MultiLine() {
			var res = Paint(new Label() {Text = "abcde\nfoobar"});

			Assert.AreEqual(
				@"
abcde
foobar", res);
		}

		[Test]
		public void MultiLineRotatedx1() {
			var res = Paint(new Label() {Text = "abcde\nfoobar"}.Rotate());

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
			var res = Paint(new Label() {Text = "abcde\nfoobar"}.Rotate().Rotate());

			Assert.AreEqual(
				@"
abcde
foobar", res);
		}
	}
}