# Genesis .Net
An orchestration-based code generation tool. Data from pretty much any source that .Net is able to consume can be used to generate a variety of boilerplate code files. 

[.Net Core 3](https://github.com/dotnet/core/blob/master/release-notes/3.0/preview/3.0.0-preview7-download.md)

[Visual Studio Win/Mac 2019](https://visualstudio.com/ "Visual Studio Win/Mac")

## How does it work?
Genesis is centered around a group of ObjectGraph objects and pieces of code that manipulate them, called Executors. 

##### `Input` executors deal with a "source". (intentionally open) 
They're responsible for interrogating some data store (or weburl, or text file, or...) and populating a group of ObjectGraphs. They're available to all other executors at any point.

##### `Output` executors do exactly that, output files.
They can use the data in the ObjectGraphs to write out classes, services, interfaces, clients, repositories etc. Anything really. They don't even have to write code.

##### `General` executors do everything else.
General executors don't necessarily "read" something like an input, and don't necessarily "write" something as an output. They do have access to the current ObjectGraphs in memory.






## Will fix up docs soon. :)
