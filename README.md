# Genesis .Net
Genesis a code generator. It is extensible and could do more than just generate boiler plate code. It's written in c#.

Given a source of data, Genesis and its Generators are free to consume schema information and/or data from an interrogated source. Right now there is only one source (Populator) written, but many more are possible. (NoSQL, CSV, Web Request)

Here's the basic flow for now:
* Configure an input source (Populator for now, cringe)
* Configure a Generator as an output target
* Execute the configuration with `go`

It's pretty cool because you can script it with a series of different sources or different generators. It doesn't even necessarily have to write code. It could call a webservice?

* This isn't really anything new, but it was a fun project that could be useful to someone else. 

# Some Possible Use Cases
Thing you can and could do...
* Write out a plain old csharp object file based on the column schema of a database table
`DatabaseTableNamePoco.cs` (for now)
* Create customized ViewModels for entities from anywhere you can write code to 'talk to'
`ViewModels/DatabaseTableNameViewModel.cs` 
* Asp.Net Core Controllers for a given entity / table column
`Controllers/DatabaseTableNameController.cs`
* Boilerplate code that is easily able to be generated. 
* Web calls or duties in general from a specific table of tasks?
* I'm not sure where this is going :D

That's just one input source right now, using the MSSqlDb Populator. 

These are all currently prototypes, and they're all coming from the same input source. 

The general read->process->write works, but needs a lot more input sources and output generators! 

# Basic Template Support
When a `scan` picks up a `Generator`, it checks to see if it has a corresponding `GeneratorClassName.gen` file. (Cheesy convention for templates?)

It simply reads all text from the file and maps it to the `.Template` property of the Generator, so when the Generator executes, it can do a simple search and replace or whatever it wants to. (Again, for now)

## .Net Standard / .Net Core
All of the code thus far is wriiten in c# (with some parts in 3.0 pre-release) The build warnings are not suppressed. 

### Notable Depenencies:
This uses whatever the preview version of .Net / Visual Studio is available for now. This will change. 

* [Microsoft.Extensions.CommandLineUtils](https://www.areilly.com/2017/04/21/command-line-argument-parsing-in-net-core-with-microsoft-extensions-commandlineutils/ "Well explained at O'Reilly") From O'Reilly
* [Microsoft.Extensions.Configuration.CommandLine](https://msdn.microsoft.com/en-us/magazine/mt763239.aspx "Mark lays this framework out quite nicely") Thanks Mark!
* [Microsoft.Extensions.Configuration.Binder](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder "Handy class for mapping a config to an object") Handy class from Microsoft
* [Microsoft.Extensions.Configuarion](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.json?view=aspnetcore-2.2 "General configuration support") Json Support
* [Microsoft.Extensions.Configuarion.Json](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.jsonconfigurationextensions?view=aspnetcore-2.2 ".json configuration support") Json extension methods
* [Microsoft.Composition](https://dotnetthoughts.net/using-mef-in-dotnet-core/ "Managed Extensibility Framework docs") Good starting point for MEF

## Quickie Example
`genesis-cli`
Open a Genesis prompt. It's just a simple loop that executes `GenesisCommand`s.
    
`genesis> <command> [argument[s]]` 
Execute a command with whatever arguments are provided, like normal command line arguments

`genesis --script "./myscript.genesis"`
Executes each line as if it were typed to the console prompt. 

Here are a few:

```dotnet
?         Inception, display this information
help      We'll try to
 
clear     Sets Populator and Generator (to be changed) to null
exit      Exit the app or currently scoped IGenesisExecutor
 
scan      Scan the filesystem for Producers and Generators within assemblies
status    Display information about the configuration

pop       Scope to a Populator
gen       Scope to a Generator
scope     (Soon) Manipulate an IGenesisExecutor's Config property        

go        Execute the current configuration.
exec      (Soon) Execute an IGenesisExecutor by alias (Populator or Generator)

update    Eventually download latest and restart
```

# Execute a script:

```bash
genesis-cli --script "./sql2poco.genesis"
```

### a script with illegal comments:
Comments <b>aren't</b> handled yet. This configures the [Genesis.Input.MSSqlDb]("https://github.com/genesisdotnet/genesis/src/Populators/Genesis.Input.MSSqlDb "Sql Server Source") to be the input, or Populator and the [Genesis.Output.Poco]("https://github.com/genesisdotnet/genesis/src/Generators/Genesis.Output.Poco "Poco Output Source") Generator as the output.

```
 scan          //load all of the Populators and Generators
 pop mssql     //configure (scope to) the Populator with the key "mssql", which reads an sql database
 config mssql ConnectionString="Server=xxxxx;Database=dbName"
 gen poco      //configure (scope to) the Generator with the key "poco". Maybe it just writes out c# classes of nothing
 go            //run the currently configured Populator then Generator. They share the GenesisContext and are worthless alone.
