using System;
using AsciiUml.Commands;

namespace AsciiUml
{
    internal class CreateLine : ICommand
    {
        private readonly int @from;
        private readonly int to;

        public CreateLine(int @from, int to)
        {
            this.@from = @from;
            this.to = to;
        }

        public State Execute(State state)
        {
            var line = new Line() {FromId = from, ToId = to};
            state.Model.Add(line);
            state.SelectedId = null;
            state.SelectedIndexInModel = null;
            return state;
        }
    }
}