using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTeacherBot.DTOs;

namespace DotNetTeacherBot.SyncDataService.Http
{
    public interface IQuestionDataClient
    {
        Task<IEnumerable<QuestionCreateDto>> GetQuestionsFromSite();
    }
}