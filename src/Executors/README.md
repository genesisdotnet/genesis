# Executors

* `This is based on .Net Core 3.1 using C# 8.0`
* [.Net Core 3.1 download](https://dotnet.microsoft.com/download "Download")
* [Visual Studio installer](https://www.visualstudio.com/ "Download Visual Studio")

An executor is a plugin to Genesis. They execute arbitrary code around a pseudo-context, or shared state. *(not inside of a context)*
Each executor is free to manipulate the ObjectGraph as they choose. Obviously you could be a dick, but that's not the point.

`````c
//TODO: Get rid of the 'only' two types of executors program flow.
`````

## Input

These are what feed an ObjectGraph context with schema information describing objects.

* `Genesis.Input.MSSqlDb`

Reads `MSSql` database schema into ObjectGraphs (max lengths, datatypes, names, etc.)

* `Genesis.Input.MySqlDb`

Reads `MySql` database schema into ObjectGraphs (max lengths, datatypes, names, etc.)

* `Genesis.Input.SwaggerUrl`

Reads a `swagger.json` schema file into ObjectGraphs (api calls, their models etc)

* `Genesis.Input.DotNetAssembly`

You could populate ObjectGraphs off of actual objects from another .Net Core assembly.

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

## General

General executors basically just inspect and/or manipulate the data in the ObjectGraphs at any point. For debug purposes there is a tools -> dump command that simply writes out the current ObjectGraphs schema to an .xml file in the `Config.OutputDir` of your choice.

* `Genesis.Executors.GraphTools`

Tools library for the ObjectGraphs.

## Executor Development

Right now, all of the executors are referenced by the main Cli project to avoid build events and an overcomplicated dev process. They're still MEF'd in though.
