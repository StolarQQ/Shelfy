# Shelfy

[![Platform](https://img.shields.io/badge/Platform-.NET%20Core%202.2-green.svg)](https://dotnet.microsoft.com/download)

Branch | Build Status
------------ | -------------
Master | [![Build Status](https://travis-ci.org/StolarQQ/Shelfy.svg?branch=master)](https://travis-ci.org/StolarQQ/Shelfy)

## What is Shelfy ?

The main goal of this project, is to get to know the ASP.NET Core framework, concepts of REST, using good patterns and practices, used on a daily basis in the world of the software development. <br>
Shelfy is RESTful API, backend for social cataloging apps, designed to help users to catalog books. Application allow collecting books, authors, reviews and can be accessed by any type of application(web, desktop, mobile applications). Application is based on ASP.NET Core 2.2 framework. <br><br>
App using MongoDB database.
Architecture type was implemented based on [Onion Architecture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/). <br>In application have been used, __repository__, __IoC__, __Depdency Injection__ patterns. Shelfy implements authorization(__JWT Token__) and authentication, password encryption, simply diagnostic logging(Serilog), domain & service exceptions handle by custom middleware. Unit and integration tests(Not completed). Integration with open-source build server called [Travis-CI](https://travis-ci.org/). <br>

#### __Application is not finished yet.__

#### TODO
+ __Complete tests__ !
+ User chat, current reading, read, want to read shelfs
+ AutoFac IoC Container, assembly scaning
+ Publish app to Azure/Digital Ocean
+ Front-end build with Angular or Aurelia
+ Recommendation system for Users
+ CQRS / Rabbit


## Prerequisite
In order to run Shelfy, you need to have installed:
+ [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)
+ [MongoDB](https://www.mongodb.com/download-center/community)

## Configuration
Mongodb connections string is setup for *27017*, default one for mongos instances.  
In order to use custom settings, please edit *appsettings.json* file, located in [Shelfy.API](src/Shelfy.API) folder<br>

## How to start application
```
git clone https://github.com/StolarQQ/Shelfy.git
cd src/Shelfy.Api
dotnet restore 
dotnet run --urls "http://*:5001"
```
Application will be available under https://localhost:5001

## Testing HTTP requests
You can find prepared [Postman Collection](/assets/ShelfyCollection.postman_collection.json) of requests, in assets [folder.](/assets) <br>
In case of tests, u can use database seeding endpoint. <br>
After that, sign in with login credentials, listed below.
```
GET /admin/seed
POST /acccount/login

- User
{
  "email": "email1@gmail.com",
  "password": "secret123"
}
- Admin
{
  "email": "admin1@gmail.com",
  "password": "admin123"
}

```
### Solution structure
+ Shelfy.Api - Actual HTTP API - Does reference to infrastructure project.
+ Shelfy.Infrastructure - Contains application services, repositories, dtos, mongo convetions, automapper configuration. Does reference to core project. 
+ Shelfy.Core - Core of our application, contains domain models, and repositories interfaces. <br> Does not reference any other project.
+ Shelfy.Tests - Unit tests.
+ Shelfy.EndToEnd - Integration tests.

### Technology stack
+ [.NET Core](https://dotnet.microsoft.com/) - an open source & cross-platform framework for building applications using C# language.
+ [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.2) - is a cross-platform, high-performance, open-source framework for building modern, cloud-based, Internet-connected applications.



