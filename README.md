# LightHTTP
[![GitHub](https://img.shields.io/github/license/javidsho/LightHTTP)](https://github.com/javidsho/LightHTTP/blob/master/LICENSE)
[![NuGet download count](https://img.shields.io/nuget/dt/LightHTTP)](https://www.nuget.org/packages/LightHTTP)

## Overview
LightHTTP is a lightweight HTTP server for .NET which can be used in the following scenarios:

- To mock HTTP APIs for unit testing
- To quickly set up a lightweight standard server for generic purposes

## Features
- Supports a broad range of .NET runtimes (anything compatible with .NET Standard 2.0)
- Asynchronous handling of HTTP requests
- Easy and quick to use

# Getting Started

## Installation (via <a href="https://www.nuget.org/packages/LightHTTP/">NuGet</a>)

    Install-Package LightHTTP -Version 1.0
    
## Step-by-step
- Create a new instance of `LightHttpServer`.
- Set up prefixes to listen to. You can access the `HttpListener` instance via the `LightHttpServer.Listener` property.
- Register your handlers. Handlers can be registered by any of the `Handles*` extension methods on the `LightHttpServer` instance.
- Notice that handlers are executed with the same order as registered. The first handler that accepts the path will be executed.

## Example

```csharp
using var server = new LightHttpServer();

// let's find an available port on the local machine, which is useful for unit tests
var testUrl = server.AddAvailableLocalPrefix();
server.Start();

server.HandlesPath("/health", context => context.Response.StatusCode = 200);
server.HandlesPath("/hello", async (context, cancellationToken) => {
	context.Response.ContentEncoding = Encoding.UTF8;
	context.Response.ContentType = "text/plain";
	var bytes = Encoding.UTF8.GetBytes("Hello World");
	await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
});
server.HandlesStaticFile("/style.css", "../assets/style.css");
```

# Why another HTTP server?
I was surprised when I could not find a valid HTTP server to use when writing tests.
Every project had some random problem. Some were targeting a small range of frameworks, such as .NET Framework. One of them could not handle simultaneous connections! The other one didn't support handling requests asynchronously. Then I figured this is a simple task to do, so I'm going to do it properly myself.