using prjSlotMachine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prjSlotMachine.View
{
    public class SlotMachineView
    {
        public void DisplayResults(int[] stopPositions, string[,] screen)
        {
            Console.WriteLine("Stop Positions: " + string.Join(", ", stopPositions));
            Console.WriteLine("Screen:");
            int rows = screen.GetLength(0);
            int cols = screen.GetLength(1);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Console.Write(screen[row, col] + " ");
                }
                Console.WriteLine();
            }
        }

        public void DisplayWinnings(List<SlotMachineModel.ValidationResult> filteredResults)
        {
            foreach (var pair in filteredResults)
            {
                Console.WriteLine($"- Ways win {string.Join(",", pair.IndexPosition)}, {pair.ConsecutiveElement} x{pair.ConsecutiveCount}, {pair.Win}");
            }
        }
    }
}
