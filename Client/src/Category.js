import React, { Component } from 'react';
import Details from './Details';
import Thumbnail from './Thumbnail';
import configuration from './configuration';

class Category extends Component {
  constructor(props) {
    super(props);
    this.state = {
      detailsHidden: true,
      detailsItem: null,
      reviews: null
    };

    this.handleThumbnailClick = this.handleThumbnailClick.bind(this);
    this.handleDetailsCloseButtonClicked = this.handleDetailsCloseButtonClicked.bind(this);
  }

  handleThumbnailClick(item) {
    this.setState({
      detailsHidden: false,
      detailsItem: item,
      reviews: null
    });

    this.getReviewsID(item);
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
          reviews={this.state.reviews}
          hidden={this.state.detailsHidden}
          handleCloseButtonClicked={this.handleDetailsCloseButtonClicked} />
      </div>
    );
  }

  getReviewsID(detailsItem) {
    const self = this;
    const url = configuration.searchReviewsUrl(detailsItem.ID);
    console.log(url);

    fetch(url)
      .then(function(response) {
        if (response.ok) {
          return response.json();
        }
        throw new Error('Request failed: ' + response.status);
      })
      .then(function(searchResult) {
        console.log(searchResult);
        if (searchResult.NumResults > 0) {
          setTimeout(() => { self.getReviews(searchResult) }, configuration.DELAY_MILLIS);
        }
      })
      .catch(function(error) {
        console.log('Error retrieving data: ', error.message);
      });
  }

  getReviews(searchResult) {
    const self = this;
    const url = configuration.getReviewsUrl(searchResult.BestMatch.BookID);
    console.log(url);
    
    fetch(url)
      .then(function(response) {
        if (response.ok) {
          return response.json();
        }
        throw new Error('Request failed: ' + response.status);
      })
      .then(function(json) {
        console.log(json);
        self.setState({
          reviews: json.Reviews
        });
      })
      .catch(function(error) {
        console.log('Error retrieving data: ', error.message);
      });
  }
}

export default Category;
