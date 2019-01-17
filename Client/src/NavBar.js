import React, { Component } from 'react';

class NavBar extends Component {
  constructor(props) {
    super(props);
    
    this.handleClick = this.handleClick.bind(this);
  }

  handleClick(e) {
    this.props.handleOpenPage(e.currentTarget.dataset.target);
  }

  render() {
    return (
      <nav className="navbar navbar-expand-md navbar-dark bg-dark fixed-top">
        <a className="navbar-brand" onClick={this.handleClick} data-target="MainPage" href="#">Bochus</a>
        <ul className="navbar-nav mr-auto">
          <li className="nav-item">
            <a className="nav-link" onClick={this.handleClick} data-target="AboutPage" href="#">Tietoja</a>
          </li>
        </ul>
      </nav>
    );
  }
}

export default NavBar;