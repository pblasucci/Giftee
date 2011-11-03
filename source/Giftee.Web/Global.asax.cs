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
    static readonly HttpMethodConstraintEx GET    = new HttpMethodConstraintEx("GET");
    static readonly HttpMethodConstraintEx PUT    = new HttpMethodConstraintEx("PUT");
    static readonly HttpMethodConstraintEx POST   = new HttpMethodConstraintEx("POST");
    static readonly HttpMethodConstraintEx DELETE = new HttpMethodConstraintEx("DELETE");
    #endregion

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      #region Registration

      routes.MapRoute(null,"register",
                      new { controller  = "Registrations",
                            action      = "Create" },
                      new { httpMethod  = GET });
      routes.MapRoute(null,"register",
                      new { controller  = "Registrations",
                            action      = "Create" },
                      new { httpMethod  = POST });
      
      #endregion

      #region Authentication

      routes.MapRoute(null,"enter",
                      new { controller  = "Sessions",
                            action      = "Create" },
                      new { httpMethod  = GET });
      routes.MapRoute(null,"sessions",
                      new { controller  = "Sessions",
                            action      = "Create" },
                      new { httpMethod  = POST });
      routes.MapRoute(null,"sessions/current",
                      new { controller  = "Sessions",
                            action      = "Delete" },
                      new { httpMethod  = DELETE });
      
      #endregion

      #region Password Management

      routes.MapRoute(null,"password",
                      new { controller  = "Passwords",
                            action      = "Create" },
                      new { httpMethod  = GET });
      routes.MapRoute(null,"password",
                      new { controller  = "Passwords",
                            action      = "Create" },
                      new { httpMethod  = POST });

      routes.MapRoute(null,"my/password",
                      new { controller  = "Passwords",
                            action      = "Update" },
                      new { httpMethod  = GET });
      routes.MapRoute(null,"my/password",
                      new { controller  = "Passwords",
                            action      = "Update" },
                      new { httpMethod  = POST });
      
      #endregion

      #region Wish Management
      
      routes.MapRoute(null,"my/wishes",
                      new { controller  = "Wishes",
                            action      = "Gather" },
                      new { httpMethod  = GET });

      routes.MapRoute(null,"my/wishes/new",
                      new { controller  = "Wishes",
                            action      = "Create" },
                      new { httpMethod  = GET });

      routes.MapRoute(null,"my/wishes",
                      new { controller  = "Wishes",
                            action      = "Create" },
                      new { httpMethod  = POST });

      routes.MapRoute(null,"my/wishes/{wishID}/form",
                      new { controller  = "Wishes",
                            action      = "Update" },
                      new { httpMethod  = GET });

      routes.MapRoute(null,"my/wishes/{wishID}",
                      new { controller  = "Wishes",
                            action      = "Update" },
                      new { httpMethod  = PUT });

      routes.MapRoute(null,"my/wishes/{wishID}",
                      new { controller  = "Wishes",
                            action      = "Delete" },
                      new { httpMethod  = DELETE });

      #endregion

      #region Administration

      routes.MapRoute(null,"admin",
                      new { controller  = "Giftors",
                            action      = "Gather" },
                      new { httpMethod  = GET });

      routes.MapRoute(null,"giftors",
                      new { controller  = "Giftors",
                            action      = "Update" },
                      new { httpMethod  = POST });

      routes.MapRoute(null,"exchanges",
                      new { controller  = "Exchanges",
                            action      = "Review" },
                      new { httpMethod  = GET });
      
      routes.MapRoute(null,"exchanges",
                      new { controller  = "Exchanges",
                            action      = "Commit" },
                      new { httpMethod  = POST });
      
      routes.MapRoute(null,"exchanges",
                      new { controller  = "Exchanges",
                            action      = "Reset" },
                      new { httpMethod  = DELETE });

      #endregion

      routes.MapRoute(null,"",
                      new { controller  = "Giftees",
                            action      = "Select" },
                      new { httpMethod  = GET });
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