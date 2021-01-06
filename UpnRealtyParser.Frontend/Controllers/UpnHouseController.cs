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
        private readonly EFGenericRepo<HouseSitelessVM, RealtyParserContext> _unitedHouseRepo;

        public UpnHouseController(EFGenericRepo<HouseSitelessVM, RealtyParserContext> unitedHouseRepo)
        {
            _unitedHouseRepo = unitedHouseRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllHouses(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<HouseSitelessVM> allHouses = _unitedHouseRepo.GetAllWithoutTracking();
            int totalCount = allHouses.Count();

            List<HouseSitelessVM> filteredHouses = allHouses
                .OrderByDescending(x => x.SimilarIdentity)
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { housesList = filteredHouses, totalCount = totalCount });
        }
    }
}