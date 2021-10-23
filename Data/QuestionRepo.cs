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

        public void Publish(Question q)
        {
            q.Published = true;
            _context.SaveChanges();
        }

        public IEnumerable<Question> PublishedQuestions()
        {
            return _context.Questions.Where(q => q.Published == true).ToList();
        }

        public void SaveQuestions(Question q)
        {
            _context.SaveChanges();
        }

        public IEnumerable<Question> UnpublishedQuestions()
        {
            return _context.Questions.Where(q => q.Published == false).ToList();
        }
    }
}