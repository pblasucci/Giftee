using Giftee.Web.Models;
using log4net;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using cmd = Giftee.Core.Commands;

namespace Giftee.Web.Controllers
{
  public class PasswordsController : Controller
  {
    static readonly ILog log = LogManager.GetLogger("Passwords");

    [HttpGet]
    public ActionResult Create()
    {
      ModelState.Merge(TempData["modelState"] as ModelStateDictionary);

      return View("ResetPwd_Form");
    }
    
    [HttpPost]
    public ActionResult Create(ForgotPassword info)
    {
      if (ModelState.IsValid)
      {
        //MAYBE: double-check validation?
        try
        {
          cmd.ResetPassword(info.Email);
        }
        catch (Exception ex)
        {
          log.Warn(ex);
          ModelState.AddModelError("","Unrecognized email address.");
        }
      }

      if (!ModelState.IsValid)
      {
        TempData["modelState"] = ModelState;
        return RedirectToAction("Create",new{httpMethod="GET"});
      }

      log.Info("Password Reset: {0})",User.Identity.Name);
      return View("ResetPwd_Success",User as GifteePrincipal);
    }

    [HttpGet,Authorize]
    public ActionResult Update()
    {
      ModelState.Merge(TempData["modelState"] as ModelStateDictionary);

      return View("ChangePwd_Form");
    }
    
    [HttpPost,Authorize]
    public ActionResult Update(PasswordSet info)
    {
      if (ModelState.IsValid)
      {
        //MAYBE: double-check validation?
        try
        {
          cmd.ChangePassword(User.Identity.Name,
                             info.OldPassword,
                             info.NewPassword);
        }
        catch (Exception ex)
        {
          log.Warn(ex);
          ModelState.AddModelError("","Error changing password.");
        }
      }

      if (!ModelState.IsValid)
      {
        TempData["modelState"] = ModelState;
      }
      else
      {
        log.Info("Password Changed: {0})",User.Identity.Name);
        TempData["flash"] = "Password successfully changed.";
      }
      return RedirectToAction("Update",new{httpMethod="GET"});
    }
  }
}
