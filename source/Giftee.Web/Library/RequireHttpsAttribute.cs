using System;
using System.Web.Mvc;
using RequireHttpsAttributeBase = System.Web.Mvc.RequireHttpsAttribute;

namespace Giftee.Web
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
                  Inherited = true,
                  AllowMultiple = false)]
  public class RequireHttpsAttribute : RequireHttpsAttributeBase
  {
    public override void OnAuthorization(AuthorizationContext context)
    {
      if (context == null)
        throw new ArgumentNullException("filterContext");

      var req = context.HttpContext.Request;
      if (req.IsSecureConnection) return;
      if (req.Headers["X-Forwarded-Proto"].IsEqualTo("https")) return;
      if (req.IsLocal) return;

      HandleNonHttpsRequest(context);
    }
  }
}
