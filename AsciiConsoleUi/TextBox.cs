using System;

namespace AsciiUml.UI.GuiLib
{
    public class TextBox : GuiComponent
    {
        public string Value { get; set; }
        private int cursor = 0;
        public Action OnUserEscape { get; set; }
        public Action OnUserSubmit { get; set; }

        public TextBox(GuiComponent parent, int width, Coord position) : base(parent, position)
        {
            Dimensions = new GuiDimensions(new Size(width), new Size(1));
            Value = "";
            BackGround = ConsoleColor.DarkCyan;
            Foreground = ConsoleColor.Yellow;
            OnUserEscape = () => { };
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.LeftArrow)
            {
                cursor = Math.Max(0, cursor - 1);
                return true;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                var maxIndexForValue = Math.Min(Value.Length, cursor + 1);
                cursor = Math.Min(Dimensions.Width.Pixels, maxIndexForValue);
                return true;
            }

            if (key.Key == ConsoleKey.Delete)
            {
                var maxIndexForValue = Math.Min(Value.Length, cursor + 1);
                Value = Value.Substring(0, cursor) + Value.Substring(maxIndexForValue);
                return true;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (cursor > 0)
                {
                    var maxIndexForValue = Math.Min(Value.Length, cursor);
                    Value = Value.Substring(0, cursor - 1) + Value.Substring(maxIndexForValue);
                    cursor--;
                }
                return true;
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                Parent.FocusPrevChild(this);
                return true;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                Parent.FocusNextChild(this);
                return true;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                OnUserEscape();
                return true;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                Parent.Focus();
                return true;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                OnUserSubmit();
                return true;
            }

            if (Value.Length < Dimensions.Width.Pixels)
            {
                Value = Value.Substring(0, cursor) + key.KeyChar + Value.Substring(cursor);
                cursor++;
            }

            return true;
        }

        public override Canvass Paint()
        {
            var c = new Canvass();
            var res = Value.PadRight(Dimensions.Width.Pixels);
            c.RawPaintString(res, 0, 0, BackGround, Foreground);

            if (IsFocused)
            {
                WindowManager.SetCursorPosition(Position.Y, Position.X+cursor);
            }

            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0, 0);
        }
    }
}
