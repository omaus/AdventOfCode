// ______
// PART 1
// ‾‾‾‾‾‾

#r "nuget: FSharpAux"

open System.IO
open FSharpAux

// read file and transform it into an int 2D array
let diagnosticReport = 
    File.ReadAllLines(Path.Combine(__SOURCE_DIRECTORY__, "input_day03.csv")) 
    |> Array.map (String.toCharArray >> Array.map (int >> (+) -48))

// define type for the different rates
type RateParameter =
| Gamma
| Epsilon

// define function to get gamma or epsilon rate
let getRate parameter bitJagArr =
    let getSingleBit arr =
        let intermediateResult =
            Array.averageBy float arr
            |> round 0
            |> int
        match parameter with
        | Gamma     -> intermediateResult
        | Epsilon   -> System.Math.Abs(intermediateResult - 1)
    Array.map getSingleBit (Array.transpose bitJagArr)

let gammaRate   = getRate Gamma     diagnosticReport
let epsilonRate = getRate Epsilon   diagnosticReport

// define function to calculate decimal from bitrate
let bitRateToDecimal (bitRate : int []) =
    let l = bitRate.Length
    bitRate 
    |> Array.foldi (fun i acc v -> acc + float v * 2. ** float (l - i - 1)) 0.
    |> int

printfn "The power consumption is %i." (bitRateToDecimal gammaRate * bitRateToDecimal epsilonRate)

// ______
// PART 2
// ‾‾‾‾‾‾

// define type for different ratings
type RatingParameter =
| Oxygen
| CO2

// define function to get the common bit of a row/column, depending on the parameter used
let determineCommonBit parameter arr =
    let l0 = Array.countByPredicate ((=) 0) arr
    let l1 = Array.countByPredicate ((=) 1) arr
    match parameter with
    | Oxygen    -> if l1 >= l0 then 1 else 0
    | CO2       -> if l1 >= l0 then 0 else 1

// define function to get the rating depending on the parameter of the whole bit matrix
let rec getRating parameter i (startArr : int [] []) =
    if i < startArr.[0].Length - 1 && startArr.Length > 2 then
        let commonBit =
            startArr
            |> Array.transpose
            |> Array.item i
            |> determineCommonBit parameter
        let filteredArr = startArr |> Array.filter (fun arr -> arr.[i] = commonBit)
        getRating parameter (i + 1) filteredArr
    else 
        let commonBit =
            startArr
            |> Array.transpose
            |> Array.item i
            |> determineCommonBit parameter
        let filteredArr = startArr |> Array.filter (fun arr -> arr.[i] = commonBit)
        Array.head filteredArr

let oxygenRating        = getRating Oxygen  0 diagnosticReport
let co2scrabberRating   = getRating CO2     0 diagnosticReport

printfn "The life support rating is %i." (bitRateToDecimal oxygenRating * bitRateToDecimal co2scrabberRating)