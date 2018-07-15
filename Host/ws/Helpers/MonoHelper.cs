using System;

namespace host.websocket.Helpers
{
  public static class MonoHelper
  {
    public static bool IsRunningOnMono ()
    {
      return Type.GetType ("Mono.Runtime") != null;
    }
  }
}

