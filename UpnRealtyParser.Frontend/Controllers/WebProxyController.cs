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
        public IActionResult GetAllProxies(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<WebProxyInfo> allProxies = _proxyRepo.GetAllWithoutTracking();
            int totalCount = allProxies.Count();

            List<WebProxyInfo> filteredProxies = allProxies
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { proxiesList = filteredProxies, totalCount = totalCount });
        }
    }
}