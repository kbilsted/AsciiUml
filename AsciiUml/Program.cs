using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;

namespace AsciiUml {
	public class State {
		public readonly List<IPaintable<object>> Model = new List<IPaintable<object>>();
		public Cursor TheCurser;
		public Canvass Canvas;
		public int? SelectedIndexInModel { get; set; }
		public int? SelectedId { get; set; }
		public int? CursorHoverId { get; set; }
	}

	class Program {
		static void Main(string[] args) {
			EnableCatchingShiftArrowPresses();

			Console.SetWindowSize(90, 50);
			var state = new State();
			state.TheCurser = new Cursor(new Coord(0, 0));

			TempModelForPlayingAround(state.Model);

			state = ReadKeyboardEvalLoop(state);
			return;
		}

		static State ResizeSelectedBox(State state, Coord delta) {
			if (state.SelectedIndexInModel.HasValue) {
				var box = state.Model[state.SelectedIndexInModel.Value] as IResizeable<object>;
				if (box != null)
					state.Model[state.SelectedIndexInModel.Value] = (IPaintable<object>) box.Resize(delta);
			}

			return state;
		}

		private static State MoveSelectedPaintable(State state, Coord delta) {
			state.TheCurser = state.TheCurser.Move(delta);
			state.SelectedIndexInModel.ToOption()
				.Match(x => state.Model[x] = (IPaintable<object>) state.Model[x].Move(delta), () => { });
			return state;
		}

		private static State ReadKeyboardEvalLoop(State state) {
			while (true) {
				state.Canvas = PrintToScreen(state);

				var key = Console.ReadKey(true);
				var model = state.Model;
				var selected = state.SelectedIndexInModel;

				if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.KeyChar == '\u0003') {
					return state;
				}

				var newState = ControlKeys(state, key)
					.Match(s => s, () => ShiftKeys(state, key))
					.Match(s => s, () => HandleKeys(state, key, selected, model))
					.IfNone(() => state);
				state = newState;
			}
		}

		private static State PerformSelectObject(State state, int id, bool moveCursor) {
			var idx = state.Model.FindIndex(0, m => m.Id == id);
			var elem = state.Model[idx] as ISelectable;
			if (elem == null)
				return state;

			state.SelectedIndexInModel = idx;
			state.SelectedId = id;
			if (moveCursor)
				state.TheCurser = new Cursor(elem.Pos);

			return state;
		}

		private static Option<State> HandleKeys(State state, ConsoleKeyInfo key, int? selected, List<IPaintable<object>> model) {
			switch (key.Key) {
				case ConsoleKey.UpArrow:
					return MoveSelectedPaintable(state, Vector.DeltaNorth);
				case ConsoleKey.DownArrow:
					return MoveSelectedPaintable(state, Vector.DeltaSouth);
				case ConsoleKey.LeftArrow:
					return MoveSelectedPaintable(state, Vector.DeltaWest);
				case ConsoleKey.RightArrow:
					return MoveSelectedPaintable(state, Vector.DeltaEast);

				case ConsoleKey.Spacebar:
					if (selected.HasValue)
						return ClearSelection(state);

					var obj = SelectObject(state)
						.Match(x => PerformSelectObject(state, x.Item1, x.Item2),
							() => ClearSelection(state));
					return obj;

				case ConsoleKey.S:
					return PrintIdsAndLetUserSelectOpbejct(state)
						.Match(x => PerformSelectObject(state, x, true), () => state);

				case ConsoleKey.X:
				case ConsoleKey.Delete:
					if (selected.HasValue) {
						model.RemoveAt(selected.Value);
						return ClearSelection(state);
					}

					PrintErrorAndWaitKey("Error. You must select an object before you can delete.");
					return state;

				case ConsoleKey.H:
					Help();
					return state;

				case ConsoleKey.B:
					CreateBox(state.TheCurser).IfSome(x => {
						model.Insert(0, x);
						state.SelectedId = x.Id;
						state.SelectedIndexInModel = 0;
					});
					return state;

				case ConsoleKey.C:
					Console.WriteLine("Connect from objecgt: ");

					var from = PrintIdsAndLetUserSelectOpbejct(state);
					var to = PrintIdsAndLetUserSelectOpbejct(state);
					@from.IfSome(ffrom => to.IfSome(tto => {
						var line = new Line() {FromId = ffrom, ToId = tto};
						model.Insert(0, line);
					}));
					return state;

				case ConsoleKey.T:
					return CreateLabel().Match(x => {
						model.Insert(0, x);
						state.SelectedId = x.Id;
						state.SelectedIndexInModel = 0;
						return state;
					}, () => state);

				case ConsoleKey.R:
					selected.ToOption().Match(x => {
							if (model[x] is Label)
								model[x] = ((Label) model[x]).Rotate();
							else
								PrintErrorAndWaitKey("Only labels can be rotated");
						},
						() => PrintErrorAndWaitKey("Nothing is selected"));
					return state;
			}
			return null;
		}

