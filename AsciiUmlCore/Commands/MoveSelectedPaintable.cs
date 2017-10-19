using AsciiConsoleUi;
using AsciiUml.Geo;
using LanguageExt;

namespace AsciiUml.Commands
{
	class MoveSelectedPaintable : ICommand
	{
		public readonly Coord delta;

		public MoveSelectedPaintable(Coord delta)
		{
			this.delta = delta;
		}

		public State Execute(State state)
		{
			state.SelectedIndexInModel.ToOption()
				.Match(x => state.Model[x] = (IPaintable<object>)state.Model[x].Move(delta), () => { });
			return state;
		}
	}
}