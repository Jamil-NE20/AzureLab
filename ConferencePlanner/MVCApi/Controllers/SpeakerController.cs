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

        public ActionResult Index()
        {
            return View();
        }

        // GET: Speaker
        public ActionResult viewData()
        {
            IEnumerable<SpeakerViewModel> speakers = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44388/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Speakers");
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
                client.BaseAddress = new Uri("https://localhost:44388/api/Speakers");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<SpeakerViewModel>("Speakers", speaker);
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
