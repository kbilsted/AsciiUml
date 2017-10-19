using System;

namespace AsciiConsoleUi
{
    public class Pixel
    {
        public char Char;
        public ConsoleColor BackGroundColor, ForegroundColor;

        public override bool Equals(object obj)
        {
            var other = (Pixel)obj;
            return Char == other.Char
                   && BackGroundColor == other.BackGroundColor
                   && ForegroundColor == other.ForegroundColor;
        }

        public static bool Compare(Pixel a, Pixel b)
        {
            var aIsEmpty = a == null || (a.Char == ' ' && a.BackGroundColor == ConsoleColor.Black);
            var bIsEmpty = b == null || (b.Char == ' ' && b.BackGroundColor == ConsoleColor.Black);

            if (aIsEmpty)
                return bIsEmpty;
            return !bIsEmpty && a.Equals(b);
        }

        public override int GetHashCode()
        {
            return Char.GetHashCode()
                ^ ForegroundColor.GetHashCode()
                ^ BackGroundColor.GetHashCode();
        }
    }
}