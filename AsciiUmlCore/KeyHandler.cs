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

namespace AsciiUml
{
    internal static class KeyHandler
    {
        private static List<ICommand> Noop => new List<ICommand>();
        private static List<ICommand> NoopForceRepaint => new List<ICommand>(){new TmpForceRepaint()};

        public static List<ICommand> HandleKeyPress(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog, UmlWindow umlWindow)
        {
            return ControlKeys(state, key, commandLog)
                .IfNone(() => ShiftKeys(state, key)
                    .IfNone(() => HandleKeys(state, key, commandLog, umlWindow)
                        .IfNone(() => Noop)));
        }

        private static Option<List<ICommand>> HandleKeys(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog, UmlWindow umlWindow)
        {
            var model = state.Model;
            var selected = state.SelectedIndexInModel;
            switch (key.Key)
            {
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
                    return new Option<List<ICommand>>();

                case ConsoleKey.X:
                case ConsoleKey.Delete:
                    return Extensions.Lst(new DeleteSelectedElement());

                case ConsoleKey.D:
                    return Extensions.Lst(new CreateDatabase(state.TheCurser.Pos));

                case ConsoleKey.H:
                    return HelpScreen(umlWindow);

                case ConsoleKey.B:
                    CreateBox(state, umlWindow);
                    break;

                case ConsoleKey.C:
                    ConnectObjects(state, umlWindow);
                    return new Option<List<ICommand>>();

                case ConsoleKey.L:
                    return SlopedLine(state);

                case ConsoleKey.T:
                    CreateText(state, umlWindow);
                    return new Option<List<ICommand>>();

                case ConsoleKey.R:
                    return Rotate(selected, model);

                case ConsoleKey.Enter:
                    CommandMode(state, commandLog, umlWindow);
                    return new Option<List<ICommand>>();
            }
            return Noop;
        }

        private static void CreateText(State state, UmlWindow umlWindow)
        {
            var input = new MultilineInputForm(umlWindow, "Create a label", "Text:", state.TheCurser.Pos)
            {
                OnCancel = () => { },
                OnSubmit = (text) =>
                {
                    umlWindow.HandleCommands(Extensions.Lst(new CreateLabel(state.TheCurser.Pos, text)));
                }
            };
            input.Focus();
        }

        private static void CreateBox(State state, UmlWindow umlWindow)
        {
            var input = new MultilineInputForm(umlWindow, "Create box", "Text:", state.TheCurser.Pos)
            {
                OnCancel = () => { },
                OnSubmit = (text) =>
                {
                    umlWindow.HandleCommands(Extensions.Lst(new CreateBox(state.TheCurser.Pos, text)));
                }
            };
            input.Focus();
        }

        private static List<ICommand> SlopedLine(State state)
        {
            return Extensions.Lst(new CreateSlopedLine(state.TheCurser.Pos));
        }

        private static List<ICommand> SelectDeselect(int? selected, State state, UmlWindow umlWindow)
        {
            if (selected.HasValue)
                return Extensions.Lst(new ClearSelection());

            var obj = state.Canvas.GetOccupants(state.TheCurser.Pos).ToOption()
                .Match(x => Extensions.Lst(new SelectObject(x, false)),
                    () =>
                    {
                        PrintIdsAndLetUserSelectObject(state, umlWindow);
                        return Noop;
                    });
            return obj;
        }

        private static List<ICommand> HelpScreen(UmlWindow umlWindow)
        {
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
ctrl + cursor ........ move selected object (only box)
b .................... Create a Box
c .................... Create a connection line between boxes
d .................... Create a Database
t .................... Create a text label
l .................... Create a free-style line
x / Del............... Delete selected object
enter ................ Enter command mode
Esc .................. Abort input
ctrl+c ............... Exit program");
            return Noop;
        }

        private static void ConnectObjects(State state, UmlWindow umlWindow)
        {
            state.PaintSelectableIds = true;

            var cmds = NoopForceRepaint;

            var connect = new ConnectForm(umlWindow, state.TheCurser.Pos)
            {
                OnCancel = () =>
                {
                    umlWindow.HandleCommands(cmds);
                    state.PaintSelectableIds = false;
                },
                OnSubmit = (from, to) =>
                {
                    cmds.Add(new CreateLine(@from, to, LineKind.Connected));
                    umlWindow.HandleCommands(cmds);
                    state.PaintSelectableIds = false;
                }
            };
            connect.Focus();
        }

