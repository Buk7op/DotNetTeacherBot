using System.Linq;
using DotNetTeacherBot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetTeacherBot.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            AppDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if(!context.Questions.Any())
            {
                context.Questions.AddRange(
                    new Question {
                        ShortQuestion = "В чем разница между абстрактным классом и интерфейсом?",
                        Description = "Когда выгоднее использовать абстрактный класс, а когда интерфейс?",
                        Answer = "Абстрактный класс — это класс, у которого не реализован один или больше методов (некоторые языки требуют такие методы помечать специальными ключевыми словами). Интерфейс — это абстрактный класс, у которого ни один метод не реализован, все они публичные и нет переменных класса. Интерфейс нужен обычно когда описывается только контракт. Например, один класс хочет дать другому возможность доступа к некоторым своим методам, но не хочет себя «раскрывать». Поэтому он просто реализует интерфейс. Абстрактный класс нужен, когда нужно семейство классов, у которых есть много общего. Конечно, можно применить и интерфейс, но тогда нужно будет писать много идентичного кода."
                    }
                );  
            }
            context.SaveChanges();
        }
    }
}