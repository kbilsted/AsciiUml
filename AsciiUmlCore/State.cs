using System.Collections.Generic;
using System.Linq;
using AsciiConsoleUi;
using AsciiUml.Geo;
using AsciiUml.UI;
using LanguageExt;

namespace AsciiUml {
	public class State {
		public const int MaxX = 80;
		public const int MaxY = 40;

		public Cursor TheCurser { get; set; }
		public Canvass Canvas;
		public int? SelectedIndexInModel { get; set; }
		public int? SelectedId { get; set; }
		public int? CursorHoverId { get; set; }
		public Configuration Config { get; set; } = new Configuration();
		public bool PaintSelectableIds { get; set; } = false;
		public GuiState Gui { get; set; } = new GuiState();
		public Model Model = new Model();

		public State() {
			Config.SaveFilename = @"c:\temp\asciiuml.txt";
		}

		public Option<IPaintable<object>> GetSelected() {
			var selected = Model.Objects.Where(x => x.Id == SelectedId).ToOption();
			return selected;
		}

		public static State ClearSelection(State state) {
			state.SelectedIndexInModel = null;
			state.SelectedId = null;
			return state;
		}
	}

	public class Model {
		public readonly List<IPaintable<object>> Objects = new List<IPaintable<object>>();

		public Model() {}

		public Model(IEnumerable<IPaintable<object>> objects) {
			Objects.AddRange(objects);
		}
	}

	public class GuiState {
		public string TopMenuTextOverride = null;
	}

	public class Configuration {
		public string SaveFilename { get; set; }
	}
}