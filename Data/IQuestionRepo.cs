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

        IQueryable<Question> Questions {get;}
    
        void SaveQuestions();
        void CreateQuestion(Question q);
        void DeleteQuestion(Question q);
        void ChangePublish(Question q);

        Question GetQuestionById(int id);
    }
}
