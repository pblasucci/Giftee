using System;
using log4net;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace Giftee.Web
{
  public static class FSharpOptionExtensions
  {
    public static _b IfSome<_a,_b>(this FSharpOption<_a>  option,
                                        Func<_a,_b>       projection,
                                        Func<   _b>       defaultValue)
    {
      return OptionModule.IsSome(option) 
           ? projection(option.Value) 
           : defaultValue();
    }

    public static _b IfSome<_a,_b>(this FSharpOption<_a>  option,
                                        Func<_a,_b>       projection)
    {
      return IfSome(option,projection,() => default(_b));
    }

    public static _a IfSome<_a>(this FSharpOption<_a> option)
    {
      return IfSome(option,v => v);
    }
  }

  public static class ComparisonExtensions
  {
    public static Boolean IsEqualTo<_a>(this _a left, _a right)
    {
      var areEq = ComparisonIdentity.Structural<_a>();
      return (areEq.Compare(left,right) != 0);
    }
  }

  public static class LoggingExtensions
  {
    public static void Info(this   ILog      log, 
                                   String    format, 
                            params Object[]  data)
    {
      log.InfoFormat(format,data);
    }

    public static void Debug(this   ILog      log, 
                                    String    format, 
                             params Object[]  data)
    {
      log.DebugFormat(format,data);
    }

    public static void Warn(this   ILog      log, 
                                   String    format, 
                            params Object[]  data)
    {
      log.WarnFormat(format,data);
    }

    public static void Error(this   ILog      log, 
                                    String    format, 
                             params Object[]  data)
    {
      log.ErrorFormat(format,data);
    }

    public static void Fatal(this   ILog      log, 
                                    String    format, 
                             params Object[]  data)
    {
      log.FatalFormat(format,data);
    }
  }
}
