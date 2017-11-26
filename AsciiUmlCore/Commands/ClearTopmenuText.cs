namespace AsciiUml.Commands {
	internal class ClearTopmenuText : ICommand {
		public State Execute(State state) {
			state.Gui.TopMenuTextOverride = null;
			return state;
		}
	}
}