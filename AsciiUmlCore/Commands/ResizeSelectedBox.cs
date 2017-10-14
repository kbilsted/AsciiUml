using System;
using AsciiUml.Geo;
using AsciiUml.UI.GuiLib;

namespace AsciiUml.Commands
{
    class ResizeSelectedBox : ICommand
    {
        public readonly Coord delta;

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