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
        Dictionary<long,QuestionCreateDto> QuestionsFromUsers = new Dictionary<long,QuestionCreateDto>();
        ReplyKeyboardMarkup mainKeyboard = new ReplyKeyboardMarkup
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton> {new KeyboardButton {Text = "Начать обучение"}, new KeyboardButton {Text = "Что делает этот бот?"}},
                new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}, new KeyboardButton {Text = "Показать все вопросы"}}
            }
        };
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
                "/start" => InitialMessageAsync(e),
                "Что делает этот бот?" => FAQAsync(e),
                "Начать обучение" => StartLearningAsync(e),
                "Показать все вопросы" => ShowAllQuestionsAsync(e),
                "Случайный вопрос" => ShowQuestionByIdAsync(e, rnd.Next(1, CountQuestions() + 1), false),
                "Предложить свой вопрос" => AddAnotherQuestion(e),
                "Выбрать вопрос" => AskNumberOfQuestionAsync(e),
                "Вопросы по порядку" => QuestionInOrderAsync(e),
                string a when a.Contains("Следующий вопрос") => ShowQuestionByIdAsync(e,int.Parse(String.Concat(e.Message.Text.Where(Char.IsDigit))),false),
                string a when a.Contains("Показать ответ на") => ShowQuestionByIdAsync(e,int.Parse(String.Concat(e.Message.Text.Where(Char.IsDigit))),true),
                _ => Recognize(e)
            };
            
        }

        
        private async Task<Message> DontUnderstandAsync(MessageEventArgs e)
        {
                return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: $"Я не понимаю команду {e.Message.Text}",
                    replyMarkup: mainKeyboard); 
        }

        private async Task<Message> FAQAsync(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Данный бот предназначен для подготовки к собеседованию по .Net. Вопросы и ответы на них собраны как с простора сети интернет, так и из личного опыта подписчиков бота. Если вы считаете что какого-либо вопроса нет в списке, вы можете предложить свой вопрос, воспользовавшись кнопкой ниже, он будет добавлен после проверки модератером.");
        }

        private async Task<Message> QuestionInOrderAsync(MessageEventArgs e)
        {
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: $"Всего в боте {CountQuestions()} вопросов, с какого начнем?");
            return await AskNumberOfQuestionAsync(e);
        }
        private async Task<Message> AskNumberOfQuestionAsync(MessageEventArgs e)
        {
            Console.WriteLine("--->"+e.Message.Chat.Id);
            return await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Введите номер вопроса",
                                ParseMode.Default,
                                replyMarkup: new ForceReplyMarkup { Selective = true });
        }
        private async Task<Message> InitialMessageAsync(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Что будем делать?",
                    replyMarkup: mainKeyboard
            );
        }
        private async Task<Message> StartLearningAsync(MessageEventArgs e)
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
        private async Task<Message> Recognize(MessageEventArgs e)
        {
            if (e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Text.Contains("Введите номер вопроса"))
                {
                    int id = default;
                    if(int.TryParse(e.Message.Text,out id) && GetQuestionDict().ContainsKey(id))
                    {
                        return await ShowQuestionByIdAsync(e,id,false);
                    }
                    else
                    {
                        await ShowAllQuestionsAsync(e);
                        await _client.SendTextMessageAsync(
                                chatId: e.Message.Chat.Id,
                                text: "Неверный номер вопроса, введите номер из списка выше.");
                        return await AskNumberOfQuestionAsync(e);
                    }
                }
            else if (e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Text.Contains("Введите короткую версию вопроса"))
            {
                QuestionsFromUsers.Add(e.Message.Chat.Id,new QuestionCreateDto{ShortQuestion = e.Message.Text});
                return await AddDescription(e);
            }
            else if (e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Text.Contains("Введите описание вопроса"))
            {
                QuestionsFromUsers[e.Message.Chat.Id].Description = e.Message.Text;
                return await AddAnswer(e);
            }
            else if (e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.Text.Contains("Введите ответ на вопрос"))
            {
                QuestionsFromUsers[e.Message.Chat.Id].Answer = e.Message.Text;
                return await PostQuestion(e);
            }
            else return await DontUnderstandAsync(e);
        }
        private async Task<Message> ShowAllQuestionsAsync(MessageEventArgs e)
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
        private async Task<Message> ShowQuestionByIdAsync(MessageEventArgs e, int id, bool showAnswer)
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

        private async Task<Message> PostQuestion(MessageEventArgs e)
        {
            var question = QuestionsFromUsers[e.Message.Chat.Id];
            if(QuestionsFromUsers.Keys.Contains(e.Message.Chat.Id) && !String.IsNullOrEmpty(question.ShortQuestion) && !String.IsNullOrEmpty(question.Answer))
            {
                
                await _dataClient.AddQuestion(QuestionsFromUsers[e.Message.Chat.Id]);
                QuestionsFromUsers.Remove(e.Message.Chat.Id);
                return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: $"Ваш вопрос успешно отправлен, после прохождения модерации он будет добавлен в список.");
            }
            else
            {
                var keyboard = new ReplyKeyboardMarkup
                {
                    Keyboard = new List<List<KeyboardButton>>
                        {
                            new List<KeyboardButton> {new KeyboardButton {Text = "Предложить свой вопрос"}}}           
                        };
                return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: $"Я не могу отправить ваш вопрос, один из его параметров не заполнен, повторите отправку вопроса.");
            }
        }

        private async Task<Message> AddAnotherQuestion(MessageEventArgs e)
        {
            await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Чтобы добавить вопрос следуйте инструкциям ниже",
                replyMarkup: new ReplyKeyboardRemove()
            );
            return await AddShortQuestion(e);
        }
        private async Task<Message> AddShortQuestion(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Введите короткую версию вопроса",
                ParseMode.Default,
                replyMarkup: new ForceReplyMarkup { Selective = true });
            
        }
        private async Task<Message> AddDescription(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: "Введите описание вопроса",
                ParseMode.Default,
                replyMarkup: new ForceReplyMarkup { Selective = true });
            
        }
        private async Task<Message> AddAnswer(MessageEventArgs e)
        {
            return await _client.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Введите ответ на вопрос",
                    ParseMode.Default,
                    replyMarkup: new ForceReplyMarkup { Selective = true });
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