using System.Linq;
using AsciiUml.Geo;

namespace AsciiUml.Commands
{
    internal class SetBoxText : ICommand
    {
        public readonly int id;
        public readonly string text;

        public SetBoxText(int id, string text)
        {
            this.id = id;
            this.text = text;
        }

        public State Execute(State state)
        {
            state.Model.Where(x => x.Id == id).OfType<Box>().Single().SetText(text);
            return state;
        }
    }
}