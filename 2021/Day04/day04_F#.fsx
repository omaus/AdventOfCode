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

// record for Bingo numbers
type BingoNumber = {
    Number  : int
    Match   : bool
}

// record for Bingo boards
type BingoBoard = {
    Board       : BingoNumber [,]
    BoardNumber : int
    HasWon      : bool
    WinningLine : int []
}

// function to transform the numbers in a 2D array into BingoNumber type
let toBingoNumber num = {Number = num; Match = false}

// second part of the input file: the bingo boards
let bingoBoards = 
    Array.skip 1 bingoReport
    |> Array.groupWhen (fun s -> s = "")
    |> Array.mapi (
        fun i ->
            Array.filter ((<>) "")
            >> Array.map (String.toWords >> Array.ofSeq >> Array.map int)
            >> Array2D.ofJaggedArray
            >> fun bb -> {Board = Array2D.map toBingoNumber bb; BoardNumber = i + 1; HasWon = false; WinningLine = [||]}
    )

let exampleNumbersDrawn = 
    exampleReport 
    |> String.toLines 
    |> Seq.head 
    |> String.split ',' 
    |> Array.map int

let exampleBingoBoards = 
    exampleReport 
    |> String.split '\n' 
    |> Array.ofSeq 
    |> Array.skip 1 
    |> Array.groupWhen (fun s -> s = "") 
    |> Array.mapi (
        fun i ->
            Array.filter ((<>) "") 
            >> Array.map (
                String.toWords 
                >> Array.ofSeq 
                >> Array.map int
            ) 
            >> Array2D.ofJaggedArray
            >> fun bb -> {Board = Array2D.map toBingoNumber bb; BoardNumber = i + 1; HasWon = false; WinningLine = [||]}
    )

// function that looks if a drawn number is a match somewhere in the Bingo board and changes that field accordingly
let matchDrawnNumber drawnNumber bingoBoard =
    {bingoBoard with 
        Board = 
            bingoBoard.Board
            |> Array2D.map (
                fun bn -> 
                    if bn.Number = drawnNumber then 
                        {bn with Match = true} 
                    else bn
            )
    }

// function that checks if there's a Bingo in a line
let isBingoLine arr = Array.exists (fun bn -> bn.Match = false) arr |> not

// function to look if there's already a Bingo
let isBingoInBoard (bingoBoard : BingoBoard) =
    let rn = Array2D.length1 bingoBoard.Board
    let cn = Array2D.length2 bingoBoard.Board
    // recursively looks for the first Bingo in a given board
    let rec isBingo bb i j check winningLine =
        if check then 
            printfn "\nBINGO!\n  on board %i with the number series: " bb.BoardNumber
            winningLine |> Array.iteri (fun i n -> if i < 4 then printf " %i," n.Number else printfn " %i.\n" n.Number)
            {bb with HasWon = true; WinningLine = winningLine |> Array.map (fun bn -> bn.Number)} 
        // iterates through the rows
        elif i < rn then 
            let currentLine = bb.Board.[i,0 ..]
            let newCheck = isBingoLine currentLine
            isBingo bb (i + 1) j newCheck currentLine
        // iterates through the columns
        elif j < cn then
            let currentLine = bb.Board.[0 ..,j]
            let newCheck = isBingoLine currentLine
            isBingo bb i (j + 1) newCheck currentLine
        // iteration complete, no Bingo found
        else bb
    isBingo bingoBoard 0 0 false [||]

// function to play Bingo
let playBingo (bingoBoards : BingoBoard []) (numbersToDraw : int []) =
    printfn "" // empty line for better readability of printed output
    let rec loop bingoBoards2 currentNumber i listOfDrawnNumbers =
        // print all drawn numbers first
        if listOfDrawnNumbers = [] then printfn "Drawn number: %i" currentNumber
        else 
            printf "Drawn numbers: " 
            List.rev listOfDrawnNumbers 
            |> List.iter (printf "%i, ")
            printfn "%i." currentNumber
        // update all boards by the current number
        let updatedBoards = bingoBoards2 |> Array.map (matchDrawnNumber currentNumber)
        // check if there's a Bingo on any board
        let boardsBingoChecked = updatedBoards |> Array.map isBingoInBoard
        // if already a Bingo occured then...
        if Array.exists (fun bb -> bb.HasWon) boardsBingoChecked then
            // take all boards that have a Bingo at that point
            let boardsWithBingo = Array.filter (fun bb -> bb.HasWon) boardsBingoChecked
            List.rev (numbersToDraw.[i] :: listOfDrawnNumbers), boardsWithBingo
        // no Bingo occured yet, go on and draw the next number
        else loop updatedBoards numbersToDraw.[i + 1] (i + 1) (numbersToDraw.[i] :: listOfDrawnNumbers)
    loop bingoBoards numbersToDraw.[0] 0 []

