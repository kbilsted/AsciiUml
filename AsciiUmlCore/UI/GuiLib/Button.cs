using System;
using System.Collections.Generic;
using System.Text;
using AsciiUml.Geo;

namespace AsciiUml.UI.GuiLib
{
    class Button : GuiComponent
    {
        private readonly string buttonText;
        private readonly Action onClick;
        private readonly ConsoleColor backgroundColor;
        private readonly ConsoleColor foregroundColor;

        public Button(GuiComponent parent, string buttonText, Action onClick, ConsoleColor backgroundColor = ConsoleColor.DarkGray, ConsoleColor foregroundColor=ConsoleColor.Green) : base(parent)
        {
            this.buttonText = buttonText;
            this.onClick = onClick;
            this.backgroundColor = backgroundColor;
            this.foregroundColor = foregroundColor;
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
            Canvass.PaintString(c, " " + focusMarker + buttonText + "  ", 0, 0, -10, backgroundColor, foregroundColor);
            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0,0);
        }
    }
}
