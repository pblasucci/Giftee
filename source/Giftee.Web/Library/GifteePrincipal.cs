using Giftee.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using userOption = Microsoft.FSharp.Core.FSharpOption<Giftee.Core.Models.User>;

namespace Giftee.Web
{
  public class GifteePrincipal : GenericPrincipal
  {
    private readonly userOption _user;

    private static IEnumerable<String> toRoles(String userData)
    {
      var roles   = new List<String> { "User" };
      var isAdmin = Convert.ToBoolean(Int32.Parse(userData));
      if (isAdmin) roles.Add("Admin");
      return roles;
    }

    private static String toFlag(IEnumerable<String> roles)
    {
      return (roles.Any(r => r.IsEqualTo("Admin")) ? 1 : 0).ToString();
    }

    public GifteePrincipal(IIdentity ident) : base(ident,new []{ "User" })
    {
      var tempUser = userOption.None;
      var identity = base.Identity as FormsIdentity;
      if (identity != null)
      {
        var userData = identity.Ticket.UserData.Split('|');
        if (userData.Length == 4)
        {
          tempUser = userOption.Some(new User(
            /* ID       */ Guid.Parse(userData[0]),
            /* FirstNm  */ userData[1],
            /* LastNm   */ userData[2],
            /* Email    */ identity.Name,
            /* Roles    */ toRoles(userData[3])
          ));
        }
      }
      _user = tempUser;
    }

    public Guid   ID        {get{ return _user.IfSome(u => u.ID);        }}
    public String FirstName {get{ return _user.IfSome(u => u.FirstName); }}
    public String LastName  {get{ return _user.IfSome(u => u.LastName ); }}
    public String FullName  {get{ return FirstName +" "+ LastName;       }}
    public String Email     {get{ return Identity.Name;                  }}

    public override bool IsInRole(String role)
    {
      return _user.IfSome(u => u.Roles.Any(r => role.IsEqualTo(r)));
    }

    public static HttpCookie BuildAuthCookie(User info,Boolean persistent)
    {
      var userData = String.Format("{0}|{1}|{2}|{3}",
                                   info.ID,
                                   info.FirstName,
                                   info.LastName,
                                   toFlag(info.Roles));

      var cookie = FormsAuthentication.GetAuthCookie(info.Email,persistent);
      
      var oldTicket = FormsAuthentication.Decrypt(cookie.Value);
      var newTicket = new FormsAuthenticationTicket(oldTicket.Version,
                                                    oldTicket.Name,
                                                    oldTicket.IssueDate,
                                                    oldTicket.Expiration,
                                                    oldTicket.IsPersistent,
                                                    userData);
      
      cookie.Value = FormsAuthentication.Encrypt(newTicket);
      
      return cookie;
    }
  }
}
