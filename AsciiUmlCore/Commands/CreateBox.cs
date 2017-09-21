using System;
using AsciiUml.Commands;

namespace AsciiUml
{
    internal class CreateBox : ICommand
    {
        private readonly Coord pos;
        private readonly string text;

        public CreateBox(Coord pos, string text)
        {
            this.pos = pos;
            this.text = text;
        }

        public State Execute(State state)
        {
            var box = new Box(pos, text);
            state.Model.Add(box);
            state.SelectedId = box.Id;
            state.SelectedIndexInModel = state.Model.Count-1;
            return state;
        }
    }
}