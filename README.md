# Genesis .Net
An orchestration-based code generation tool. Data from pretty much any source that .Net is able to consume can be used to generate a variety of boilerplate code files. 

**Preview bits are required**

[.Net Core 3 Release Candidate](https://dotnet.microsoft.com/download/dotnet-core/3.0)

[Visual Studio Win/Mac Preview](https://visualstudio.com/preview "Visual Studio Win/Mac Preview") This will **only** compile using a preview version of Visual Studio until 3.0 and C# 8.0 are released. (For now)

## How does it work?
Genesis is centered around a group of ObjectGraph objects and pieces of code that manipulate them, called Executors. 

##### `Input` executors deal with a "source". (intentionally open) 
They're responsible for interrogating some data store (or weburl, or text file, or...) and populating a group of ObjectGraphs. They're available to all other executors at any point. (It's currently serial execution)

##### `Output` executors do exactly that, output files.
They can use the data in the ObjectGraphs to write out classes, services, interfaces, clients, repositories etc. Anything really. They don't even have to write code.

##### `General` executors do everything else.
General executors don't necessarily "read" something like an input, and don't necessarily "write" something as an output. They do have access to the current ObjectGraphs in memory.






## Will fix up docs soon. :)
