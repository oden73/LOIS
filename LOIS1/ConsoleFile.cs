using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOIS1
{
    class ConsoleFile
    {
        static void Main()
        {
            LogicalExpression l = new LogicalExpression("(A\\/(B\\/(C\\/(D\\/E))))");
            List<String> table = l.truth_table_print_strings();
            for (int i = 0; i < table.Count; i++)
            {
                Console.WriteLine(table[i]);
            }
        }
    }
}
