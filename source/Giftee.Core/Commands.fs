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
        Password  = encrypted
        GroupAs   = Some(lastName) } 
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
  let gatherWishes giftorID =
    Db.query<Wish> R.SQL.giftorWishes ["giftorID" @= giftorID]

  [<CompiledName("InsertWish")>]
  let insertWish giftorID rank summary =
    //TODO: send notification to giftor
    Db.insert { ID       = Guid.NewGuid()
                GiftorID = giftorID
                Rank     = rank
                Summary  = summary } |> ignore

  [<CompiledName("SelectWish")>]
  let selectWish wishID = Db.find<Wish> [ wishID ]

  [<CompiledName("UpdateWish")>]
  let updateWish wishID rank summary = 
    //TODO: send notification to giftor
    let wish = Db.find<Wish> [ wishID ]
    Db.update {wish with Rank    = rank; 
                         Summary = summary} |> ignore

  [<CompiledName("DeleteWish")>]
  let deleteWish wishID =
    //MAYBE: send notification to giftor?
    Db.execute R.SQL.deleteWish [ "wishID" @= wishID ]

  [<CompiledName("GatherGiftors")>]
  let gatherGiftors () = Db.query<Giftor> R.SQL.allGiftors []

  [<CompiledName("UpdateGiftor")>]
  let updateGiftor giftorID groupAs = 
    groupAs |> validate "Group As"
    let giftor = Db.find<Giftor> [ giftorID ]
    Db.update {giftor with GroupAs = Some(groupAs)} |> ignore

  let pickNames giftors =
    let rand = Random DateTime.Now.Millisecond
    //HACK: the logic in ``pickNames'`` doesn't "feel" right!
    let rec pickNames' giftors =
      giftors |> List.fold (fun picked current ->
        let {ID = currentID; GroupAs = currentGroupAs} = current
        let unpicked = 
          giftors 
          |> List.filter (fun giftor ->
                giftor.ID      <> currentID      &&
                giftor.GroupAs <> currentGroupAs &&
                picked 
                |> List.forall (fun {GifteeID = gifteeID} -> 
                      gifteeID |> Option.exists (fun v -> v <> giftor.ID)))  
          |> List.toArray
        match unpicked.Length with
        | 0 ->  //Something went wrong... just start over
                pickNames' giftors
        | n ->  let giftee = unpicked.[rand.Next n]
                {current with GifteeID = Some(giftee.ID)} :: picked
      ) []
    let canPick giftors =
      let groups = giftors |> Seq.groupBy (fun g -> g.GroupAs)
      let groupCount = groups |> Seq.map (fun (k,_) -> k) |> Seq.length
      let userCount = giftors |> Seq.length
      let moreThanOne (_,v) = v |> Seq.length > 1
      (groupCount > 2)                                              ||
      (groupCount > 1         && groups |> Seq.forall moreThanOne)  ||
      (groupCount = userCount && userCount > 1)
    if giftors |> canPick then giftors |> pickNames' else List.Empty

  [<CompiledName("ReviewExchanges")>]
  let reviewExchanges () =
    let giftors = Db.query<Giftor> R.SQL.allGiftors []
    let werePicked = giftors |> List.forall (fun g -> g.GifteeID.IsSome)
    match werePicked with
    | true  ->  (true, giftors)
    | false ->  (false,giftors |> pickNames)
             
  [<CompiledName("CommitExchanges")>]
  let commitExchanges exchanges =
    use tx = new System.Transactions.TransactionScope()
    let exchanges' = exchanges |> dict
    let giftors = Db.query<Giftor> R.SQL.allGiftors []
    giftors 
      |> List.map (fun g -> {g with GifteeID = Some(exchanges'.[g.ID])})
      |> List.iter (Db.update >> ignore)
    tx.Complete()

  [<CompiledName("DeleteExchanges")>]
  let deleteExchanges () =
    use tx = new System.Transactions.TransactionScope()
    let giftors = Db.query<Giftor> R.SQL.allGiftors []
    giftors 
      |> List.map (fun g -> {g with GifteeID = None})
      |> List.iter (Db.update >> ignore)
    tx.Complete()

  [<CompiledName("SelectGiftee")>]
  let selectGiftee giftorID =
    let giftee = Db.query<Giftor> R.SQL.giftorGiftee 
                                  [ "giftorID" @= giftorID ]
    match giftee with
    | g :: [] ->  let wishes = Db.query<Wish> R.SQL.giftorWishes 
                                              [ "giftorID" @= g.ID ]
                  Some(Giftee.ofGiftor g wishes)
    | _       ->  None
    