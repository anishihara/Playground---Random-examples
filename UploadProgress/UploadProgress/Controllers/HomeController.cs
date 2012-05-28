using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace UploadProgress.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Upload");
        }

        public ActionResult Upload()
        {
            return View();
        }

        /* IMPORTANTE SETAR A CONFIG ABAIXO NO WEB.CONFIG PARA QUE POSSA FAZER O UPLOAD
         * DE ARQUIVOS GRANDES NO IIS 7
      
        <system.webServer>
            <security>
                <requestFiltering>
                    <requestLimits maxAllowedContentLength="209715200"/>
                </requestFiltering>
            </security>
        </system.webServer>
        <system.web>
            <httpRuntime maxRequestLength="204800" executionTimeout="7200"/>
        
         * */

        [HttpPost]
        public ActionResult Upload(FormCollection form)
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Uploaded");
            string filename = String.Empty;
            string fileType;
            byte[] fileContents = new byte[0];

            if (Request.Files.Count > 0)
            {
                // Normal Request
                var file = Request.Files[0];
                fileContents = new byte[file.ContentLength];
                file.InputStream.Read(fileContents, 0, file.ContentLength);
                fileType = file.ContentType;
                filename = file.FileName;
            }
            else if (Request.ContentLength > 0)
            {
                // using FileAPI the content is in Request.InputStream!!!
                fileContents = new byte[Request.ContentLength];
                Request.InputStream.Read(fileContents, 0, Request.ContentLength);
                filename = Request.Headers["X-File-Name"];
                fileType = Request.Headers["X-File-Type"];
            }

            string filePath = Path.Combine(path, filename);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(fileContents, 0, fileContents.Length);
                fs.Close();
            }

            return Json("Uploaded");

        }
    }
}
