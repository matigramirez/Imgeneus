# Imgeneus

Imgeneus is a simple and elegant Shaiya EP3 socket server over the TCP/IP protocol, built with C# 8 and .NET Core Framework 3.0.

This project has been created for learning purposes about the network and game logic problematics on the server-side.
We choose [Shaiya](https://shaiya.fandom.com/wiki/Main_Page) because  it is a simple game, but enough complex to learn the basic functions of an MMO game architecture.

This repo also uses the best parts of these repos: [Drakkus/ShaiyaGenesis](https://github.com/Drakkus/ShaiyaGenesis), [Origin](https://github.com/aosyatnik/Origin) and [Imgeneus](https://github.com/Mun1z/Imgeneus).

## Build and run
1. Build the project.
2. Setup `Imgeneus.Database` as startup project. Open View -> Other Windows -> Package Manager Console and run `Update-Database`. This should create database and run migrations.
3. Run Imgeneus.Login.exe (or Imgeneus.Login project).
4. Run Imgeneus.World.exe (or Imgeneus.World project).

## Details
- Language:  `C# 8`
- Framework:  `.NET Core 3.0`
- Application type:  `Console`
- Database type:  `MySQL`
- Configuration files type:  `JSON`
- External libraries used:
	- [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
	- [NLog](https://github.com/NLog/NLog)
	- [BinarySerializer](https://github.com/jefffhaynes/BinarySerializer)
- Environment: `Visual Studio 2019`

## Results
![image1](images/image1.JPG?raw=true "Title")
![image2](images/image2.JPG?raw=true "Title")

