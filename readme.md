# LlamaCpp.Net

<center>

![LlamaCpp.Net logo](assets/llama-glyph-256.png)

</center>

LlamaCpp.Net is a .NET wrapper for [llama.cpp](https://github.com/ggerganov/llama.cpp), a C++ library for dealing with large language models.

In particular, we try to abstract away the complexity of dealing with the C++ code and provide a simple interface for using the library in .NET applications.
Nobody wants to deal with pointers and memory management in C#.

## Features

- **Fast** - LlamaCpp.Net is a thin wrapper around llama.cpp, which is written in C++ and is very fast
- **Idiomatic** - If there's a C# way of doing something, we do it the C# way.
- **Cross-platform** - LlamaCpp.Net is written in C# and can be used on Windows, Linux and macOS
- **Open-source** - LlamaCpp.Net is open-source and is licensed under the terms of the Apache 2.0 license
- **Batteries included** - LlamaCpp.Net comes with pre-built runtime binaries for different platforms and architectures

## Installation

LlamaCpp.Net is available as a [NuGet package](https://www.nuget.org/packages/LlamaCpp.Net/).
Additionally, you'll need to install the runtime binaries for your platform and architecture.
These are, conveniently, also available as [NuGet packages](https://www.nuget.org/packages?q%253DLlamaCpp.Net).

## Getting started

For a quick start, check out the [examples](examples) folder, or check out the Wiki in Github.

## Building from source

We use [Cake](https://cakebuild.net/) as our build system, so our build scripts are written in C#, and can be run on Windows, Linux and macOS.

As the underlying dependencies may be changed, please refer to the [llama.cpp](https://github.com/ggerganov/llama.cpp) repositories for which dependencies are required.
After that, it's as simple as running `.\build.ps1 -t SetupDevelopmentEnvironment` on Windows, or `./build.sh -t SetupDevelopmentEnvironment` on Linux and macOS.

## Llama icon

Llama icon by [Gedeon Maheux](https://www.iconhot.com/icon/the-emperor39s-new-groove/llama-glyph.html)

## Contact

If you have any questions or need help, please contact us on [Discord](https://discord.gg/GtPWFSGbye).

## License

LlamaCpp.Net is licensed under the terms of the Apache 2.0 license.

[llama.cpp](https://github.com/ggerganov/llama.cpp) is licensed under the terms of the MIT license.

