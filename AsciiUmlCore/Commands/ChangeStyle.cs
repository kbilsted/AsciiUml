using System.Linq;
using AsciiUml.Geo;

namespace AsciiUml.Commands {
	internal class ChangeStyle : ICommand {
		public readonly int id;
		private readonly StyleChangeKind change;

		public ChangeStyle(int id, StyleChangeKind change) {
			this.id = id;
			this.change = change;
		}

		public State Execute(State state) {
			var elem = state.Model.Objects.FirstOrDefault(x => x.Id == id);
			if (elem is IStyleChangeable property) {
				property.ChangeStyle(change);
			}
			return state;
		}
	}
}