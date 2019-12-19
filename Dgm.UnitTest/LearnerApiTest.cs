using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dgm.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

namespace Dgm.UnitTest
{
    public class LearnerApiTest
    {
        [Fact]
        public async Task should_return_bad_request_when_file_extensions_invalid()
        {
            using (var stream = File.OpenRead(Environment.CurrentDirectory + "/Test.xls"))
            {
                var file = new FormFile(stream, 0, stream.Length, null,
                    Path.GetFileName(Environment.CurrentDirectory + "/Test.xls"));
                var controller = new LearnerController();
                var result = await controller.ImportLearners(file);
                var returnBadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("File type not supported, please upload Excel Package", returnBadRequestResult.Value);
            }
        }
        
        [Fact]
        public async Task should_return_bad_request_when_file_invalid()
        {
            using (var stream = File.OpenRead(Environment.CurrentDirectory + "/Empty.xlsx"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(Environment.CurrentDirectory + "/Empty.xlsx"));
                var controller = new LearnerController();
                var result = await controller.ImportLearners(file);
                var returnBadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Invalid File", returnBadRequestResult.Value);
            }
        }
        
        [Fact]
        public async Task should_return_bad_request_when_duplicate_column()
        {
            using (var stream = File.OpenRead(Environment.CurrentDirectory + "/Duplicate.xlsx"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(Environment.CurrentDirectory + "/Duplicate.xlsx"));
                var controller = new LearnerController();
                var result = await controller.ImportLearners(file);
                var returnBadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Duplicate column name", returnBadRequestResult.Value);
            }
        }
        
        [Fact]
        public async Task should_return_exception_when_worksheet_invalid()
        {
            using (var stream = File.OpenRead(Environment.CurrentDirectory + "/Exception.xlsx"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(Environment.CurrentDirectory + "/Exception.xlsx"));
                var controller = new LearnerController();
                Task Result() => controller.ImportLearners(file);
                await Assert.ThrowsAsync<NullReferenceException>(Result);
            }
        }
        
        [Fact]
        public async Task should_import_learners()
        {
            using (var stream = File.OpenRead(Environment.CurrentDirectory + "/Sample.xlsx"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(Environment.CurrentDirectory + "/Sample.xlsx"));
                var controller = new LearnerController();
                var result = await controller.ImportLearners(file);
                var returnOkResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(9, returnOkResult.Value);
            }
        }
    }
}