using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
namespace DotNetTeacherBot
{
    public static class BotTeacher
    {
        
        private static string botToken;
        private static TelegramBotClient client;

        public static void ConfigureBot(IConfiguration config)
        {
            botToken = config["BotToken"];
            client = new TelegramBotClient(botToken);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
        }
        
        private async static void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg.Text != null)
            {
                switch(msg.Text)
                {
                    case "Начать обучение":
                        StartLearning(e);
                        break;
                    case "Показать все вопросы":
                        ShowAllQuestions(e);
                        break;
                    case "Предложить свой вопрос":
                        AddAnotherQuestion(e);
                        break;      
                }
                InitialMessage(e);
            }
        }
        private async static void InitialMessage(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Показать все вопросы"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Заглушка"}}
                }
            }; 
            await client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Что будем делать?",
                replyMarkup: keyboard 
            );
        }
        private async static void StartLearning(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Вопросы по порядку"}, new KeyboardButton {Text = "Вопросы в случайном порядке"}},
                }
            }; 
            await client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "В каком порядке задавать вопросы?",
                replyMarkup: keyboard 
            );
        }
        private async static void ShowAllQuestions(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Предложить свой вопрос"}}
                }
            }; 
            await client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Что будем делать?",
                replyMarkup: keyboard 
            );
        }
        private async static void AddAnotherQuestion(MessageEventArgs e)
        { 
            await client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Чтобы добавить вопрос необходимо ввести короткую версию вопроса, расширенную версию вопроса и ответ на вопрос, после чего нажать кнопку отправить вопрос.",
                replyMarkup: new ReplyKeyboardRemove()
            );
            await client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Введите короткую версию вопроса"
            );

        }
    }
}