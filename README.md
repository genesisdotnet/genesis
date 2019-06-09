# Genesis .Net
---
A framework for generating "things" based on a schema from a "source". It's designed to make starting a project a lot faster. It's entirely configurable; even scriptable. 

## How does it work?
---
Genesis is centered around a group of ObjectGraph objects and pieces of code that manipulate them, called Executors. 

There are currently two types of executors, though more could certainly be added.

`Input` executors deal with a "source". They're responsible for interrogating some data store (or weburl, or text file, or...) and populating a group of ObjectGraphs. They're available to all other executors at any point. (It's currently serial execution) 

`Output` executors do the other half of the work. They can use the data in the ObjectGraphs to write out classes, services, interfaces, clients etc. Anything really. They don't even have to write code.

* They each have their own configuration.json as well as an auto-mapped & Typed Configuration object. 

* Generators can have dependencies, or support files required to make the generated code work.

* Configuration of executors may also be done by commands at the Genesis Prompt. 

## genesis>
---
Genesis is a console application based on Microsoft's .Net standard/core. The commands are fairly simple to implement and use. Its usage and syntax are common to a shell. 

Here is a basic syntax of console commands:

``` bash
commandName [argument1] [anotherArg="Some Value"] 
```

The '<b>?</b>' command will list all of the commands that have been discovered. (They're extensible)
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/commands.png?raw=true "Command List")

## Commands
---
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
---
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
