using System;
using AsciiUml.Geo;

namespace AsciiUml.Commands {
	internal enum LineKind {
		Connected,
		Sloped
	}

	internal class CreateLine : ICommand {
		public readonly int from;
		private readonly LineKind kind;
		public readonly int to;

		public CreateLine(int from, int to, LineKind kind) {
			this.from = from;
			this.to = to;
			this.kind = kind;
		}

		public State Execute(State state) {
			switch (kind) {
				case LineKind.Connected:
					var line = new Line {FromId = from, ToId = to};
					state.Model.Objects.Add(line);
					state.SelectedId = null;
					state.SelectedIndexInModel = null;
					return state;
				case LineKind.Sloped:
					throw new NotImplementedException();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}