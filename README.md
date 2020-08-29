# Imgeneus

Imgeneus is a simple and elegant Shaiya EP8 socket server over the TCP/IP protocol, built with C# 8 and .NET Core Framework 3.0.

This project has been created for learning purposes about the network and game logic problematics on the server-side.
We choose [Shaiya](https://shaiya.fandom.com/wiki/Main_Page) because  it is a simple game, but enough complex to learn the basic functions of an MMO game architecture.
It's not about playing a game or competing with any services provided by Aeriagames or its partners, and we don't endorse such actions.

This repo also uses the best parts of these repos: [Drakkus/ShaiyaGenesis](https://github.com/Drakkus/ShaiyaGenesis), [Origin](https://github.com/aosyatnik/Origin) and original Imgeneus (removed by creator).

## Build and run
1. Build the project.
2. Find file in `Imgeneus.Database\config\database.json`. Update there to your values.
3. Setup `Imgeneus.Database` as startup project. Open View -> Other Windows -> Package Manager Console and run `Update-Database`. This should create database and run migrations.
4. To fulfill the database with example data open `src\Imgeneus.Database\Migrations\sql` folder and run `setup.bat` file (Don't forget to set your password there). This will populate your database with ep6 original data.
5. Run Imgeneus.Login.exe (or Imgeneus.Login project).
6. Run Imgeneus.World.exe (or Imgeneus.World project).

## Details
- Language:  `C# 8`
- Framework:  `.NET Core 3.0`
- Application type:  `Console`
- Database type:  `MySQL`
- Configuration files type:  `JSON`
- Environment: `Visual Studio 2019`

## Results
![image1](images/image1.JPG?raw=true "Title")
![image2](images/image2.JPG?raw=true "Title")

