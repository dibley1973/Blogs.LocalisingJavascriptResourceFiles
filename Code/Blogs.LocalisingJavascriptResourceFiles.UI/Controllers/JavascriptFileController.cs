using Blogs.LocalisingJavascriptResourceFiles.UI.Constants;
using Blogs.LocalisingJavascriptResourceFiles.UI.Helpers;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;

namespace Blogs.LocalisingJavascriptResourceFiles.UI.Controllers
{
    public class JavascriptFileController : Controller
    {
        // GET: JavascriptResourceFile
        [HttpGet]
        //[OutputCache(Duration = Duration.InSeconds.OneHour, Location=OutputCacheLocation.Client, NoStore=true)]    // PROD ONLY
        //[OutputCache(Duration = Duration.InSeconds.TenSeconds, Location=OutputCacheLocation.Client, NoStore=true)] // TEST  ONLY
        [OutputCache(Duration = Duration.InSeconds.OneSecond, Location = OutputCacheLocation.Client, NoStore = true)]  // DEV  ONLY
        public ActionResult CulturisedResourceFile(string controllerName, string viewName)
        {
            if (string.IsNullOrWhiteSpace(controllerName)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "controllerName not supplied");
            if (string.IsNullOrWhiteSpace(viewName)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "viewName not supplied");

            const string baseScriptFolderPath = @"\Scripts";
            try
            {
                var originalFileContent = new JavascriptResourceFileContentLoader(
                    baseScriptFolderPath: baseScriptFolderPath,
                    controllerName: controllerName,
                    viewName: viewName)
                    .Load()
                    .Output;

                var culturisedFileContent = new JavascriptResourceFileContentCulturiser(
                    content: originalFileContent,
                    controllerName: controllerName,
                    viewName: viewName)
                    .Culturise()
                    .Output;

                return JavaScript(culturisedFileContent);
            }
            catch (FileNotFoundException)
            {
                return new HttpNotFoundResult("The requested resource was not found on the server. ");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "The requested resource causes an error on the server. ");
            }
        }
    }
}