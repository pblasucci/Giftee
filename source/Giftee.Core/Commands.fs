namespace Giftee.Core

open Giftee.Core.Models
open Giftee.Core.Security
open System
open System.Net.Mail
open Soma.Core

module Db = Giftee.Core.Database
module R  = Giftee.Core.Resources

module Commands =
  
  let validate name value =
    if value |> String.IsNullOrWhiteSpace then 
      invalidArg name (sprintf "The %s field is required." name)

  [<CompiledName("Register")>]
  let register firstName lastName email =
    
    firstName |> validate "First Name"
    lastName  |> validate "Last Name"
    
    let email'    = MailAddress email
    let password  = generatePhrase 4
    let encrypted = encrypt email'.User password
    
    use tx = new System.Transactions.TransactionScope()

    { Giftor.Empty with
        ID        = Guid.NewGuid()
        FirstName = firstName
        LastName  = lastName
        Email     = email'.Address
        Password  = encrypted } 
    |> Database.insert 
    |> ignore
    
    Mail.sendRegistered email password
    
    tx.Complete()

  [<CompiledName("ResetPassword")>]
  let resetPassword email =

    let email'    = MailAddress email
    let password  = generatePhrase 4
    let encrypted = encrypt email'.User password
    
    use tx = new System.Transactions.TransactionScope()

    let giftors = Db.query<Giftor> R.SQL.giftorByEmail ["email" @= email]
    match giftors with
    | g :: [] -> Db.update {g with Password = encrypted} |> ignore
    | _       -> failwithf "Unable to determine account for '%s'" email
                 //LANG

    Mail.sendPasswordReset email password
    
    tx.Complete()

  [<CompiledName("Authenticate")>]
  let authenticate email password =
    
    password  |> validate "Password"
    email     |> validate "Email"
  
    let given   = (encrypt (MailAddress email).User password)
    let giftors = Db.query<Giftor> R.SQL.giftorByEmail ["email" @= email]

    match giftors with
    | actual :: []  ->  if given = actual.Password
                          then Some(User.OfGiftor actual)
                          else None
    | _             ->  None

  [<CompiledName("ChangePassword")>]
  let changePassword email oldPassword newPassword =
    
    let email'    = MailAddress email
    let encrypted = encrypt email'.User newPassword
    
    use tx = new System.Transactions.TransactionScope()

    let giftors = Db.query<Giftor> R.SQL.giftorByEmail ["email" @= email]
    match giftors with
    | g :: [] ->  if g.Password <> (encrypt email'.User oldPassword)
                    then failwithf "Invalid password for '%s'" email  
                  Db.update {g with Password = encrypted} |> ignore
    | _       ->  failwithf "Unable to determine account for '%s'" email
                  //LANG

    tx.Complete()

  [<CompiledName("GatherWishes")>]
  let gatherWishes (giftorID:Guid) =
    Db.query<Wish> R.SQL.giftorWishes ["giftorId" @= giftorID]

  [<CompiledName("InsertWish")>]
  let insertWish giftorID rank summary =
    Db.insert { ID       = Guid.NewGuid()
                GiftorID = giftorID
                Rank     = rank
                Summary  = summary } |> ignore
