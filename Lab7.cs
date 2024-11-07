using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;

namespace Lab7
{
    class Program
    {
        public static void Main()
        {
            DataTable table = new("MyTable");
            string pattern = @"^\d+,\d{2}$";
            string input;
            string amount;
            int result;

            // Ввод количества критериев
            table.Columns.Add(" ", typeof(string)); // Первый столбец для названий критериев
            while (true)
            {
                Console.WriteLine("Enter amount of criterias:");
                amount = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(amount, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Incorrect input.");
                }
            }

            string[] names = new string[result];
            double totalSum = 0;

            // Ввод имен критериев
            for (int i = 0; i < result; i++)
            {
                Console.WriteLine($"Enter a name of criteria {i + 1}:");
                string a = Console.ReadLine() ?? string.Empty;
                table.Columns.Add(a, typeof(string)); // Создаем столбец с именем критерия
                names[i] = a;
            }

            table.Columns.Add("sum", typeof(string));
            table.Columns.Add("compr. coef.", typeof(string));
            Console.WriteLine("For comparing criteria, a scale is used where 1,00 indicates that the criteria are equally important,\n" +
            "and 9,00 indicates that the first criterion is significantly more important than the other. (If the first criterion is less important, \n" +
            "its coefficient will be 1/x, where x is the comparison coefficient of criterion 2 to criterion 1.");

            // Заполнение таблицы коэффициентами
            for (int j = 0; j < result; j++)
            {
                DataRow row = table.NewRow();
                row[0] = names[j]; // Имя критерия в первой ячейке строки

                double[] nums = new double[result];
                double rowSum = 0; // Для суммы коэффициентов текущей строки

                for (int n = 0; n < result; n++)
                {
                    while (true)
                    {
                        Console.WriteLine($"Enter a comparison coefficient of criteria {names[j]} for criteria {names[n]}:");
                        input = Console.ReadLine() ?? string.Empty;
                        if (Regex.IsMatch(input, pattern))
                        {
                            double parsedInput = double.Parse(input);
                            row[n + 1] = input;
                            nums[n] = parsedInput;
                            rowSum += parsedInput;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect input. Try again.");
                        }
                    }
                }

                totalSum += rowSum; // Добавляем сумму текущей строки к общей сумме
                row[result + 1] = rowSum.ToString("F2"); // Записываем сумму строки в столбец `sum`
                table.Rows.Add(row);
            }

            // Заполняем столбец `compr. coef.` после ввода всех значений
            foreach (DataRow row in table.Rows)
            {
                double rowSum = double.Parse(("" + row[result + 1]).ToString());
                row[result + 2] = (rowSum / totalSum).ToString("F2"); // Вычисляем `compr. coef.` как отношение суммы строки к общей сумме
            }

            // Вывод таблицы
            PrintTable(table);
        }

        public static void PrintTable(DataTable table)
        {
            int[] columnWidths = new int[table.Columns.Count];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                columnWidths[i] = table.Columns[i].ColumnName.Length;
                foreach (DataRow row in table.Rows)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], ("" + row[i].ToString()).Length);
                }
            }

            // Печать заголовков и строк таблицы
            PrintSeparator(columnWidths);
            Console.Write("|");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Console.Write($" {table.Columns[i].ColumnName.PadRight(columnWidths[i])} |");
            }
            Console.WriteLine();
            PrintSeparator(columnWidths);

            foreach (DataRow row in table.Rows)
            {
                Console.Write("|");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Console.Write($" {("" + row[i].ToString()).PadRight(columnWidths[i])} |");
                }
                Console.WriteLine();
            }
            PrintSeparator(columnWidths);
        }

        // Метод для печати разделительной линии
        static void PrintSeparator(int[] columnWidths)
        {
            Console.Write("+");
            foreach (int width in columnWidths)
            {
                Console.Write(new string('-', width + 2) + "+");
            }
            Console.WriteLine();
        }
    }
}
