using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class APIController : Controller
{

    private readonly ApplicationDbContext _context;

        public APIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("availableListings")]
        public async Task<List<ListingProjectsDTO>> GetListings()
        {
            
            var listings = from b in _context.ListingDTO_DBTable
                select new ListingProjectsDTO()
                {
                    ListingId = b.ListingId,
                    ListingName = b.ListingName
                };

            return listings.ToList();
            
        }
        [ProducesResponseType(typeof(List<ListingProjectsDTO>), 200)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id < 1)
                return BadRequest();
            var product = await _context.ListingDTO_DBTable.FirstOrDefaultAsync(m => m.ListingId == id);
            if (product == null)
                return NotFound();
            return Ok(product);

        }
        
        [ProducesResponseType(typeof(List<ListingProjectsDTO>), 200)]
        [HttpPost]
        public async Task<IActionResult> Post(ListingProjects listing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.Add(listing);
            await _context.SaveChangesAsync();
            _context.Entry(listing).Reference(x => x.ListingName).Load();

            var dto = new ListingProjectsDTO()
            {
                ListingId = listing.ListingId,
                ListingName = listing.ListingName

            };
            
            return CreatedAtRoute("DefaultApi", new { id = listing.ListingId }, dto);
        }
        
        [HttpPut]
        public async Task<IActionResult> Put(ListingProjects listingData)
        {
            if (listingData == null || listingData.ListingId == 0)
                return BadRequest();

            var listingTask = await _context.ListingDTO_DBTable.FindAsync(listingData.ListingId);
            if (listingTask == null)
                return NotFound();
            listingTask.ListingName = listingData.ListingName;
            //listingTask.ImageUrl = listingData.ImageUrl;
            //listingTask.Category = listingData.Category;
            //listingTask.Location = listingData.Location;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
                return BadRequest();
            var listingDel = await _context.ListingDTO_DBTable.FindAsync(id);
            if (listingDel == null) 
                return NotFound();
            _context.ListingDTO_DBTable.Remove(listingDel);
            await _context.SaveChangesAsync();
            return Ok();

        }
    
   
}