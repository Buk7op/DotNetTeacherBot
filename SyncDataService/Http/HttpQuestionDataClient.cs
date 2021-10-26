using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DotNetTeacherBot.DTOs;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetTeacherBot.SyncDataService.Http
{
    public class HttpQuestionDataClient : IQuestionDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpQuestionDataClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task AddQuestion(QuestionCreateDto question)
        {
            var httpContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(question),
                Encoding.UTF8,
                "application/json");
            var response = await _httpClient.PostAsync($"{_config["PostQuestion"]}", httpContent);
        }

        public async Task<QuestionReadDto> GetQuestionById(int id)
        {
            var response = await _httpClient.GetAsync($@"{_config["GetAllQuestions"]}/{id}");
            var responsebody = response.Content.ReadAsStringAsync();
            QuestionReadDto result = JsonConvert.DeserializeObject<QuestionReadDto>(responsebody.Result);
            return result;
        }

        public async Task<IEnumerable<QuestionReadDto>> GetQuestionsFromSite()
        {
            var response = await _httpClient.GetAsync($"{_config["GetAllQuestions"]}");
            var responsebody = response.Content.ReadAsStringAsync();
            IEnumerable<QuestionReadDto> result = JsonConvert.DeserializeObject<IEnumerable<QuestionReadDto>>(responsebody.Result);
            return result;
        }

        
    }
}