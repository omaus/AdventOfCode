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
    let submarine_report = fs::read_to_string(filename).expect("Something went wrong reading the file");
    
    let submarine_report_string = submarine_report.

    println!("submarine_report is {}", submarine_report)

    // ______
    // PART 2
    // ‾‾‾‾‾‾


}