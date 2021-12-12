// ______
// PART 1
// ‾‾‾‾‾‾

#r "nuget: FSharpAux"

open System.IO
open FSharpAux

// define types for position and direction
type Position = {
    Horizontal  : int
    Depth       : int
}

type Direction =
| Forward   of int
| Up        of int
| Down      of int

// define function for parsing input of the file to a direction
let parseDirection input = 
    let splitStr = String.split ' ' input
    let positionChange = int splitStr.[1]
    match splitStr.[0] with
    | "forward" -> Forward  positionChange
    | "up"      -> Up       positionChange
    | "down"    -> Down     positionChange
    | _         -> failwith "ERROR: Improper input given."

// define function for changing the position via a given direction
let changePosition (currentPosition : Position) directionProgress =
    match directionProgress with
    | Forward   x -> {currentPosition with Horizontal   = currentPosition.Horizontal    + x}
    | Up        x -> {currentPosition with Depth        = currentPosition.Depth         - x}
    | Down      x -> {currentPosition with Depth        = currentPosition.Depth         + x}

let submarineReport = File.ReadAllLines(Path.Combine(__SOURCE_DIRECTORY__, "input_day02.csv")) |> Array.map parseDirection

let endPosition = submarineReport |> Array.fold changePosition {Horizontal = 0; Depth = 0}

printfn "The end position of the submarine is %i" (endPosition.Horizontal * endPosition.Depth)

// ______
// PART 2
// ‾‾‾‾‾‾

type Position2 = {
    Horizontal  : int
    Depth       : int
    Aim         : int
}

let changePosition2 (currentPosition : Position2) directionProgress =
    match directionProgress with
    | Forward   x -> {currentPosition with Horizontal   = currentPosition.Horizontal    + x; Depth = currentPosition.Depth + currentPosition.Aim * x}
    | Up        x -> {currentPosition with Aim          = currentPosition.Aim           - x}
    | Down      x -> {currentPosition with Aim          = currentPosition.Aim           + x}

let endPosition2 = submarineReport |> Array.fold changePosition2 {Horizontal = 0; Depth = 0; Aim = 0}

printfn "The end position of the submarine is %i" (endPosition2.Horizontal * endPosition2.Depth)