        private static void CommandMode(State state, List<List<ICommand>> commandLog, UmlWindow umlWindow)
        {
            var input = new SinglelineInputForm(umlWindow, "Enter command", "database,save-file,set-save-filename:", 20,
                state.TheCurser.Pos)
            {
                OnCancel = () => { },
                OnSubmit = (cmd) =>
                {
                    switch (cmd)
                    {
                        case "set-save-filename":
                            var filename =
                                new SinglelineInputForm(umlWindow, "Set state", "Filename", 20, state.TheCurser.Pos)
                                {
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
                            new Popup(umlWindow, $"file saved to \'{logname}\'");
                            break;
                        case "database":
                            umlWindow.HandleCommands(Extensions.Lst(new CreateDatabase(state.TheCurser.Pos)));
                            break;
                    }
                }
            };
            input.Focus();
        }

        private static List<ICommand> Rotate(int? selected, List<IPaintable<object>> model)
        {
            return selected.Match(x =>
                {
                    if (model[x] is Label)
                        return Extensions.Lst(new RotateSelectedElement(x));
                    Screen.PrintErrorAndWaitKey("Only labels can be rotated");
                    return NoopForceRepaint;
                },
                () =>
                {
                    Screen.PrintErrorAndWaitKey("Nothing is selected");
                    return NoopForceRepaint;
                });
        }


        private static Option<List<ICommand>> ControlKeys(State state, ConsoleKeyInfo key, List<List<ICommand>> commandLog)
        {
            if ((key.Modifiers & ConsoleModifiers.Control) == 0)
                return Option<List<ICommand>>.None;

            switch (key.Key)
            {
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

        private static Option<List<ICommand>> ControlCursor(State state, ConsoleKeyInfo key)
        {
            return SelectTemporarily(state, x =>
            {
                return x.GetSelected().Match(el=>
                {
                    if (el is Box)
                    {
                        switch (key.Key)
                        {
                            case ConsoleKey.UpArrow:
                                return Extensions.Lst(new ResizeSelectedBox(Vector.DeltaNorth));
                            case ConsoleKey.DownArrow:
                                return Extensions.Lst(new ResizeSelectedBox(Vector.DeltaSouth));
                            case ConsoleKey.LeftArrow:
                                return Extensions.Lst(new ResizeSelectedBox(Vector.DeltaWest));
                            case ConsoleKey.RightArrow:
                                return Extensions.Lst(new ResizeSelectedBox(Vector.DeltaEast));
                        }
                    }
                    if (el is SlopedLine2)
                    {
                        switch (key.Key)
                        {
                            case ConsoleKey.UpArrow:
                                return Extensions.Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaNorth), new MoveCursor(Vector.DeltaNorth));
                            case ConsoleKey.DownArrow:
                                return Extensions.Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaSouth), new MoveCursor(Vector.DeltaSouth));
                            case ConsoleKey.LeftArrow:
                                return Extensions.Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaWest), new MoveCursor(Vector.DeltaWest));
                            case ConsoleKey.RightArrow:
                                return Extensions.Lst(new DragLinePixel(state.TheCurser.Pos, Vector.DeltaEast), new MoveCursor(Vector.DeltaEast));
                        }
                    }
                    return Noop;

                }, ()=> Noop).ToList();
            
            });
        }

        private static Option<List<ICommand>> ShiftKeys(State state, ConsoleKeyInfo key)
        {
            if ((key.Modifiers & ConsoleModifiers.Shift) == 0)
                return Option<List<ICommand>>.None;

            var commands = SelectTemporarily(state, x => {
                switch (key.Key)
                {
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

        private static List<ICommand> MoveCursorAndSelectPaintable(Coord direction)
        {
            return Extensions.Lst(new MoveCursor(direction), new MoveSelectedPaintable(direction));
        }

        private static void PrintIdsAndLetUserSelectObject(State state, UmlWindow umlWindow)
        {
            state.PaintSelectableIds = true;
            var cmds = NoopForceRepaint;

            var selectedform = new SelectObjectForm(umlWindow, state.Model.Select(x=>x.Id).ToArray(), state.TheCurser.Pos)
            {
                OnCancel = () =>
                {
                    umlWindow.HandleCommands(cmds);
                    state.PaintSelectableIds = false;
                },
                OnSubmit = (selected) =>
                {
                    if (state.Model.SingleOrDefault(b => b.Id == selected) is ISelectable)
                        cmds.Add(new SelectObject(selected, true));
                    umlWindow.HandleCommands(cmds);
                    state.PaintSelectableIds = false;
                }
            };
            selectedform.Focus();
        }

        public static Option<List<ICommand>> SelectTemporarily(State state, Func<State, List<ICommand>> code)
        {
            if (state.SelectedId.HasValue)
                return code(state);

            var occupants = state.Canvas.GetOccupants(state.TheCurser.Pos);
            if (!occupants.HasValue)
                return Noop;

            state.SelectedId = occupants;

            var commands = code(state);
            var result = commands.Count == 0
                ? Noop
                : Extensions.Lst(new SelectObject(occupants.Value, false))
                    .Append(commands)
                    .Append(Extensions.Lst(new ClearSelection()))
                    .ToList();
            state.SelectedId = null;
            return result;
        }
    }
}
