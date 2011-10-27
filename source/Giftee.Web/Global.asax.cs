using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Giftee.Web
{
  public class MvcApplication : HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
    }

    protected void Application_AuthenticateRequest(Object _, EventArgs e)
    {
      var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
      if(cookie != null)
      {
        var ticket    = FormsAuthentication.Decrypt(cookie.Value);
        Context.User  = new GifteePrincipal(new FormsIdentity(ticket));
      }
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }
  }
}