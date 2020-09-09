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
    public class UpnAgencyController : Controller
    {
        private readonly EFGenericRepo<UpnAgency, RealtyParserContext> _upnAgencyRepo;

        public UpnAgencyController(EFGenericRepo<UpnAgency, RealtyParserContext> upnAgencyRepo)
        {
            _upnAgencyRepo = upnAgencyRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllAgencies(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<UpnAgency> allAgencies = _upnAgencyRepo.GetAllWithoutTracking();
            int totalCount = allAgencies.Count();

            List<UpnAgency> filteredAgencies = allAgencies
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { agenciesList = filteredAgencies, totalCount = totalCount });
        }
    }
}