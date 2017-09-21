using AsciiUml.Commands;
using LanguageExt;

namespace AsciiUml
{
    class MoveSelectedPaintable : ICommand
    {
        readonly Coord delta;

        public MoveSelectedPaintable(Coord delta)
        {
            this.delta = delta;
        }

        public State Execute(State state)
        {
            state.TheCurser = state.TheCurser.Move(delta);
            state.SelectedIndexInModel.ToOption()
                .Match(x => state.Model[x] = (IPaintable<object>)state.Model[x].Move(delta), () => { });
            return state;
        }
    }
}