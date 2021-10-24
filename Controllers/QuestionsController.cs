using AutoMapper;
using DotNetTeacherBot.Data;
using DotNetTeacherBot.DTOs;
using DotNetTeacherBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTeacherBot.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepo _repo;
        private readonly IMapper _mapper;

        public QuestionsController(IQuestionRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost]
        public ActionResult<Question> CreateQuestion(QuestionCreateDto questionCreateDto)
        {
            var questionModel = _mapper.Map<Question>(questionCreateDto);
            _repo.CreateQuestion(questionModel);
            _repo.SaveQuestions();
            return CreatedAtRoute(nameof(GetQuestionById),new {Id = questionModel.ID}, questionModel);
        }

        [HttpGet("{id}", Name = "GetQuestionById")]
        public ActionResult<Question> GetQuestionById(int id)
        {
            var questionItem = _repo.GetQuestionById(id);
            if(questionItem != null)
            {
                return Ok(_mapper.Map<Question>(questionItem));
            }
            else
            {
                return NotFound();
            }
            
        }
    }
}