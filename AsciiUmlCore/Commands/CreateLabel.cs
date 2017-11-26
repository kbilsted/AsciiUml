using AsciiConsoleUi;
using AsciiUml.Geo;

namespace AsciiUml.Commands {
	internal class CreateLabel : ICommand {
		public readonly Coord pos;
		public readonly string text;

		public CreateLabel(Coord pos, string text) {
			this.pos = pos;
			this.text = text;
		}

		public State Execute(State state) {
			var label = new Label(pos, text);
			state.Model.Objects.Add(label);
			state.SelectedId = label.Id;
			state.SelectedIndexInModel = state.Model.Objects.Count - 1;
			return state;
		}
	}
}