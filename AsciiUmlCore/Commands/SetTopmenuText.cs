namespace AsciiUml.Commands {
	internal class SetTopmenuText : ICommand {
		private readonly string text;

		public SetTopmenuText(string text) {
			this.text = text;
		}

		public State Execute(State state) {
			state.Gui.TopMenuTextOverride = text;
			return state;
		}
	}
}