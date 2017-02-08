using System.Linq;
using DoneApi.Models;

namespace DoneApi.Data
{
    public static class DoneDbInitialiser
    {
        public static void Initialise(DoneDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Items.Any())
            {
                return;
            }

            var items = new[]
            {
                new Item { Description = "Do this task." },
                new Item { Description = "Do this task too." },
                new Item { Description = "Do this task also." },
                new Item { Description = "You did this task." },
            };

            foreach (var item in items)
            {
                context.Items.Add(item);
            }

            context.SaveChanges();
        }
    }
}
