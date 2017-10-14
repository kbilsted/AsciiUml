using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.Commands {
	internal class CreateDatabase : ICommand
	{
		public readonly Coord pos;

		public CreateDatabase(Coord pos)
		{
			this.pos = pos;
		}

		public State Execute(State state)
		{
			var db = new Database(pos);
			state.Model.Add(db);
			state.SelectedId = db.Id;
			state.SelectedIndexInModel = state.Model.Count - 1;
			return state;
		}
	}
}