using System;
using System.Collections.Generic;

namespace AsciiUml.UI.GuiLib
{
    public static class Ext
    {
        public static void Each<T>(this IEnumerable<T> coll, Action<T> code)
        {
            foreach (var c in coll)
            {
                code(c);
            }
        }

        public static void Each<T>(this IEnumerable<T> coll, Action<T, int> code)
        {
            int i = 0;
            foreach (var c in coll)
            {
                code(c, i++);
            }
        }
    }
}