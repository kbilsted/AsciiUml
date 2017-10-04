namespace AsciiUml.Commands
{
    /// <summary>
    /// remove this once we have gui components for all user input
    /// </summary>
    internal class TmpForceRepaint : ICommand
    {
        public State Execute(State state)
        {
            return state;
        }
    }
}