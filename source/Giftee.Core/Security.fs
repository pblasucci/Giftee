namespace Giftee.Core

open System
open System.Security.Cryptography
open System.Text
open System.Text.RegularExpressions
  
module Security =
  
  let scrub = Regex ("[,.]",RegexOptions.Compiled)
  
  let source = 
    scrub.Replace(Resources.loremIpsum,"").Split [|' '|]
    |> Array.filter (fun s -> s.Length > 2)
  
  let rnd = Random DateTime.Now.Millisecond

  let generatePhrase count =    
    let rec generate acc =
      match acc |> Set.count with
      | c when c = count -> acc |> Set.toArray
      | _ ->  let n = source.[rnd.Next source.Length]
              if acc |> Set.contains n
                then generate  acc
                else generate (acc |> Set.add n)
    let words = generate Set.empty
    String.Join(" ",words)

  let encrypt salt value =
    use sha = new SHA512Managed()
    (sprintf "%s^%s" salt value)
    |> Encoding.ASCII.GetBytes
    |> sha.ComputeHash
