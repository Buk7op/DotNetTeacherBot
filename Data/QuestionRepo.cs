using System;
using System.Collections.Generic;
using System.Linq;
using DotNetTeacherBot.Models;

namespace DotNetTeacherBot.Data
{
    public class QuestionRepo : IQuestionRepo
    {
        private readonly AppDbContext _context;

        public QuestionRepo(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Question> UnpublishedQuestions => _context.Questions.Where(q => q.Published == false);

        public IQueryable<Question> PublishedQuestions => _context.Questions.Where(q => q.Published == true);

        public IQueryable<Question> Questions => _context.Questions;

        public void CreateQuestion(Question q)
        {
            if(q == null)
            {
                throw new ArgumentNullException(nameof(q));
            }
            _context.Add(q);
            _context.SaveChanges();
        }

        public void DeleteQuestion(Question q)
        {
            _context.Remove(q);
            _context.SaveChanges();
        }

        public void ChangePublish(Question q)
        {
            q.Published = !q.Published;
            _context.SaveChanges();
        }

        
        public void SaveQuestions()
        {
            _context.SaveChanges();
        }

        public Question GetQuestionById(int id)
        {
            return _context.Questions.FirstOrDefault(q => q.ID == id);
        }
    }
}