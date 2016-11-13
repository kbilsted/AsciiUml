// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open AsciiUml
open System
open Geometry
open Canvas
open System.Text

let cprint c =
    let insts = Canvas.toColorPring c 
    let prevColor = List.append [Option.None] (List.map (fun x -> Option.Some x.color) insts)  
    let commands = List.zip insts prevColor    
    for (inst, prev) in commands do
        if prev.IsNone or prev.Value <> inst.color then
            let (Colourr (f, b)) = inst.color       
            Console.ForegroundColor = f |> ignore
            Console.BackgroundColor = b |> ignore
        Console.SetCursorPosition(inst.coord.x, inst.coord.y) |> ignore
        Console.Write(inst.c)


[<EntryPoint>]
let main argv =  
    let c = Canvas.create 80 40
    c.cells.[0].[0] <-  { cell = 'd'; figures = [ Label ("1", Id 2) ]; color = Colourr (ConsoleColor.Blue, ConsoleColor.Black)} 
    c.cells.[1].[1] <-  { cell = 'a'; figures = [ Label ("1", Id 2) ]; color = Colourr (ConsoleColor.Yellow, ConsoleColor.Black)} 
    c.cells.[2].[2] <-  { cell = 'd'; figures = [ Label ("1", Id 2) ]; color = Colourr (ConsoleColor.Blue, ConsoleColor.Black)} 
//    Console.WriteLine( Canvas.toString c )
    cprint c |> ignore

    0 // return an integer exit code

