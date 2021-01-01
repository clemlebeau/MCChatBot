using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace MCChatTest
{
    class Program
    {
        static string serverFileName = "server.jar";
        static string chatMessageRegex = @".*?(<(.*?)>)|(\[Server\]).*?";
        static bool serverOpened = false;
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
                if (output.Contains("Time elapsed:"))
                {
                    serverOpened = true;
                }

                if(Regex.Match(output,chatMessageRegex).Success)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Chat message detected!: {output.Substring(33)}");
                    Console.ResetColor();
                }

                Console.WriteLine(output);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Process has exited, exiting in 3 seconds...");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }

        static void HandleInputs()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Waiting for the server to be started before handling inputs");
            Console.ResetColor();

            while (!serverOpened) ;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Now handling inputs!");
            Console.ResetColor();

            while (!process.HasExited)
            {
                string input = Console.ReadLine();

                SendMessage(input, "kekw");
                //process.StandardInput.WriteLine(input);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"The following input was handled: {input}");
                Console.ResetColor();
                process.StandardInput.Flush();
            }
        }

        static void SendMessage(string message, string authorName)
        {
            string tellraw= "tellraw @a {\"text\":\"<"+authorName+"> "+message+ "\"}";
            process.StandardInput.WriteLine(tellraw);
            process.StandardInput.Flush();
        }
    }
}
