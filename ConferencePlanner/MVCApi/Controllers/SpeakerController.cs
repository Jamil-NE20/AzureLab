using MVCApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MVCApi.Controllers
{
    public class SpeakerController : Controller
    {
        // GET: Speaker
        public ActionResult Index()
        {
            IEnumerable<SpeakerViewModel> speakers = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63589/");
                //HTTP GET
                var responseTask = client.GetAsync("speaker");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SpeakerViewModel>>();
                    readTask.Wait();

                    speakers = readTask.Result;
                }
                else
                {


                    speakers = Enumerable.Empty<SpeakerViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(speakers);
        }

        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(SpeakerViewModel speaker)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63589/speaker");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<SpeakerViewModel>("speaker", speaker);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(speaker);
        }
    }
}
