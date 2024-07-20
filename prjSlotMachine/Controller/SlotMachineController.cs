using prjSlotMachine.Model;
using prjSlotMachine.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace prjSlotMachine.Controller
{
    public class SlotMachineController
    {
        private readonly SlotMachineModel model;
        private readonly SlotMachineView view;
        private readonly Random random;

        public SlotMachineController(SlotMachineModel model, SlotMachineView view)
        {
            this.model = model;
            this.view = view;
            this.random = new Random();
        }

        public void Spin()
        {
            int[] newPositions = GetNewPositions();
            string[,] screen = GetScreen(newPositions);

            view.DisplayResults(newPositions, screen);
            CalculateAndDisplayWinnings(screen);
            ContinueOrStop();
        }

        private int[] GetNewPositions()
        {
            int[] stopPositions = new int[5];
            for (int i = 0; i < 5; i++)
            {
                stopPositions[i] = random.Next(model.ReelSets[i].Length);
            }
            return stopPositions;
        }

        private string[,] GetScreen(int[] newPositions)
        {
            string[,] screen = new string[3, 5];
            for (int col = 0; col < 5; col++)
            {
                int start = newPositions[col];
                for (int row = 0; row < 3; row++)
                {
                    screen[row, col] = model.ReelSets[col][(start + row) % model.ReelSets[col].Length];
                }
            }
            return screen;
        }

        private void CalculateAndDisplayWinnings(string[,] screen)
        {
            int[,] screenIndex = model.ScreenIndexPositions;

            var pairs = GenerateAllPairs(screenIndex);
            Dictionary<int, string> keyValuePosition = GetScreenDictionary(screen);

            var stringPairDics = ConvertPairsToStrings(pairs, keyValuePosition);

            var results = ValidateStringPairs(stringPairDics, pairs);

            var filteredResults = FilterResult(results);

            view.DisplayWinnings(filteredResults);
        }

        public Dictionary<int, string> GetScreenDictionary(string[,] inputScreen)
        {
            var screenDictionary = new Dictionary<int, string>();
            int rows = model.ScreenIndexPositions.GetLength(0);
            int cols = model.ScreenIndexPositions.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col= 0; col < cols; col++)
                {
                    int index = model.ScreenIndexPositions[row, col];
                    string value = inputScreen[row, col];
                    screenDictionary[index] = value;
                }
            }
            return screenDictionary;
        }

        private void findSameSymbol(int inputRow, int inputCol, string[,] inputScreen, int sameSymbolCount)
        {
            int currentPositionIndexSum = inputRow + inputCol;
            int nextCol = inputCol + 1;
            List<int> recordSymbol = new List<int>();

            for (int row = 0; row < 3; row++)
            {
                int nextPositionIndexSum = nextCol + row;
                if (nextPositionIndexSum - currentPositionIndexSum < 3 && nextPositionIndexSum - currentPositionIndexSum >= 0)
                {
                    if (inputScreen[inputRow, inputCol] == inputScreen[row, nextCol])
                    {
                        sameSymbolCount++;
                        if (nextCol < 4)
                        {

                            findSameSymbol(row, nextCol, inputScreen, sameSymbolCount);
                        }

                        if (nextCol == 4)
                        {
                            Console.WriteLine("This is same symbol count =>" + sameSymbolCount);
                            sameSymbolCount--;
                        }
                    }
                }
            }
        }

        public static List<List<int>> GenerateAllPairs(int[,] array)
        {
            var result = new List<List<int>>();

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            var columnCombinations = GetCombinations(Enumerable.Range(0, cols).ToList(), 5);

            foreach (var colCombination in columnCombinations)
            {
                var rowCombinations = GetRowCombinations(rows, colCombination.Count);
                foreach (var rowCombination in rowCombinations)
                {
                    var itemList = new List<int>();
                    for (int i = 0; i < colCombination.Count; i++)
                    {
                        itemList.Add(array[rowCombination[i], colCombination[i]]);
                    }
                    result.Add(itemList);
                }
            }
            return result;
        }

        static List<List<int>> GetCombinations(List<int> list, int length)
        {
            if (length == 1)
                return list.Select(t => new List<int> { t }).ToList();

            return GetCombinations(list, length - 1)
                .SelectMany(t => list.Where(e => e > t.Last()),
                            (t1, t2) => t1.Concat(new List<int> { t2 }).ToList()).ToList();
        }

        static List<List<int>> GetRowCombinations(int rows, int length)
        {
            List<List<int>> combinations = new List<List<int>>();
            GetRowCombinationsHelper(new List<int>(), combinations, rows, length);
            return combinations;
        }

        static void GetRowCombinationsHelper(List<int> current, List<List<int>> combinations, int rows, int length)
        {
            if (current.Count == length)
            {
                combinations.Add(new List<int>(current));
                return;
            }

            for (int i = 0; i < rows; i++)
            {
                current.Add(i);
                GetRowCombinationsHelper(current, combinations, rows, length);
                current.RemoveAt(current.Count - 1);
            }
        }

        public List<SlotMachineModel.ValidationResult> FilterResult(List<SlotMachineModel.ValidationResult> results)
        {
            var filteredResults = new List<SlotMachineModel.ValidationResult>();
            var finalResults = new List<SlotMachineModel.ValidationResult>();

            foreach (var result in results)
            {
                string resultIndexPosition = string.Join(",", result.IndexPosition);
                if (!filteredResults.Any() || !filteredResults.Any(x => string.Join(",", x.IndexPosition) == resultIndexPosition))
                {

                    filteredResults.Add(result);
                }
            }

            // check if the result is a subset of another result
            foreach (var result in filteredResults)
            {
                bool isSubset = false;
                foreach (var otherResult in filteredResults)
                {
                    if (result != otherResult && result.IndexPosition.All(otherResult.IndexPosition.Contains))
                    {
                        isSubset = true;
                        break;
                    }
                }
                if (!isSubset)
                {
                    finalResults.Add(result);
                }
            }
            return finalResults;
        }

        public Dictionary<int, List<string>> ConvertPairsToStrings(List<List<int>> indexPairs, Dictionary<int, string> screenDictionary)
        {
            var stringPairsDict = new Dictionary<int, List<string>>();

            for (int i = 0; i < indexPairs.Count; i++)
            {
                var stringPair = indexPairs[i].Select(index => screenDictionary[index]).ToList();
                stringPairsDict[i] = stringPair;
            }

            return stringPairsDict;
        }

        public List<SlotMachineModel.ValidationResult> ValidateStringPairs(Dictionary<int, List<string>> stringPairsDict, List<List<int>> indexPosition)
        {
            var results = new List<SlotMachineModel.ValidationResult>();

            foreach (var kvp in stringPairsDict)
            {
                var pairIndex = kvp.Key;
                var pair = kvp.Value;

                if (pair == null || !indexPosition.Any() || pair.Count == 0)
                    continue;

                int consecutiveElementCount = 1;
                string consecutiveElement = pair[0];
                List<int> lstIndexPositions = new List<int>();

                for (int j = 1; j < pair.Count; j++)
                {
                    if (pair[j] == pair[j - 1])
                    {
                        lstIndexPositions.Add(indexPosition[pairIndex][j - 1]);
                        consecutiveElementCount++;
                    }
                    else
                    {
                        if (consecutiveElementCount > 2)
                        {
                            lstIndexPositions.Add(indexPosition[pairIndex][j - 1]);

                            var payout = model.Paytable[consecutiveElement];
                            var win = payout[consecutiveElementCount - 3];

                            results.Add(new SlotMachineModel.ValidationResult
                            {
                                IndexPosition = new List<int>(lstIndexPositions),
                                ConsecutiveElement = consecutiveElement,
                                ConsecutiveCount = consecutiveElementCount,
                                Win = win
                            });
                        }

                        // Reset for new sequence
                        consecutiveElement = pair[j];
                        consecutiveElementCount = 1;
                        //lstIndexPositions = new List<int> { indexPosition[pairIndex][j - 1] };
                    }
                }

                // Check the last sequence
                if (consecutiveElementCount > 2 && pair[pair.Count-1] == pair[pair.Count-2])
                {
                    lstIndexPositions.Add(indexPosition[pairIndex][pair.Count-1]);
                    var payout = model.Paytable[consecutiveElement];
                    var win = payout[consecutiveElementCount - 3];

                    results.Add(new SlotMachineModel.ValidationResult
                    {
                        IndexPosition = new List<int>(lstIndexPositions),
                        ConsecutiveElement = consecutiveElement,
                        ConsecutiveCount = consecutiveElementCount,
                        Win = win
                    });
                }
            }
            return results;
        }

        private void ContinueOrStop()
        {
            Console.WriteLine("Press any key to spin again or 'q' to quit.");
            if (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                Spin();
            }
        }
    }
}
