using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using log4net.Config;

namespace Giftee.Web
{
  public class MvcApplication : HttpApplication
  {
    #region Routing Constraints
    static readonly HttpMethodConstraint GET    = new HttpMethodConstraint("GET");
    static readonly HttpMethodConstraint PUT    = new HttpMethodConstraint("PUT");
    static readonly HttpMethodConstraint POST   = new HttpMethodConstraint("POST");
    static readonly HttpMethodConstraint DELETE = new HttpMethodConstraint("DELETE");
    #endregion

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      #region Registrations

      routes.MapRoute(null,"register",
                      new { controller  = "Registrations",
                            action      = "Create" },
                      new { httpMethod  = GET });
      routes.MapRoute(null,"register",
                      new { controller  = "Registrations",
                            action      = "Create" },
                      new { httpMethod  = POST });
      
      #endregion
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
      XmlConfigurator.Configure();

      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }
  }
}