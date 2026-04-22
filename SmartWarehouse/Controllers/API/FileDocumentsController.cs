using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;

namespace SmartWarehouse.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileDocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FileDocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/filedocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileDocument>>> GetFileDocuments()
        {
            return await _context.FileDocuments.ToListAsync();
        }

        // GET: api/filedocuments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FileDocument>> GetFileDocument(Guid id)
        {
            var fileDocument = await _context.FileDocuments.FindAsync(id);

            if (fileDocument == null)
            {
                return NotFound();
            }

            return fileDocument;
        }

        // PUT: api/filedocuments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileDocument(Guid id, FileDocument fileDocument)
        {
            if (id != fileDocument.Id)
            {
                return BadRequest();
            }

            _context.Entry(fileDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileDocumentExists(id))
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

        // POST: api/filedocuments
        [HttpPost]
        public async Task<ActionResult<FileDocument>> PostFileDocument(FileDocument fileDocument)
        {
            _context.FileDocuments.Add(fileDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFileDocument", new { id = fileDocument.Id }, fileDocument);
        }

        // DELETE: api/filedocuments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileDocument(Guid id)
        {
            var fileDocument = await _context.FileDocuments.FindAsync(id);
            if (fileDocument == null)
            {
                return NotFound();
            }

            _context.FileDocuments.Remove(fileDocument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileDocumentExists(Guid id)
        {
            return _context.FileDocuments.Any(e => e.Id == id);
        }
    }
}
