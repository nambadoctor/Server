using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.ServiceProvider
{
    public class OrganisationController : Controller
    {
        // GET: OrganisationController
        public ActionResult Index()
        {
            return View();
        }

        // GET: OrganisationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrganisationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrganisationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrganisationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrganisationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrganisationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrganisationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
