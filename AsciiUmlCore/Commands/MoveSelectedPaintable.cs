﻿using AsciiConsoleUi;
using AsciiUml.Geo;
using LanguageExt;

namespace AsciiUml.Commands {
	internal class MoveSelectedPaintable : ICommand {
		public readonly Coord delta;

		public MoveSelectedPaintable(Coord delta) {
			this.delta = delta;
		}

		public State Execute(State state) {
			state.SelectedIndexInModel.ToOption()
				.Match(x => state.Model.Objects[x] = (IPaintable<object>) state.Model.Objects[x].Move(delta), () => { });
			return state;
		}
	}
}