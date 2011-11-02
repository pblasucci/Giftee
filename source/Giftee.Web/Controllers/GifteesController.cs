using System.Web.Mvc;
using log4net;

namespace Giftee.Web.Controllers
{
  [Authorize]
  public class GifteesController : Controller
  {
    static readonly ILog log = LogManager.GetLogger("Giftee.Web.Models.Giftee");

    public GifteePrincipal CurrentUser 
    { 
      get { return (GifteePrincipal)User; } 
    }

    [HttpGet]
    public ActionResult Select()
    {
      ViewBag.DisplayName = CurrentUser.FullName;
      return View("GifteeDetails");
    }
  }
}
