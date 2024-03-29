﻿using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;
using MCChatBot;

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
        static Bot bot;
        static void Main(string[] args)
        {
            new Thread(StartBot).Start();
            StartServer();
        }

        static void StartBot()
        {
            bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }

        static void StartServer()
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
            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                if (output.Contains("Time elapsed:"))
                {
                    serverOpened = true;
                }

                if (Regex.Match(output, chatMessageRegex).Success)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    bot.SendMessage(output.Substring(33));
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

                //SendMessage(input, "kekw");
                process.StandardInput.WriteLine(input);
                //Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"The following input was handled: {input}");
                Console.ResetColor();
                process.StandardInput.Flush();
            }
        }

        public static void SendMessage(string message, string authorName)
        {
            string tellraw = "tellraw @a [\"\",{\"text\":\"DISCORD \",\"bold\":true,\"color\":\"blue\"},\" <"+authorName+"> " + message + "\"]";//"tellraw @a {\"text\":\"<" + authorName + "> " + message + "\"}";
            process.StandardInput.WriteLine(tellraw);
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] [Server thread/INFO]: <{authorName}> {message}");
            process.StandardInput.Flush();
        }
    }
}
