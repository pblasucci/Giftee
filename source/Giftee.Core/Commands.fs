namespace Giftee.Core

open Giftee.Core.Models
open Giftee.Core.Security
open System
open System.Net.Mail
open Soma.Core

module Db = Giftee.Core.Database

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
