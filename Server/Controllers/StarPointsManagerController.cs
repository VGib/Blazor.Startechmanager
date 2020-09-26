using Microsoft.AspNetCore.Mvc;

namespace Blazor.Startechmanager.Server.Controllers
{
    [Route("{controller}/{startechType}/{action}/{userId:int?}")]
    public class StarPointsManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
