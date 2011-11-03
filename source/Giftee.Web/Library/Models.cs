using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Giftee.Web.Models
{
  public class Registration
  {
    [Required, 
     StringLength(64), 
     Display(Name="First Name")]
    public String FirstName { get; set; }

    [Required, 
     StringLength(64), 
     Display(Name="Last Name")]
    public String LastName { get; set; }
    
    [Required, 
     StringLength(255), 
     Display(Name="E-Mail")]
    public String Email { get; set; }
    
    [Required, 
     Compare("Email",
             ErrorMessage="This does not match the E-Mail field."), 
     Display(Name="E-Mail (again)")]
    public String EmailConfirm { get; set; }
  }

  public class LogOn
  {
    [Required,
     Display(Name="E-Mail")]
    public String Email { get; set; }
    
    [Required,
     Display(Name="Password")]
    public String Password { get; set; }
    
    [Display(Name="Remember Me")]
    public Boolean Persist { get; set; }
  }

  public class ForgotPassword
  {
    [Required,
     Display(Name="E-Mail")]
    public String Email { get; set; }
  }

  public class PasswordSet
  {
    [Required,
     Display(Name="Current Password")]
    public String OldPassword { get; set; }
    
    [Required,
     Display(Name="Desired Password")]
    public String NewPassword { get; set; }
    
    [Required,
     Compare("NewPassword",
             ErrorMessage="This does not match the Desired Password field."),
     Display(Name="Desired Password (again)")]
    public String NewPasswordConfirm { get; set; }
  }

  public class Wish
  {
    public Guid ID { get; set; }
    
    [Required] public Int32 Rank     { get; set; }
    [Required] public String Summary { get; set; }
  }

  public class Giftor
  {
    [Required] 
    public Guid ID { get; set; }

    public String FirstName { get; set; }
    public String LastName  { get; set; }
    
    [Required, 
     StringLength(64), 
     Display(Name="Group As")]
    public String GroupAs { get; set; }
  }

  public class Exchange
  {
    [Required,StringLength(64)] public String GiftorFirstName { get;set; }
    [Required,StringLength(64)] public String GiftorLastName  { get;set; }
    [Required,StringLength(64)] public String GifteeFirstName { get;set; }
    [Required,StringLength(64)] public String GifteeLastName  { get;set; }
  }
}
