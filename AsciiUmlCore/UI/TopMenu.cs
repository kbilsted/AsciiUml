using System;
using AsciiConsoleUi;
using AsciiUml.UI;

namespace AsciiUml
{
    class TopMenu : GuiComponent
    {
        private readonly State state;

        public TopMenu(WindowManager manager, State state) : base(manager)
        {
            this.state = state;
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            return false;
        }

        public override Canvass Paint()
        {
            var c = new Canvass();
            var menu = $"AsciiUml v0.1.2 Selected: {state.SelectedId?.ToString() ?? "None"}. ({state.TheCurser}) Press \'h\' for help";
            Canvass.PaintString(c, menu, 0, 0, -1, ConsoleColor.DarkGreen, ConsoleColor.Green);
            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0,1);
        }
    }
}