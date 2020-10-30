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
using System.Security.Cryptography.X509Certificates;

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

        public JsonResult GetLocatorDB(string cep)
        {
            var result = _context.Locator.Where(x => x.cep == cep).SingleOrDefault();

            if(result != null)
            {
                return Json(Url.Action("RetornoApi", "Locator", new Locator
                {
                    cep = result.cep,
                    complemento = result.complemento,
                    bairro = result.bairro,
                    localidade = result.localidade,
                    uf = result.uf,
                    ibge = result.ibge,
                    gia = result.gia,
                    ddd = result.ddd,
                    siafi = result.siafi
                }));
            }
            else
            {
                return Json(Url.Action("Error", "Home"));
            }
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
                        cep = locator.cep,
                        complemento = locator.complemento,
                        bairro = locator.bairro,
                        localidade = locator.localidade,
                        uf = locator.uf,
                        ibge = locator.ibge,
                        gia = locator.gia,
                        ddd = locator.ddd,
                        siafi = locator.siafi
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
                var verify = _context.Locator.Where(x => x.cep == locator.cep).Count();

                if(verify == 0)
                {
                    _context.Add(locator);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ConsultaCep));
                }
                else
                {
                    return View("Error", "Home");
                }
            }
            return View(locator);
        }

        private bool LocatorExists(int id)
        {
            return _context.Locator.Any(e => e.id == id);
        }
    }
}
