using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Giftee.Web
{
  public class HttpMethodConstraintEx : HttpMethodConstraint
  {
    const String OVERRIDE = "X-HTTP-Method-Override";

    public HttpMethodConstraintEx(params String[] allowed) : base(allowed){}

    protected override Boolean Match(HttpContextBase      httpContext, 
                                     Route                route, 
                                     String               parameterName, 
                                     RouteValueDictionary values, 
                                     RouteDirection       routeDirection)
    {
      switch (routeDirection)
      {
        case RouteDirection.UrlGeneration:
          var verb = values.ContainsKey(parameterName)
                   ? values[parameterName].ToString()
                   : HttpVerbs.Get.ToString();
          return AllowedMethods.Any(v => v.IsEqualTo(verb));

        case RouteDirection.IncomingRequest:
          if (httpContext.Request.HttpMethod.IsEqualTo("POST") && 
              httpContext.Request.Params.AllKeys.Contains(OVERRIDE))
          {
            var fakeVerb = httpContext.Request.Params[OVERRIDE];
            return AllowedMethods.Any(v => v.IsEqualTo(fakeVerb));
          }
          
          return base.Match(httpContext,
                            route,
                            parameterName,
                            values,
                            routeDirection);
      }

      return false;
    }
  }
}