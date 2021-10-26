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
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DotNetTeacherBot
{
    public class BotTeacher
    {

        private readonly string _botToken;
        private readonly TelegramBotClient _client;
        private readonly IQuestionDataClient _dataClient;

        Random rnd = new Random();
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


            var action = e.Message.Text switch
            {
                "/start" => InitialMessage(e),
                "Что делает этот бот?" => FAQ(e),
                "Начать обучение" => StartLearning(e),
                "Показать все вопросы" => ShowAllQuestions(e),
                "Случайный вопрос" => ShowQuestionById(e, rnd.Next(1, CountQuestions() + 1), false),
                //"Предложить свой вопрос" => //TODO
                "Выбрать вопрос" => AskNumberOfQuestion(e),
                "Вопросы по порядку" => QuestionInOrder(e),
                string a when a.Contains("Следующий вопрос") => ShowQuestionById(e,int.Parse(String.Concat(e.Message.Text.Where(Char.IsDigit))),false),
                string a when a.Contains("Показать ответ на") => ShowQuestionById(e,int.Parse(String.Concat(e.Message.Text.Where(Char.IsDigit))),true),
                _ => IDontUnderstand(e)
            };
            // var msg = e.Message;

            // if (msg.Text != null)
            // {
            //     // switch(msg.Text)
            //     // {
            //     //     case "/start":
            //     //         InitialMessage(e);
            //     //         break;
            //     //     case "Что делает этот бот?":
            //     //         FAQ(e);
            //     //         break;
            //     //     case "Начать обучение":
            //     //         StartLearning(e);
            //     //         break;
            //     //     case "Показать все вопросы":
            //     //         ShowAllQuestions(e);
            //     //         break;
            //     //     case "Случайный вопрос":
            //     //         ShowQuestionById(e,rnd.Next(1, CountQuestions()+1),false);
            //     //         break;
            //     //     case "Предложить свой вопрос":
            //     //         AddAnotherQuestion(e);
            //     //         break;
            //     //     case "Вопросы по порядку":
            //     //         await _client.SendTextMessageAsync(
            //     //                 chatId: e.Message.Chat.Id,
            //     //                 text: $"Всего в боте {CountQuestions()} вопросов, с какого начнем?");
            //     //         AskNumberOfQuestion(e);
            //     //         break;
            //     //     case "Выбрать вопрос":
            //     //         AskNumberOfQuestion(e);
            //     //         break;
            //     // }
            //     if(msg.Text.Contains("Следующий вопрос"))
            //     {
            //         int id = int.Parse(String.Concat(msg.Text.Where(Char.IsDigit)));
            //         ShowQuestionById(e,id,false);
            //     }
            //     else if(msg.Text.Contains("Показать ответ на"))
            //     {
            //         int id = int.Parse(String.Concat(msg.Text.Where(Char.IsDigit)));
            //         ShowQuestionById(e,id,true);
            //     }
            //     else if (msg.ReplyToMessage != null && msg.ReplyToMessage.Text.Contains("Введите номер вопроса"))
            //     {
            //         int id = default;
            //         if(int.TryParse(msg.Text,out id) && GetQuestionDict().ContainsKey(id))
            //         {
            //             ShowQuestionById(e,id,false);
            //         }
            //         else
            //         {
            //             ShowAllQuestions(e);
            //             await _client.SendTextMessageAsync(
            //                     chatId: e.Message.Chat.Id,
            //                     text: "Неверный номер вопроса, введите номер из списка выше.");
            //             AskNumberOfQuestion(e);
            //         }
            //     }

            // }
        }

        private async Task<Message> IDontUnderstand(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Показать все вопросы"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Что делает этот бот?"}}
                }
            };
            
            if (e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Text.Contains("Введите номер вопроса"))
                {
                    int id = default;
                    if(int.TryParse(e.Message.Text,out id) && GetQuestionDict().ContainsKey(id))
                    {
                        return await ShowQuestionById(e,id,false);
                    }
                    else
                    {
                        await ShowAllQuestions(e);
                        await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Неверный номер вопроса, введите номер из списка выше.");
                        return await AskNumberOfQuestion(e);
                    }
                }
            else
            {
                return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: $"Я не понимаю команду {e.Message.Text}",
                    replyMarkup: keyboard);
            }
            
        }

        private async Task<Message> FAQ(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Данный бот предназначен для подготовки к собеседованию по .Net. Вопросы и ответы на них собраны как с простора сети интернет, так и из личного опыта подписчиков бота. Если вы считаете что какого-либо вопроса нет в списке, вы можете предложить свой вопрос, воспользовавшись кнопкой ниже, он будет добавлен после проверки модератером.");
        }

        private async Task<Message> QuestionInOrder(MessageEventArgs e)
        {
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: $"Всего в боте {CountQuestions()} вопросов, с какого начнем?");
            return await AskNumberOfQuestion(e);
        }
        private async Task<Message> AskNumberOfQuestion(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Введите номер вопроса",
                                ParseMode.Default,
                                replyMarkup: new ForceReplyMarkup { Selective = true });
        }
        private async Task<Message> InitialMessage(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Показать все вопросы"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Что делает этот бот?"}}
                }
            };
            return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Что будем делать?",
                    replyMarkup: keyboard
            );
        }
        private async Task<Message> StartLearning(MessageEventArgs e)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Вопросы по порядку"}, new KeyboardButton {Text = "Случайный вопрос"}},
                }
            };
            return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "В каком порядке задавать вопросы?",
                replyMarkup: keyboard
            );
        }
        private async Task<Message> ShowAllQuestions(MessageEventArgs e)
        {
            int questionCounter = 1;
            var questions = _dataClient.GetQuestionsFromSite();
            StringBuilder sb = new StringBuilder();
            foreach (var q in questions.Result)
            {
                sb.Append(questionCounter + "." + $" {q.ShortQuestion}\n");
                questionCounter++;
            }
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Выбрать вопрос"}},
                    new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Показать все вопросы"}}
                }
            };
            return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: sb.ToString(),
                replyMarkup: keyboard
            );
        }
        private async Task<Message> ShowQuestionById(MessageEventArgs e, int id, bool showAnswer)
        {
            int nextId = default;
            QuestionReadDto currentQuestion = new QuestionReadDto();
            currentQuestion = _dataClient.GetQuestionById(GetQuestionDict()[id]).Result;
            if (id < CountQuestions())
            {
                nextId = id + 1;
            }
            else
            {
                nextId = id;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(currentQuestion.ShortQuestion + Environment.NewLine + currentQuestion.Description);
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {new KeyboardButton {Text = "Случайный вопрос"}, new KeyboardButton {Text = $"Следующий вопрос {nextId}"}},
                        new List<KeyboardButton> {new KeyboardButton {Text = $"Показать ответ на {id} вопрос"}, new KeyboardButton {Text = "Показать все вопросы"}}
                    }
            };
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: sb.ToString(),
                replyMarkup: keyboard
            );
            if (showAnswer)
            {
                keyboard = new ReplyKeyboardMarkup
                {
                    Keyboard = new List<List<KeyboardButton>>
                        {
                            new List<KeyboardButton> {new KeyboardButton {Text = "Случайный вопрос"}, new KeyboardButton {Text = $"Следующий вопрос {nextId}"}},
                            new List<KeyboardButton> {new KeyboardButton {Text = "Выбрать вопрос"}, new KeyboardButton {Text = "Показать все вопросы"}}
                        }
                };
                return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: currentQuestion.Answer,
                ParseMode.Default,
                replyMarkup: keyboard
                );
            }
            else
            {
                return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: $"Показать ответ на вопрос: {id}?",
                ParseMode.Default,
                replyMarkup: keyboard
            );
            }


        }
        private async void AddAnotherQuestion(MessageEventArgs e)
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
        private int CountQuestions() => _dataClient.GetQuestionsFromSite().Result.Count();

        private Dictionary<int, int> GetQuestionDict()
        {
            int counter = 1;
            Dictionary<int, int> ListToId = new Dictionary<int, int>();
            var questions = _dataClient.GetQuestionsFromSite().Result;
            foreach (var q in questions)
            {
                ListToId.Add(counter, q.ID);
                counter++;
            }
            return ListToId;
        }
    }
}