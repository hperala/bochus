import React, { Component } from 'react';
import Goodreads from './Goodreads';
import configuration from './configuration';

class Details extends Component {
  setDefaults(item) {
    if (item.Locations === null) {
      item.Locations = [];
    }
    if (item.Subjects === null) {
      item.Subjects = [];
    }
  }

  render() {
    const item = this.props.item;
    if (item === null) {
      return <div className="Details" hidden></div>
    }

    this.setDefaults(item);

    const subjects = item.Subjects.map((subject) =>
      subject.FullText
    ).join(' | ');
    const locations = item.Locations.map((location) =>
      <li key={location.ID}>{location.Name}</li>
    );
    const finnaLink = configuration.FINNA_BASE_URL + item.ExternalRelativeUrl;

    return (
      <div className="Details card" hidden={this.props.hidden}>
        <div className="card-body">
        <button className="DetailsCloseButton btn"
          onClick={this.props.handleCloseButtonClicked}>
          ×
        </button>
        <h3>
          <span className="AuthorName">{item.AuthorName}</span><br />
          {item.Title}
        </h3>
        <div>{item.PublicationYear}</div>
        <div>{item.Series}</div>
        <div className="row">
          <div className="col-md">
            <p className="Subjects">{subjects}</p>
            <p>Heili-kirjastot, joiden kokoelmiin nimike kuuluu:</p>
            <ul>{locations}</ul>
            <p>Tarkista tämänhetkinen saatavuus <a href={finnaLink}>Finnasta</a>.</p>
          </div>
          <div className="col-md">
            <Goodreads reviews={this.props.reviews} />
          </div>
        </div>
        </div>
      </div>
    );
  }
}

export default Details;
