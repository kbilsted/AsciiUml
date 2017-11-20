using System;
using System.Linq;
using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public class Note : IPaintable<Note>, ISelectable, IHasTextProperty {
		public int Id { get; }

		public int X => Pos.X;
		public int W { get; private set; }
		public int Y => Pos.Y;
		public int H { get; private set; }
		public Coord Pos { get; private set; }

		private string text;
		public string Text
		{
			get => text;
			set => SetText(value);
		}
	
		public Note(Coord pos, string text) {
			Id = PaintAbles.GlobalId++;
			Pos = pos;
			SetText(text);
		}

		public void SetText(string text)
		{
			var rows = text.Split('\n');

			var requiredWidth = rows.Select(x => x.Length).Max() + 5;
			W = Math.Max(W, requiredWidth);

			var requiredHeight = rows.Length + 3;
			H = Math.Max(H, requiredHeight);

			this.text = text;
		}

		public Note Move(Coord delta) {
			Pos = Pos.Move(delta);
			return this;
		}

		public void Check()
		{
			if (H < 2)
			{
				throw new ArgumentException("Height must be at least 2");
			}
			if (W < 2)
			{
				throw new ArgumentException("Width must be at least 2");
			}
		}
	}
}