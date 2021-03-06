﻿using System;
using System.Linq;
using AsciiConsoleUi;

namespace AsciiUml {
	internal class SelectObjectForm {
		private readonly int[] legalInput;
		private readonly TextBox selected;
		private readonly TitledWindow titled;
		private readonly TextLabel validationErrors;
		public Action OnCancel = () => { };
		public Action<int> OnSubmit = selected => { };

		public SelectObjectForm(GuiComponent parent, int[] legalInput, Coord position) {
			this.legalInput = legalInput;
			titled = new TitledWindow(parent, "Select object") {Position = position};

			new TextLabel(titled, "Object:", new Coord(0, 0));
			selected = new TextBox(titled, 5, new Coord(0, 1)) {OnUserEscape = titled.RemoveMeAndChildren, OnUserSubmit = Submit};

			validationErrors = new TextLabel(titled, "", new Coord(0, 2)) {
				BackGround = ConsoleColor.White,
				Foreground = ConsoleColor.Red
			};
		}

		private void Submit() {
			if (string.IsNullOrWhiteSpace(selected.Value) || !int.TryParse(selected.Value, out var ifrom)) {
				validationErrors.Text = "Need to fill in a number";
				return;
			}
			if (!legalInput.Contains(ifrom)) {
				validationErrors.Text = "No object with that ID";
				return;
			}

			titled.RemoveMeAndChildren();
			OnSubmit(ifrom);
		}

		public void Focus() {
			selected.Focus();
		}
	}
}