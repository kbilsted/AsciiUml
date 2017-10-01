using System;
using System.Collections.Generic;
using AsciiUml.Commands;
using AsciiUml.Geo;
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

    class Program {
        static void Main(string[] args) {
            var state = new State
            {
                TheCurser = new Cursor(new Coord(10, 10))
            };

            var man = new WindowManager("AsciiUML (c) Kasper B. Graversen 2016-");
            var topmenu = new TopMenu(man, state);
            //var umlWindow = new UmlWindow(topmenu);
            var umlWindow = new UmlWindow(topmenu, TempModelForPlayingAround(state));
            umlWindow.Focus();
            man.Start();
        }

        public static void PrintCommandLog(List<List<ICommand>> commandLog)
		{
			var ser = new JsonSerializer() {
				TypeNameHandling = TypeNameHandling.Auto,
			};
			
			ser.Serialize(Console.Out, commandLog);
			Console.WriteLine("press a key");
			Console.ReadKey(true);
		}

	    private static State TempModelForPlayingAround(State state) {
	        var model = state.Model;
            model.Add(new SlopedLine2(new Coord(10,10)));
   //         model.Add(new Box(new Coord(0, 0), "Foo\nMiddleware\nMW1"));
   //         //model.Add(new Box() { Y = 14, Text = "goo\nand\nbazooka" });
   //         model.Add(new Box(new Coord(19, 27), "foo\nServer\nbazooka"));
			//model.Add(new Box(new Coord(13, 20), "goo\nWeb\nServer"));
			//model.Add(new Line() {FromId = 0, ToId = 1});
			//model.Add(new Label(new Coord(5, 5), "Server\nClient\nAAA"));

	        return state;
	    }
	}
}