using AsciiConsoleUi;

namespace AsciiUml.Geo {
	public class UmlUser : IPaintable<UmlUser>, ISelectable, IHasTextProperty {
		public int Id { get; }
		public Coord Pos { get; private set; }
		public string Text { get; set; }

		public UmlUser(Coord pos, string text) {
			Id = PaintAbles.GlobalId++;
			Pos = pos;
			Text = text;
		}

		public UmlUser Move(Coord delta) {
			Pos = Pos.Move(delta);
			return this;
		}
	}
}