using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Wpm.Management.APi.DataAccess;

namespace Wpm.Management.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreedsController(ManagementDbContext dbContext,ILogger<BreedsController> logger) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var breeds = await dbContext.breeds.ToListAsync();
            if (breeds == null)
                return NotFound();
            return Ok(breeds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) 
        {
            var breed = await dbContext.breeds.FirstOrDefaultAsync(x => x.Id == id);
            if (breed == null)
                return NotFound();
            return Ok(breed);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewBreed newBreed) 
        {
            try
            {
                var breed = newBreed.ConvertToBreed();
                await dbContext.breeds.AddAsync(breed);
                await dbContext.SaveChangesAsync();
                return CreatedAtRoute(nameof(GetById), new { Id = breed.Id }, breed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new breed.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

   
    }

    public record NewBreed(string name)
    {
        public Breed ConvertToBreed()
        {
            return new Breed(0,name);
        }
    }
}
