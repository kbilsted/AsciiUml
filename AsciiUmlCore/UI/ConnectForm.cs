using System;
using System.Linq;
using AsciiConsoleUi;

namespace AsciiUml.UI {
	internal class ConnectForm {
		private readonly TextBox from, to;
		private readonly TitledWindow titled;
		private readonly TextLabel validationErrors;
		public Action OnCancel = () => { };
		public Action<int, int> OnSubmit = (from, to) => { };
		private int[] legalInput;

		public ConnectForm(GuiComponent parent, Coord position, int[] legalInput) {
			this.legalInput = legalInput;
			titled = new TitledWindow(parent, "Connect...") {Position = position};

			new TextLabel(titled, "From object:", new Coord(0, 0));
			from = new TextBox(titled, 5, new Coord(0, 1)) {OnUserEscape = titled.RemoveMeAndChildren};
			new TextLabel(titled, "To object:", new Coord(0, 2));
			to = new TextBox(titled, 5, new Coord(0, 3)) {OnUserEscape = titled.RemoveMeAndChildren, OnUserSubmit = Submit};

			from.OnUserSubmit = to.Focus;

			validationErrors = new TextLabel(titled, "", new Coord(0, 4)) {
				BackGround = ConsoleColor.White,
				Foreground = ConsoleColor.Red
			};
		}

		private void Submit() {
			if (string.IsNullOrWhiteSpace(from.Value) || !int.TryParse(from.Value, out var ifrom)) {
				validationErrors.Text = "Need to fill in 'from'";
				return;
			}

			if (!legalInput.Contains(ifrom)) {
				validationErrors.Text = $"No object with id '{ifrom}'";
				return;
			}

			if (string.IsNullOrWhiteSpace(to.Value) || !int.TryParse(to.Value, out var ito)) {
				validationErrors.Text = "Need to fill in 'to'";
				return;
			}

			if (!legalInput.Contains(ito))
			{
				validationErrors.Text = $"No object with id '{ito}'";
				return;
			}

			titled.RemoveMeAndChildren();
			OnSubmit(ifrom, ito);
		}

		public void Focus() {
			from.Focus();
		}
	}
}