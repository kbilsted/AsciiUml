using AsciiUml.Geo;

namespace AsciiUml.Commands
{
    internal class DragLinePixel : ICommand
    {
        private readonly Coord @from;
        private readonly Coord delta;

        public DragLinePixel(Coord @from, Coord delta)
        {
            this.@from = @from;
            this.delta = delta;
        }

        public State Execute(State state)
        {
            return state.GetSelected()
                .Match(x =>
                {
                    (x as SlopedLine2).Drag(@from, @from + delta);
                    return state;
                }, () => state);
        }
    }
}