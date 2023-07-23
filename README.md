# DeliverCom

DeliverCom is designed to be an example of delivery system. Developed according to standards of hexagonal architecture.

## Prerequities
   - ##### Docker(https://www.docker.com/get-started)
   - ##### [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed 
   - ##### Postgres DB

## Libraries that used

   - [Cap](https://github.com/dotnetcore/CAP) - For implementing Outbox Patter
   - [MediatR](https://github.com/jbogard/MediatR) - For Processing Commands, Queries
   - [EntityFramework](https://github.com/dotnet/efcore) - For implementing object-database mapping
   - [Mapster](https://github.com/MapsterMapper/Mapster) - For mapping domain objects with DTO objects used in API.

## Dependency Graph

  ![Dependency Graph](https://github.com/1bcrona/deliver.com/blob/master/assets/DeliverCom.png?raw=true)
    
## Sequence Diagram

  ![Sequence Diagram](https://github.com/1bcrona/deliver.com/blob/master/assets/DeliverCom_Sequence_Diagram.png?raw=true)
  

## Solution Structure

- ### DeliverCom.API
  
  It is the application that contains the API.

- ### DeliverCom.Application

  Library contains all application domain. Act as a bridge betwwen API and Domain

- ### DeliverCom.Container

  Library that contains IoC Container of application

- ### DeliverCom.Core

  Library that contains specifications of ports and some helper classes.

- ### DeliverCom.Data

    It is the library that contains the Store types used in the application.
 
    * KeyValue Store
      - It includes Key/Value store features. It has a InMemory and Redis Implementation.
      - InMemory KeyValue Store is currently used by Application 

- ### DeliverCom.Domain

  -  Library that contains the entity, value objects, data context used in the application.
    
- ### DeliverCom.Resolver

  -  Library that is used for resolving parameters for use-cases.

- ### DeliverCom.Routing

   -  Library that used for map http requests with use-cases.

- ### DeliverCom.UseCase

  - Library that contains all business logic and validations of application

 ## Installation
 
To run the application, please do the followings
 * Clone the project
 * Open the terminal at project directory
 * Execute following command
 <pre><code>docker-compose up</code></pre>
 
After the command is executed, an application with three containers on docker will be launched.
These;
 * Postgres DB[^1]
 * DeliverCom.API[^2]
 
 [^1]: By default, Postgres access port is set to **5432** and port forwarding is enabled.</sup>
 [^2]: By default, DeliverCom.API is listening  **http:5000** and port forwarding is enabled.</sup>
 
 ## Tests
 
To run tests;
 * Build the solution.
 * Open the terminal at **DeliverCom.Test/bin/Debug/net7.0** directory
 * Execute following command
 <pre><code>dotnet test DeliverCom.Test.dll</code></pre>
 * Or use Visual Studio's **Test Explorer** pane
