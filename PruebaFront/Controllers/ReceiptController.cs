using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PruebaFront.Models.Receipt;
using PruebaFront.Services;

namespace PruebaFront.Controllers
{
    [Authorize]
    public class ReceiptController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        public ReceiptController(IConfiguration iConfig, IHttpContextAccessor httpContextAccessor)
        {
            _config = iConfig;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            List<Receipt> receipts = new List<Receipt>();
            ReceiptService service = new ReceiptService(_config, _httpContextAccessor);
            var AllReceipts = await service.GetAll();

            ViewBag.YourEnums = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(Enum.GetValues(typeof(Currency)));
            return View(AllReceipts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Receipt receipt)
        {
            ReceiptService service = new ReceiptService(_config, _httpContextAccessor);
            await service.CreateReceipt(receipt);

            var AllReceipts = await service.GetAll();
            ViewBag.YourEnums = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(Enum.GetValues(typeof(Currency)));

            return View("Index", AllReceipts);
        }

        public async Task<IActionResult> Update(Receipt receipt)
        {
            ReceiptService service = new ReceiptService(_config, _httpContextAccessor);
            await service.Update(receipt);


            var AllReceipts = await service.GetAll();
            ViewBag.YourEnums = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(Enum.GetValues(typeof(Currency)));

            return View("Index", AllReceipts);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            ReceiptService service = new ReceiptService(_config, _httpContextAccessor);
            await service.Delete(Id);

            var AllReceipts = await service.GetAll();
            ViewBag.YourEnums = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(Enum.GetValues(typeof(Currency)));

            return View("Index", AllReceipts);
        }

    }
}