		public static State SelectTemporarily(State state, Func<State, State> code) {
			if (state.SelectedId.HasValue) 
				return code(state);

			return SelectObject(state).Match(x => {
					state = PerformSelectObject(state, x.Item1, x.Item2);
					state = code(state);
					return ClearSelection(state);
				}, () => state);
		}

		private static Option<State> ControlKeys(State state, ConsoleKeyInfo key) {
			if ((key.Modifiers & ConsoleModifiers.Control) == 0)
				return null;

			return SelectTemporarily(state, x => {
				switch (key.Key) {
					case ConsoleKey.UpArrow:
						return ResizeSelectedBox(x, Vector.DeltaNorth);
					case ConsoleKey.DownArrow:
						return ResizeSelectedBox(x, Vector.DeltaSouth);
					case ConsoleKey.LeftArrow:
						return ResizeSelectedBox(x, Vector.DeltaWest);
					case ConsoleKey.RightArrow:
						return ResizeSelectedBox(x, Vector.DeltaEast);
				}
				return null;
			});
		}

		private static Option<State> ShiftKeys(State state, ConsoleKeyInfo key) {
			var model = state.Model;
			if ((key.Modifiers & ConsoleModifiers.Shift) == 0)
				return null;

			var objectId = state.Canvas.GetOccupants(state.TheCurser.Pos);
			if (!objectId.HasValue)
				return null;

			PerformSelectObject(state, objectId.Value, false);

			switch (key.Key) {
				case ConsoleKey.UpArrow:
					return MoveSelectedPaintable(state, Vector.DeltaNorth);
				case ConsoleKey.DownArrow:
					return MoveSelectedPaintable(state, Vector.DeltaSouth);
				case ConsoleKey.LeftArrow:
					return MoveSelectedPaintable(state, Vector.DeltaWest);
				case ConsoleKey.RightArrow:
					return MoveSelectedPaintable(state, Vector.DeltaEast);
			}
			return null;
		}

		private static void EnableCatchingShiftArrowPresses() {
			Console.TreatControlCAsInput = true;
		}

		private static State ClearSelection(State state) {
			state.SelectedIndexInModel = null;
			state.SelectedId = null;
			return state;
		}

		private static Canvass PrintToScreen(State state) {
			Console.Clear();
			PaintTopMenu(state);

			var canvass = PaintServiceCore.Paint(state);

			Console.WriteLine(canvass.ToString());
			PrintCursor(state.TheCurser, canvass);

			return canvass;
		}

		private static void PaintTopMenu(State state) {
			Screen.SetConsoleGetInputColors();
			Console.WriteLine(
				$"AsciiUml v1.0. Selected: {state.SelectedId?.ToString() ?? "None"}. ({state.TheCurser.X}, {state.TheCurser.Y}) Press \'h\' for help");
			Screen.SetConsoleStandardColor();
		}

		private static void PrintCursor(Cursor curser, Canvass canvass) {
			var top = Console.CursorTop;
			var left = Console.CursorLeft;
			Screen.SetConsoleSelectColor();
			var x = curser.X;
			var y = curser.Y;
			const int TopMenuSize = 1;
			Console.SetCursorPosition(x, y + TopMenuSize);
			Console.Write(canvass.GetCell(curser.Pos));
			Screen.SetConsoleStandardColor();
			Console.SetCursorPosition(left, top);
		}

