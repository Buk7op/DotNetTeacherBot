using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetTeacherBot.Models;

namespace DotNetTeacherBot.Data
{
    public interface IQuestionRepo
    {
        IQueryable<Question> UnpublishedQuestions {get;}
        IQueryable<Question> PublishedQuestions {get;}
    
        void SaveQuestions(Question q);
        void CreateQuestion(Question q);
        void DeleteQuestion(Question q);
        void Publish(Question q);
    }
}
