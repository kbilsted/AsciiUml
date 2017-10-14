using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.Commands
{
    class MoveCursor : ICommand
    {
        public readonly Coord delta;

        public MoveCursor(Coord delta)
        {
            this.delta = delta;
        }

        public State Execute(State state)
        {
            state.TheCurser = state.TheCurser.Move(delta);
            return state;
        }
    }
}