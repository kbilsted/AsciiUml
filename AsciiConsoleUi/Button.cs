using System;
using System.Text;

namespace AsciiUml.UI.GuiLib
{
    class Button : GuiComponent
    {
        private readonly string buttonText;
        private readonly Action onClick;

        public Button(GuiComponent parent, string buttonText, Action onClick, Coord position) : base(parent, position)
        {
            this.buttonText = buttonText;
            this.onClick = onClick;
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            if (IsFocused)
            {
                if (key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.Enter || key.Key==ConsoleKey.Escape)
                {
                    onClick();
                }
                if (key.Key==ConsoleKey.Tab)
                    Parent.Focus();
                return true;
            }
            return false;
        }

        public override Canvass Paint()
        {
            var c = new Canvass();
            var focusMarker = (IsFocused?">": " ");
            Canvass.PaintString(c, " " + focusMarker + buttonText + "  ", 0, 0, -10, BackGround, Foreground);
            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0,0);
        }
    }
}
