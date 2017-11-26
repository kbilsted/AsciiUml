using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public class UmlUser : IPaintable<UmlUser>, ISelectable, IHasTextProperty {
		public UmlUser(Coord pos, string text) {
			Id = PaintAbles.GlobalId++;
			Pos = pos;
			Text = text;
		}

		public string Text { get; set; }
		public int Id { get; }

		public UmlUser Move(Coord delta) {
			Pos = Pos.Move(delta);
			return this;
		}

		public Coord Pos { get; private set; }
	}
}