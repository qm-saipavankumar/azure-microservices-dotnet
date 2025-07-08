using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Wpm.Management.APi.DataAccess;

namespace Wpm.Management.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController(ManagementDbContext dbContext,ILogger<PetsController> logger) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pets = await dbContext.pets.Include(p => p.Breed).ToListAsync();
            return Ok(pets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id) 
        {
          var pet = await dbContext.pets.Include(p => p.Breed).FirstOrDefaultAsync(p => p.Id == id);
            return Ok(pet);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewPet newpet) 
        {
            try
            {
                var pet = newpet.ConvertToPet();
                await dbContext.pets.AddAsync(pet);
                await dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetByID), new { id = pet.Id }, pet);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new Pet.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        public record NewPet(string Name, int Age, int BreedId)
        {
            public Pet ConvertToPet() 
            {
                return new Pet { Name = Name, Age = Age, BreedId = BreedId };
            }
        
        }
    }
}
