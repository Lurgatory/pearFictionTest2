using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prjSlotMachine.Model
{
    public class SlotMachineModel
    {
        public string[][] ReelSets { get; } = new string[][]
        {
            new string [] {"sym2", "sym7", "sym7", "sym1", "sym1", "sym5", "sym1", "sym4", "sym5", "sym3", "sym2", "sym3", "sym8", "sym4", "sym5", "sym2", "sym8", "sym5", "sym7", "sym2" },
            new string [] { "sym1", "sym6", "sym7", "sym6", "sym5", "sym5", "sym8", "sym5", "sym5", "sym4", "sym7", "sym2", "sym5", "sym7", "sym1", "sym5", "sym6", "sym8", "sym7", "sym6", "sym3", "sym3", "sym6", "sym7", "sym3" },
            new string [] { "sym5", "sym2", "sym7", "sym8", "sym3", "sym2", "sym6", "sym2", "sym2", "sym5", "sym3", "sym5", "sym1", "sym6", "sym3", "sym2", "sym4", "sym1", "sym6", "sym8", "sym6", "sym3", "sym4", "sym4", "sym8", "sym1", "sym7", "sym6", "sym1", "sym6"},
            new string [] { "sym2", "sym6", "sym3", "sym6", "sym8", "sym8", "sym3", "sym6", "sym8", "sym1", "sym5", "sym1", "sym6", "sym3", "sym6", "sym7", "sym2", "sym5", "sym3", "sym6", "sym8", "sym4", "sym1", "sym5", "sym7" },
            new string [] { "sym7", "sym8", "sym2", "sym3", "sym4", "sym1", "sym3", "sym2", "sym2", "sym4", "sym4", "sym2", "sym6", "sym4", "sym1", "sym6", "sym1", "sym6", "sym4", "sym8" }

        };

        public Dictionary<string, int[]> Paytable { get; } = new Dictionary<string, int[]>
        {
            { "sym1", new int[] { 1, 2, 3 } },
            { "sym2", new int[] { 1, 2, 3 } },
            { "sym3", new int[] { 1, 2, 5 } },
            { "sym4", new int[] { 2, 5, 10 } },
            { "sym5", new int[] { 5, 10, 15 } },
            { "sym6", new int[] { 5, 10, 15 } },
            { "sym7", new int[] { 5, 10, 20 } },
            { "sym8", new int[] { 10, 20, 50 } }
        };

        public int[,] ScreenIndexPositions { get; } = new int[,]
        {
            {  0,  1,  2,  3,  4 },
            {  5,  6,  7,  8,  9 },
            { 10, 11, 12, 13, 14 }
        };

        public class ValidationResult
        {
            public List<int> IndexPosition { get; set; }
            public string ConsecutiveElement { get; set; }
            public int ConsecutiveCount { get; set; }
            public int Win { get; set; }

        }
    }
}
