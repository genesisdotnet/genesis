# Genesis.Net
Genesis.Net is a code execution engine that produces an output based on an arbitrary source or schema.

* <b>`This is based on .Net Core 3.0 & c# 8.0 previews`</b>
* [<b>.Net Core 3.0 Preview download</b>](https://dotnet.microsoft.com/download/dotnet-core/3.0 "Download")
* [<b>Visual Studio Preview installer</b>](https://www.visualstudio.com/preview "Download Visual Studio Preview")

# Executors
An executor is a plugin to Genesis. They execute whatever code around a pseudo-context, or shared state. (not inside of a context) 
Each executor is free to manipulate the ObjectGraph as they choose. Obviously you could be a dick, but that's not the point. 

There are currently two types of Executors:

## Input
These are what feed an ObjectGraph context with schema information describing objects. 

* `Genesis.Input.MSSqlDb` 
    
Reads MSSql database schema into ObjectGraphs (max lengths, datatypes, names, etc.)

* `Genesis.Input.SwaggerUrl` 
    
Reads a swagger web address into ObjectGraphs (api calls, their models etc)

* `Genesis.Input.DotNetAssembly`

You could populate ObjectGraphs off of actual objects for whatever reason. 

## Output
There's a ton of things to potentially do and/or generate. Here's a few...

* `Genesis.Output.Poco`

Write out a (P)lain (O)ld (C)# (O)bject file according to whats in the ObjectGraphs.

* `Genesis.Output.AspNetMvcController`

A .Net Controller class that is tailored to schema information from the ObjectGraphs

* `Genesis.Output.AspNetDbContextCachedRepositories`

Sample caching repository for an Entity that was described in the ObjectGraphs

* `Genesis.Output.XamarinViewModel`

Generate a Xamarin Forms based ViewModel with INotifyPropertyChanged support

* `Genesis.Output.XamarinView`

Generate the .xaml markup for a Xamarin Forms Create/Edit view.

* `Genesis.Output.ProtobufEntitiy`

Generate a simple .proto file with CRUD actions and an entity

## Executor Development
Right now, all of the executors are referenced by the main Cli project to avoid build events and an overcomplicated dev process. They're still MEF'd in though.
