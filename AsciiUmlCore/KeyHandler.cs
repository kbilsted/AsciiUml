using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AsciiConsoleUi;
using AsciiConsoleUi.CompositeComponents;
using AsciiUml.Commands;
using AsciiUml.Geo;
using AsciiUml.UI;
using LanguageExt;
using static AsciiUml.Extensions;

namespace AsciiUml {
	internal static class KeyHandler {
		private static HandlerState handlerState = HandlerState.Normal;

		private static List<ICommand> Noop => new List<ICommand>();
		private static List<ICommand> NoopForceRepaint => new List<ICommand> {new TmpForceRepaint()};
		private static Option<List<ICommand>> OptNoop => new Option<List<ICommand>>();

		public static List<ICommand> HandleKeyPress(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog, UmlWindow umlWindow) {
			switch (handlerState) {
				case HandlerState.Normal:
					return ControlKeys(state, key, commandLog)
						.IfNone(() => ShiftKeys(state, key)
							.IfNone(() => HandleKeys(state, key, commandLog, umlWindow)
								.IfNone(() => Noop)));

				case HandlerState.Insert:
					(var done, var cmds) = new KeyHandlerInsertState().HandleKeyPress(state, key, commandLog, umlWindow);

					if (done) {
						handlerState = HandlerState.Normal;
						cmds = cmds.Concat(Lst(new ClearTopmenuText())).ToList();
					}
					return cmds;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static Option<List<ICommand>> HandleKeys(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog,
			UmlWindow umlWindow) {
			var model = state.Model;
			var selected = state.SelectedIndexInModel;
			switch (key.Key) {
				case ConsoleKey.UpArrow:
					if (state.TheCurser.Pos.Y > 0)
						return MoveCursorAndSelectPaintable(Vector.DeltaNorth);
					break;
				case ConsoleKey.DownArrow:
					if (state.TheCurser.Pos.Y < State.MaxY - 2)
						return MoveCursorAndSelectPaintable(Vector.DeltaSouth);
					break;
				case ConsoleKey.LeftArrow:
					if (state.TheCurser.Pos.X > 0)
						return MoveCursorAndSelectPaintable(Vector.DeltaWest);
					break;
				case ConsoleKey.RightArrow:
					if (state.TheCurser.Pos.X < State.MaxX - 2)
						return MoveCursorAndSelectPaintable(Vector.DeltaEast);
					break;

				case ConsoleKey.Spacebar:
					return SelectDeselect(selected, state, umlWindow);

				case ConsoleKey.S:
					PrintIdsAndLetUserSelectObject(state, umlWindow);
					return OptNoop;

				case ConsoleKey.X:
				case ConsoleKey.Delete:
					return Lst(new DeleteSelectedElement());

				case ConsoleKey.H:
					return HelpScreen(umlWindow);

				case ConsoleKey.I:
					handlerState = HandlerState.Insert;
					return Lst(new SetTopmenuText("INSERTMODE> d: database | t: text | n: note | u: user"));

				case ConsoleKey.B:
					CreateBox(state, umlWindow);
					break;

				case ConsoleKey.E:
					EditUnderCursor(state, umlWindow);
					break;

				case ConsoleKey.C:
					ConnectObjects(state, umlWindow);
					return OptNoop;

				case ConsoleKey.L:
					return SlopedLine(state);

				case ConsoleKey.R:
					return Rotate(selected, model);

				case ConsoleKey.Enter:
					CommandMode(state, commandLog, umlWindow);
					return OptNoop;

				case ConsoleKey.OemPeriod:
					return ChangeStyleUnderCursor(state, umlWindow, StyleChangeKind.Next);
				case ConsoleKey.OemComma:
					return ChangeStyleUnderCursor(state, umlWindow, StyleChangeKind.Previous);
			}
			return Noop;
		}


		private static List<ICommand> ChangeStyleUnderCursor(State state, UmlWindow umlWindow, StyleChangeKind change) {
			var id = state.Canvas.GetOccupants(state.TheCurser.Pos);
			if (!id.HasValue)
				return Noop;

			var elem = state.Model.Objects.Single(x => x.Id == id);
			if (elem is IStyleChangeable) return Lst(new ChangeStyle(elem.Id, change));
			new Popup(umlWindow, "Only works on boxes");
			return Noop;
		}

		private static void CreateBox(State state, UmlWindow umlWindow) {
			var input = new MultilineInputForm(umlWindow, "Create box", "Text:", "", state.TheCurser.Pos) {
				OnCancel = () => { },
				OnSubmit = text => { umlWindow.HandleCommands(Lst(new CreateBox(state.TheCurser.Pos, text))); }
			};
			input.Focus();
		}

		private static void EditUnderCursor(State state, UmlWindow umlWindow) {
			var id = state.Canvas.GetOccupants(state.TheCurser.Pos);
			if (!id.HasValue)
				return;

			var elem = state.Model.Objects.Single(x => x.Id == id);
			if (elem is IHasTextProperty property) {
				var text = property.Text;
				var input = new MultilineInputForm(umlWindow, "Edit..", "Text:", text, state.TheCurser.Pos) {
					OnCancel = () => { },
					OnSubmit = newtext => { umlWindow.HandleCommands(Lst(new SetText(id.Value, newtext))); }
				};
				input.Focus();
			}
			else {
				new Popup(umlWindow, "Only works on labels and boxes");
			}
		}

		private static List<ICommand> SlopedLine(State state) {
			return Lst(new CreateSlopedLine(state.TheCurser.Pos));
		}

		private static List<ICommand> SelectDeselect(int? selected, State state, UmlWindow umlWindow) {
			if (selected.HasValue)
				return Lst(new ClearSelection());

			var obj = state.Canvas.GetOccupants(state.TheCurser.Pos).ToOption()
				.Match(x => Lst(new SelectObject(x, false)),
					() => {
						PrintIdsAndLetUserSelectObject(state, umlWindow);
						return Noop;
					});
			return obj;
		}

		private static List<ICommand> HelpScreen(UmlWindow umlWindow) {
			new Popup(umlWindow, @"*  *  ****  *     ****    
*  *  *     *     *  *    
****  **    *     ****    
*  *  *     *     *       
*  *  ****  ****  *       
space ................ (un)select object at cursor or choose object
s .................... select an object
cursor keys........... move cursor or selected object
shift + cursor ....... move object under cursor
r .................... rotate selected object (only text label)
ctrl + cursor ........ Resize selected object (only box)
b .................... Create a Box
e .................... Edit element at cursor (box/label/note)
c .................... Create a connection line between boxes
i .................... InsertMode
  d ..................   Create a Database
  n ..................   Create a note
  t ..................   Create a text label
  u ..................   Create a user
l .................... Create a free-style line
., ................... Change the style (only box)
x / Del............... Delete selected object
enter ................ Enter command mode
Esc .................. Abort input / InsertMode
ctrl+c ............... Exit program");
			return Noop;
		}

		private static void ConnectObjects(State state, UmlWindow umlWindow) {
			state.PaintSelectableIds = true;

			var cmds = NoopForceRepaint;

			var connect = new ConnectForm(umlWindow, state.TheCurser.Pos, state.Model.Objects.Select(x=>x.Id).ToArray()) {
				OnCancel = () => {
					umlWindow.HandleCommands(cmds);
					state.PaintSelectableIds = false;
				},
				OnSubmit = (from, to) => {
					cmds.Add(new CreateLine(from, to, LineKind.Connected));
					umlWindow.HandleCommands(cmds);
					state.PaintSelectableIds = false;
				}
			};
			connect.Focus();
		}

		private static void CommandMode(State state, List<List<ICommand>> commandLog, UmlWindow umlWindow) {
			var input = new SinglelineInputForm(umlWindow, "Enter command", "commands: database, save-file, set-save-filename:",
				"Enter a command", 25,
				state.TheCurser.Pos) {
				OnCancel = () => { },
				OnSubmit = cmd => {
					switch (cmd) {
						case "set-save-filename":
							var filename =
								new SinglelineInputForm(umlWindow, "Set state", "Filename", "Enter a filename", 20, state.TheCurser.Pos) {
									OnSubmit = fname => state.Config.SaveFilename = fname
								};
							filename.Focus();
							break;

						case "save-file":
							var log = Program.Serialize(commandLog);
							var logname = state.Config.SaveFilename + ".log";
							File.WriteAllText(logname, log);

							var model = Program.Serialize(state.Model);
							File.WriteAllText(state.Config.SaveFilename, model);
							new Popup(umlWindow, $"file saved to \'{state.Config.SaveFilename}\'");
							break;

						case "database":
							umlWindow.HandleCommands(Lst(new CreateDatabase(state.TheCurser.Pos)));
							break;
					}
				}
			};
			input.Focus();
		}

		private static List<ICommand> Rotate(int? selected, Model model) {
			return selected.Match(x => {
					if (model.Objects[x] is Label)
						return Lst(new RotateSelectedElement(x));
					Screen.PrintErrorAndWaitKey("Only labels can be rotated");
					return NoopForceRepaint;
				},
				() => {
					Screen.PrintErrorAndWaitKey("Nothing is selected");
					return NoopForceRepaint;
				});
		}

		private static Option<List<ICommand>> ControlKeys(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog) {
			if ((key.Modifiers & ConsoleModifiers.Control) == 0)
				return Option<List<ICommand>>.None;

			switch (key.Key) {
				case ConsoleKey.UpArrow:
				case ConsoleKey.DownArrow:
				case ConsoleKey.LeftArrow:
				case ConsoleKey.RightArrow:
					return ControlCursor(state, key);

				case ConsoleKey.W:
					Program.Serialize(commandLog);
					return NoopForceRepaint;

				default:
					return Option<List<ICommand>>.None;
			}
		}

		private static Option<List<ICommand>> ControlCursor(State state, ConsoleKeyInfo key) {
			return SelectTemporarily(state, x => {
				return x.GetSelected().Match(el => {
					if (el is Box)
						switch (key.Key) {
							case ConsoleKey.UpArrow:
								return Lst(new ResizeSelectedBox(Vector.DeltaNorth));
							case ConsoleKey.DownArrow:
								return Lst(new ResizeSelectedBox(Vector.DeltaSouth));
							case ConsoleKey.LeftArrow:
								return Lst(new ResizeSelectedBox(Vector.DeltaWest));
							case ConsoleKey.RightArrow:
								return Lst(new ResizeSelectedBox(Vector.DeltaEast));
						}
					if (el is SlopedLine2)
						switch (key.Key) {
							case ConsoleKey.UpArrow:
								return Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaNorth),
									new MoveCursor(Vector.DeltaNorth));
							case ConsoleKey.DownArrow:
								return Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaSouth),
									new MoveCursor(Vector.DeltaSouth));
							case ConsoleKey.LeftArrow:
								return Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaWest),
									new MoveCursor(Vector.DeltaWest));
							case ConsoleKey.RightArrow:
								return Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaEast),
									new MoveCursor(Vector.DeltaEast));
						}
					return Noop;
				}, () => Noop).ToList();
			});
		}

		private static Option<List<ICommand>> ShiftKeys(State state, ConsoleKeyInfo key) {
			if ((key.Modifiers & ConsoleModifiers.Shift) == 0)
				return Option<List<ICommand>>.None;

			var commands = SelectTemporarily(state, x => {
				switch (key.Key) {
					case ConsoleKey.UpArrow:
						return MoveCursorAndSelectPaintable(Vector.DeltaNorth);
					case ConsoleKey.DownArrow:
						return MoveCursorAndSelectPaintable(Vector.DeltaSouth);
					case ConsoleKey.LeftArrow:
						return MoveCursorAndSelectPaintable(Vector.DeltaWest);
					case ConsoleKey.RightArrow:
						return MoveCursorAndSelectPaintable(Vector.DeltaEast);
				}
				return Noop;
			});
			return commands;
		}

		private static List<ICommand> MoveCursorAndSelectPaintable(Coord direction) {
			return Lst(new MoveCursor(direction), new MoveSelectedPaintable(direction));
		}

		private static void PrintIdsAndLetUserSelectObject(State state, UmlWindow umlWindow) {
			state.PaintSelectableIds = true;
			var cmds = NoopForceRepaint;

			var selectedform = new SelectObjectForm(umlWindow, state.Model.Objects.Select(x => x.Id).ToArray(), state.TheCurser.Pos) {
				OnCancel = () => {
					umlWindow.HandleCommands(cmds);
					state.PaintSelectableIds = false;
				},
				OnSubmit = selected => {
					if (state.Model.Objects.SingleOrDefault(b => b.Id == selected) is ISelectable)
						cmds.Add(new SelectObject(selected, true));
					umlWindow.HandleCommands(cmds);
					state.PaintSelectableIds = false;
				}
			};
			selectedform.Focus();
		}

		public static Option<List<ICommand>> SelectTemporarily(State state, Func<State, List<ICommand>> code) {
			if (state.SelectedId.HasValue)
				return code(state);

			var occupants = state.Canvas.GetOccupants(state.TheCurser.Pos);
			if (!occupants.HasValue)
				return Noop;

			state.SelectedId = occupants;

			var commands = code(state);
			var result = commands.Count == 0
				? Noop
				: Lst(new SelectObject(occupants.Value, false))
					.Append(commands)
					.Append(Lst(new ClearSelection()))
					.ToList();
			state.SelectedId = null;
			return result;
		}

		private enum HandlerState {
			Normal,
			Insert
		}
	}

	internal class KeyHandlerInsertState {
		private static Option<List<ICommand>> OptNoop => new Option<List<ICommand>>();
		private static List<ICommand> Noop => new List<ICommand>();

		public (bool, List<ICommand>) HandleKeyPress(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog, UmlWindow umlWindow) {
			switch (key.Key) {
				case ConsoleKey.C:
					return (true, Noop);

				case ConsoleKey.D:
					return (true, Lst(new CreateDatabase(state.TheCurser.Pos)));

				case ConsoleKey.N:
					CreateNote(state, umlWindow);
					return (true, Noop);

				case ConsoleKey.T:
					CreateText(state, umlWindow);
					return (true, Noop);

				case ConsoleKey.U:
					CreateUmlUser(state, umlWindow);
					return (true, Noop);

				case ConsoleKey.Escape:
					return (true, Noop);
			}
			return (false, Noop);
		}

		private static void CreateText(State state, UmlWindow umlWindow) {
			var input = new MultilineInputForm(umlWindow, "Create a label", "Text:", "", state.TheCurser.Pos) {
				OnCancel = () => { },
				OnSubmit = text => { umlWindow.HandleCommands(Lst(new CreateLabel(state.TheCurser.Pos, text))); }
			};
			input.Focus();
		}

		private static void CreateNote(State state, UmlWindow umlWindow) {
			var input = new MultilineInputForm(umlWindow, "Create a note", "Text:", "", state.TheCurser.Pos) {
				OnCancel = () => { },
				OnSubmit = text => { umlWindow.HandleCommands(Lst(new CreateNote(state.TheCurser.Pos, text))); }
			};
			input.Focus();
		}

		private static void CreateUmlUser(State state, UmlWindow umlWindow) {
			var input = new MultilineInputForm(umlWindow, "Text for user (optional)", "Text:", "", state.TheCurser.Pos) {
				OnCancel = () => { },
				OnSubmit = text => { umlWindow.HandleCommands(Lst(new CreateUmlUser(state.TheCurser.Pos, text))); }
			};
			input.Focus();
		}
	}
}