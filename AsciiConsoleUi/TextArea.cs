using System;
using System.Collections.Generic;
using System.Linq;

namespace AsciiConsoleUi
{
    // TODO we need scrollbars! 
    // todo support word wrapping
    public class TextArea : GuiComponent
    {
        public string Value => string.Join("\n", lines);
        private Coord cursor = new Coord(0,0);
        public Action OnUserEscape { get; set; }
        readonly List<string> lines;

        public TextArea(GuiComponent parent, int width, int height, string content, Coord position) : base(parent, position)
        {
            lines = (content ?? "").Split(new[] {'\n'}, StringSplitOptions.None).ToList();
            
            Dimensions = new GuiDimensions(new Size(width), new Size(height));

            BackGround = ConsoleColor.DarkCyan;
            Foreground = ConsoleColor.Yellow;
            OnUserEscape = () => { RemoveMeAndChildren(); };
        }

        public override bool HandleKey(ConsoleKeyInfo key)
        {
            var x = cursor.X;
            var y = cursor.Y;
            var currentLine = lines[y];

            if (key.Key == ConsoleKey.LeftArrow)
            {
                if (x > 0)
                {
                    cursor = new Coord(x - 1, y);
                    return true;
                }

                if (y > 0)
                {
                    cursor = new Coord(lines[y - 1].Length, y - 1);
                    return true;
                }
                
                return true;
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                if (x < currentLine.Length)
                {
                    cursor = new Coord(x + 1, y);
                    return true;
                }

                if (y < lines.Count-1)
                {
                    cursor = new Coord(0, y + 1);
                    return true;
                }

                return true;
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (y == 0)
                    return true;
                cursor = new Coord(Math.Min(x, lines[y - 1].Length), y - 1);
                return true;
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                if (y == lines.Count-1)
                    return true;
                cursor = new Coord(Math.Min(x, lines[y + 1].Length), y + 1);
                return true;
            }

            if (key.Key == ConsoleKey.Delete)
            {
                var cursorOnLastCharOfLine = x == currentLine.Length;
                if (!cursorOnLastCharOfLine)
                {
                    var maxIndexForValue = Math.Min(currentLine.Length, x + 1);
                    lines[y] = lines[y].Substring(0, x) + lines[y].Substring(maxIndexForValue);
                   return true;
                }

                var cursorIsOnLastLine = y == lines.Count-1;
                if (!cursorIsOnLastLine)
                {
                    lines[y] += lines[y + 1];
                    lines.RemoveAt(y+1);
                    return true;
                }
                return true;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (x == 0)
                {
                    if(y == 0)
                        return true;
                    var linelength = lines[y - 1].Length;
                    lines[y - 1] += lines[y];
                    lines.RemoveAt(y);
                    cursor = new Coord(linelength, y - 1);
                    return true;
                }

                var maxIndexForValue = Math.Min(currentLine.Length, x);
                lines[y] = currentLine.Substring(0, x - 1) + currentLine.Substring(maxIndexForValue);
                cursor = new Coord(x - 1, y);
                return true;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                OnUserEscape();
                return true;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                Parent.FocusNextChild(this);
                return true;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                if (lines.Count < Dimensions.Height.Pixels)
                {
                    var index = y + 1;
                    lines[y] = currentLine.Substring(0, x);
                    lines.Insert(index, currentLine.Substring(x));
                    cursor = new Coord(0, index);
                    return true;
                }
                return true;
            }

            if (currentLine.Length < Dimensions.Width.Pixels)
            {
                lines[y] = currentLine.Substring(0, x) + key.KeyChar + currentLine.Substring(x);
                cursor = new Coord(x + 1, y);
            }

            return true;
        }

        public override Canvass Paint()
        {
            var c = new Canvass();

            for (int i = 0; i < Dimensions.Height.Pixels; i++)
            {
                var width = Dimensions.Width.Pixels;

                var line = (i < lines.Count ? lines[i] : "").PadRight(width);
                if (line.Length > width)
                    line = line.Substring(0, width);
                c.RawPaintString(line, 0, i, BackGround, Foreground);
            }

            if (IsFocused)
            {
                WindowManager.SetCursorPosition(Position.Y + cursor.Y, Position.X + cursor.X);
                Console.CursorVisible = true;
            }

            return c;
        }

        public override Coord GetInnerCanvasTopLeft()
        {
            return new Coord(0, 0);
        }
    }
}
