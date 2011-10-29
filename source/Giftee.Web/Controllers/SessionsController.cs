using Giftee.Web.Models;
using log4net;
using Microsoft.FSharp.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;

using cmd = Giftee.Core.Commands;
using userOption = Microsoft.FSharp.Core.FSharpOption<Giftee.Core.Models.User>;

namespace Giftee.Web.Controllers
{
  public class SessionsController : Controller
  {
    static readonly ILog log = LogManager.GetLogger(typeof(LogOn));

    [HttpGet]
    public ActionResult Create()
    {
      ModelState.Merge(TempData["modelState"] as ModelStateDictionary);

      return View("LogOn_Form");
    }

    [HttpPost]
    public ActionResult Create(LogOn info, String returnUrl)
    {
      var user = userOption.None;
      if (ModelState.IsValid)
      {
        //MAYBE: double-check validation?
        try
        {
          user = cmd.Authenticate(info.Email,info.Password);

          user.IfSome( u => log.Info("Log On Success: {0}",u.Email),
                      () => log.Warn("Log On Failure: {0}/'{1}'",
                                     info.Email,info.Password));

          if (OptionModule.IsNone(user))
            ModelState.AddModelError("","Invalid e-mail or password.");
        }
        catch (Exception ex)
        {
          log.Warn(ex);
          ModelState.AddModelError("","Invalid e-mail or password.");
        }
      }

      if (!ModelState.IsValid || OptionModule.IsNone(user))
      {
        TempData["modelState"] = ModelState;
        return RedirectToAction("Create",new {httpMethod = "GET"});
      }

      var defaultUrl = Url.Action("Select","Giftees",new{httpMethod="GET"});
      var cookie = GifteePrincipal.BuildAuthCookie(user.Value,info.Persist);
      Response.Cookies.Add(cookie);
      return Redirect(returnUrl ?? defaultUrl);
    }

    [HttpDelete,Authorize]
    public ActionResult Delete()
    {
      if (User.Identity.IsAuthenticated) 
      {
        FormsAuthentication.SignOut();
        return RedirectToAction("Create",new{httpMethod="GET"});
      }

      return new EmptyResult();
    }
  }
}
