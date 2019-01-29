import React, { Component } from 'react';

class About extends Component {
  render() {
    return (
      <div>
        <h2>Tietoja Bochus-palvelusta</h2>
        <p>Bochus sisältää pienen valikoiman Etelä-Karjalan <a href="https://heili.finna.fi/">Heili-kirjastojen</a> kokoelmista. Palvelun etusivu esittelee tästä aineistosta poimittuja teoksia avainsanojen tai lajityypin mukaan ryhmiteltynä.</p>
        <p>Valitsemalla nimikkeen näet muun muassa kirjastot, joiden kokoelmiin teos kuuluu. Lisäksi saatetaan näyttää <a href="https://www.goodreads.com/">Goodreads-sivustolta</a> peräisin olevaa lisätietoa kuten käyttäjien arvosteluja.</p>
        <p>Bochus käyttää Finna.fi- ja Goodreads-sivustojen julkisia rajapintoja, mutta ei ole niiden tai Heili-kirjastojen hyväksymä tai suosittelema.</p>
        <p>P.S. Muinaisenglannin <i><a href="https://en.wiktionary.org/wiki/bochus">bōchūs</a></i> tarkoitti ”kirjastoa”.</p>
      </div>
    );
  }
}

export default About;