		private static void TempModelForPlayingAround(List<IPaintable<object>> model) {
			model.Add(new Box(new Coord(0, 0)).SetText("Foo\nMiddleware\nMW1"));
			//model.Add(new Box() {Y = 14, Text = "goo\nand\nbazooka"});
			model.Add(new Box(new Coord(19, 27)).SetText("foo\nServer\nbazooka"));
			model.Add(new Box(new Coord(13, 20)).SetText("goo\nWeb\nServer"));
			model.Add(new Line() {FromId = 0, ToId = 1});
			model.Add(new Label(new Coord(5, 5), "Server\nClient\nAAA"));
		}

		private static Option<Label> CreateLabel() {
			Screen.SetConsoleGetInputColors();
			Console.Write("Create a label. Text: ");
			var res = CommandParser.TryReadLineWithCancel().Match(x => new Label(x), () => Option<Label>.None);
			Screen.SetConsoleStandardColor();
			return res;
		}

		private static Option<Box> CreateBox(Cursor cursor) {
			Screen.SetConsoleGetInputColors();
			Console.Write("Create box. Title: ");
			var res = CommandParser.TryReadLineWithCancel()
				.Match(x => new Box(cursor.Pos).SetText(x), () => Option<Box>.None);
			Screen.SetConsoleStandardColor();
			return res;
		}

		private static void PrintErrorAndWaitKey(string text) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.BackgroundColor = ConsoleColor.White;
			Console.WriteLine(text);
			Screen.SetConsoleStandardColor();
			Console.ReadKey();
		}

		private static void Help() {
			Console.WriteLine("");
			Console.WriteLine("");
			Console.WriteLine("");
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("* *  ****  *     ****    ");
			Console.WriteLine("* *  *     *     *  *    ");
			Console.WriteLine("***  **    *     ****    ");
			Console.WriteLine("* *  *     *     *       ");
			Console.WriteLine("* *  ****  ****  *       ");
			Console.WriteLine("*************************");
			Screen.SetConsoleStandardColor();
			Console.WriteLine("space ................ (un)select object at cursor or choose object");
			Console.WriteLine("s .................... select an object");
			Console.WriteLine("r .................... rotate selected object (only text label)");
			Console.WriteLine("cursor keys........... move cursor or selected object");
			Console.WriteLine("shift + cursor ....... resize selected object (only box)");

			Console.WriteLine("");
			Console.WriteLine("b .................... Create a Box");
			Console.WriteLine("c .................... Create a connection between boxes");
			Console.WriteLine("t .................... Create a text label");
			Console.WriteLine("x .................... Delete selected object");
			Console.WriteLine("Delete ............... Delete selected object");
			Console.WriteLine("Esc .................. Abort input");
			Console.WriteLine("ctrl+c ............... Exit program");

			// todo quick select next/prev eg using ctrl+cursor left/right. Up/down could be first/last obj
			// TODO undo/redo
			// TODO delete selected which is connected.. change into connect to a coord
			// Todo pine like menu
			// Todo colour themes
			// Todo saving picture or state
			// Todo change style of box
			// TODO change arrow style
			// TODO change arrow head + bottom
			// Todo change z orderof object by moving place in the model

			Console.ReadKey();
		}

		private static Option<Tuple<int, bool>> SelectObject(State state) {
			var occupant = state.Canvas.GetOccupants(state.TheCurser.Pos);
			if (occupant != null)
				return Tuple.Create(occupant.Value, false);
			return PrintIdsAndLetUserSelectOpbejct(state).Select(x => Tuple.Create(x, true));
		}

		private static Option<int> PrintIdsAndLetUserSelectOpbejct(State state) {
			var cursorTop = Console.CursorTop;
			Screen.SetConsoleGetInputColors();
			PrintIdsOfModel(state.Model);
			Console.SetCursorPosition(0, cursorTop);

			var res = GetISelectableElement(state.Model);

			Screen.SetConsoleStandardColor();

			return res;
		}

		private static Option<int> GetISelectableElement(List<IPaintable<object>> model) {
			return CommandParser.ReadInt(new Range<int>(0, model.Count - 1), "Select object: ")
				.Bind(x => {
					if (model[x] is ISelectable)
						return x;
					PrintErrorAndWaitKey("Not a selectable object");
					return GetISelectableElement(model);
				});
		}

		private static void PrintIdsOfModel(List<IPaintable<object>> model) {
			foreach (var selectable in model.OfType<ISelectable>()) {
				Console.SetCursorPosition(selectable.Pos.X, selectable.Pos.Y + 1);
				Console.Write(selectable.Id);
			}
		}
	}
}