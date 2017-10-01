using System;
using AsciiUml.Geo;
using Microsoft.VisualBasic.CompilerServices;

namespace AsciiUml
{
    public class GuiDimensions
    {
        public Size Width, Height;

        public GuiDimensions(Size width, Size height)
        {
            Width = width;
            Height = height;
        }

        public bool IsFullyAutosize()
        {
            return Width.Kind == SizeKind.Auto && Height.Kind == SizeKind.Auto;
        }

        public bool IsFullyFixed()
        {
            return Width.Kind == SizeKind.Fixed && Height.Kind == SizeKind.Fixed;
        }

        public static GuiDimensions operator +(GuiDimensions a, GuiDimensions b)
        {
            if (a.IsFullyFixed() && b.IsFullyFixed())
                return new GuiDimensions(new Size(a.Width.Pixels + b.Width.Pixels), new Size(a.Height.Pixels + b.Height.Pixels));

            if (a.IsFullyAutosize() && b.IsFullyAutosize())
                return new GuiDimensions(new Size(), new Size());

            throw new ArgumentException("Cannot add auto with fixed size");
        }

        public static GuiDimensions operator +(GuiDimensions a, Coord b)
        {
            if (a.IsFullyFixed())
                return new GuiDimensions(new Size(a.Width.Pixels + b.X), new Size(a.Height.Pixels + b.Y));

            throw new ArgumentException("Cannot add auto with fixed size");
        }
    }

    public enum SizeKind
    {
        Fixed, Auto
    }

    public class Size
    {
        public SizeKind Kind;

        private int pixels;
        public int Pixels
        {
            get => pixels;
            set
            {
                pixels = value;
                Kind = SizeKind.Fixed;
            }
        }

        public Size()
        {
            Kind = SizeKind.Auto;
            this.pixels = -1;
        }

        public Size(int pixels)
        {
            Kind = SizeKind.Fixed;
            this.pixels = pixels;
        }
    }
}