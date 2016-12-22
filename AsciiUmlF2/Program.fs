namespace AsciiUml



module Start =
    open Canvas
    open Figures
    open System

    let handleKey (model: Model) = 
        let c = Console.ReadKey()
        match c.KeyChar with
        | 'b' -> 
            printfn "Enter box text: "
            let text = Console.ReadLine()
            { model with Figures = (box text (0,0)) :: model.Figures }
        | 'd' -> {model with Cursor = {X = model.Cursor.X; Y = model.Cursor.Y+1}} 
        | 'u' -> {model with Cursor = {X = model.Cursor.X; Y = model.Cursor.Y-1}} 
        | _ -> model

    let rec printEvalLoop model =
        Console.SetCursorPosition(0, 0)
        model |> Canvas.Print 40 40 |> printfn "%s" 
        
        let model2 = handleKey model 
        printEvalLoop model2

    [<EntryPoint>]
    let main argv = 
        let figlist = [label "foo" (0,3); label "bar" (0,0); box "barbox" (4,4)]
        printEvalLoop {Figures= figlist; Cursor = {X=0; Y=0}} 
        0 // return an integer exit code
