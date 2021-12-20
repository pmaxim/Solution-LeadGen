using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public FilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("sounds/{name}")]
        public FileContentResult ReturnSounds(string name)
        {
            var path = $"{_env.ContentRootPath}\\App_Data\\sounds\\{name}";
            var myfile = System.IO.File.ReadAllBytes(path);
            return new FileContentResult(myfile, "application/json");
        }

        [HttpGet("streams/{name}")]
        public FileContentResult ReturnStream(string name)
        {
            var path = $"{_env.ContentRootPath}\\App_Data\\sounds\\streams\\{name}";
            var myfile = System.IO.File.ReadAllBytes(path);
            return new FileContentResult(myfile, "audio/mpeg");
        }
    }
}
