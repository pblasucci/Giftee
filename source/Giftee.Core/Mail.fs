namespace Giftee.Core

open System
open System.Net

type formData = System.Collections.Specialized.NameValueCollection
type cfgMgr   = System.Configuration.ConfigurationManager

module Resx = Giftee.Core.Resources

module Mail =

  let apiUrl = Uri(cfgMgr.AppSettings.["mailgun_url"])
  let apiKey =     cfgMgr.AppSettings.["mailgun_key"]
  
  let systemAddress = cfgMgr.AppSettings.["system_email"]
  
  let credential =
    let cc = CredentialCache()
    cc.Remove(apiUrl,"Basic")
    cc.Add(apiUrl,"Basic",NetworkCredential("api",apiKey))
    cc
    
  let poster = 
    new WebClient(UseDefaultCredentials=false,Credentials=credential)

  let toForm items =
    let data = formData()
    for (key,value) in items do 
      data.Add(key,value)
    data

  let send from tos subject message tags =
    let url   = Uri(apiUrl,"messages")
    let data  = [ ("from"   ,from   )
                  ("subject",subject) 
                  ("text"   ,message) ]
                |> Seq.append (tos  |> Seq.map (fun r -> ("to"   ,r)))
                |> Seq.append (tags |> Seq.map (fun t -> ("o:tag",t)))
                |> toForm
    poster.UploadValues(url,data) |> ignore

  let sendRegistered email password =
      send  systemAddress [email]
            Resx.registeredLbl
            (String.Format(Resx.registeredMsg,email,password))
            ["registration"] 

  let sendPasswordReset email password =
    send  systemAddress [email]
          Resx.pwdChangedLbl
          (String.Format(Resx.pwdChangedMsg,email,password))
          ["password_reset"] 
