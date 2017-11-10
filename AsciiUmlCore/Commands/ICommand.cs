namespace AsciiUml.Commands {
	public interface ICommand {
		State Execute(State state);
	}
}