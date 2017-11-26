using AsciiConsoleUi;
using AsciiUml.Geo;

namespace AsciiUml.Commands {
	internal class ResizeSelectedBox : ICommand {
		public readonly Coord delta;

		public ResizeSelectedBox(Coord delta) {
			this.delta = delta;
		}

		public State Execute(State state) {
			if (state.SelectedIndexInModel.HasValue)
				if (state.Model.Objects[state.SelectedIndexInModel.Value] is IResizeable<object> box)
					state.Model.Objects[state.SelectedIndexInModel.Value] = box.Resize(delta);

			return state;
		}
	}
}