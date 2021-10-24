using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
namespace DotNetTeacherBot
{
    public class BotTeacher : IBotTeacher
    {
        private readonly string botToken;
        private readonly TelegramBotClient client;

        public BotTeacher(IConfiguration config)
        {
            botToken = config["BotToken"];
            client = new TelegramBotClient(botToken);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
        }

        private async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg.Text != null)
            {
                Console.WriteLine($"Пришло сообщение с текстом {msg.Text}");
                await client.SendTextMessageAsync(msg.Chat.Id, "Сообщение получено", replyToMessageId: msg.MessageId);
            }
            
        }
    }
}