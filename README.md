# Genesis .Net
An input-agnostic code generation tool written in C# 8.0 on .Net 3.0 core/standard. Data from pretty much any source that .Net is able to consume can be used to generate a variety of boilerplate code files. 

**Preview bits are required**

[.Net Core 3.0.100 Preview 6](https://github.com/dotnet/corefx/releases/tag/v3.0.0-preview6.19303.8) (or whatever version is current)

[Visual Studio Win/Mac Preview](https://visualstudio.com/preview "Visual Studio Win/Mac Preview") This will **only** compile using a preview version of Visual Studio until 3.0 and C# 8.0 are released.

## How does it work?
Genesis is centered around a group of ObjectGraph objects and pieces of code that manipulate them, called Executors. 

##### `Input` executors deal with a "source". (intentionally vague) 
They're responsible for interrogating some data store (or weburl, or text file, or...) and populating a group of ObjectGraphs. They're available to all other executors at any point. (It's currently serial execution)

##### `Output` executors do exactly that, output files.
They can use the data in the ObjectGraphs to write out classes, services, interfaces, clients, repositories etc. Anything really. They don't even have to write code. They could document...

##### `General` executors do everything else.
General executors don't necessarily "read" something like an input, and don't necessarily "write" something as an output. They do have access to the "current" ObjectGraphs in memory.

* They each have their own configuration.json as well as an auto-mapped (not AutoMapper :D) & Typed Configuration object. 

* OutputExecutors can have dependencies, or support files required to make the generated code work. This is open to refactoring

* Configuration of executors may also be done by commands at the Genesis Prompt or from within a script. 

## genesis>
Genesis is a console application (REPL or scripted REPL) based on Microsoft's .Net standard/core. The commands are fairly simple to implement and use. Its usage and syntax are common to a shell. 

Here is a basic syntax of console commands:

``` bash
commandName [argument1] [anotherArg="Some Value"] 
```

The '<b>?</b>' command will list all of the commands that have been discovered. (They're extensible)
![alt text](https://github.com/genesisdotnet/genesis/docs/images/commands.png?raw=true "Command List")

## Commands
An example of the <b>scan</b> command. This loads configurations and initializes new executors. There's probably a case for it to automatically scan on startup, but for now you have to trigger it.
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/scan.png?raw=true "Scan Example")

This will let you know what the commands they expose actually are. Once you know that you're able to manipulate their configurations and / or execute them.

<dl>
    <dt>scan</dt>
    <dd>Loads a default configuration and initializes executors that have been discovered.</dd>
    <dt>status</dt>
    <dd>Displays information about the current state of "things". You can see what's been discovered, what the names of executors are etc.</dd>
    <dt>add</dt>
    <dd>Add an executor to the current Chain. The chain is a linked list of executors. It's not quite a pipeline in that they don't feed into each other. They do share the same context though.</dd>
    <dt>exec</dt>
    <dd>Tells an executor to run immediately, or tells the chain to execute sequentially</dd>
</dl>

There are quite a few more commands, but those should produce some results. 

## Configuration
In addition to a .json file for each executor, they're able to be configured during execution from the prompt.

Say I wanted to set the connection string for an sql input executor:
```bash
scan
```
To prepare things and list out what executors are available. One of them is <b>'mssql'</b>. 

Use the <b>config</b> command to modify a configuration.
```bash
config mssql ConnectionString="Server=test;Database=db"
```

This changes the value of the ConnectionString property that lives on that executor's configuration class. Defaults are loaded at scan time and may be overwritten.
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/config.png?raw=true "Scan Example")

* non-string types don't use quotes

## Executing Executors
---
Once things are configured the way you want them, you would execute a generator.

Here is the <b>mssql</b> executor being used to populate a few ObjectGraphs for the other executors:
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/exec_mssql.png?raw=true "Execution Example")

At this point, any other executor (OutputExecutor / Output) would have ObjectGraphs that contained some schema. 

Lets create some <b>P</b>lain <b>O</b>ld <b>C</b># <b>O</b>bjects. (Pocos)
* <i>If you'll notice, there is an executor listed from the scan called <b>pocos</b>.</i>
 
They can use this schema to write out / act upon / completeley ignore the schema that was identified.

Here is the '<b>poco</b>' generator being executed after the '<b>mssql</b>' populator has been executed. The result is a class file with properties for each column in the source (mssql database table)
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/exec_poco.png?raw=true "Execution Example")
<i>Evidently it doesn't output anything right now. That'll be fixed ;)</i>

## Templates & Dependencies
---
Each output executor has a `[GeneratingExecutorClassName].gen` file that contains a simple "template" for it to use during its `Execute` procedure. In the case of the <b>poco</b> executor, it's just a class file that all the generated code will look like.

There are search and replace tokens that the generator can use. 

They have a `[GeneratingExecutorClassName].deps` file that contains all of the dependencies that the specific piece of code it generates will need to run properly. (Base classes, interfaceses, abstractions etc.)

## This sounds and looks like a pain in the ass
---
Agreed. It's not really though. 

Fortunately, <b>`you can script it`</b>. Its yet another tool for your development toolbox.  We all end up writing boilerplate code repetitively and often. When you realize you have code that will need typed a ton, write a OutputExecutor for them. More than likely they come from or are dictated-by some other source or authority. 

This is obviously not perfect. It's a simple scriptable prompt that feeds a REPL line-by-line. It can read a .genesis ('script') file and pass along each line to the interpreter as if it were typed to the console. 

It's activated by a <b>`--script [path]'</b> option:
```bash
genesis-cli --script "\path\to\script.genesis"
```

You could have a script to write out classes based on one source/input, then turn around and switch the input to something completely different, then write out different support classes with a seperate OutputExecutor. 

## What if?
* Pretty odd to think about, but you could integrate it into a build chain.
* Genesis doesn't <i>HAVE</i> to write out code. It could act on data that was within a table, rather than on its schema.
* The sources could literally come from anywhere. There's a ton of "schema definition" out there on the web.
* Generate a .Net Standard HttpClient wrapper for a specific Endpoint/Yaml.  

There are tools to do pretty much anything this could do. Though, if they had executors for Genesis..., it would be pretty slick!
