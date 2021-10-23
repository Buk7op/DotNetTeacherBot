using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetTeacherBot.Models;

namespace DotNetTeacherBot.Data
{
    public interface IQuestionRepo
    {
        IEnumerable<Question> UnpublishedQuestions();
        IEnumerable<Question> PublishedQuestions();
        void SaveQuestions(Question q);
        void CreateQuestion(Question q);
        void DeleteQuestion(Question q);
        void Publish(Question q);
    }
}
