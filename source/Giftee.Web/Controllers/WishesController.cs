using Giftee.Web.Models;
using log4net;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using cmd = Giftee.Core.Commands;

namespace Giftee.Web.Controllers
{
  [Authorize]
  public class WishesController : Controller
  {
    static readonly ILog log = LogManager.GetLogger(typeof(Wish));

    public GifteePrincipal CurrentUser 
    { 
      get { return (GifteePrincipal)User; } 
    }

    [HttpGet]
    public ActionResult Gather()
    {
      var wishes = cmd.GatherWishes(CurrentUser.ID)
                      .Select(w => new Wish { ID      = w.ID, 
                                              Rank    = w.Rank, 
                                              Summary = w.Summary } );
      return View("Wish_List",wishes);
    }

    [HttpGet]
    public ActionResult Create()
    {
       ModelState.Merge(TempData["modelState"] as ModelStateDictionary);

      return View("Wish_Form");
    }

    [HttpPost]
    public ActionResult Create(Wish info)
    {
      if (ModelState.IsValid)
      {
        //MAYBE: double-check validation?
        try
        {
          cmd.InsertWish(CurrentUser.ID,info.Rank,info.Summary);
        }
        catch (Exception ex)
        {
          log.Warn(ex);
          ModelState.AddModelError("","Unable to create wish.");
        }
      }

      if (!ModelState.IsValid)
      {
        TempData["modelState"] = ModelState;
        return RedirectToAction("Create",new{httpMethod="GET"});
      }

      return RedirectToAction("Gather",new{httpMethod="GET"});
    }

    [HttpGet]
    public ActionResult Update(Guid wishID)
    {
      ModelState.Merge(TempData["modelState"] as ModelStateDictionary);
      var w = cmd.SelectWish(wishID);
      return View("Wish_Form",new Wish { ID      = w.ID,
                                         Rank    = w.Rank,
                                         Summary = w.Summary } );
    }

    [HttpPut]
    public ActionResult Update(Guid wishID,Wish info)
    {
      if (ModelState.IsValid)
      {
        //MAYBE: double-check validation?
        try
        {
          cmd.UpdateWish(wishID,info.Rank,info.Summary);
        }
        catch (Exception ex)
        {
          log.Warn(ex);
          ModelState.AddModelError("","Unable to update wish.");
        }
      }

      if (!ModelState.IsValid)
      {
        TempData["modelState"] = ModelState;
        return RedirectToAction("Update",new{httpMethod="GET"});
      }

      return RedirectToAction("Gather",new{httpMethod="GET"});
    }

    [HttpDelete]
    public ActionResult Delete(Guid wishID)
    {
      try
      {
        cmd.DeleteWish(wishID);
      }
      catch (Exception ex)
      {
        log.Warn(ex);
        TempData["error"] = "Unable to delete wish.";
      }
      
      return RedirectToAction("Gather",new{httpMethod="GET"});
    }

  }
}
