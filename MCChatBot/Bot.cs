using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using MCChatTest;

namespace MCChatBot
{
    class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            string json = string.Empty;

            using (FileStream fs = File.OpenRead("config.json"))
            {
                using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            ConfigJson configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,

            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            Client.MessageCreated += OnMessageCreated;

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = false,
                CaseSensitive = false
            };

            InteractivityConfiguration interactivityConfiguration = new InteractivityConfiguration
            {

            };

            Client.UseInteractivity(interactivityConfiguration);

            Commands = Client.UseCommandsNext(commandsConfig);

            //Commands.RegisterCommands<KBotCommands>();
            //Commands.RegisterCommands<FunCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                if (e.Message.Channel.Name == "mc-chat")
                {
                    string message = e.Message.Content.ToString();
                    string author = e.Author.Username.ToString();
                    Program.SendMessage(message, author);
                }
            }
        }

        public async Task SendMessage(string message)
        {
            DSharpPlus.Entities.DiscordChannel channel = await Client.GetChannelAsync(794687180387647499).ConfigureAwait(false);
            await channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
