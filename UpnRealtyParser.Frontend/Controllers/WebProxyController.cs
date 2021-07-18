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
    public class WebProxyController : Controller
    {
        private readonly EFGenericRepo<WebProxyInfo, RealtyParserContext> _proxyRepo;

        public WebProxyController(EFGenericRepo<WebProxyInfo, RealtyParserContext> proxyRepo)
        {
            _proxyRepo = proxyRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllProxies(int? page, int? pageSize, string sortField, string sortOrder)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<WebProxyInfo> allProxies = _proxyRepo.GetAllWithoutTracking();
            int totalCount = allProxies.Count();

            allProxies = applyProxyFiltering(allProxies, sortField, sortOrder);

            List<WebProxyInfo> filteredProxies = allProxies
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { proxiesList = filteredProxies, totalCount = totalCount });
        }

        IQueryable<WebProxyInfo> applyProxyFiltering(IQueryable<WebProxyInfo> allProxies, string sortField, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
            {
                allProxies = allProxies.OrderBy(x => x.Id);
                return allProxies;
            }

            if (sortField == "id" && sortOrder == "descend") allProxies = allProxies.OrderByDescending(x => x.Id);
            else if (sortField == "id") allProxies = allProxies.OrderBy(x => x.Id);

            if (sortField == "creationDatePrintable" && sortOrder == "descend") allProxies = allProxies.OrderByDescending(x => x.CreationDateTime);
            else if (sortField == "creationDatePrintable") allProxies = allProxies.OrderBy(x => x.CreationDateTime);

            if (sortField == "successAmount" && sortOrder == "descend") allProxies = allProxies.OrderByDescending(x => x.SuccessAmount);
            else if (sortField == "successAmount") allProxies = allProxies.OrderBy(x => x.SuccessAmount);

            if (sortField == "failureAmount" && sortOrder == "descend") allProxies = allProxies.OrderByDescending(x => x.FailureAmount);
            else if (sortField == "failureAmount") allProxies = allProxies.OrderBy(x => x.FailureAmount);

            if (sortField == "lastSuccessDateTime" && sortOrder == "descend") allProxies = allProxies.OrderByDescending(x => x.LastSuccessDateTime);
            else if (sortField == "lastSuccessDateTime") allProxies = allProxies.OrderBy(x => x.LastSuccessDateTime);

            if (sortField == "lastUseDateTime" && sortOrder == "descend") allProxies = allProxies.OrderByDescending(x => x.LastUseDateTime);
            else if (sortField == "lastUseDateTime") allProxies = allProxies.OrderBy(x => x.LastUseDateTime);

            return allProxies;
        }
    }
}