using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantApi3.Controllers;

[ApiController]
[Route("files")]
// [Authorize]
public class FileController: ControllerBase
{
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 1200, VaryByQueryKeys = new []{"filename"})]
    public ActionResult GetFile([FromQuery] string filename)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var filePath = $"{rootPath}\\PrivateFiles\\{filename}";

        var filesExists = System.IO.File.Exists(filePath);
        _logger.LogWarning($"{AppConstants.LoggerPrefix}  file directory: {filePath}, fileExists: {filesExists}");

        if (!filesExists)
        {
            return NotFound();
        }

        var fileContent = System.IO.File.ReadAllBytes(filePath);
        var fileContentTypeProvider = new FileExtensionContentTypeProvider();
        string fileContentType;
        var contentTypeExists = fileContentTypeProvider.TryGetContentType(filePath, out fileContentType);
        if (!contentTypeExists)
        {
            return NotFound();
        }
        
        _logger.LogWarning($"{AppConstants.LoggerPrefix}  file directory: {filePath}, fileExists: {filesExists}, fileType: {fileContentType}");

        return File(fileContent, fileContentType, filename);
    }

    [HttpPost]
    public ActionResult UploadFile([FromForm] IFormFile file)
    {
        if (file is null || file.Length <= 0)
        {
            return BadRequest();
        }

        var rootPath = Directory.GetCurrentDirectory();
        var fileName = file.FileName;
        var fullPath = $"{rootPath}/PrivateFiles/{fileName}";
        // using (var stream = new FileStream(fullPath, FileMode.Create))
        // {
        //     file.CopyTo(stream);
        // } do not know why in tutorial using statement is used

        var stream = new FileStream(fullPath, FileMode.Create);
        file.CopyTo(stream);

        return Ok();

    }
    
}