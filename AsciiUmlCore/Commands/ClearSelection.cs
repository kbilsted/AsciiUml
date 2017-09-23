using AsciiUml.Commands;

namespace AsciiUml
{
	internal class ClearSelection : ICommand
	{
		public State Execute(State state)
		{
			return State.ClearSelection(state);
		}
	}
}