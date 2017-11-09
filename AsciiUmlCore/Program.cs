using System;
using System.Collections.Generic;
using System.IO;
using AsciiConsoleUi;
using AsciiConsoleUi.CompositeComponents;
using AsciiUml.Commands;
using AsciiUml.Geo;
using AsciiUml.UI;
using Newtonsoft.Json;

namespace AsciiUml
{
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
    // TODO transitive select using hotkey such that a graph is easily selected

    class Program {
        const string Version = "v0.1.2";

        static void Main(string[] args) {
            RestoreConsoleOnExit(CreateWindow);
        }

        private static void  CreateWindow() {
            var state = new State
            {
                TheCurser = new Cursor(new Coord(10, 10))
            };

            var man = new WindowManager($"AsciiUML {Version} (c) Kasper B. Graversen 2016-", State.MaxX, State.MaxY);
            var topmenu = new TopMenu(man, state);
            //var umlWindow = new UmlWindow(topmenu, state);
            var umlWindow = new UmlWindow(topmenu, TempModelForPlayingAround(state));
            umlWindow.Focus();
            ShowLogo(umlWindow);
            man.Start();
        }

        private static void RestoreConsoleOnExit(Action code) {
            var foreground = Console.ForegroundColor;
            var background = Console.BackgroundColor;
            try
            {
                code();
            }
            finally
            {
                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
            }
        }

        private static void ShowLogo(UmlWindow umlWindow)
        {
            new PopupNoButton(umlWindow, $@"
          _____  _____ ______ _____   _    _ __  __ _      
    /\   / ____|/ ____|__   _|_   _| | |  | |  \/  | |     
   /  \  | (___ | |      | |   | |   | |  | | \  / | |     
  / /\ \  \___ \| |      | |   | |   | |  | | |\/| | |     
 / ____ \ ____) | |____ _| |_ _| |_  | |__| | |  | | |____ 
/_/    \_\_____/ \_____|_____|_____|  \____/|_|  |_|______|
                              {Version} by Kasper B. Graversen
")
            {
                BackGround = ConsoleColor.Black
            };
        }

        public static string Serialize(object o)
        {
            var ser = new JsonSerializer() {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var w = new StringWriter();
            ser.Serialize(w, o);
            return w.ToString();
        }

        private static State TempModelForPlayingAround(State state) {
            var model = state.Model;

            //model.Add(new SlopedLine2(new Coord(10,10)));


            //model.Add(new Box(new Coord(0, 0), "Foo\nMiddleware\nMW1"));
            //         //model.Add(new Box() { Y = 14, Text = "goo\nand\nbazooka" });
            //         model.Add(new Box(new Coord(19, 27), "foo\nServer\nbazooka"));
            //model.Add(new Box(new Coord(13, 20), "goo\nWeb\nServer"));
            //model.Add(new Line() {FromId = 0, ToId = 1});
            //model.Add(new Label(new Coord(5, 5), "Server\nClient\nAAA"));

            return state;
        }
    }
}