using AutoMapper;
using DotNetTeacherBot.DTOs;
using DotNetTeacherBot.Models;

namespace DotNetTeacherBot.Profiles
{
    public class QuestionsProfile : Profile
    {
        public QuestionsProfile()
        {
            // Source -> target
            CreateMap<QuestionCreateDto, Question>();
            CreateMap<Question,QuestionReadDto>();
        }
    }
}