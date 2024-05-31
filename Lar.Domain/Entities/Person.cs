using Lar.Domain.Dto;

namespace Lar.Domain.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public DateTime DateBirth { get; set; }
        public bool Active { get; set; }
        public List<TelephonesDto> Telephones { get; set; }
    }
}
