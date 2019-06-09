# Genesis .Net
A framework for generating "things" based on a schema from a "source". It's designed to make starting a project a lot faster. It's entirely configurable; even scriptable. 

### How does it work?
Genesis is centered around a group of ObjectGraph objects and pieces of code that manipulate them, called Executors. 

There are currently two types of executors, though more could certainly be added.

`Input` executors deal with a "source". They're responsible for interrogating some data store (or weburl, or text file, or...) and populating a group of ObjectGraphs. They're available to all other executors at any point. (It's currently serial execution) 

`Output` executors do the other half of the work. They can use the data in the ObjectGraphs to write out classes, services, interfaces, clients etc. Anything really. They don't even have to write code.

* They each have their own configuration.json as well as an auto-mapped & Typed Configuration object. 

* Generators can have dependencies, or support files required to make the generated code work.

* Configuration of executors may also be done by commands at the Genesis Prompt. 

### genesis>
Genesis is a console application based on Microsoft's .Net standard/core. The commands are fairly simple to implement and use. Its usage and syntax are common to a shell. 

Here is a basic syntax of console commands:

``` bash
commandName [argument1] [anotherArg="Some Value"] 
```

The '<b>?</b>' command will list all of the commands that have been discovered. (They're extensible)
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/commands.png?raw=true "Command List")

### scan - Command

An example of the <b>scan</b> command. This loads configurations and initializes new executors. There's probably a case for it to automatically scan on startup, but for now you have to trigger it.
![alt text](https://github.com/genesisdotnet/genesis/blob/master/docs/images/scan.png?raw=true "Scan Example")

This will let you know what the commands they expose actually are. Once you know that you're able to manipulate their configurations and / or execute them.