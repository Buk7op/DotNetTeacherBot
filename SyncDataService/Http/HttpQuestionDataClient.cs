using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DotNetTeacherBot.DTOs;
using System.Collections.Generic;

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
        public async Task<IEnumerable<QuestionCreateDto>> GetQuestionsFromSite()
        {
            var response = await _httpClient.GetAsync($"{_config["GetAllQuestions"]}");
            if(response.IsSuccessStatusCode)
            {
                System.Console.WriteLine("--> Sync POST to Site was OK!");
            }
            else
            {
                System.Console.WriteLine("--> Sync POST to Site was NOT OK!");
            }
            
            return new List<QuestionCreateDto>();
        }

        
    }
}