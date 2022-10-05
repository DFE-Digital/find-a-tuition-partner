using Infrastructure.DataImport;

if (await Import.RunImport(args)) return;

Console.WriteLine("Pass one of the following commands: import");