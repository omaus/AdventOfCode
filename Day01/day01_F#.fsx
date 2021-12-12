// ______
// PART 1
// ‾‾‾‾‾‾

open System.IO

// read in the submarine's depth report and transform its values from string to int
let submarineReport = File.ReadAllLines(Path.Combine(__SOURCE_DIRECTORY__, "input_day01.csv")) |> Array.map int

// create a list of occurences where a value was bigger than its predecessor (or not)
let largerThanBefore = [for i = 0 to submarineReport.Length - 2 do yield submarineReport.[i] < submarineReport.[i + 1]]

// count how often a measurement was bigger than its predecessor
let countsLarger = largerThanBefore |> List.fold (fun acc v -> if v then acc + 1 else acc) 0

printfn "The depth measurement increased %i times." countsLarger

// ______
// PART 2
// ‾‾‾‾‾‾

// create windows of 3 measurements
let submarineReportWindowed = Array.windowed 3 submarineReport

// calculate the sums of those windows
let submarineReportWindowSums = Array.map Array.sum submarineReportWindowed

// same as in part 1...
let largerThanBeforeWindows = [for i = 0 to submarineReportWindowSums.Length - 2 do yield submarineReportWindowSums.[i] < submarineReportWindowSums.[i + 1]]
let countsLargerWindows = largerThanBeforeWindows |> List.fold (fun acc v -> if v then acc + 1 else acc) 0

printfn "The depth measurement increased %i times in triple windows."