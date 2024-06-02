using ExcelDataReader;
using ExcellFile.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ExcellFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcellFileController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public ExcellFileController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("Import")]
        public IActionResult Import([FromForm] IFormFile file)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("File not imported");
                }

                var importPath = Directory.GetCurrentDirectory();
                if (!Directory.Exists(importPath))
                {
                    Directory.CreateDirectory(importPath);
                }

                var filepath = Path.Combine(importPath, file.FileName);
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                using (var stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        bool isHeaderSkipped = false;
                        do
                        {
                            while (reader.Read())
                            {
                                if (!isHeaderSkipped)
                                {
                                    isHeaderSkipped = true;
                                    continue;
                                }

                                Coustomer coustomer = new Coustomer();
                                try
                                {
                                    coustomer.Company = reader.GetValue(0)?.ToString();
                                    coustomer.Age = int.Parse(reader.GetValue(1)?.ToString() ?? "0");
                                    coustomer.Address = reader.GetValue(2)?.ToString();

                                    
                                    if (string.IsNullOrEmpty(coustomer.Company) || coustomer.Age <= 0 || string.IsNullOrEmpty(coustomer.Address))
                                    {
                                       
                                        continue;
                                    }

                                    dbContext.Coustomers.Add(coustomer);
                                    dbContext.SaveChanges();
                                }
                                catch (Exception rowEx)
                                {
                                    
                                    Console.WriteLine($"Error processing row: {rowEx.Message}");
                                    Console.WriteLine(rowEx.StackTrace);
                                    continue; 
                                }
                            }
                        } while (reader.NextResult());
                    }
                }

                return Ok("Saved");
            }
            catch (Exception ex)
            {
              
                var errorMessage = "An error occurred while saving the entity changes. See the inner exception for details.";
                var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
                var stackTrace = ex.StackTrace ?? "No stack trace";
                return StatusCode(500, new { Message = errorMessage, Details = innerExceptionMessage, StackTrace = stackTrace });
            }
        }
    }
}

