import React, { Component } from 'react';
import configuration from './configuration';

class Thumbnail extends Component {
  constructor(props) {
    super(props);

    this.clicked = this.clicked.bind(this);
  }

  clicked(e) {
    this.props.handleClick(this.props.item);
  }

  render() {
    const item = this.props.item;
    return (
      <img key={item.ID}
        src={configuration.THUMBNAILS_BASE_URL + item.ExternalID + '.png'}
        className="Thumbnail img-fluid img-thumbnail"
        alt={item.Title} 
        onClick={this.clicked} />
    );
  }
}

export default Thumbnail;