using System;
using AsciiUml.Geo;

namespace AsciiUml.Commands
{
    enum LineKind
    {
        Connected, Sloped
    }

    internal class CreateLine : ICommand
    {
        public readonly int @from;
        public readonly int to;
        private readonly LineKind kind;

        public CreateLine(int @from, int to, LineKind kind)
        {
            this.@from = @from;
            this.to = to;
            this.kind = kind;
        }

        public State Execute(State state)
        {
            switch (kind)
            {
                case LineKind.Connected:
                    var line = new Line() { FromId = from, ToId = to };
                    state.Model.Add(line);
                    state.SelectedId = null;
                    state.SelectedIndexInModel = null;
                    return state;
                    break;
                case LineKind.Sloped:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}