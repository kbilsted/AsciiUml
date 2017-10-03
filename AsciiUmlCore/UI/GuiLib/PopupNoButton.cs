using System;
using System.Linq;
using System.Text;
using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.UI
{
    public class PopupNoButton : GuiComponent
    {
        private readonly string[] msglines;

        private Canvass c;

        public PopupNoButton(GuiComponent parent, string message) : base(parent)
        {
            msglines = message.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            Dimensions.Height.Pixels = msglines.Length;
            Dimensions.Width.Pixels = msglines.Max(x => x.Length) + 6;

            Position = new Coord((State.MaxX - Dimensions.Width.Pixels) / 2, ((State.MaxY - Dimensions.Height.Pixels) / 2) - 1);
            this.Focus();
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            if (IsFocused)
            {
                RemoveMeAndChildren();
                return true;
            }
            return false;
        }

        public override Canvass Paint()
        {
            c = new Canvass();
            for (int y = 0; y < msglines.Length; y++)
                c.RawPaintString("   " + msglines[y], 0, y, BackGround, Foreground);

            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return Parent.GetInnerCanvasTopLeft();
        }
    }
}
