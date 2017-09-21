using System;
using AsciiUml.Commands;

namespace AsciiUml
{
    internal class CreateLabel : ICommand
    {
        private readonly Coord pos;
        private readonly string text;

        public CreateLabel(Coord pos, string text)
        {
            this.pos = pos;
            this.text = text;
        }

        public State Execute(State state)
        {
            var label = new Label(pos, text);
            state.Model.Add(label);
            state.SelectedId = label.Id;
            state.SelectedIndexInModel = state.Model.Count-1;
            return state;
        }
    }
}