using System;
using System.Diagnostics;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.Title = "Лабораторная работа: Управление процессами";

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Лабараторная работа: управление процессами");
            Console.WriteLine("1. Задание 1: Мониторинг и анализ текущего процесса");
            Console.WriteLine("2. Задание 2: Создание и управление дочерним процессом");
            Console.WriteLine("3. Задание 3: Манипулирование и принудительное завершение");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите номер задания: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ExecuteTask1();
                    break;
                case "2":
                    ExecuteTask2();
                    break;
                case "3":
                    ExecuteTask3();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный ввод. Попробуйте снова");
                    break;
            }

            Console.WriteLine("Нажмите любую клавишу для возврата в меню");
            Console.ReadKey();
        }
    }

    static void ExecuteTask1()
    {
        Console.WriteLine("Задание 1. Информация о текущем процессе");

        Process current = Process.GetCurrentProcess();

        Console.WriteLine($"Имя процесса: {current.ProcessName}");
        Console.WriteLine($"Идентификатор: {current.Id}");
        Console.WriteLine($"Время запуска: {current.StartTime}");

        double memoryInMb = (double)current.WorkingSet64 / 1024 / 1024;
        Console.WriteLine($"Используемая физическая память: {memoryInMb:F2} МБ");
        Console.WriteLine($"Количество потоков: {current.Threads.Count}");

        Console.WriteLine("Топ-5 процессов по объему физической памяти");

        var topProcesses = Process.GetProcesses()
            .OrderByDescending(p => p.WorkingSet64)
            .Take(5);

        int rank = 1;
        foreach (var proc in topProcesses)
        {
            double procMemMb = (double)proc.WorkingSet64 / 1024 / 1024;
            Console.WriteLine($"{rank}. PID: {proc.Id} | Имя: {proc.ProcessName} | Память: {procMemMb:F2} МБ");
            rank++;
        }
    }

    static void ExecuteTask2()
    {
        Console.WriteLine("Задание 2. Запуск дочернего процесса");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c ping 8.8.8.8 -n 4",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.GetEncoding(866)
        };

        Console.WriteLine("Запуск процесса cmd.exe с командой ping");

        using (Process childProcess = Process.Start(startInfo))
        {
            if (childProcess != null)
            {
                string output = childProcess.StandardOutput.ReadToEnd();

                Console.WriteLine("Полученный вывод от дочернего процесса:");
                Console.WriteLine(output);

                childProcess.WaitForExit();
                Console.WriteLine($"Процесс завершился с кодом: {childProcess.ExitCode}");
            }
            else
            {
                Console.WriteLine("Не удалось запустить процесс.");
            }
        }
    }

    static void ExecuteTask3()
    {
        Console.WriteLine("Задание 3. Тайм-аут и принудительное завершение");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c timeout /t 60",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process longProcess = Process.Start(startInfo))
        {
            if (longProcess != null)
            {
                Console.WriteLine($"Процесс запущен, ожидаем 3 секунды...");

                bool exitedInTime = longProcess.WaitForExit(3000);

                if (!exitedInTime)
                {
                    Console.WriteLine("Предупреждение: Превышено время ожидания! Процесс не завершился сам.");
                    Console.WriteLine("Принудительно завершаем процесс методом Kill()");
                    longProcess.Kill();

                    longProcess.WaitForExit();
                    Console.WriteLine("Ресурсы процесса успешно освобождены операционной системой.");
                }
                else
                {
                    Console.WriteLine("Процесс завершился сам.");
                }
            }
        }
    }
}
