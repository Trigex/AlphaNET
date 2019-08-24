<p align="center">
  <img width="512" height="256" src="https://github.com/Trigex/AlphaNET/blob/master/Assets/Images/Banners/alphanet_banner_512.png">
</p>

It's AlphaNET, but the 4th rewrite!

Hacking games are a very niche and underexplored genre. You’ve got the bigger games like Hacker Experience, Hacknet, Hackmud, and Dark Signs/Hacking Simulator. However, here’s an issue with all these games: it’s nothing like actual hacking.

They are essentially puzzle games, with a hacking/computer theme. Some have programming (they’re the better ones!), which does make them a bit more accurate and realistic, but they’re not too in-depth. Hackmud for example, makes heavy use of JavaScript scripting, but to accomplish what in the end? Solving puzzles to get more of the in game currency. Hacking Simulator had programming (in an awful, awful original language), but it was quite limited. You could program your given server project, and do things locally, but that was about it. All these limitations frustrate me! What I want in a hacking game is a nice, open sandbox, with a complete scripting engine and API, so I can write whatever the hell I want; I want hacking that’s more than just a simple puzzle; I want a complete, virtual operating system environment to play in to my heart's content!

And that’s the goal of AlphaNET. AlphaNET aims to be a complete sandbox, with a fully featured scripting API, realistic and open operating system internals, complete customization, and most importantly: actual hacking and cracking!

## Building and editing

### Requirements
* .NET Core SDK 2.2 or higher
* .NET Framework 4.6.1 or higher (For `AlphaNET.Editor.Windows`)
* A solid IDE or editor with C# support, such as Visual Studio or Visual Studio Code

### Windows

For building and editing on Windows, it's recommended to use Visual Studio, with the default Build/Run functionallity. For building `AlphaNET.Clent.Console` for example, the primary client of this project, load `AlphaNET.sln` at the root of this project into Visual Studio, and set your startup project to `AlphaNET.Client.Console`, and click run.

### Linux

For Linux, it's recommended to use Visual Studio Code, or the dotnet cli. Before any build should be attempted, run `dotnet restore` at the root of this project, which pulls Nuget dependencies. Then, you can build and run `AlphaNET.Client.Console` for example, using `dotnet run --project AlphaNET.Client.Console` (Do note that you can use the dotnet cli method on Windows too, but you have Visual Studio on Windows, which is the greatest, so what's the point?!)

## Solution Structure

### AlphaNET.Framework (.NET Standard 2.0)

The core class library of AlphaNET, contains most client side functionality, and various components used by other AlphaNET projects

### AlphaNET.Client.Console (.NET Core 2.2 Console application)

The console client for the game, which depends on `AlphaNET.Framework`. Contains most platform specific changes that need to be made, and gets the game up and running.

### AlphaNET.Client.Visual (.NET Core 2.2 MonoGame application)

The visual client for the game, which depends on `AlphaNET.Framework`, it's like `AlphaNET.Client.Console`, but with added interfaces that allow for a graphics API, so users can create their own fully featured visual shells and applications. Out of the box, it comes with a basic window manager, display server, and tty implementation.

### AlphaNET.Editor (.NET Standard 2.0)

A GUI editor to interact with an AlphaNET Filesystem, utilizing `Eto.Forms`. Gives a tree view of a filesystem, and allows you to import arbitrary files, create files and directories, edit files, and planned to allow you to edit a live filesystem (Running in an AlphaNET client)

### AlphaNET.Editor.Windows/Linux (.NET Framework 4.6.1, .NET Core 2.2)

Projects for platform specific builds of AlphaNET.Editor, including respective UI toolkit dependences

### AlphaNET.Server (.NET Core 2.2)

The primary TCP server implementation for AlphaNET, facilitates all networking components of the game (Mainly Virtual Sockets and Virtual IPs, allowing for communication between two AlphaNET clients). It's planned to offer paid services for AlphaNET as well, such as AlphaHosting (Service which allows a user to control a server-hosted AlphaNET client, sort of like a VPS. Great for people who want to run an in game persistent application, but don't have the resources to run a client 24/7 themselves.)

## Discord

[AlphaNET Discord Invite](https://discord.gg/FynHbZM)

Personally, I would have opted for IRC, but...

## Documentation

[Binary Filesystem Schema](Docs/FS_SCHEMA.md)

[Networking Protocol](Docs/NETWORKING.md)