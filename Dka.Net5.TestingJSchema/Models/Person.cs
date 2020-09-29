using System.ComponentModel.DataAnnotations;

namespace Dka.Net5.TestingJSchema.Models
{
    public class Person
    {
        [Required]
        [Display(Name = "First name", Description = "Fill your first name")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last name", Description = "Fill your last name")]
        public string LastName { get; set; }
    }
}