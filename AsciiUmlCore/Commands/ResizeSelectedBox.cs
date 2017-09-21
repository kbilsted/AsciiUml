using AsciiUml.Commands;

namespace AsciiUml
{
    class ResizeSelectedBox : ICommand
    {
        readonly Coord delta;

        public ResizeSelectedBox(Coord delta)
        {
            this.delta = delta;
        }

        public State Execute(State state)
        {
            if (state.SelectedIndexInModel.HasValue)
            {
                var box = state.Model[state.SelectedIndexInModel.Value] as IResizeable<object>;
                if (box != null)
                    state.Model[state.SelectedIndexInModel.Value] = (IPaintable<object>)box.Resize(delta);
            }

            return state;
        }
    }

    
}