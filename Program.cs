using System;
using System.IO;
using System.Text.RegularExpressions;

class LogStandardizer
{
    static void Main(string[] args)
    {
        Console.WriteLine("Программа стандартизации лог-файлов");
        Console.WriteLine("----------------------------------");

        // Запрос пути к входному файлу
        Console.Write("Введите путь к входному файлу: ");
        string inputFile = Console.ReadLine();

        // Запрос пути к выходному файлу
        Console.Write("Введите путь к выходному файлу: ");
        string outputFile = Console.ReadLine();

        string problemsFile = Path.Combine(Path.GetDirectoryName(outputFile), "problems.txt");

        try
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Ошибка: входной файл не найден: {inputFile}");
                return;
            }

            using (StreamReader reader = new StreamReader(inputFile))
            using (StreamWriter writer = new StreamWriter(outputFile))
            using (StreamWriter problemsWriter = new StreamWriter(problemsFile))
            {
                string line;
                int processedCount = 0;
                int problemCount = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (TryProcessLine(line, out string standardizedLine))
                    {
                        writer.WriteLine(standardizedLine);
                        processedCount++;
                    }
                    else
                    {
                        problemsWriter.WriteLine(line);
                        problemCount++;
                    }
                }

                Console.WriteLine($"\nОбработка завершена:");
                Console.WriteLine($"- Стандартизировано записей: {processedCount}");
                Console.WriteLine($"- Проблемных записей: {problemCount}");
                Console.WriteLine($"\nРезультаты:");
                Console.WriteLine($"- Стандартизированные логи сохранены в: {Path.GetFullPath(outputFile)}");
                Console.WriteLine($"- Проблемные записи сохранены в: {Path.GetFullPath(problemsFile)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОшибка: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    private static bool TryProcessLine(string line, out string standardizedLine)
    {
        standardizedLine = null;

        if (TryProcessFormat1(line, out standardizedLine))
        {
            return true;
        }

        if (TryProcessFormat2(line, out standardizedLine))
        {
            return true;
        }

        return false;
    }

    private static bool TryProcessFormat1(string line, out string standardizedLine)
    {
        standardizedLine = null;
        var regex = new Regex(@"^(\d{2})\.(\d{2})\.(\d{4})\s(\d{2}:\d{2}:\d{2}(?:\.\d+)?)\s+(INFO|INFORMATION|WARN|WARNING|ERROR|DEBUG)\s+(.*)$");
        var match = regex.Match(line);

        if (!match.Success) return false;

        try
        {
            string day = match.Groups[1].Value;
            string month = match.Groups[2].Value;
            string year = match.Groups[3].Value;
            string time = match.Groups[4].Value;
            string logLevel = NormalizeLogLevel(match.Groups[5].Value);
            string message = match.Groups[6].Value.Trim();

            standardizedLine = $"{year}-{month}-{day}\t{time}\t{logLevel}\tDEFAULT\t{message}";
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryProcessFormat2(string line, out string standardizedLine)
    {
        standardizedLine = null;
        var regex = new Regex(@"^(\d{4})-(\d{2})-(\d{2})\s(\d{2}:\d{2}:\d{2}(?:\.\d+)?)\|\s*(INFO|INFORMATION|WARN|WARNING|ERROR|DEBUG)\|\d+\|([^|]*)\|(.*)$");
        var match = regex.Match(line);

        if (!match.Success) return false;

        try
        {
            string year = match.Groups[1].Value;
            string month = match.Groups[2].Value;
            string day = match.Groups[3].Value;
            string time = match.Groups[4].Value;
            string logLevel = NormalizeLogLevel(match.Groups[5].Value);
            string method = string.IsNullOrWhiteSpace(match.Groups[6].Value) ? "DEFAULT" : match.Groups[6].Value.Trim();
            string message = match.Groups[7].Value.Trim();

            standardizedLine = $"{year}-{month}-{day}\t{time}\t{logLevel}\t{method}\t{message}";
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string NormalizeLogLevel(string logLevel)
    {
        return logLevel.ToUpper() switch
        {
            "INFORMATION" => "INFO",
            "WARNING" => "WARN",
            _ => logLevel.ToUpper()
        };
    }
}