using FluentValidation;
using Lar.Domain.Dto;
using Lar.Domain.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lar.WebApi.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly IPersonService _personService;

        public PersonController(ILogger<PersonController> logger, IPersonService personService)
        {
            _logger = logger;
            _personService = personService;
        }

        [HttpPost]
        public async Task<IActionResult> PersonRegistration(PersonDto person)
        {
            try
            {
                var newPerson = await _personService.RegisterPerson(person);
                _logger.LogInformation("New person registered successfully.");
                return Ok(newPerson);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering a person.");
                return BadRequest($"Error occurred while registering a person: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            try
            {
                var person = await _personService.GetPerson(id);
                if (person == null)
                    return NotFound();

                return Ok(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving a person.");
                return StatusCode(500, $"Error occurred while retrieving a person: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, PersonDto person)
        {
            try
            {
                var updatedPerson = await _personService.UpdatePerson(id, person);
                if (updatedPerson == null)
                    return NotFound();

                return Ok(updatedPerson);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating a person.");
                return StatusCode(500, $"Error occurred while updating a person: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                var result = await _personService.DeletePerson(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a person.");
                return StatusCode(500, $"Error occurred while deleting a person: {ex.Message}");
            }
        }
    }
}
