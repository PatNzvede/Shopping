The Fruit&Vege online shop is developed in .net core 5, Angular 13, SQL server database and Identity server for authentication.

Web API uses entity framework and code first to generate the database.

The database has 3 import tables.
1. The Products tables that stores all the products available for sale.
2. The Cart table that stores all itemised purchased products by a specific customer. An order is also stored.
3. The Orders table stores the summary of each order and a processed flag that is used to indicate when email is send to the customer or not.

A gmail smtp is used to send proof of orders processed.

Pictures used are stored in the assets folder in front end.
