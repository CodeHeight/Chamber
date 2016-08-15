using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Chamber.Web.Controllers
{
    public class FileController : Controller
    {
        public FileResult DownloadForestry()
        {
            string file = HostingEnvironment.MapPath("~/App_Data/Files/ForestryTaxSeminarFlyer.pdf");
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(file));
        }

        public FileResult MembershipFormJune2016()
        {
            string file = HostingEnvironment.MapPath("~/App_Data/Files/membership.pdf");
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(file));
        }

        public FileResult PiedmontMay2016()
        {
            string file = HostingEnvironment.MapPath("~/App_Data/Files/Piedmont/NewsletterMAY2016.pdf");
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(file));
        }
    }
}