// ______
// PART 1
// ‾‾‾‾‾‾

using System;
using System.IO;

// read in the submarine's depth report and transform its values from string to int
int [] submarineReport = 
    Array.ConvertAll(
        File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "input_day01.csv")),
        s => Int32.Parse(s)
    );

// create default bool array 1 item less long than submarineReport
bool [] largerThanBefore = new bool[submarineReport.Length - 1];

// fill the array with bools that define if a depth measurement is bigger than that before
for (int i = 0; i < (largerThanBefore.Length); i++) {
    largerThanBefore[i] = submarineReport[i] < submarineReport[i + 1];
};

// define count that will get used after
int count = 0;

// every time the item in largerThanBefore is `true`, the count will get raised by 1
foreach (bool item in largerThanBefore) {
    if (item) {
        count = ++count;
    }
}

Console.WriteLine($"The depth measurement increased {count} times.");

// ______
// PART 2
// ‾‾‾‾‾‾

// create default int array which is 2 elements shorter than the report (due to the last 2 windows must not be smaller than 3 elements)
var submarineReportWindowedSum = new int[submarineReport.Length - 2];

// fill the windows with the respective elements from the report and calculate the sum of them
for (int i = 0; i < submarineReportWindowedSum.Length; i++) {
    submarineReportWindowedSum[i] = submarineReport.Skip(i).Take(3).Sum();
}

// now it's basically the same as above
var largerThanBeforeWindowed = new bool[submarineReportWindowedSum.Length - 1];

for (int i = 0; i < (largerThanBeforeWindowed.Length); i++) {
    largerThanBeforeWindowed[i] = submarineReportWindowedSum[i] < submarineReportWindowedSum[i + 1];
};

// reset the count from above
count = 0;

foreach (bool item in largerThanBeforeWindowed) {
    if (item) {
        count = ++count;
    }
}

Console.WriteLine($"The depth measurement increased {count} times in windows of 3.");