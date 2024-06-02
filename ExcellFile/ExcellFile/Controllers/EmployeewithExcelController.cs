using ExcellFile.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcellFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeewithExcelController : ControllerBase
    {
        private readonly ExcelImportService excel;
        public EmployeewithExcelController(ExcelImportService excel)
        {
            this.excel = excel;
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportEmployees(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var employees = await excel.ImportEmployeesAsync(stream);

                return Ok(new { count = employees.Count });
            }
        }
    }
}
