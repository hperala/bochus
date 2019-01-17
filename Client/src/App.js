import React, { Component } from 'react';
import Category from './Category';
import About from './About';
import NavBar from './NavBar';
import './App.css';

class App extends Component {
  constructor(props) {
    super(props);
    this.state = {
      categories: [],
      activePage: 'MainPage'
    };

    this.setActivePage = this.setActivePage.bind(this);
  }

  setActivePage(pageID) {
    this.setState({
      activePage: pageID
    });
  }

  componentDidMount() {
    const self = this;

    fetch('http://localhost:59031/api/Recommendations')
      .then(function(response) {
        if (response.ok) {
          return response.json();
        }
        throw new Error('Request failed');
      })
      .then(function(json) {
        self.setState({categories: json});
      })
      .catch(function(error) {
        console.log('Error retrieving data: ', error.message);
      });
  }

  render() {
    const categories = this.state.categories.map((cat) => 
      <Category key={cat.Name} category={cat} />
    );
    return (
      <div className="App container">
        <NavBar handleOpenPage={this.setActivePage} />
        <div id="MainPage" hidden={this.state.activePage !== 'MainPage'}>
          {categories}
        </div>
        <div id="AboutPage" hidden={this.state.activePage !== 'AboutPage'}>
          <About />
        </div>
      </div>
    );
  }
}

export default App;
