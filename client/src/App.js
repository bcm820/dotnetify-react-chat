import React from 'react';
import UserWindow from './viewModels/UserWindow';
import Login from './components/Login';

class App extends React.Component {
  state = { loggedIn: false, username: '' };
  login = username => this.setState({ loggedIn: true, username });

  render() {
    return this.state.loggedIn ? (
      // to replace with ChatRoom VM cmp
      // in ChatRoom VM cmp, iterate through User objects
      // pass entire User objects into separate UserWindows.
      <UserWindow username={this.state.username} />
    ) : (
      <Login setLogin={this.login} />
    );
  }
}

export default App;
