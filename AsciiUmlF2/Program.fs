namespace AsciiUml

module Start =
    open Canvas
    open Figures
    open System
    open State
    open ViewModel

    let handleKey (state: State) = 
        let c = Console.ReadKey()
        match c.KeyChar with
        | 'b' -> 
            printfn "Enter box text: "
            let text = Console.ReadLine()
            { state with Model = (box text (0,0)) :: state.Model }
        | 'd' -> {state with ViewModel = { state.ViewModel with Cursor = Cursor.moveDown state.ViewModel.Cursor }} 
        | 'u' -> {state with ViewModel = { state.ViewModel with Cursor = Cursor.moveUp state.ViewModel.Cursor }} 
        | 'l' -> {state with ViewModel = { state.ViewModel with Cursor = Cursor.moveLeft state.ViewModel.Cursor}}
        | 'r' -> {state with ViewModel = { state.ViewModel with Cursor = Cursor.moveRight state.ViewModel.Cursor } }
        | _ -> state

    let rec printEvalLoop state =
        Console.SetCursorPosition(0, 0)
        state |> Canvas.Print |> printfn "%s" 
        
        let state2= handleKey state
        printEvalLoop state2

    [<EntryPoint>]
    let main argv = 
        let figlist = [label "foo" (0,3); label "bar" (0,0); box "barbox" (4,4)]
        let viewmodel = { Cursor = Cursor.create(); Size = {H=40;W=40}}
        printEvalLoop {Model = figlist; ViewModel = viewmodel} 
        0 // return an integer exit code
