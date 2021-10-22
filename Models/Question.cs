using System.ComponentModel.DataAnnotations;

namespace DotNetTeacherBot.Models
{
    public class Question
    {
        public int ID { get; set; }
        [Required]
        public string ShortQuestion { get; set; }
        public string Description { get; set; }
        [Required]
        public string Answer { get; set; }
    }
}