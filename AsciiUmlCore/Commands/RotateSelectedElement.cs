using AsciiUml.Geo;

namespace AsciiUml.Commands
{
    internal class RotateSelectedElement : ICommand
    {
        public readonly int id;

        public RotateSelectedElement(int id)
        {
            this.id = id;
        }

        public State Execute(State state)
        {
            var idx = state.Model.FindIndex(x => x.Id == id);
            state.Model[idx] =((Label) state.Model[idx]).Rotate();
            return state;
        }
    }
}