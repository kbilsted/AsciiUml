using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.Commands
{
    public class CreateSlopedLine : ICommand
    {
        private readonly Coord pos;

        public CreateSlopedLine(Coord pos)
        {
            this.pos = pos;
        }

        public State Execute(State state)
        {
            var line = new SlopedLine2(pos);
            state.Model.Add(line);
            state.SelectedId = null;
            state.SelectedIndexInModel = null;
            return state;
        }
    }
}