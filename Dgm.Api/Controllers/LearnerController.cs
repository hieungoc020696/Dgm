using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dgm.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;


namespace Dgm.Api.Controllers
{
    [Route("api/learner")]
    public class LearnerController : ControllerBase
    {
        public async Task<IActionResult> ImportLearners(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToUpper();
            if (fileExtension != ".XLSX")
            {
                return BadRequest("File type not supported, please upload Excel Package");
            }

            if (file.Length <= 0)
            {
                return BadRequest("Invalid File");
            }
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();
                var rowStart = worksheet.Dimension.Start.Row;
                var rowEnd = worksheet.Dimension.End.Row;
                var columnEnd = worksheet.Dimension.End.Column;
                var columns = new List<string>();

                for (int col = 1; col <= columnEnd; col++)
                {
                    var columnName = Convert.ToString(worksheet.Cells[rowStart, col].Value);
                    if (columns.Contains(columnName))
                    {
                        return BadRequest("Duplicate column name");
                    }
                    columns.Add(columnName);
                }
                var dataLearners = new List<LearnerDto>();
                for (int row = rowStart + 1; row <= rowEnd; row++)
                {
                    var learner = new LearnerDto()
                    {
                        Email = worksheet.Cells[row, 1].Value.ToString(),
                        FullName = worksheet.Cells[row, 1].Value.ToString(),
                        Password = worksheet.Cells[row, 1].Value.ToString()
                    };
                    dataLearners.Add(learner);
                }
                return Ok(dataLearners.Count);
            }
        }
    }
}