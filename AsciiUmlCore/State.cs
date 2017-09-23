using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AsciiUml.Geo;
using AsciiUml.UI;

namespace AsciiUml
{
    public class State {
		public const int MaxX = 80;
		public const int MaxY = 40;

		public readonly List<IPaintable<object>> Model = new List<IPaintable<object>>();
		public Cursor TheCurser;
		public Canvass Canvas;
		public int? SelectedIndexInModel { get; set; }
		public int? SelectedId { get; set; }
		public int? CursorHoverId { get; set; }
	}
}