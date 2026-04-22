using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;

namespace SmartWarehouse.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/appsettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppSetting>>> GetAppSettings()
        {
            return await _context.AppSettings.ToListAsync();
        }

        // GET: api/appsettings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AppSetting>> GetAppSetting(Guid id)
        {
            var appSetting = await _context.AppSettings.FindAsync(id);

            if (appSetting == null)
            {
                return NotFound();
            }

            return appSetting;
        }

        // PUT: api/appsettings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppSetting(Guid id, AppSetting appSetting)
        {
            if (id != appSetting.Id)
            {
                return BadRequest();
            }

            _context.Entry(appSetting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppSettingExists(id))
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

        // POST: api/appsettings
        [HttpPost]
        public async Task<ActionResult<AppSetting>> PostAppSetting(AppSetting appSetting)
        {
            _context.AppSettings.Add(appSetting);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppSetting", new { id = appSetting.Id }, appSetting);
        }

        // DELETE: api/appsettings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppSetting(Guid id)
        {
            var appSetting = await _context.AppSettings.FindAsync(id);
            if (appSetting == null)
            {
                return NotFound();
            }

            _context.AppSettings.Remove(appSetting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppSettingExists(Guid id)
        {
            return _context.AppSettings.Any(e => e.Id == id);
        }
    }
}
