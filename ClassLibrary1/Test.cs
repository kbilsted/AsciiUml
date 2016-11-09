using System.Linq;
using AsciiUml;
using NUnit.Framework;

namespace AsciiUmlTests {
	public static class Test {
		public static void AssertString(string expected, string actual) {
			Assert.AreEqual(expected.Replace("" + (char) 13, ""), actual.Replace("" + (char) 13, ""));
		}

		public static string Paint(params IPaintable<object>[] p) {
			return @"
" + PaintServiceCore.PaintModel(p.ToList()).TrimEndToString();
		}

		public static string PaintOneLine(params IPaintable<object>[] p) {
			return PaintServiceCore.PaintModel(p.ToList()).TrimEndToString();
		}
	}
}