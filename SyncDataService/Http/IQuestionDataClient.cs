using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTeacherBot.DTOs;

namespace DotNetTeacherBot.SyncDataService.Http
{
    public interface IQuestionDataClient
    {
        Task<IEnumerable<QuestionReadDto>> GetQuestionsFromSite();
        Task<QuestionReadDto> GetQuestionById(int id);
        Task AddQuestion(QuestionCreateDto q);
    }
}