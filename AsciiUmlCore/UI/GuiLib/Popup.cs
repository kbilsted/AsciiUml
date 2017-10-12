using System;
using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.UI
{
    public class Popup : GuiComponent
    {
        private readonly Button ok;

        public Popup(GuiComponent parent, string message) : base(parent)
        {
            var label = new TextLabel(this, message, new Coord(4,0));

            Dimensions = label.GetSize();
            Dimensions.Height.Pixels += 5;
            Dimensions.Width.Pixels += 6;

            Position = new Coord((State.MaxX - Dimensions.Width.Pixels) / 2, ((State.MaxY - Dimensions.Height.Pixels) / 2) - 1);
            label.SetPosition(new Coord(4,0));

            var screenCenter = new Coord(Dimensions.Width.Pixels / 2, Position.Y + label.Height + 3);
            ok = new Button(this, "OK", () => { RemoveMeAndChildren(); })
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
            var c = new Canvass();
            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return Parent.GetInnerCanvasTopLeft();
        }
    }
}
