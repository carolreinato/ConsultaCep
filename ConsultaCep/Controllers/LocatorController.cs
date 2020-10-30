using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConsultaCep.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ConsultaCep.Controllers
{
    public class LocatorController : Controller
    {
        private readonly LocatorContext _context;

        private string Baseurl = "https://viacep.com.br/";

        public LocatorController(LocatorContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetLocator(string cep)
        {
            Locator locator = new Locator();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("ws/"+cep+"/json/");

                if (response.IsSuccessStatusCode)
                {
                    var locatorResponse = response.Content.ReadAsStringAsync().Result;
                    locator = JsonConvert.DeserializeObject<Locator>(locatorResponse);

                    return Json(Url.Action("RetornoApi", "Locator", new Locator {
                        Cep = locator.Cep,
                        Complemento = locator.Complemento,
                        Bairro = locator.Bairro,
                        Localidade = locator.Localidade,
                        Uf = locator.Uf,
                        Ibge = locator.Ibge,
                        Gia = locator.Gia,
                        Ddd = locator.Ddd,
                        Siafi = locator.Siafi
                    }));
                }
                else
                {
                    return Json(Url.Action("Error", "Home"));
                }
            }
        }

        // GET: Locator/Details/5
        public IActionResult RetornoApi(Locator locatorDetails)
        {
            if (locatorDetails == null)
            {
                return NotFound();
            }
            return View(locatorDetails);
        }

        public IActionResult ConsultaCep()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsereDB(Locator locator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locator);
        }

        private bool LocatorExists(int id)
        {
            return _context.Locator.Any(e => e.Id == id);
        }
    }
}
