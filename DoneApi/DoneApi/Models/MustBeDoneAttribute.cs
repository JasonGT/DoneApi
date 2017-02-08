using System.ComponentModel.DataAnnotations;

namespace DoneApi.Models
{
    public class MustBeDoneAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return (bool)value;
        }
    }
}
