# Thomas Technical Test

## API

The API is documented using Swagger, meaning you can see all the available endpoints by going to the root of the application. Swagger gives you both the ability to see the objects sent and received from the API, as well as allow you to call the endpoints right from the browser. This makes it easier for developers to view APIs and also gives them the ability to play around with it before attempting to integrate.

## Prometheus

The API also comes with Prometheus metrics to provide an insight to how the application is performing, and allow for performance to be monitored and graphed. You can see these metrics by going to `<root of application>/metrics` although, the formate it more designed to be digested by a Prometheus server.

## Database

The database provider is SQLite as it was the most convenient to setup, and could be used anywhere as its a binary file. The Repo comes with one already and it contains test data to get you started. However, if you do want to create a new database, it was built code first using migrations. This means you can run `dotnet ef database update` to create a new database (providing it doesn't already exist, so make sure you delete the old one first before trying this).