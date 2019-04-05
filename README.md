# React.Js, .Net Core(2.2) Web Api using Entity Framework Code First interfacing MS-Sql db 

Project showcases React.js frontend UI consuming data from the sql database which communicates using .Net Core by exposing Web Api endpoints (for Json output) supplemented by entity framework scaffolded models

<br/>

# Steps to get the project running

Pre-requisites:

>1. [.Net Core 2.2 SDK](https://www.microsoft.com/net/download/dotnet-core/2.2)
>2. [Visual Studio Code](https://code.visualstudio.com/) or Recommended - [Visual Studio 2017 Community editon](https://visualstudio.microsoft.com/vs/community/) or later editor

<br/>

Clone the current repository locally as
 `git clone https://github.com/NileshSP/aspnetcoreTransformersApp.git`

<br/>

Steps: using Visual Studio community edition editor
>1. Open the solution file (aspnetcoreTransformersApp.sln) available in the root folder of the downloaded repository
>2. Await until the project is ready as per the status shown in taskbar which loads required packages in the background
>3. Hit -> F5 or select 'Debug -> Start Debugging' option to run the project

<br/>

Steps: using Visual Studio code editor
>1. Open the root folder of the downloaded repository 
>2. Await until the project is ready as per the status shown in taskbar which loads required packages in the background
>3. Open Terminal - 'Terminal -> New Terminal' and execute commands as `cd aspnetcoreTransformersApp` & `dotnet build` & `dotnet run` sequentially
OR
>4. Hit -> F5 or select 'Debug -> Start Debugging' option to run the project

<br/>

Once the project is build and run, a browser page would be presented with navigation menu options wherein `Web Api's` option contains functionality related to data access from sql database


![alt text](https://github.com/NileshSP/aspnetcoreTransformersApp/blob/master/screenshot.gif "Working example..")
<br/>

# Root folder contents: 
>1. aspnetcoreTransformersApp folder: contains frontend UI built using React.js(in ClientApp folder) and .Net Core Web Api endpoints
>2. aspnetcoreTransformersApp.Tests folder: unit tests for Web Api Endpoints
>3. aspnetcoreTransformersApp.sln solution file
>4. Readme.md file for project information
