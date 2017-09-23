using System.Collections.Generic;
using AsciiUml.Geo;
using AsciiUml.UI;

namespace AsciiUml
{
	public class State
	{
		public const int MaxX = 80;
		public const int MaxY = 40;

		public readonly List<IPaintable<object>> Model = new List<IPaintable<object>>();
		public Cursor TheCurser;
		public Canvass Canvas;
		public int? SelectedIndexInModel { get; set; }
		public int? SelectedId { get; set; }
		public int? CursorHoverId { get; set; }

		public static State ClearSelection(State state)
		{
			state.SelectedIndexInModel = null;
			state.SelectedId = null;
			return state;
		}
	}

}
