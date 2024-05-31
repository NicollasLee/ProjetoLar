using Lar.Domain.Dto;
using Lar.Domain.Entities;
using Lar.Domain.Interface.Repositories;
using Lar.Domain.Interface.Services;
using Microsoft.Extensions.Logging;

namespace Lar.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonRepository personRepository, ILogger<PersonService> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task<Person> RegisterPerson(PersonDto person)
        {
            try
            {
                var newPerson = await _personRepository.RegisterPerson(person);

                // Log success
                _logger.LogInformation("New person registered successfully.");

                return newPerson;
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(ex, "Error occurred while registering a person.");
                throw;
            }
        }

        public async Task<Person> GetPerson(int id)
        {
            try
            {
                var person = await _personRepository.GetPerson(id);
                if (person == null)
                {
                    _logger.LogWarning($"Person with ID {id} not found.");
                    throw new KeyNotFoundException($"Person with ID {id} not found.");
                }
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching person with ID {id}.");
                throw;
            }
        }

        public async Task<Person> UpdatePerson(int id, PersonDto person)
        {
            try
            {
                var updatedPerson = await _personRepository.UpdatePerson(id, person);
                if (updatedPerson == null)
                {
                    _logger.LogWarning($"Person with ID {id} not found for update.");
                    throw new KeyNotFoundException($"Person with ID {id} not found for update.");
                }
                return updatedPerson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating person with ID {id}.");
                throw;
            }
        }

        public async Task<bool> DeletePerson(int id)
        {
            try
            {
                var success = await _personRepository.DeletePerson(id);
                if (!success)
                {
                    _logger.LogWarning($"Person with ID {id} not found for deletion.");
                    throw new KeyNotFoundException($"Person with ID {id} not found for deletion.");
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting person with ID {id}.");
                throw;
            }
        }
    }
}
