using System.Web.Mvc;

namespace Giftee.Web.Controllers
{
  public class GifteesController : Controller
  {
    [HttpGet,Authorize]
    public ActionResult Select()
    {
      return View("GifteeDetails");
    }
  }
}
