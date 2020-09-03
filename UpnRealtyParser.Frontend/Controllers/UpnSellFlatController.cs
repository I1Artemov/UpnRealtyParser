using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpnSellFlatController : Controller
    {
        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats(int page)
        {
            List<UpnFlat> sellFlats = new List<UpnFlat> {
                new UpnFlat { Id = 15, Description = "First flat" },
                new UpnFlat { Id = 76, Description = "Second flat" }
            };
            return Json(sellFlats);
        }
    }
}