using FluentValidation;
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
        private readonly IValidator<PersonDto> _validator;

        public PersonService(IPersonRepository personRepository, ILogger<PersonService> logger, IValidator<PersonDto> validator)
        {
            _personRepository = personRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Person> RegisterPerson(PersonDto person)
        {
            var validationResult = await _validator.ValidateAsync(person);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            return await _personRepository.RegisterPerson(person);
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
            var validationResult = await _validator.ValidateAsync(person);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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
