using System.Web.Mvc;

namespace Chamber.Web.Controllers
{
    public class EventController : Controller
    {
        public ActionResult Calendar()
        {
            return View();
        }
    }
}