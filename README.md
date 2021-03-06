[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2Fgenesisdotnet%2Fgenesis%2Fbadge%3Fref%3Dmaster&style=flat)](https://actions-badge.atrox.dev/genesisdotnet/genesis/goto?ref=master)

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


## How to install as dotnet tool

if you nuget package is configured on your machine you can do this...

```shell
dotnet tool install genesis-cli --version 0.8.5-dev-UpdateBuildScript.0
```

if you want to test directly without publishing then reference the nuget folder 

```shell
dotnet tool install genesis-cli --version 0.8.5-dev-UpdateBuildScript.0 --add-source=Publish/Nuget
```

As a note, once published off of master and no pre-release info then this will install without the version