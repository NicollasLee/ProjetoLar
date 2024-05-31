using Lar.Domain.Dto;
using Lar.Domain.Entities;

namespace Lar.Domain.Interface.Repositories
{
    public interface IPersonRepository
    {
        public Task<Person> RegisterPerson(PersonDto person);
        public Task<Person> GetPerson(int id);
        public Task<Person> UpdatePerson(int id, PersonDto person);
        public Task<bool> DeletePerson(int id);
    }
}
