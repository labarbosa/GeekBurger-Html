using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using GeekBurger_HTML.Models;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using GeekBurger_HTML.Services;
using Microsoft.Extensions.Configuration;
using GeekBurger_HTML.Configuration;

namespace GeekBurger_HTML.Controllers
{
    public class HtmlController : Controller
    {
        private readonly UiApiConfiguration _uIApiConfiguration;
        private readonly IHostingEnvironment _env;
        private readonly IDebugService _debugService;
        public HtmlController(IHostingEnvironment env, IDebugService debugService)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _uIApiConfiguration = config.GetSection("UIApi").Get<UiApiConfiguration>();

            _env = env;
            _debugService = debugService;
        }

        public IActionResult Index()
        {
            ViewBag.FoodRestrictionsApi = _uIApiConfiguration.FoodRestrictionsApi;
            ViewBag.OrderApi = _uIApiConfiguration.OrderApi;
            ViewBag.OrderPayApi = _uIApiConfiguration.OrderPayApi;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Face(string image)
        {
            var imageArray = image.Split(',');

            if (imageArray.Length <= 1)
                return BadRequest();

            var webrootPath = _env.WebRootPath;
            var path = Path.Combine(webrootPath, "face.png");
            var face = Convert.FromBase64String(imageArray[1]);

            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    var data = face;
                    bw.Write(data);
                    bw.Close();
                }
            }

            //submit to UI service
            //var faceToPost = new FaceToPost() { Face = face };
            
            PostToApi(face, _uIApiConfiguration.FaceApi);

            return Ok();
        }

        private static async void PostToApi(dynamic data, string apiUrl)
        {
            var client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PostAsync(apiUrl, content);
            }

        }

        [HttpGet]
        public IActionResult Debug(string command)
        {
            var message = command?.Split('|');
            if (message?.Length > 1)
                _debugService.SendMessageAsync(message[0], message[1], message.Length > 2 ? message[2] : null).Wait();

            return Json("OK");
        }
    }
}
