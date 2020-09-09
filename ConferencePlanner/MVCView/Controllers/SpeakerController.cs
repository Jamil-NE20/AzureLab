using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCView.Models;

namespace MVCView.Controllers
{
    public class SpeakerController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<Speaker> speakers = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:44388/api/");
                //HTTP GET
                var responseTask = client.GetAsync("speaker");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Speaker>>();
                    readTask.Wait();

                    speakers = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    speakers = Enumerable.Empty<Speaker>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(speakers);
        }
    }
}
