using log4net;
using System;
using System.Web.Mvc;

using cmd = Giftee.Core.Commands;

namespace Giftee.Web.Controllers
{
  [RequireHttps,Authorize]
  public class GifteesController : Controller
  {
    const String MODEL = "Giftee.Web.Models.Giftee";

    static readonly ILog log = LogManager.GetLogger(MODEL);

    public GifteePrincipal CurrentUser 
    { 
      get { return (GifteePrincipal)User; } 
    }

    [HttpGet]
    public ActionResult Select()
    {
      var giftee = cmd.SelectGiftee(CurrentUser.ID);
      ViewBag.DisplayName = CurrentUser.FullName;
      return View("GifteeDetails",giftee);
    }
  }
}
