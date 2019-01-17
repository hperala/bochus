import React, { Component } from 'react';
import Details from './Details';
import Thumbnail from './Thumbnail';

class Category extends Component {
  constructor(props) {
    super(props);
    this.state = {
      detailsHidden: true,
      detailsItem: null
    };

    this.handleThumbnailClick = this.handleThumbnailClick.bind(this);
    this.handleDetailsCloseButtonClicked = this.handleDetailsCloseButtonClicked.bind(this);
  }

  handleThumbnailClick(item) {
    this.setState({
      detailsHidden: false,
      detailsItem: item
    });
  }

  handleDetailsCloseButtonClicked(e) {
    this.setState({
      detailsHidden: true,
      detailsItem: null
    });
  }

  render() {
    const images = this.props.category.Items.map((item) => 
      <Thumbnail key={item.ID} item={item} handleClick={this.handleThumbnailClick} />
    );
    return (
      <div className="Category">
        <h2>{this.props.category.Name}</h2>
        <div className="Slider">
          {images}
        </div>
        <Details item={this.state.detailsItem} 
          hidden={this.state.detailsHidden}
          handleCloseButtonClicked={this.handleDetailsCloseButtonClicked} />
      </div>
    );
  }
}

export default Category;
