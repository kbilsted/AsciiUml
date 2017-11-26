using System;
using System.Collections.Generic;
using System.Linq;
using AsciiConsoleUi;
using AsciiUml.Commands;
using AsciiUml.UI;

namespace AsciiUml {
	internal class UmlWindow : GuiComponent {
		private readonly List<List<ICommand>> commandLog = new List<List<ICommand>>();
		private State state;

		public UmlWindow(GuiComponent parent, State state) : base(parent) {
			this.state = state;
		}

		public override bool HandleKey(ConsoleKeyInfo key) {
			var commands = KeyHandler.HandleKeyPress(state, key, commandLog, this);

			HandleCommands(commands);

			return commands.Any();
		}

		public void HandleCommands(List<ICommand> commands) {
			AddToCommandLog(commands);

			foreach (var command in commands) state = command.Execute(state);

			if (commands.Any(x => x is TmpForceRepaint))
				TemporarilyForceRepaint();
		}

		private void AddToCommandLog(List<ICommand> commands) {
			if (commands.Any())
				commandLog.Add(commands);
		}

		public override Canvass Paint() {
			var canvass = PaintServiceCore.Paint(state);
			state.Canvas = canvass; // todo should be more functional, ie a clone
			return canvass;
		}

		public override Coord GetInnerCanvasTopLeft() {
			return Parent.GetInnerCanvasTopLeft();
		}

		public override void OnException(Exception e) {
			Console.WriteLine("something unexpected happened " + e.Message + " :: " + e.StackTrace);

			Program.Serialize(commandLog);
		}
	}
}