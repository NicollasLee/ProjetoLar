namespace Lar.Domain.Dto
{
    public class PersonDto
    {
        public string Name { get; set; }
        public string CPF { get; set; }
        public DateTime DateBirth { get; set; }
        public bool Active { get; set; }
        public List<TelephonesDto> Telephones { get; set; }
    }
}
