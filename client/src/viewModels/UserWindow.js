import React from 'react';
import connect from '../dotnetify.config';

class UserWindow extends React.Component {
  constructor(props) {
    super(props);
    this.state = { Username: props.username };
    this.vm = connect(this);
  }

  componentWillUnmount = () => this.vm.$destroy();

  syncState = key => e => {
    this.setState({ [key]: e.target.value }, () =>
      this.vm.$dispatch({
        [`Set${key}`]: { [key]: this.state[key] }
      })
    );
  };

  render() {
    return (
      <div>
        <p>{this.state.Username || ''}</p>
        <div>
          <textarea
            placeholder="Update Message"
            value={this.state.Message || ''}
            onChange={this.syncState('Message')}
          />
        </div>
      </div>
    );
  }
}

export default UserWindow;
