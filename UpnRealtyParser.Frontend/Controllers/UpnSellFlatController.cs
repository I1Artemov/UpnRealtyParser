using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using UpnRealtyParser.Business.Contexts;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Repositories;

namespace UpnRealtyParser.Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpnSellFlatController : Controller
    {
        private readonly EFGenericRepo<UpnFlat, RealtyParserContext> _upnFlatRepo;

        public UpnSellFlatController(EFGenericRepo<UpnFlat, RealtyParserContext> upnFlatRepo)
        {
            _upnFlatRepo = upnFlatRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllFlats(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<UpnFlat> allSellFlats = _upnFlatRepo.GetAllWithoutTracking();
            int totalCount = allSellFlats.Count();

            List<UpnFlat> filteredFlats = allSellFlats
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new {flatsList = filteredFlats, totalCount = totalCount});
        }
    }
}