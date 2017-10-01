using System;
using System.Linq;
using System.Text;
using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.UI
{
    public class Popup : GuiComponent
    {
        private readonly string[] msglines;

        private Canvass c;
        private readonly Button ok;

        public Popup(GuiComponent parent, string message) : base(parent)
        {
            msglines = message.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            Dimensions.Height.Pixels = msglines.Length + 5;
            Dimensions.Width.Pixels = msglines.Max(x => x.Length) + 6;

            Position = new Coord((State.MaxX - Dimensions.Width.Pixels) / 2, ((State.MaxY - Dimensions.Height.Pixels) / 2) - 1);

            var screenCenter = new Coord(Dimensions.Width.Pixels / 2, Position.Y + msglines.Length + 3);
            ok = new Button(this, "OK", () => { Remove();parent.Remove();})
            {
                Position = screenCenter
            };
            ok.Focus();
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            if (IsFocused)
            {
                if(key.Key== ConsoleKey.Tab)
                    ok.Focus();
                return true;
            }
            return false;
        }

        public override Canvass Paint()
        {
            c = new Canvass();
            for (int y = 0; y < msglines.Length; y++)
                c.RawPaintString("   " + msglines[y], 0, y, Parent.BackGround, Parent.Foreground);

            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return Parent.GetInnerCanvasTopLeft();
        }
    }
}
