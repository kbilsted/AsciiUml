using AsciiConsoleUi;
using AsciiUml.Geo;

namespace AsciiUml.Commands {
	internal class CreateUmlUser : ICommand
	{
		public readonly Coord pos;
		public readonly string text;

		public CreateUmlUser(Coord pos, string text)
		{
			this.pos = pos;
			this.text = text;
		}

		public State Execute(State state)
		{
			var box = new UmlUser(pos, text);
			state.Model.Objects.Add(box);
			state.SelectedId = box.Id;
			state.SelectedIndexInModel = state.Model.Objects.Count - 1;
			return state;
		}
	}
}