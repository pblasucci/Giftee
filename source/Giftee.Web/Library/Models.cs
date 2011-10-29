using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Giftee.Web.Models
{
  public class Registration
  {
    [Required, StringLength(64)]
    public String FirstName { get; set; }
    [Required, StringLength(64)]
    public String LastName { get; set; }
    [Required, StringLength(255)]
    public String Email { get; set; }
    [Required, Compare("Email")]
    public String EmailConfirm { get; set; }
  }

  public class LogOn
  {
    [Required]
    public String Email { get; set; }
    [Required]
    public String Password { get; set; }
    public Boolean Persist { get; set; }
  }

  public class ForgotPassword
  {
    [Required]
    public String Email { get; set; }
  }

  public class PasswordSet
  {
    [Required]
    public String OldPassword { get; set; }
    [Required]
    public String NewPassword { get; set; }
    [Required,
     Compare("NewPassword")]
    public String NewPasswordConfirm { get; set; }
  }

  public class Wish
  {
    public Guid ID { get; set; }

    [Required]
    public Int32 Rank { get; set; }
    [Required]
    public String Summary { get; set; }
  }
}
