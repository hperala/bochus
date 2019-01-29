# Bochus

Bochus is a simple mashup that combines data from [Finna](https://finna.fi/?lng=en-gb) (Finnish libraries) ja [Goodreads](https://www.goodreads.com/). When its database has been populated with book metadata from Finna, Bochus presents the user with a selection of items in a Netflix-style user interface. Goodreads ratings and reviews are loaded on demand, if available.

A demo may be [available online](https://hperala.github.io/Bochus/index.html).

## Server setup

The server is an ASP.NET Web API application that uses Entity Framework. SQL Server Express has been used as the database during development.

* Edit the configuration in `Web/Bochus.config`.
    * `GoodreadsApiKey`: your personal developer key
    * `AdminPassword`: give a nonempty value to allow access to the import method
* Add the correct connection string to application configuration if the defaults are not right for your environment.
* Change the `EnableCors` attribute in `ItemsController` and `RecommendationsController` as needed to allow running the client from a different origin.
* Build and run `Bochus.sln`. Run the Mgttool executable from the `Tools` project with the `get-finna` command to fetch data and the  `import-finna` command to import it to Bochus (run Mgttool without arguments to see available commands and their parameters). To change what is fetched from the Finna API, you must edit the tool source code.

## Client setup

The client is a single-page application built with React (generated with Create React App).

* Edit the configuration in `Client/src/configuration.js`.
    * `server`: the address of the server
* Use `npm start` to run the client and `npm run build` to build a deployment-ready version. See the `Client/README.md` file for more options.

## Note on external APIs

See the Finna and Goodreads websites for information about terms and conditions and data licensing. If you run this application, you are responsible for using these APIs correctly.
