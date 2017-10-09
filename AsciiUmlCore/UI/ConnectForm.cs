using System;
using AsciiUml.Geo;
using AsciiUml.UI;
using AsciiUml.UI.GuiLib;

namespace AsciiUml
{
    class ConnectForm : GuiComponent
    {
        private readonly TextLabel connectA, connectB, validationErrors;
        private readonly TextBox from, to;

        public ConnectForm(GuiComponent parent, Coord position) : base(parent, position)
        {
            connectA = new TextLabel(this, "From object:", new Coord(0, 0));
            from = new TextBox(this, 5, new Coord(0, 1)) {OnUserEscape = RemoveMeAndChildren, OnUserSubmit = OnSubmit};
            connectB = new TextLabel(this, "To object:", new Coord(0, 2));
            to = new TextBox(this, 5, new Coord(0, 3)) {OnUserEscape = RemoveMeAndChildren, OnUserSubmit = OnSubmit};

            validationErrors = new TextLabel(this, "", new Coord(0, 4))
                {
                    BackGround = ConsoleColor.White,
                    Foreground = ConsoleColor.Red
                };
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            return false;
        }

        public override Canvass Paint()
        {
            return new Canvass();
        }

        public override void Focus()
        {
            from.Focus();
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0,0);
        }

        void OnSubmit()
        {
            if (string.IsNullOrWhiteSpace(from.Value))
                validationErrors.Text = "Need to fill in 'from'";
            if (string.IsNullOrWhiteSpace(to.Value))
                validationErrors.Text = "Need to fill in 'to'";
        }

        public override GuiDimensions GetSize()
        {
            var size = base.GetSize();
            return size;
        }
    }
}