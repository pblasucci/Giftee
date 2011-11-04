using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Giftee.Web.Models;
using log4net;

using cmd = Giftee.Core.Commands;

namespace Giftee.Web.Controllers
{
  [RequireHttps,Authorize(Roles="Admin")]
  public class ExchangesController : Controller
  {
    static readonly ILog log = LogManager.GetLogger(typeof(Exchange));

    [HttpGet]
    public ActionResult Review()
    {
      if (!ControllerContext.IsChildAction) 
        return new HttpStatusCodeResult(404);
      
      var info    = cmd.ReviewExchanges();
      var isFinal = info.Item1;
      var items   = info.Item2;

      var names = items.ToDictionary(g => g.ID,
                                     g => g.FirstName +" "+ g.LastName);
      
      var list = items.Select(g => new Exchange {
        GiftorID = g.ID,
        GiftorDisplayName = names[g.ID],
        GifteeID = g.GifteeID.Value,
        GifteeDisplayName = names[g.GifteeID.Value]
      }).ToList();

      ViewBag.IsFinal = isFinal;
      return PartialView("_Exchanges",list);
    }

    [HttpPost]
    public ActionResult Commit(IList<Exchange> info)
    {
      //MAYBE: add some exception handling?
      var data = info.Select(x => Tuple.Create(x.GiftorID,x.GifteeID));
      cmd.CommitExchanges(data);
      return RedirectToAction("Gather","Giftors",
                              new{ httpMethod = "GET" });
    }

    [HttpDelete]
    public ActionResult Delete()
    {
      //MAYBE: add some exception handling?
      cmd.DeleteExchanges();
      return RedirectToAction("Gather","Giftors",
                              new{ httpMethod = "GET" });
    }
  }
}
