namespace Giftee.Core

open Soma.Core

module Database =

  let config = 
    { new MsSqlConfig() with
        member __.ConnectionString = 
          cfgMgr.ConnectionStrings.["giftee"].ConnectionString }

  let query<'T>         = Db.query<'T>          config
  let queryOnDemand<'T> = Db.queryOnDemand<'T>  config

  let execute sql expr              = Db.execute  config sql expr
  let call<'T when 'T : not struct> = Db.call<'T> config

  let    find<'T when 'T : not struct> = Db.find<'T>    config
  let tryFind<'T when 'T : not struct> = Db.tryFind<'T> config

  let insert<'T when 'T : not struct> = Db.insert<'T> config
  let update<'T when 'T : not struct> = Db.update<'T> config
  let delete<'T when 'T : not struct> = Db.delete<'T> config
