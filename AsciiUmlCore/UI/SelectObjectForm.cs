using System;
using AsciiConsoleUi;

namespace AsciiUml
{
    class SelectObjectForm
    {
        private readonly TitledWindow titled;
        private readonly TextLabel validationErrors;
        private readonly TextBox selected;
        public Action<int> OnSubmit = selected => { };
        public Action OnCancel = () => { };

        public SelectObjectForm(GuiComponent parent, Coord position)
        {
            titled = new TitledWindow(parent, "Select object"){Position = position};

            new TextLabel(titled, "Object:", new Coord(0, 0));
            selected = new TextBox(titled, 5, new Coord(0, 1)) { OnUserEscape = titled.RemoveMeAndChildren, OnUserSubmit = Submit};

            validationErrors = new TextLabel(titled, "", new Coord(0, 2))
                {
                    BackGround = ConsoleColor.White,
                    Foreground = ConsoleColor.Red
                };
        }
  
        void Submit()
        {
            if (string.IsNullOrWhiteSpace(selected.Value) || !int.TryParse(selected.Value, out var ifrom))
            {
                validationErrors.Text = "Need to fill in a number";
                return;
            }
            titled.RemoveMeAndChildren();
            OnSubmit(ifrom);
        }

        public void Focus() { selected.Focus();}
    }
}