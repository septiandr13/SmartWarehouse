using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;

namespace SmartWarehouse.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseBinsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarehouseBinsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/warehousebins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseBin>>> GetBins()
        {
            return await _context.Bins.ToListAsync();
        }

        // GET: api/warehousebins/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseBin>> GetWarehouseBin(Guid id)
        {
            var bin = await _context.Bins.FindAsync(id);

            if (bin == null)
            {
                return NotFound();
            }

            return bin;
        }

        // PUT: api/warehousebins/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouseBin(Guid id, WarehouseBin bin)
        {
            if (id != bin.Id)
            {
                return BadRequest();
            }

            _context.Entry(bin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseBinExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/warehousebins
        [HttpPost]
        public async Task<ActionResult<WarehouseBin>> PostWarehouseBin(WarehouseBin bin)
        {
            _context.Bins.Add(bin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarehouseBin", new { id = bin.Id }, bin);
        }

        // DELETE: api/warehousebins/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouseBin(Guid id)
        {
            var bin = await _context.Bins.FindAsync(id);
            if (bin == null)
            {
                return NotFound();
            }

            _context.Bins.Remove(bin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WarehouseBinExists(Guid id)
        {
            return _context.Bins.Any(e => e.Id == id);
        }
    }
}
