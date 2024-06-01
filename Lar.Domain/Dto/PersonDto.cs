using System.ComponentModel.DataAnnotations;

namespace Lar.Domain.Dto
{
    public class PersonDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"\d{11}", ErrorMessage = "CPF deve conter 11 dígitos.")]
        public string CPF { get; set; }

        [Required]
        public DateTime DateBirth { get; set; }
        public bool Active { get; set; }
        public List<TelephonesDto> Telephones { get; set; }
    }
}
