using System;
using System.Collections.Generic;
using System.Threading;
using winTimer = System.Timers;
using Chat.Models;

namespace Chat.Services {

  public class Broadcaster : IDisposable {

    public class UpdatedUsersEventArgs : EventArgs { public List<User> Users { get; set; } }
    public class LeavingUsersEventArgs : EventArgs { public List<long> UserIds { get; set; } }

    // Occurs when the state of a user changed.
    public event EventHandler<UpdatedUsersEventArgs> UpdatedUsers;
    public event EventHandler<LeavingUsersEventArgs> LeavingUsers;

    // Occurs when a user leaves then Broadcast.
    public static Broadcaster Singleton {
      get {
        lock (_singletonSync) {
          if (_singleton == null)
            _singleton = new Broadcaster();
        }
        return _singleton;
      }
    }

    // This constant sets the milliseconds rate interval to raise events to control the performance.
    private const int UPDATE_RATE = 100;

    private static Broadcaster _singleton;
    private static object _singletonSync = new object();

    private long _userCount;
    private long _userSequence;

    private object _clientUpdateLock = new object();
    private object _serverUpdateLock = new object();

    private List<User> _allUsers = new List<User>();
    private List<User> _updatedUsers = new List<User>();
    private List<long> _leavingUserIds = new List<long>();

    private UpdatedUsersEventArgs _changedEventArgs = new UpdatedUsersEventArgs();
    private LeavingUsersEventArgs _departedEventArgs = new LeavingUsersEventArgs();

    private winTimer.Timer _updateTimer = new winTimer.Timer(UPDATE_RATE);

    protected Broadcaster() {
      _changedEventArgs.Users = new List<User>();
      _departedEventArgs.UserIds = new List<long>();
      _updateTimer.Elapsed += UpdateTimer_Elapsed;
      _updateTimer.Start();
    }

    public void Dispose() {
      _updateTimer.Stop();
      _updateTimer.Elapsed -= UpdateTimer_Elapsed;
    }

    public User Add(out List<User> others, string username) {
      // Add a new bunny. The pen can be accessed by view models on multiple threads, so it needs to be thread-safe.
      Interlocked.Increment(ref _userCount);
      var id = Interlocked.Increment(ref _userSequence);
      var user = new User { Id = id, Username = username, Message = "" };

      lock (_clientUpdateLock) {
        others = new List<User>(_allUsers);
        _allUsers.Add(user);
        _updatedUsers.Add(user);
      }

      return user;
    }

    public void Remove(User user) {
      lock (_clientUpdateLock) {
        _allUsers.Remove(user);
        _leavingUserIds.Add(user.Id);
      }
      Interlocked.Decrement(ref _userCount);
    }

    public void MarkAsChanged(User user) {
      lock (_clientUpdateLock) {
        if (_updatedUsers.IndexOf(user) < 0)
          _updatedUsers.Add(user);
      }
    }

    // This timer regulates the interval of when events
    // are raised to control update rate to the clients.
    private void UpdateTimer_Elapsed(object sender, winTimer.ElapsedEventArgs e) {
      lock (_clientUpdateLock) {
        lock (_serverUpdateLock) {
          if (_updatedUsers.Count > 0)
            _changedEventArgs.Users.AddRange(_updatedUsers);

          if (_leavingUserIds.Count > 0)
            _departedEventArgs.UserIds.AddRange(_leavingUserIds);
        }

        _updatedUsers.Clear();
        _leavingUserIds.Clear();
      }

      lock (_serverUpdateLock) {
        if (UpdatedUsers != null && _changedEventArgs.Users.Count > 0)
          UpdatedUsers(this, _changedEventArgs);

        if (LeavingUsers != null && _departedEventArgs.UserIds.Count > 0)
          LeavingUsers(this, _departedEventArgs);

        _changedEventArgs.Users.Clear();
        _departedEventArgs.UserIds.Clear();
      }

      // When there are no more users, dispose this singleton.
      if (Interlocked.Read(ref _userCount) == 0) {
        lock (_singletonSync) {
          Dispose();
          _singleton = null;
        }
      }
    }
  }

}