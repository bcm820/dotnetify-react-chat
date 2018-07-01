using System;
using System.Threading;
using DotNetify;

namespace Chat.Models {

  public class User {
    public long Id { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
  }

}