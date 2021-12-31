fn main() {
    
// ______
// PART 1
// ‾‾‾‾‾‾
    
    use std::fs;
    use std::env;
    
    // use input of CLI tool as filename
    let args : Vec<String> = env::args().collect();
    let filename = &args[1];
    
    // read in the submarine's depth report
    let submarine_report = fs::read_to_string(filename).expect("ERROR: Something went wrong reading the file.");

    // split the single string into a vector of string at each line break
    let submarine_report_split = submarine_report.lines().collect::<Vec<_>>();

    // create an int vector from the  string vector
    let submarine_report_int : Vec<i32> = 
        submarine_report_split
        .iter()
        .map(|s| s.parse::<i32>().unwrap())
        .collect();

    let l = submarine_report_int.len() - 1;

    // create an empty vector which will inhabit all occurences where a value of the submarine report was bigger than its predecessor (or not)
    let mut larger_than_before = Vec::with_capacity(l);
    // fill that vector
    for i in 0 .. l {
        larger_than_before.push(submarine_report_int[i] < submarine_report_int[i + 1]);
    };

    // count how often a measurement was bigger than its predecessor
    let counts_larger = larger_than_before.iter().filter(|&&b| b).count();
    
    println!("The depth measurement increased {} times.", counts_larger);

// ______
// PART 2
// ‾‾‾‾‾‾

    // create windows of 3 measurements
    let submarine_report_windowed = submarine_report_int.windows(3);
    let submarine_report_windowsums = 
        submarine_report_windowed
            .map(|w| w.iter().sum())
            .collect::<Vec<i32>>();

    // the rest is basically the same as above...
    let mut larger_than_before_windowsums = Vec::new();
    for i in 0 .. submarine_report_windowsums.len() - 2 {
        larger_than_before_windowsums.push(submarine_report_windowsums[i] < submarine_report_windowsums[i + 1]);
    };

    let counts_larger_windowsums = larger_than_before_windowsums.iter().filter(|&&b| b).count();
    
    println!("The depth measurement increased {} times in windows of 3.", counts_larger_windowsums);
}