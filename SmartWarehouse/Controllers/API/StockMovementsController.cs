using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;

namespace SmartWarehouse.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/stockmovements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockMovement>>> GetStockMovements()
        {
            return await _context.StockMovements
                .Include(s => s.Product)
                .ToListAsync();
        }

        // GET: api/stockmovements/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StockMovement>> GetStockMovement(Guid id)
        {
            var stockMovement = await _context.StockMovements
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockMovement == null)
            {
                return NotFound();
            }

            return stockMovement;
        }

        // PUT: api/stockmovements/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockMovement(Guid id, StockMovement stockMovement)
        {
            if (id != stockMovement.Id)
            {
                return BadRequest();
            }

            _context.Entry(stockMovement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockMovementExists(id))
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

        // POST: api/stockmovements
        [HttpPost]
        public async Task<ActionResult<StockMovement>> PostStockMovement(StockMovement stockMovement)
        {
            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStockMovement", new { id = stockMovement.Id }, stockMovement);
        }

        // DELETE: api/stockmovements/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockMovement(Guid id)
        {
            var stockMovement = await _context.StockMovements.FindAsync(id);
            if (stockMovement == null)
            {
                return NotFound();
            }

            _context.StockMovements.Remove(stockMovement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StockMovementExists(Guid id)
        {
            return _context.StockMovements.Any(e => e.Id == id);
        }
    }
}
