using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using System.IO;

namespace Capstone_CodeComparison.Controllers
{
    public class FileProcessingController : Controller
    {
        // GET: FileProcessing
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        var fileName = Path.GetFileName(file);
                        
                        byte[] data = new byte[stream.Length];
                        int br = stream.Read(data, 0, data.Length);


                        Session["FileContent"] = data;
                        Session["FileContentName"] = fileName;

                        //https://stackoverflow.com/questions/31914568/save-an-attachment-in-session

                        return Json("File uploaded successfully", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed", JsonRequestBehavior.AllowGet);
            }

            return Json("exiting upload", JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult Download()
        {
            if (Session["FileContent"] != null)
            {
                return File(Session["FileContent"] as byte[], System.Net.Mime.MediaTypeNames.Application.Octet, Session["FileContentName"].ToString());
            }
            else
            {
                return null;
            }
        }

    }
}