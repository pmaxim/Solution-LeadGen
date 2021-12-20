using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    public class ServerViewController : Controller
    {
        //https://localhost:7104/server-view/?subjectId=1
        [Route("server-view")]
        public IActionResult Index(string subjectId)
        {
            return View("Index", subjectId);
        }
    }
}
