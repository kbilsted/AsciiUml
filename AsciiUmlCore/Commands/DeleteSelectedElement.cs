using AsciiUml.UI;

namespace AsciiUml.Commands
{
    internal class DeleteSelectedElement : ICommand
    {
        public State Execute(State state)
        {
            var selected = state.SelectedId;
            if (selected.HasValue)
            {
                state.Model.RemoveAt(state.Model.FindIndex(x => x.Id == selected));
                return State.ClearSelection(state);
            }

            Screen.PrintErrorAndWaitKey("Error. You must select an object before you can delete.");
            return state;
        }
    }
}