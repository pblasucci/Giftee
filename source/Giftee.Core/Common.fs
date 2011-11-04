namespace Giftee.Core

open System
open System.Resources
open System.Reflection

module Resources =
    
  let resxMgr = ResourceManager("Giftee.Core",
                                Assembly.GetExecutingAssembly())

  let loremIpsum    = resxMgr.GetString("LoremIpsum")

  module MailMsg =

    let registered  = resxMgr.GetString("MailMsg_Registered")
    let pwdChanged  = resxMgr.GetString("MailMsg_PwdChanged")
    let listChanged = resxMgr.GetString("MailMsg_WishlistChanged")
    
  module MailSubj =

    let registered  = resxMgr.GetString("MailLbl_Registered")
    let pwdChanged  = resxMgr.GetString("MailLbl_PwdChanged")
    let listChanged = resxMgr.GetString("MailLbl_WishlistChanged")
  
  module SQL =
    let allGiftors    = resxMgr.GetString("SQL_AllGiftors")
    let giftorByEmail = resxMgr.GetString("SQL_GiftorByEmail")
    let giftorWishes  = resxMgr.GetString("SQL_GiftorWishes")
    let deleteWish    = resxMgr.GetString("SQL_DeleteWishByID")
    let giftorGiftee  = resxMgr.GetString("SQL_GiftorGiftee")
