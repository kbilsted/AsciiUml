using System;
using AsciiUml.Geo;

namespace AsciiUml.UI.GuiLib
{
    public class TextLabel : GuiComponent
    {
        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value; 
                Dimensions = new GuiDimensions(new Size(text.Length), new Size(1));
            }
        }

        public TextLabel(GuiComponent parent, string text, Coord position) : base(parent, position)
        {
            Text = text;
            IsFocusable = false;
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            return Parent.HandleKey(key);
        }

        public override Canvass Paint()
        {
            var c = new Canvass();
            c.RawPaintString(Text, 0, 0, BackGround, Foreground);
            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(Text.Length,1);
        }
    }
}