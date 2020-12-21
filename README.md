# Imgeneus

Imgeneus is a simple and elegant Shaiya EP8 socket server over the TCP/IP protocol, built with C# 8 and .NET Core Framework 3.1.

This project has been created for learning purposes about the network and game logic problematics on the server-side.
We choose [Shaiya](https://shaiya.fandom.com/wiki/Main_Page) because  it is a simple game, but enough complex to learn the basic functions of an MMO game architecture.
It's not about playing a game or competing with any services provided by Aeriagames or its partners, and we don't endorse such actions.

This repo also uses the best parts of these repos: [Drakkus/ShaiyaGenesis](https://github.com/Drakkus/ShaiyaGenesis), [Origin](https://github.com/aosyatnik/Origin) and original Imgeneus (removed by creator).

## Build and run
1. Build the solution.
2. Create 3 files `appsettings.Development.json` in Imgeneus.Login & Imgeneus.World & Imgeneus.Database projects. Override default configs with your development config. E.g. you can provide your password as follows:
```
{
  "Database": {
    "Password": "your_password"
  }
}
```
These files are added to the ignore list, so you can be sure, that you won't commit any of your credentials.
3. To fulfill the database with example data open `src\Imgeneus.Database\Migrations\sql` folder and run `setup.bat` file (Don't forget to set your password there). This will populate your database with ep6 original data.
4. Run Imgeneus.Login.exe (or Imgeneus.Login project).
5. Run Imgeneus.World.exe (or Imgeneus.World project).

## Details
- Language:  `C# 8`
- Framework:  `.NET Core 3.1`
- Application type:  `Console`
- Database type:  `MySQL`
- Configuration files type:  `JSON`
- Environment: `Visual Studio 2019`

## Results
![image1](images/image1.JPG?raw=true "Title")
![image2](images/image2.JPG?raw=true "Title")

## Hall of fame :star:
* __KSExtrez__ - firstly started the project and setup DI containers and web socket pool.
* __Cups__ - the first person, that created a dummy emulator, with his source code we could understand packet structure.
* __anton1312__ - helped with packet encryption algorithm implementation in C#.
* __Juuf__ - helped with Dye system packet structure.
* __matigramirez__ - helped with the implementation of different features.
* maybe __YOU__?

## Uknown issues
1. Sometimes Login server can not decrypt the first packet (with user name and password). Try to restart game.exe several times. Might be, that client caches old encryption key (not sure).
2. Many others, usually marked as `// TODO: <some comment>` throughout the code base.

Find the current state of development here: https://trello.com/b/lHvyQDuH/shaiya-imgeneus
