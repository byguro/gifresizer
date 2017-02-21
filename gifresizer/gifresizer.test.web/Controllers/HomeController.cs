using gifresizer.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gifresizer.test.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadFile()
        {
            HttpPostedFileBase myFile = Request.Files["fileInput"];

            var originalFilePath = Helper.BaseGifFileDirectory + "original/" + myFile.FileName;
            var resizeFilePath = Helper.BaseGifFileDirectory + "resize/" + myFile.FileName;
            var filePath = Server.MapPath("~" + originalFilePath);
            var resizePath = Server.MapPath("~" + resizeFilePath);
            

            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            if (System.IO.File.Exists(resizePath)) System.IO.File.Delete(resizePath);
            myFile.SaveAs(filePath);

            var width = Request.Form["inputWidth"].ToInt();
            var height = Request.Form["inputHeight"].ToInt();

            GifResize.Resize(filePath, width, height, resizePath);

            return Json(new { original = originalFilePath, resize = resizeFilePath }, JsonRequestBehavior.AllowGet);
        }
    }
}