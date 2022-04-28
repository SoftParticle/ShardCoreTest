# ShardCore Test
This is an example project to showcase ShardCore horizontal sharding capabilities.

### Setup
To setup the project just replace the server name on the SQL connection strings, and the DatabaseServerName variables, on the Startup class.

Create a database to store the shards information and another database for each one of the shards you'll be using.

Make sure you have full-text  search enabled on your SQL Server instance, you can get more information [here](https://www.mssqltips.com/sqlservertip/6841/add-full-text-search-sql-server/).

### Running the Project
Just run the project normally, this will apply the migrations to each database automatically.

### Seeding the Database
To seed the database just go to the Shards page and click on Seed Products. 

This will seed 1 million products to the database and the result will be updated  in realtime each 50000 products.

### Testing 
You can text ShardCore's feature set from the Home page. There you can order, page and read sharded information.
