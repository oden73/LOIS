// Выполнили студенты группы 221701 БГУИР:
// - Карпук Максим Витальевич (oden73)
// - Телица Илья Денисович
// Вариант 4
// 
// Консольный ввод-вывод
// 22.05.2024
//
// Источники:
// - Проектирование программного обеспечения интеллектуальных систем (3 семестр)
//


namespace LOIS1
{
    class ConsoleFile
    {
        static void Main()
        {

            while (true)
            {
                run();
                while (true)
                {
                    Console.WriteLine("Желаете продолжить? y/n");
                    String? key = Console.ReadLine();
                    if (key == "y")
                    {
                        break;
                    }
                    else if (key == "n")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Неправильный формат ввода");
                        continue;
                    }
                }
            }
        }

        // (A\/(B\/(C\/(D\/(E\/(F\/(G\/(H\/(I\/(J\/(K\/(L\/(M\/(N\/(O\/(P\/(Q\/(R\/(S\/(T\/(U\/(V\/(W\/(X\/(Y\/Z)))))))))))))))))))))))))
        // (A/\(B/\(C/\(D/\(E/\(F/\(G/\(H/\(I/\(J/\(K/\(L/\(M/\(N/\(O/\(P/\(Q/\(R/\(S/\(T/\(U/\(V/\(W/\(X/\(Y/\Z)))))))))))))))))))))))))

        static public void run()
        {
            Console.WriteLine("Введите логическое выражение:");
            String? expression = Console.ReadLine();
            try
            {
                LogicalExpression logical_expression = new LogicalExpression(expression);
                
                if (logical_expression.is_neutral())
                {
                    Console.WriteLine("Выражение является нейтральным");
                }
                else
                {
                    Console.WriteLine("Выражение не является нейтральным");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Введенное выражение не является корректным");
                return;
            }
        }
    }
}
