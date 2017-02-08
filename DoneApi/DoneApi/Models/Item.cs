using System.ComponentModel.DataAnnotations;

namespace DoneApi.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [MustBeDone(ErrorMessage = "You may only add completed items!")]
        public bool Done { get; set; }
    }
}