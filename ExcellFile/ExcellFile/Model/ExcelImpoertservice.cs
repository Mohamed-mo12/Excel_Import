using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExcellFile.Model;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExcellFile.Model
{
    public class ExcelImportService
    {
        private readonly ApplicationDbContext dbContext;

        public ExcelImportService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Employee>> ImportEmployeesAsync(Stream excelStream)
        {
            var employees = new List<Employee>();

            using (var package = new ExcelPackage(excelStream))
            {
                var Excelsheet = package.Workbook.Worksheets.First();
                var rowcount = Excelsheet.Dimension.Rows;

                for (int row = 2; row <= rowcount; row++)
                {
                    try
                    {
                        var company = Excelsheet.Cells[row, 1].Text;
                        if (!int.TryParse(Excelsheet.Cells[row, 2].Text, out int age))
                        {
                            continue; 
                        }
                        var address = Excelsheet.Cells[row, 3].Text;

                        var employee = new Employee
                        {
                            Company = company,
                            Age = age,
                            Address = address
                        };

                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message); 
                    }
                }
            }

            dbContext.Employees.AddRange(employees);
            await dbContext.SaveChangesAsync();

            return employees;
        }
    }
}
