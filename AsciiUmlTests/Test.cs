using System;
using System.Linq;
using AsciiUml;
using AsciiUml.Geo;
using AsciiUml.UI;
using NUnit.Framework;

namespace AsciiUmlTests {
	public static class Test {
		public static void AssertString(string expected, string actual) {
			var exp = expected.Replace("" + (char) 13, "");
			var act = actual.Replace("" + (char) 13, "");
			if(exp!=act)
				Console.WriteLine(act);
			Assert.AreEqual(exp, act);
		}

		public static string Paint(params IPaintable<object>[] p) {
			return @"
" + PaintServiceCore.PaintModel(new Model(p), false).TrimEndToString();
		}

		public static string PaintOneLine(params IPaintable<object>[] p) {
			return PaintServiceCore.PaintModel(new Model(p), false).TrimEndToString();
		}
	}
}