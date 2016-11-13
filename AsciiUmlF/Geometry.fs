namespace AsciiUml
open System

module Geometry =
    type Coord = { x: int; y: int }
    type Direction = North | South | East | West
    type Vector = { From: Coord; To: Coord; Direction: Direction }

    type FigureId = Id of int

    type Figure =
        | Label of string * FigureId
        | Line of FigureId * Vector list


module Canvas =
    open Geometry
    open System
    open System.Text

    type Colour = Colourr of ConsoleColor * ConsoleColor
    type Cell = { cell: char; figures: Figure list; color: Colour }
    type T = { cells: Cell[][] }

    let create maxX maxY =
        let c = Colourr (ConsoleColor.Cyan, ConsoleColor.Black)
        { cells = Array.init maxY (fun y -> Array.init maxX (fun x -> {cell =' ' ; figures = List.empty; color = c })) } 

    let cellsAsStrings canvas =
        canvas.cells |> Array.map (fun x -> new string (Array.map (fun c -> c.cell) x)) 

    let toString (canvas : T) =
        cellsAsStrings canvas 
        |> Array.fold (fun (sb:StringBuilder) s -> sb.AppendLine(s)) (new StringBuilder()) 
        |> fun x-> x.ToString()

//        let sb = new StringBuilder()
//        for carr in canvas.cells do
//            sb.AppendLine(Array.map (fun x -> x.cell) carr |> fun x -> x.ToString()) |> ignore     
//        sb.ToString()

    let toTrimedString (canvas : T) =
        let lines = cellsAsStrings canvas
        let last = Array.findIndex (fun (x:string) -> x.Trim().Length = 0) lines
        let result = 
            lines.[..last]
            |> Array.map (fun x -> x.TrimEnd()) 
            |> Seq.fold(fun (sb:StringBuilder) s -> sb.AppendLine(s)) (new StringBuilder())
            |> fun x -> x.ToString() 
        result

    type PrintInstr = { c: char; color: Colour; coord: Coord }

    let toColorPring (canvas: T) : PrintInstr list =
        let mutable l = List.empty
        for y in 0..39 do
            for x in 0..79 do
                let cell = canvas.cells.[y].[x]
                l <- l @ [{ c = cell.cell; color = cell.color; coord = {x=x; y=y} }]
        l

    let diff a b =
        List.map2 (fun x y -> (x,y)) a b 
        |> List.filter (fun (old,neww) -> old <> neww) 
        |> List.map (fun (old,neww) -> neww)

//    let rec findDiff (olds : PrintInstr list) (news :PrintInstr list) diffs : PrintInstr list =
//        match olds with 
//        | [] -> diffs @  news
//        | old::os -> 
//            match news with
//            | [] -> diffs @ old :: os        
//            | neww :: ns when old <> neww -> findDiff os ns diffs @ [neww]
//            | neww :: ns -> findDiff os ns diffs
//
//       