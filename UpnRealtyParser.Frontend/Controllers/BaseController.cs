using Microsoft.AspNetCore.Mvc;

namespace UpnRealtyParser.Frontend.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult makeErrorResult(string message)
        {
            return Json(new { error = message });
        }
    }
}