let drawnNumbersExample, wonBoardsExample = playBingo exampleBingoBoards exampleNumbersDrawn
let drawnNumbers, wonBoards = playBingo bingoBoards bingoNumbersDrawn

// does the final calculation (multiplying all unmarked board numbers with the last drawn number)
let getFinalResult (bingoBoards : BingoBoard []) drawnNumbers =
    printfn "drawnNumbers %A" drawnNumbers
    let lastDrawnNumber = List.last drawnNumbers
    printfn "last drawn number: %i" lastDrawnNumber
    let sumOfUnmarkedNumbers = 
        (Array.head bingoBoards).Board
        |> Array2D.toLongColumnArray
        |> Array.choose (fun bn -> if not bn.Match then Some bn.Number else None)
        |> fun x -> printfn "unmarked numbers %A" x; x
        |> Array.sum
        |> fun x -> printfn "sum is %i" x; x
    lastDrawnNumber * sumOfUnmarkedNumbers

getFinalResult wonBoardsExample drawnNumbersExample
getFinalResult wonBoards drawnNumbers

// ______
// PART 2
// ‾‾‾‾‾‾

// function to get a list of the drawn numbers in chronological order as well as a list of all Bingo boards, sorted anti-chronologically by win
let loseBingo (bingoBoards : BingoBoard []) (numbersToDraw : int []) =
    printfn ""
    let rec loop bingoBoards2 currentNumber i listOfDrawnNumbers winnerBoards =
        if listOfDrawnNumbers = [] then printfn "Drawn number: %i." currentNumber
        else 
            printf "Drawn numbers: " 
            List.rev listOfDrawnNumbers |> List.iter (printf "%i, ")
            printfn "%i." currentNumber
        let updatedBoards = bingoBoards2 |> Array.map (matchDrawnNumber currentNumber)
        let boardsBingoChecked = updatedBoards |> Array.map isBingoInBoard
        // for every Bingo that occurs on a board and there's more than 1 un-Bingoed board left then...
        if Array.exists (fun bb -> bb.HasWon) boardsBingoChecked && updatedBoards.Length > 1 then
            let boardsWithBingo = boardsBingoChecked |> Array.filter (fun bb -> bb.HasWon)
            let updatedBoardsWithoutWinner = boardsBingoChecked |> Array.filter (fun bbc -> not bbc.HasWon)
            // boards won this round appended to the previously winning boards
            let updatedWinnerBoards = boardsWithBingo |> Array.fold (fun wb bb -> bb :: wb) winnerBoards
            loop updatedBoardsWithoutWinner numbersToDraw.[i + 1] (i + 1) (numbersToDraw.[i] :: listOfDrawnNumbers) updatedWinnerBoards
        // if a Bingo occured on the last remaining board then...
        elif Array.length updatedBoards = 1 && (Array.head boardsBingoChecked).HasWon then
            let boardWithBingo = Array.head boardsBingoChecked
            List.rev (numbersToDraw.[i] :: listOfDrawnNumbers), boardWithBingo :: winnerBoards
        // no Bingo occured yet, go on and draw the next number
        else loop updatedBoards numbersToDraw.[i + 1] (i + 1) (numbersToDraw.[i] :: listOfDrawnNumbers) winnerBoards
    loop bingoBoards numbersToDraw.[0] 0 [] []

let drawnNumbersExample2, wonBoardsExample2 = loseBingo exampleBingoBoards exampleNumbersDrawn
let drawnNumbers2, wonBoards2 = loseBingo bingoBoards bingoNumbersDrawn

getFinalResult (Array.ofList wonBoardsExample2) drawnNumbersExample2
getFinalResult (Array.ofList wonBoards2) drawnNumbers2