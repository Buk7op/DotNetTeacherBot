using DotNetTeacherBot.SyncDataService.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using DotNetTeacherBot.DTOs;
namespace DotNetTeacherBot
{
    public class BotTeacher
    {
        
        private readonly string _botToken;
        private readonly TelegramBotClient _client;
        private readonly IQuestionDataClient _dataClient;
        Dictionary<int,int> ListToId = new Dictionary<int, int>();
        public BotTeacher(IConfiguration config, IQuestionDataClient dataClient)
        {
            _botToken = config["BotToken"];
            _client = new TelegramBotClient(_botToken);
            _client.StartReceiving();
            _client.OnMessage += OnMessageHandler;
            _dataClient = dataClient;
        }
        
        
        private async void OnMessageHandler(object sender, MessageEventArgs e)
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
                    case "Выбрать вопрос":
                        await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Введите номер вопроса",
                                ParseMode.Default, 
                                replyMarkup: new ForceReplyMarkup { Selective = true }); 
                        break;          
                }
                if (msg.ReplyToMessage != null && msg.ReplyToMessage.Text.Contains("Введите номер вопроса"))
                {
                    int id = default;
                    if(int.TryParse(msg.Text,out id) && ListToId.ContainsKey(id))
                    {
                        ShowQuestionById(e,id);
                    }
                    else
                    {
                        ShowAllQuestions(e);
                        await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Неверный номер вопроса, введите номер из списка выше.");
                        await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Введите номер вопроса",
                                ParseMode.Default, 
                                replyMarkup: new ForceReplyMarkup { Selective = true });
                    }
                }
                //InitialMessage(e);
            }
        }
        private async  void InitialMessage(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Показать все вопросы"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Заглушка"}}
                }
            }; 
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Что будем делать?",
                replyMarkup: keyboard 
            );
        }
        private async  void StartLearning(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Вопросы по порядку"}, new KeyboardButton {Text = "Вопросы в случайном порядке"}},
                }
            }; 
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "В каком порядке задавать вопросы?",
                replyMarkup: keyboard 
            );
        }
        private async void ShowAllQuestions(MessageEventArgs e)
        {
            int questionCounter = 1;
            ListToId.Clear();
            var questions = _dataClient.GetQuestionsFromSite();
            StringBuilder sb = new StringBuilder();
            foreach(var q in questions.Result)
            {
                ListToId.Add(questionCounter,q.ID);
                sb.Append(questionCounter+"."+$" {q.ShortQuestion}\n");
                questionCounter++;
            }
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Показать все вопросы"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Выбрать вопрос"}}
                }
            }; 
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: sb.ToString(),
                replyMarkup: keyboard 
            );
        }
    private async void ShowQuestionById(MessageEventArgs e, int id)
        {  
            var question = _dataClient.GetQuestionById(ListToId[id]);
            StringBuilder sb = new StringBuilder();
            sb.Append(question.Result.ShortQuestion + Environment.NewLine + question.Result.Description);
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Случайный вопрос"}, new KeyboardButton {Text = "Показать все вопросы"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Показать ответ"}, new KeyboardButton {Text = "Выбрать вопрос"}}
                }
            }; 
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: sb.ToString(),
                replyMarkup: keyboard 
            );
        }
        private async  void AddAnotherQuestion(MessageEventArgs e)
        { 
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Чтобы добавить вопрос необходимо ввести короткую версию вопроса, расширенную версию вопроса и ответ на вопрос, после чего нажать кнопку отправить вопрос.",
                replyMarkup: new ReplyKeyboardRemove()
            );
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Введите короткую версию вопроса"
            );

        }
    }
}