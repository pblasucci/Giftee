namespace Giftee.Core

open System.Resources
open System.Reflection

module Resources =
    
  let resxMgr = ResourceManager("Giftee.Core",
                                Assembly.GetExecutingAssembly())

  let loremIpsum    = resxMgr.GetString("LoremIpsum")
  let registeredMsg = resxMgr.GetString("MailMsg_Registered")
  let registeredLbl = resxMgr.GetString("MailLbl_Registered")
  let pwdChangedMsg = resxMgr.GetString("MailMsg_PwdChanged")
  let pwdChangedLbl = resxMgr.GetString("MailLbl_PwdChanged")
