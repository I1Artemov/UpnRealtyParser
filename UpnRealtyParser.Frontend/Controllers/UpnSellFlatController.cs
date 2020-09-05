using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpnSellFlatController : Controller
    {
        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            List<UpnFlat> allSellFlats = new List<UpnFlat>();
            for (int i = 0; i < 50; i++)
            {
                allSellFlats.Add(new UpnFlat{Id = i, Description = "Квартира номер " + i});
            }

            List<UpnFlat> filteredFlats = allSellFlats
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new {flatsList = filteredFlats, totalCount = allSellFlats.Count});
        }
    }
}