using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace NZWalks.UI.Controllers
{
    public class RegionController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();
            // Get all regions from webapi
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.GetAsync("https://localhost:7153/api/Region");

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());

            }
            catch (Exception ex)
            {
                //Log exception
                throw;
            }

            return View(response);
        }

        [HttpGet]

        public IActionResult Add() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model) 
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7153/api/Region"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

			var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if(response != null) 
            {
                return RedirectToAction("Index", "Region");
            }

            return View();
		}

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id) 
        {
            var client = httpClientFactory.CreateClient();

            var ressponse = await client.GetFromJsonAsync<RegionDto>
                ($"https://localhost:7153/api/Region/{id.ToString()}");
            if (ressponse != null) 
            {
                return View(ressponse);
            }
       
          
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request) 
        {
            var client = httpClientFactory.CreateClient();

            var httprequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7153/api/Region/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

           var httpResponseMessage = await client.SendAsync(httprequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

			var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();
            if(response is not null) 
            {
                return RedirectToAction("Edit", "Region");
            }

            return View();
		}

        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request) 
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7153/api/Region/{request.Id}");
                httpResponseMessage.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Region");
            }
            catch (Exception ex)
            {

              // Console
            }

            return View("Edit");
        }
    }
}
