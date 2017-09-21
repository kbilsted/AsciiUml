using System.Linq;
using AsciiUml.Commands;

namespace AsciiUml
{
    internal class DeleteSelectedElement : ICommand
    {
        public State Execute(State state)
        {
            var selected = state.SelectedId;
            if (selected.HasValue)
            {
                state.Model.RemoveAt(state.Model.FindIndex(x => x.Id == selected));
                return Program.ClearSelection(state);
            }

            Program.PrintErrorAndWaitKey("Error. You must select an object before you can delete.");
            return state;
        }
    }
}