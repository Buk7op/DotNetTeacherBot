using System.ComponentModel.DataAnnotations;

namespace DotNetTeacherBot.DTOs
{
    public class QuestionReadDto
    {
        public int ID { get; set; }
        [Required]
        public string ShortQuestion { get; set; }
        public string Description { get; set; }
        [Required]
        public string Answer { get; set; }
    }
}