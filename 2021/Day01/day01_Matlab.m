% ______
% PART 1
% ??????

example_report = [199
200
208
210
200
207
240
269
260
263];

% define variable to use later (not a must but a bit more elegant)
example_report_increased = 0;

% whenever the following element is larger than the current one, the
% counter increases by one
for i = 1 : size(example_report) - 1
    if example_report(i + 1) > example_report(i)
        example_report_increased = example_report_increased + 1;
    end
end

fprintf('The depth measurement increased %i times.\n', example_report_increased);

% now the same but with the real values
actual_report = load('input_day01.csv');

actual_report_increased = 0;

for i = 1 : size(actual_report) - 1
    if actual_report(i + 1) > actual_report(i)
        actual_report_increased = actual_report_increased + 1;
    end
end

fprintf('The depth measurement increased %i times.\n', actual_report_increased);

% ______
% PART 2
% ??????

% create array for the sum of sliding windows of 3
triplesSum = zeros(size(example_report,1) - 2,1);

% fill the empty windows from before and calculate the sum of each window
for i = 1 : size(triplesSum)
    singleWindow = zeros(3,1);
    for j = 1 : size(singleWindow) % unfortunately `j = 0 : size(singleWindow) - 1` is not possible
        singleWindow(j) = example_report(i + j - 1,1);
    end
    triplesSum(i) = sum(singleWindow);
end

example_report_windows_increased = 0;

for i = 1 : size(triplesSum) - 1
    if triplesSum(i + 1) > triplesSum(i)
        example_report_windows_increased = example_report_windows_increased + 1;
    end
end

fprintf('The depth measurement increased %i times.\n', example_report_windows_increased);

% again for the real report

triplesSum = zeros(size(actual_report,1) - 2,1);

for i = 1 : size(triplesSum)
    singleWindow = zeros(3,1);
    for j = 1 : size(singleWindow) % unfortunately `j = 0 : size(singleWindow) - 1` is not possible
        singleWindow(j) = actual_report(i + j - 1,1);
    end
    triplesSum(i) = sum(singleWindow);
end

actual_report_windows_increased = 0;

for i = 1 : size(triplesSum) - 1
    if triplesSum(i + 1) > triplesSum(i)
        actual_report_windows_increased = actual_report_windows_increased + 1;
    end
end

fprintf('The depth measurement increased %i times.\n', actual_report_windows_increased);