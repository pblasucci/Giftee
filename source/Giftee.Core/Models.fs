namespace Giftee.Core.Models

open Soma.Core
open System

[<Table(Name="Giftors")>]
type Giftor =
  { [<Id>] ID : Guid
    FirstName : string
    LastName  : string
    Email     : string
    Password  : byte array
    IsAdmin   : bool
    GroupAs   : string option 
    GifteeID  : Guid option }
  with
    static member Empty =
      { ID        = Guid.Empty
        FirstName = String.Empty
        LastName  = String.Empty
        Email     = String.Empty
        Password  = Array.empty
        IsAdmin   = false
        GroupAs   = None
        GifteeID  = None }
      
[<Table(Name="Wishes")>]
type Wish =
  { [<Id>] ID : Guid
    GiftorID  : Guid
    Rank      : int
    Summary   : string}
  with
    static member Empty =
      { ID        = Guid.Empty
        GiftorID  = Guid.Empty
        Rank      = 0
        Summary   = String.Empty }

type User =
  { ID        : Guid
    FirstName : string
    LastName  : string
    Email     : string 
    Roles     : string seq}
  with
    static member OfGiftor {Giftor.FirstName = fName;
                                   LastName  = lName;
                                   ID        = identity;
                                   Email     = email;
                                   IsAdmin   = isAdmin} =
      {User.ID        = identity;
            FirstName = fName;
            LastName  = lName;
            Email     = email;
            Roles     = [ yield "User"
                          if isAdmin then yield "Admin" ] }

type Giftee =
  { ID        : Guid
    FirstName : string
    LastName  : string
    Email     : string 
    Wishlist  : string seq }
  with 
    static member ofGiftor {Giftor.FirstName = fName;
                                   LastName  = lName;
                                   ID        = identity;
                                   Email     = email;}
                            wishlist =
      {Giftee.ID        = identity;
              FirstName = fName;
              LastName  = lName;
              Email     = email;
              Wishlist  = wishlist 
                          |> Seq.map (fun {Wish.Summary = text} -> text);}
