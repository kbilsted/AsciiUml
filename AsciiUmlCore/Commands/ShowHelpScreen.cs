using System;
using AsciiUml.Commands;
using AsciiUml.UI;

namespace AsciiUml
{
    internal class ShowHelpScreen:ICommand
    {
        public State Execute(State state)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("*  *  ****  *     ****    ");
            Console.WriteLine("*  *  *     *     *  *    ");
            Console.WriteLine("****  **    *     ****    ");
            Console.WriteLine("*  *  *     *     *       ");
            Console.WriteLine("*  *  ****  ****  *       ");
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
            // TODO all changes to the model must be arise from an event, thus enabling us to persist the model and run test-runs

            Console.ReadKey();

            return state;
        }
    }
}