﻿using Giftee.Web.Models;
using log4net;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using cmd = Giftee.Core.Commands;

namespace Giftee.Web.Models
{
  public class Registration
  {
    [Required,StringLength( 64)] public String FirstName    { get;set; }
    [Required,StringLength( 64)] public String LastName     { get;set; }
    [Required,StringLength(255)] public String Email        { get;set; }
    [Required,Compare("Email") ] public String EmailConfirm { get;set; }
  }
}

namespace Giftee.Web.Controllers
{
  public class RegistrationsController : Controller
  {
    static readonly ILog log = LogManager.GetLogger(typeof(Registration));

    [HttpGet]
    public ActionResult Create()
    {
      ModelState.Merge(TempData["modelState"] as ModelStateDictionary);

      return View("Register_Form");
    }

    [HttpPost]
    public ActionResult Create(Registration info)
    {
      if (ModelState.IsValid)
      {
        //MAYBE: double-check validation?
        try
        {
          cmd.Register(info.FirstName,info.LastName,info.Email);
          log.Info("Registered: {0} {1} ({2})",
                   info.FirstName,
                   info.LastName,
                   info.Email);
        }
        catch (Exception ex)
        {
          log.Warn(ex);
          ModelState.AddModelError("","Error processing registration.");
        }
      }

      if (!ModelState.IsValid)
      {
        TempData["modelState"] = ModelState;
        return RedirectToAction("Create");
      }

      return View("Register_Success",info);
    }
  }
}
