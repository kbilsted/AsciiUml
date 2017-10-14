using System;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.UI
{
    public class PopupNoButton : GuiComponent
    {
        public PopupNoButton(GuiComponent parent, string message) : base(parent)
        {
            var label = new TextLabel(this, message, new Coord(0, 4)){BackGround = ConsoleColor.Black};
            Dimensions = label.GetSize();
            Dimensions.Width.Pixels +=  6;

            Position = new Coord((State.MaxX - Dimensions.Width.Pixels) / 2, ((State.MaxY - Dimensions.Height.Pixels) / 2) - 1);
            label.AdjustWhenParentsReposition();
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
            var c = new Canvass();
            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return Parent.GetInnerCanvasTopLeft();
        }
    }
}
