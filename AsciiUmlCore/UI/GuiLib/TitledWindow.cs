using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsciiUml.Geo;

namespace AsciiUml.UI.GuiLib
{
    class TitledWindow : GuiComponent
    {
        private readonly string title;

        public TitledWindow(WindowManager manager, string title) : base(manager)
        {
            this.title = title;
        }

        public TitledWindow(GuiComponent parent, string title) : base(parent)
        {
            this.title = title;
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            return false;
        }

        public override Canvass Paint()
        {
            var size = Dimensions;
            if (Dimensions.IsFullyAutosize())
            {
                size = Children.Single().Dimensions+GetInnerCanvasTopLeft();
                Position = Children.Single().Position - GetInnerCanvasTopLeft();
            }
            else
            {
                Position = Parent.GetInnerCanvasTopLeft();
            }

            var titleline = title.PadRight(size.Width.Pixels - 4) + "[x] ";

            var c = new Canvass();
            c.RawPaintString(titleline, 0, 0, ConsoleColor.DarkGray, ConsoleColor.Gray);
            var line = "".PadRight(size.Width.Pixels);
            for (int y = 1; y < size.Height.Pixels; y++)
                c.RawPaintString(line, 0, y, BackGround, Foreground);

            return c;
        }

        public override void RemoveChild(GuiComponent child)
        {
            RemoveMeAndChildren();
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0, 1);
        }
    }
}
