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
    public class LogEntryController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly EFGenericRepo<ParsingState, RealtyParserContext> _logRepo;

        public LogEntryController(EFGenericRepo<ParsingState, RealtyParserContext> logRepo)
        {
            _logRepo = logRepo;
        }

        [Route("getall")]
        [HttpGet]
        public IActionResult GetAllLogEntries(int? page, int? pageSize)
        {
            int targetPage = page.GetValueOrDefault(1);
            int targetPageSize = pageSize.GetValueOrDefault(10);

            IQueryable<ParsingState> allEntries = _logRepo.GetAllWithoutTracking();
            int totalCount = allEntries.Count();

            List<ParsingState> filteredEntries = allEntries
                .Skip((targetPage - 1) * targetPageSize)
                .Take(targetPageSize).ToList();

            return Json(new { logEntriesList = filteredEntries, totalCount = totalCount });
        }
    }
}