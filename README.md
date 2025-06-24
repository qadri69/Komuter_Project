KTM Komuter Ticketing System
Overview
This project is a web-based application for the KTM Komuter train service, providing ticket booking, user account management, and customer support through a chatbot interface.

Technologies Used
ASP.NET Core MVC
Microsoft SQL Server (local database)
C# Programming Language
Entity Framework Core (implied by the database models)
HTML/CSS/JavaScript (standard for MVC web applications)
Features
User Account Management: Registration, login, and profile management through the AccountController
Ticket Booking System: Purchase and manage train tickets via the TicketController
Customer Support ChatBot: Interactive assistance for users through the ChatBotController
Fare Calculator: Calculate ticket prices based on routes using the FareTableViewModel
Email Notifications: Send booking confirmations and updates using the Mail service
Setup Instructions
Prerequisites
Visual Studio 2019 or newer
.NET Core 3.1 or .NET 5.0+ SDK
SQL Server (LocalDB is sufficient)
Installation Steps
Clone the repository
Open the solution file ktm_project.sln in Visual Studio
Restore NuGet packages
The database files (KtmKomuterDB.mdf and KtmKomuterDB_log.ldf) are included in the project - they should be automatically attached
Run the application using IIS Express or Kestrel
Project Structure
Controllers/: Contains MVC controllers that handle HTTP requests
Models/: Data models and view models
Views/: UI templates (Razor views)
wwwroot/: Static files (CSS, JavaScript, images)
MailSettings/: Email configuration and services
Security Features
The application implements PBKDF2 password hashing for secure user authentication as seen in the PBKDF2Hash model.

Database
The application uses a SQL Server database (KtmKomuterDB.mdf) to store user information, ticket data, and train schedules.
