# Genesis .Net
An exploritory orchestration-based code generation tool. Data from pretty much any source that .Net is able to consume can be used to generate a variety of boilerplate code files or execute arbitrary code.

*To run:*
[.Net Core 3](https://dotnet.microsoft.com/download/dotnet-core/3.0) Is required, but the Visual Studio 2019 installer should install it, so...

* [Visual Studio Win/Mac 2019](https://visualstudio.com "Visual Studio Win/Mac 2019") 

## How does it work?
Genesis is centered around a group of ObjectGraph objects and pieces of code that manipulate them, called [Executors](https://github.com/genesisdotnet/genesis/blob/master/src/Genesis/GenesisExecutor.cs). They describe objects, their properties and methods, events... datatypes of each... etc. 

##### `Input` executors deal with a "source". (intentionally arbitrary) 
They're responsible for interrogating some data store (or weburl, or text file, or...) and populating a group of ObjectGraphs described above. They're available to all other executors at any point. It's currently serial execution, via multicast delegate.

##### `Output` executors do exactly that, output files. (or anything else you code)
They can use the data in the ObjectGraphs to write out classes, services, interfaces, clients, repositories etc. Anything really. They don't even have to write code.

##### `General` executors do everything else.
General executors don't necessarily "read" something like an input, and don't necessarily "write" something as an output. They do have access to the current ObjectGraphs in memory though.

# First run
* The first thing that needs to happen is for the cli to scan for executors and make them available to Genesis. Do this by simply typing `scan`. (this is scriptable)
  * You should see what *[Executors](https://github.com/genesisdotnet/genesis/blob/master/src/Genesis/IGenesisExecutor.cs)* were found in the output.
  * They're addressable by their green text.
  
* Forgive the short docs... but
  * exec 'something in green text'
  
![Usage gif](https://github.com/genesisdotnet/genesis/blob/master/docs/gifs/demo.gif?raw=true)
