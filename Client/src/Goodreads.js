import React, { Component } from 'react';

class Goodreads extends Component {
  render() {
    if (this.props.reviews === null) {
      return (
        <div></div>
      );
    }

    return (
      <div className="Goodreads">
        <p>Arvosana Goodreads-sivustolla:<br />
        {this.formatRating(this.props.reviews.AverageRating)}/5</p>
        <div className="ReviewsWrapper" 
          dangerouslySetInnerHTML={{__html: this.props.reviews.ReviewsHtml}}>
        </div>
      </div>
    );
  }

  formatRating(rating) {
    return rating.toLocaleString('fi-FI', {minimumFractionDigits: 2, maximumFractionDigits: 2})
  }
}

export default Goodreads;