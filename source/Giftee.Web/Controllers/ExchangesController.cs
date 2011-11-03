using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Giftee.Web.Models;
using log4net;

namespace Giftee.Web.Controllers
{
  [RequireHttps,Authorize(Roles="Admin")]
  public class ExchangesController : Controller
  {
    static readonly ILog log = LogManager.GetLogger(typeof(Exchange));

    [HttpGet]
    public ActionResult Review()
    {
    }

    [HttpPost]
    public ActionResult Commit(IList<Exchange> mappings)
    {
    }

    [HttpDelete]
    public ActionResult Reset()
    {
    }
  }
}
