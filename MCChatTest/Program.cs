using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace MCChatTest
{
    class Program
    {
        static string serverFileName = "server.jar";
        //RAM variables are in Gigabytes
        static int minRAM = 1;
        static int maxRAM = 4;
        static Process process;
        static void Main(string[] args)
        {
            new Thread(HandleInputs).Start();

            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-Xms{minRAM}G -Xmx{maxRAM}G -jar {serverFileName} nogui",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                }
            };

            process.Start();
            while(!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                Console.WriteLine(output);
            }
        }

        static void HandleInputs()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Waiting for 10 seconds before handling inputs");
            Console.ForegroundColor = ConsoleColor.White;
            
            Thread.Sleep(10000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Now handling inputs!");
            Console.ForegroundColor = ConsoleColor.White;

            while(!process.HasExited)
            {
                string input = Console.ReadLine();

                process.StandardInput.WriteLine(input);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"The following input was handled: {input}");
                Console.ForegroundColor = ConsoleColor.White;
                process.StandardInput.Flush();
            }

            Console.WriteLine("Process has exited, exiting in 3 seconds...");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }
    }
}
