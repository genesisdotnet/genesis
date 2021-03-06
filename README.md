# Genesis .Net
An exploritory orchestration-based code generation tool. Data from pretty much any source that .Net is able to consume can be used to generate a variety of boilerplate code files or execute arbitrary code.

*To run:*
[Install DotNet](https://dotnet.microsoft.com/).

Visual Studio installer should install it, so...

* [Visual Studio Win/Mac 2019](https://visualstudio.com "Visual Studio Win/Mac 2019") 

## How does it work?

Busy year, will get back to this soon.

# First run
* The first thing that needs to happen is for the cli to scan for executors and make them available to Genesis. Do this by simply typing `scan`. (this is scriptable)
  * You should see what *[Executors](https://github.com/genesisdotnet/genesis/blob/master/src/Genesis/IGenesisExecutor.cs)* were found in the output.
  * They're addressable by their green text.
  
* Forgive the short docs... but
  * exec 'something in green text'
  
![Usage gif](https://github.com/genesisdotnet/genesis/blob/master/docs/gifs/demo.gif?raw=true)
