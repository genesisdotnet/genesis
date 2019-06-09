# Executors
Executors are basically chunks of code you can configure and execute.
They're broken up into two types as it stands, though this is arbitrary. 

## Input
* `Genesis.Input.MSSqlDb` 
    
Reads MSSql database schema into ObjectGraphs

* `Genesis.Input.YamlAddress` 
    
Reads a web address for a /swagger.yaml into ObjectGraphs

* `Genesis.Input.DotNetAssembly`

You could populate ObjectGraphs off of actual objects for whatever reason.

## Output
* `Genesis.Output.Poco`

Write out a (P)lain (O)ld (C)# (O)bject file according to whats in the ObjectGraphs

* `Genesis.Output.AspNetMvcController`

A .Net Controller class that is tailored to data found in the ObjectGraphs

* `Genesis.Output.AspNetDbContextCachedRepositories`

Sample caching repository for an Entity that was build from the ObjectGraphs

* `Genesis.Output.XamarinViewModel`

Generate a Xamarin Forms based ViewModel with INotifyPropertyChanged support

## Executor Development
Right now, all of the executors are referenced by the main Cli project to avoid build events and an overcomplicated dev process. They're still MEF'd in though.

