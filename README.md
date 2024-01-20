.NET Core Web API for Shopping Cart with SQL, Authentication, and Advanced Features

Overview

This project serves as the backend implementation of a feature-rich shopping cart using .NET Core Web API. It integrates SQL for data storage, authentication with Identity, JWT token for secure user sessions, and includes advanced features such as server-side pagination and searching.

Technologies Used
.NET Core Web API
Entity Framework Core
SQL Server
Authentication and Authorization with Identity
JWT Token for Secure Sessions

Project Structure
Controllers: Contains API controllers for handling various shopping cart functionalities.

Models: Defines the data models used for representing products, users, and shopping cart items.

Data: Includes the database context and migrations for setting up the SQL database.

Services: Houses services responsible for business logic, authentication, and advanced features like pagination and searching.

Getting Started

Configure Database:

Update the database connection string in the appsettings.json file.
Run EF Core migrations to create the database schema.
dotnet ef database update

Run the Application:

Build and run the application using Visual Studio or the command line.
Access the API at https://localhost:5001 by default.

Authentication and JWT Token:

Use the provided authentication endpoints to register and log in.
Obtain the JWT token for subsequent authenticated requests.

Authentication

POST /api/account/register: Register a new user.

POST /api/account/login: Log in and obtain the JWT token.

Feel free to contribute to the project by creating issues or submitting pull requests. All contributions are welcome!

Feel free to customize the content based on your project's specifics and add or modify sections as needed.
