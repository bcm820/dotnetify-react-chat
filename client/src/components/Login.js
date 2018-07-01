import React from 'react';

class Login extends React.Component {
  state = { username: '' };
  startSession = () => this.props.setLogin(this.state.username);
  handleChange = e => this.setState({ username: e.target.value });
  handleKeyPress = e => (e.key === 'Enter' ? this.startSession() : null);

  render() {
    return (
      <div>
        <input
          type="text"
          placeholder="Set Username"
          value={this.state.username}
          onChange={this.handleChange}
          onKeyPress={this.handleKeyPress}
        />
        <button onClick={this.startSession}>Login</button>
      </div>
    );
  }
}

export default Login;
