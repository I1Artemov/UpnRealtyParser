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
    public class UpnHouseController : Controller
    {
        private readonly EFGenericRepo<UpnHouseInfo, RealtyParserContext> _upnHouseRepo;

        public UpnHouseController(EFGenericRepo<UpnHouseInfo, RealtyParserContext> upnHouseRepo)
        {
            _upnHouseRepo = upnHouseRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllHouses(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<UpnHouseInfo> allHouses = _upnHouseRepo.GetAllWithoutTracking();
            int totalCount = allHouses.Count();

            List<UpnHouseInfo> filteredHouses = allHouses
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { housesList = filteredHouses, totalCount = totalCount });
        }
    }
}