using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Giftee.Web.Models;
using log4net;
using cmd = Giftee.Core.Commands;

namespace Giftee.Web.Controllers
{
  [RequireHttps,Authorize(Roles="Admin")]
  public class GiftorsController : Controller
  {
    static readonly ILog log = LogManager.GetLogger(typeof(Giftor));

    [HttpGet]
    public ActionResult Gather()
    {
      ModelState.Merge(TempData["modelState"] as ModelStateDictionary);
      var items = cmd.GatherGiftors()
                     .Select(g => new Giftor {
                        ID        = g.ID, 
                        FirstName = g.FirstName,
                        LastName  = g.LastName,
                        GroupAs   = g.GroupAs.IfSome(v => v,() => "")
                     })
                     .ToList();
      return View("Giftor_List",items);
    }

    [HttpPost]
    public ActionResult Update(IList<Giftor> giftors)
    {
      try
      {
        //MAYBE: double-check validation?
        foreach(var g in giftors)
          cmd.UpdateGiftor(g.ID,g.GroupAs);
      }
      catch (Exception ex)
      {
        log.Warn(ex);
        ModelState.AddModelError("",ex.Message);
        TempData["modelState"] = ModelState;
      }
      return RedirectToAction("Gather","Giftors",new{ httpMethod="GET" });
    }

    //[HttpPut]
    //public ActionResult Update(Guid giftorID, Giftor info)
    //{
    //  try
    //  {
    //    //MAYBE: double-check validation?
    //    //cmd.UpdateGiftor(giftorID,groupAs);
    //    //throw new DivideByZeroException();
    //  }
    //  catch (Exception ex)
    //  {
    //    log.Warn(ex);
    //    ModelState.AddModelError("",ex.Message);
    //    TempData["modelState"] = ModelState;
    //  }
    //  return RedirectToAction("Gather","Giftors",new{ httpMethod="GET" });
    //}
  }
}
