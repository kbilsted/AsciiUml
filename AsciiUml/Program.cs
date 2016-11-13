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
	}

	class Program {
		static void Main(string[] args) {
			EnableCatchingShiftArrowPresses();

			Console.SetWindowSize(90, 50);
			var state = new State();
			state.TheCurser = new Cursor(0, 0);

			TempModelForPlayingAround(state.Model);

			state = ReadKeyboardEvalLoop(state);
			return;
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

				if ((key.Modifiers & ConsoleModifiers.Shift) != 0) {
					switch (key.Key) {
						case ConsoleKey.UpArrow:
							if (selected.HasValue) {
								var box = model[selected.Value] as IResizeable<object>;
								if (box != null)
									model[selected.Value] = (IPaintable<object>) box.Resize(0, -1);
							}
							break;

						case ConsoleKey.DownArrow:
							if (selected.HasValue) {
								var box = model[selected.Value] as IResizeable<object>;
								if (box != null)
									model[selected.Value] = (IPaintable<object>) box.Resize(0, 1);
							}
							break;

						case ConsoleKey.LeftArrow:
							if (selected.HasValue) {
								var box = model[selected.Value] as IResizeable<object>;
								if (box != null)
									model[selected.Value] = (IPaintable<object>) box.Resize(-1, 0);
							}
							break;

						case ConsoleKey.RightArrow:
							if (selected.HasValue) {
								var box = model[selected.Value] as IResizeable<object>;
								if (box != null)
									model[selected.Value] = (IPaintable<object>) box.Resize(1, 0);
							}
							break;
					}
				}
				else {
					switch (key.Key) {
						case ConsoleKey.UpArrow:
							state.TheCurser = state.TheCurser.Move(0, -1);
							selected.ToOption()
								.Match(x => state.Model[x] = (IPaintable<object>) model[x].Move(0, -1), () => { });
							break;

						case ConsoleKey.DownArrow:
							state.TheCurser = state.TheCurser.Move(0, 1);
							selected.ToOption()
								.Match(x => model[x] = (IPaintable<object>) model[x].Move(0, 1),
									() => { });
							break;

						case ConsoleKey.LeftArrow:
							state.TheCurser = state.TheCurser.Move(-1, 0);
							selected.ToOption()
								.Match(x => model[x] = (IPaintable<object>) model[x].Move(-1, 0),
									() => { });
							break;

						case ConsoleKey.RightArrow:
							state.TheCurser = state.TheCurser.Move(1, 0);
							selected.ToOption()
								.Match(x => model[x] = (IPaintable<object>) model[x].Move(1, 0),
									() => { });
							break;

						case ConsoleKey.Spacebar:
							if (selected.HasValue) {
								state = ClearSelection(state);
							}
							else {
								var obj = SelectObject(state);
								obj.Match(x => {
										var idx = model.FindIndex(0, m => m.Id == x.Item1);
										var elem = model[idx] as ISelectable;
										if (elem != null) {
											state.SelectedIndexInModel = idx;
											state.SelectedId = x.Item1;
											if (x.Item2)
												state.TheCurser = new Cursor(elem.X, elem.Y);
										}
									},
									() => { state = ClearSelection(state); });
							}
							break;

						case ConsoleKey.S:
							PrintIdsAndLetUserSelectOpbejct(state)
								.IfSome(x => {
									var idx = model.FindIndex(0, m => m.Id == x);
									var elem = model[idx] as ISelectable;
									if (elem != null) {
										state.SelectedIndexInModel = idx;
										state.SelectedId = x;
										state.TheCurser = new Cursor(elem.X, elem.Y);
									}
								});
							break;

						case ConsoleKey.X:
						case ConsoleKey.Delete:
							if (selected.HasValue) {
								model.RemoveAt(selected.Value);
								state = ClearSelection(state);
							}
							else {
								PrintErrorAndWaitKey("Error. You must select an object before you can delete.");
							}
							break;

						case ConsoleKey.H:
							Help();
							break;

						case ConsoleKey.B:
							CreateBox(state.TheCurser).IfSome(x => {
								model.Insert(0, x);
								state.SelectedId = x.Id;
								state.SelectedIndexInModel = 0;
							});
							break;

						case ConsoleKey.C:
							Console.WriteLine("Connect from objecgt: ");

							var from = PrintIdsAndLetUserSelectOpbejct(state);
							var to = PrintIdsAndLetUserSelectOpbejct(state);
							@from.IfSome(ffrom => to.IfSome(tto => {
								var line = new Line() {FromId = ffrom, ToId = tto};
								model.Insert(0, line);
							}));
							break;

						case ConsoleKey.T:
							CreateLabel().IfSome(x => {
								model.Insert(0, x);
								state.SelectedId = x.Id;
								state.SelectedIndexInModel = 0;
							});
							break;

						case ConsoleKey.R:
							selected.ToOption().Match(x => {
									if (model[x] is Label)
										model[x] = ((Label) model[x]).Rotate();
									else
										PrintErrorAndWaitKey("Only labels can be rotated");
								},
								() => PrintErrorAndWaitKey("Nothing is selected"));
							break;
					}
				}
			}
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
			SetConsoleGetInputColors();
			Console.WriteLine(
				$"AsciiUml v1.0. Selected: {state.SelectedId?.ToString() ?? "None"}. ({state.TheCurser.X}, {state.TheCurser.Y}) Press \'h\' for help");
			SetConsoleStandardColor();

			var canvass = PaintServiceCore.Paint(state);

			Console.WriteLine(canvass.ToString());
			PrintCursor(state.TheCurser, canvass);

			return canvass;
		}

		private static void PrintCursor(Cursor curser, Canvass canvass) {
			var top = Console.CursorTop;
			var left = Console.CursorLeft;
			SetConsoleSelectColor();
			var x = curser.X;
			var y = curser.Y;

			Console.SetCursorPosition(x, y + 1);
			Console.Write(canvass.Lines[y][x]);
			SetConsoleStandardColor();
			Console.SetCursorPosition(left, top);
		}

		private static void TempModelForPlayingAround(List<IPaintable<object>> model) {
			model.Add(new Box() {Text = "goo\nand\nbazooka"});
			//model.Add(new Box() {Y = 14, Text = "goo\nand\nbazooka"});
			model.Add(new Box() {X = 19, Y = 27, Text = "goo\nand\nbazooka"});
			model.Add(new Box() {Y = 20, X = 13, Text = "goo\nand\nbazooka"});
			model.Add(new Line() {FromId = 0, ToId = 1});
			model.Add(new Label() {Y = 5, X = 5, Text = "Server\nservice\noriented"});
		}

		private static Option<Label> CreateLabel() {
			SetConsoleGetInputColors();
			Console.Write("Create a label. Text: ");
			var res = CommandParser.TryReadLineWithCancel().Match(x => new Label() {Text = x}, () => Option<Label>.None);
			SetConsoleStandardColor();
			return res;
		}

		private static Option<Box> CreateBox(Cursor cursor) {
			SetConsoleGetInputColors();
			Console.Write("Create box. Title: ");
			var res = CommandParser.TryReadLineWithCancel()
				.Match(x => new Box() {X = cursor.X, Y = cursor.Y, Text = x}, () => Option<Box>.None);
			SetConsoleStandardColor();
			return res;
		}

		private static void PrintErrorAndWaitKey(string text) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.BackgroundColor = ConsoleColor.White;
			Console.WriteLine(text);
			SetConsoleStandardColor();
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
			SetConsoleStandardColor();
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
			var occupant = state.Canvas.Occupants[state.TheCurser.Y, state.TheCurser.X];
			if (occupant != null)
				return Tuple.Create(occupant.Value, false);
			return PrintIdsAndLetUserSelectOpbejct(state).Select(x => Tuple.Create(x, true));
		}

		private static Option<int> PrintIdsAndLetUserSelectOpbejct(State state) {
			var cursorTop = Console.CursorTop;
			SetConsoleGetInputColors();
			PrintIdsOfModel(state.Model);
			Console.SetCursorPosition(0, cursorTop);

			var res = GetISelectableElement(state.Model);

			SetConsoleStandardColor();

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
				Console.SetCursorPosition(selectable.X, selectable.Y + 1);
				Console.Write(selectable.Id);
			}
		}

		public static void SetConsoleGetInputColors() {
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.Green;
		}

		public static void SetConsoleStandardColor() {
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void SetConsoleSelectColor() {
			Console.BackgroundColor = ConsoleColor.DarkYellow;
			Console.ForegroundColor = ConsoleColor.Yellow;
		}
	}
}