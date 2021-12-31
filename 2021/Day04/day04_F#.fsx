// ______
// PART 1
// ‾‾‾‾‾‾

#r "nuget: FSharpAux"

open System.IO
open FSharpAux

let exampleReport =
    "7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7"

// introduce helper array function from F#Aux, not available in current NuGet release
module Array =
    /// Iterates over elements of the input array and groups adjacent elements.
    /// A new group is started when the specified predicate holds about the element
    /// of the array (and at the beginning of the iteration).
    ///
    /// For example: 
    ///    Array.groupWhen isOdd [|3;3;2;4;1;2|] = [|[|3|]; [|3; 2; 4|]; [|1; 2|]|]
    let groupWhen f (array : 'T []) =
        let inds = findIndices f array
        if inds.Length = 0 then [|array|]
        else 
            Array.init (inds.Length) (
                fun i ->
                    if i + 1 = inds.Length then
                        array.[Array.last inds ..]
                    else
                        array.[inds.[i] .. inds.[i + 1] - 1]
                )

let bingoReport = File.ReadAllLines(Path.Combine(__SOURCE_DIRECTORY__, "input_day04.csv")) 

// divide input file into different aspects. Here: the numbers drawn
let bingoNumbersDrawn = Array.head bingoReport |> String.split ',' |> Array.map int

// second part of the input file: the bingo boards
let bingoBoards = 
    Array.skip 1 bingoReport
    |> Array.groupWhen (fun s -> s = "")
    |> Array.map (
        Array.filter ((<>) "")
        >> Array.map (String.toWords >> Array.ofSeq >> Array.map int)
        >> Array2D.ofJaggedArray
    )

// type for Bingo numbers
type BingoNumber = {
    Number  : int
    Match   : bool
}

let exampleNumbersDrawn = exampleReport |> String.toLines |> Seq.head |> String.split ',' |> Array.map int
let exampleBingoBoards = exampleReport |> String.split '\n' |> Array.ofSeq |> Array.skip 1 |> Array.groupWhen (fun s -> s = "") |> Array.map (Array.filter ((<>) "") >> Array.map (String.toWords >> Array.ofSeq >> Array.map int) >> Array2D.ofJaggedArray)

// function to transform the numbers in a 2D array into BingoNumber type
let toBingoNumber num = {Number = num; Match = false}

let exampleBingoBoardsBingoed = exampleBingoBoards |> Array.map (Array2D.map toBingoNumber)

// function that looks if a drawn number is a match somewhere in the Bingo board and changes that field accordingly
let matchDrawnNumber drawnNumber (bingoBoard : BingoNumber [,]) =
    Array2D.map (fun bn -> if bn.Number = drawnNumber then {bn with Match = true} else bn) bingoBoard

// function that checks if there's a Bingo in a line
let isBingoLine arr = Array.exists (fun bn -> bn.Match = false) arr |> not

// function to look if there's already a Bingo
let isBingoInBoard (bingoBoard : BingoNumber [,]) =
    let rn = Array2D.length1 bingoBoard
    let cn = Array2D.length2 bingoBoard
    // recursively looks for the first Bingo in a given board
    let rec isBingo i j check =
        if check then printfn "BINGO!"; check
        elif i < rn - 1 then 
            let newCheck = isBingoLine bingoBoard.[i,0 ..]
            isBingo (i + 1) j newCheck
        elif j < cn - 1 then
            let newCheck = isBingoLine bingoBoard.[0 ..,j]
            isBingo i (j + 1) newCheck
        else check
    isBingo 0 0 false

let playBingo bingoBoards (numbersToDraw : int []) =
    let rec loop bingoBoards2 currentNumber i listOfDrawnNumbers =
        let updatedBoards = bingoBoards2 |> Array.map (matchDrawnNumber currentNumber)
        let boardsBingoChecked = bingoBoards2 |> Array.mapi (fun i bb -> i, isBingoInBoard bb)
        if Array.exists (snd >> (=) true) boardsBingoChecked then
            let boardsWithBingo = Array.filter (fun (i,bb) -> bb) boardsBingoChecked
            if boardsWithBingo.Length = 1 then 
                printf "The Bingo board with the number %i won.\nWinning number series:" (Array.head boardsWithBingo |> fst)
                listOfDrawnNumbers 
                |> List.take 5 
                |> List.iteri (fun i n -> if i < 4 then printf " %i," n else printf " %i." n)
                List.rev listOfDrawnNumbers
            else 
                printf "The Bingo boards with the numbers "
                boardsWithBingo |> Array.iter (fun (i,bb) -> printf "%i " i)
                printf "won.\nWinning number series:"
                listOfDrawnNumbers 
                |> List.take 5 
                |> List.iteri (fun i n -> if i < 4 then printf " %i," n else printf " %i." n)
                List.rev listOfDrawnNumbers
        else
            printf "Drawn numbers: " 
            List.rev listOfDrawnNumbers 
            |> List.iter (printf )
            loop updatedBoards numbersToDraw.[i + 1] (i + 1) (numbersToDraw.[i] :: listOfDrawnNumbers)
    loop bingoBoards numbersToDraw.[0] 0 []

playBingo exampleBingoBoardsBingoed exampleNumbersDrawn