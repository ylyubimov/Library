using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;
using System.Threading.Tasks;
using GhostscriptSharp;


namespace Library.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> fileUpload)
        {
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                string path = AppDomain.CurrentDomain.BaseDirectory + "Data/";
                string filename = System.IO.Path.GetFileName(file.FileName);
                if (filename != null) file.SaveAs(System.IO.Path.Combine(path, filename));
                GhostscriptSharp.GhostscriptWrapper.GeneratePageThumb(@"C:\Users\acerPC\Documents\GitHub\Library\Library\Data\test.pdf", "test.png", 1, 100, 100);
            }

            return RedirectToAction("Index");
        }
        public FileResult downloadFile()
        {
            return File("../Data/test.pdf", "application/pdf");
        }
	}
}