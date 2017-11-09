using System.Linq;
using AsciiUml.Geo;

namespace AsciiUml.Commands
{
    internal class SetText : ICommand
    {
        public readonly int id;
        public readonly string text;

        public SetText(int id, string text)
        {
            this.id = id;
            this.text = text;
        }

        public State Execute(State state)
        {
            var elem = state.Model.FirstOrDefault(x => x.Id == id);
            if(elem is IHasTextProperty property)
                property.Text=text;
            return state;
        }
    }
}