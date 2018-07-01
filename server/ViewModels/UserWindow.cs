using System;
using System.Threading;
using DotNetify;
using Chat.Models;

namespace Chat.ViewModels {

  public class UserWindow : BaseVM {

    public long Id { get; set; }

    public string Username {
      get => Get<string>();
      set => Set(value);
    }

    public string Message {
      get => Get<string>();
      set => Set(value);
    }

    public Action<User> SetUsername => user => Username = user.Username;
    public Action<User> SetMessage => user => Message = user.Message;
  }

}