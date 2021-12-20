using System.Net;
using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = $"{Constants.RoleAdmin},{Constants.RoleAgent}")]
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly ILogger<UploadController> _logger;
        private readonly IWebHostEnvironment _env;

        public UploadController(IUploadService uploadService,
            ILogger<UploadController> logger,
            IWebHostEnvironment env)
        {
            _uploadService = uploadService;
            _logger = logger;
            _env = env;
        }

        [HttpPost("[action]")]
        public async Task<int> UploadPhones(List<string> phones)
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return 0;
            return await _uploadService.UploadPhones(phones, userName);
        }

        [HttpGet("[action]")]
        public async Task<TablePhonesViewModel> GetTablePhones()
        {
            var model = new TablePhonesViewModel();
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return model;

            var role = string.Empty;
            if (User.IsInRole(Constants.RoleAdmin)) role = Constants.RoleAdmin;
            return await _uploadService.GetTablePhones(userName, role);
        }

        [HttpGet("[action]")]
        public async Task<bool> RemovePhones(bool isCall)
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return false;

            await _uploadService.RemovePhones(userName, isCall);
            return true;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<IList<UploadResult>>> UploadVonageKey(string appId, [FromForm] IEnumerable<IFormFile> files)
        {
            const int maxAllowedFiles = 3;
            const long maxFileSize = 1024 * 1024 * 15;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResult> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay =
                    WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        _logger.LogInformation("{FileName} length is 0 (Err: 1)",
                            trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                    }
                    else if (file.Length > maxFileSize)
                    {
                        _logger.LogInformation("{FileName} of {Length} bytes is " +
                            "larger than the limit of {Limit} bytes (Err: 2)",
                            trustedFileNameForDisplay, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                    }
                    else
                    {
                        try
                        {
                            var path = Path.Combine(_env.ContentRootPath, $"App_Data\\VonageKey\\{appId}");

                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                            path = Path.Combine(path, "private.key");
                            await using FileStream fs = new(path, FileMode.Create);
                            await file.CopyToAsync(fs);

                            _logger.LogInformation("{FileName} saved at {Path}",
                                trustedFileNameForDisplay, path);
                            uploadResult.Uploaded = true;
                        }
                        catch (IOException ex)
                        {
                            _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    _logger.LogInformation("{FileName} not uploaded because the " +
                        "request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(resourcePath, uploadResults);
        }
    }
}
