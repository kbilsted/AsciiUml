using AsciiConsoleUi;
using AsciiUml.Geo;

namespace AsciiUml.Commands
{
	internal class CreateBox : ICommand
	{
		public readonly Coord pos;
		public readonly string text;

		public CreateBox(Coord pos, string text)
		{
			this.pos = pos;
			this.text = text;
		}

		public State Execute(State state)
		{
			var box = new Box(pos, text);
			state.Model.Add(box);
			state.SelectedId = box.Id;
			state.SelectedIndexInModel = state.Model.Count - 1;
			return state;
		}
	}
}