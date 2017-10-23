using System;

namespace AsciiConsoleUi.CompositeComponents
{
    public class SinglelineInputForm
    {
        private readonly TitledWindow titled;
        private readonly TextLabel validationErrors;
        private readonly TextBox selected;
        public Action<string> OnSubmit = selected => { };
        public Action OnCancel = () => { };

        public SinglelineInputForm(GuiComponent parent, string title, string explanation, int width, Coord position)
        {
            titled = new TitledWindow(parent, title){Position = position};

            new TextLabel(titled, explanation, new Coord(0, 0));
            selected = new TextBox(titled, width, new Coord(0, 1)) { OnUserEscape = titled.RemoveMeAndChildren, OnUserSubmit = Submit};

            validationErrors = new TextLabel(titled, "", new Coord(0, 2))
                {
                    BackGround = ConsoleColor.White,
                    Foreground = ConsoleColor.Red
                };
        }
  
        void Submit()
        {
            if (string.IsNullOrWhiteSpace(selected.Value))
            {
                validationErrors.Text = "Need to fill in a number";
                return;
            }
            titled.RemoveMeAndChildren();
            OnSubmit(selected.Value);
        }

        public void Focus() { selected.Focus();}
    }
}