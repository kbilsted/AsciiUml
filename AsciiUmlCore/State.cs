using System.Collections.Generic;
using System.Linq;
using AsciiConsoleUi;
using AsciiUml.Geo;
using AsciiUml.UI;
using LanguageExt;

namespace AsciiUml
{
	public class State
	{
		public const int MaxX = 80;
		public const int MaxY = 40;

		public readonly List<IPaintable<object>> Model = new List<IPaintable<object>>();
		public Cursor TheCurser { get; set; }
		public Canvass Canvas;
		public int? SelectedIndexInModel { get; set; }
		public int? SelectedId { get; set; }
		public int? CursorHoverId { get; set; }
	    public Configuration Config { get; set; }
	    public bool PaintSelectableIds { get; set; } = false;

	    public State()
	    {
	        Config = new Configuration();
	        Config.SaveFilename = @"c:\temp\asciiuml.txt";
	    }
	    public Option<IPaintable<object>> GetSelected()
	    {
	        var selected = Model.Where(x => x.Id == SelectedId).ToOption();
	        return selected;
	    }

		public static State ClearSelection(State state)
		{
			state.SelectedIndexInModel = null;
			state.SelectedId = null;
			return state;
		}
	}

    public class Configuration
    {
        public string SaveFilename { get; set; }
    }
}
