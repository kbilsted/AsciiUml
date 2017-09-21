using System.Collections.Generic;
using AsciiUml.Geo;
using AsciiUml.UI;

namespace AsciiUml
{
    public class State {
		public readonly List<IPaintable<object>> Model = new List<IPaintable<object>>();
		public Cursor TheCurser;
		public Canvass Canvas;
		public int? SelectedIndexInModel { get; set; }
		public int? SelectedId { get; set; }
		public int? CursorHoverId { get; set; }
	}
}