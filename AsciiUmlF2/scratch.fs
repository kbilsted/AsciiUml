namespace AsciiUml

module Figures =
    type Coord = { X:int; Y:int }
    type Size = { H:int; W:int }

    type Label = { Text:string; }
    type Box = { BorderChar: char ; Text: string }
    type FigureType = | Label of Label | Box of Box
    type Figure = {Type: FigureType; Position:Coord; Size:Size}

    type Cursor = Coord
    type Figures = Figure list
    type ViewModel = { Cursor: Cursor; Size: Size}
    type State = { Model: Figures; ViewModel: ViewModel}

    let label s (x,y) : Figure =
        let l = Label {Text=s; }
        {Type=l; Position = {X=x;Y=y}; Size={H=1; W=s.Length} }

    let box s (x,y) = 
        let b = Box {BorderChar='*'; Text=s; }
        {Type=b; Position = {X=x;Y=y}; Size={H=3; W=s.Length+2} }

module Canvas = 
    open Microsoft.FSharp.Collections
    open System
    open System.Text
    open Figures

    let printFigure (canvas : char[][]) ft  =
        match ft.Type with
        | Label l -> l.Text |> Seq.iteri (fun i x -> canvas.[ft.Position.Y].[ft.Position.X + i] <- x)
        | Box b -> 
            for y in 1 .. ft.Size.H do
                for x in 1 .. ft.Size.W do
                    if y = 1 || y = ft.Size.H then canvas.[ft.Position.Y + y - 1].[ft.Position.X+x-1] <- b.BorderChar
                    if x = 1 || x = ft.Size.W then canvas.[ft.Position.Y + y - 1].[ft.Position.X+x-1] <- b.BorderChar
            b.Text |> Seq.iteri (fun i x -> canvas.[ft.Position.Y+1].[ft.Position.X + 1 + i] <- x)

    let Print (model: State) =
        let canvas = Array.init model.ViewModel.Size.H (fun _ -> Array.init model.ViewModel.Size.W (fun _ -> new char()))
        model.Model |> Seq.iter (printFigure canvas)
        canvas.[model.ViewModel.Cursor.Y].[model.ViewModel.Cursor.X] <- '@'

        let sb = new StringBuilder()
        canvas 
            |> Seq.map (fun x -> (new string(x)))
            |> Seq.iter (fun x -> sb.AppendLine(x) |> ignore)
        sb.ToString()

