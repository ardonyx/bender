﻿namespace Bender;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core;

internal class Program
{
    /// <summary>
    ///     Get version of this assembly
    /// </summary>
    private static string Version =>
        typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

    private static void Main(string[] args)
    {
        var parser = new ArgParser();
        var opts = parser.Parse(args);
        if (!opts.NoLogo)
        {
            Console.WriteLine($"Bender v{Version} Copyright {DateTime.Now.Year} Ardonyx LLC");
        }

        var ret = Run(opts);
        Environment.Exit(ret);
    }

    private static int Run(Options opts)
    {
        if (!opts)
        {
            Console.WriteLine(opts.Message);
            return 1;
        }

        var file = DataFile.From(opts.SpecFile);
        if (file.Empty)
        {
            Console.WriteLine("Spec file is empty, cannot be read... etc. I can't read it.");
            return 1;
        }

        var binary = DataFile.From(opts.BinaryFile);
        if (binary.Empty)
        {
            Console.WriteLine("Binary file is empty, cannot be read... etc. I can't read it.");
            return 1;
        }

        var spec = GetSpecFile(opts);
        if (spec == null)
        {
            return 1;
        }

        try
        {
            var bender = new BinaryParser(spec).Parse(binary);
            WriteToConsole(spec, bender);
            return 0;
        }
        catch (ParseException ex)
        {
            Console.WriteLine("Failed to parse binary file: {0}", ex.Message);
            return 1;
        }
    }

    /// <summary>
    ///     Print bender file to console
    /// </summary>
    /// <param name="spec">Original spec file</param>
    /// <param name="bender">Parsed binder</param>
    private static void WriteToConsole(SpecFile spec, Bender bender)
    {
        Console.WriteLine("Binary File - {0}{1}", spec.Name, Environment.NewLine);
        using var stream = new MemoryStream();
        var sw = new StreamWriter(Console.OpenStandardOutput())
        {
            AutoFlush = true
        };
        Console.SetOut(sw);

        try
        {
            var printer = new BenderPrinter();
            printer.WriteStream(bender, sw);
        }
        finally
        {
            sw.Dispose();
        }
    }

    private static SpecFile GetSpecFile(Options opts)
    {
        var paths = new List<string>();
        var specs = new List<SpecFile>();

        // Prefer to use the explicit file
        if (!string.IsNullOrEmpty(opts.SpecFile))
        {
            paths.Add(opts.SpecFile);
        }
        else if (!string.IsNullOrEmpty(opts.Root))
        {
            paths.AddRange(Directory.EnumerateFiles(opts.Root, "*.yaml", SearchOption.AllDirectories));
        }

        foreach (var p in paths)
        {
            try
            {
                specs.Add(SpecFile.Parse(DataFile.From(p)));
            }
            catch (ParseException ex)
            {
                Console.WriteLine("Error reading {0}: {1}", p, ex.Message);
            }
        }

        if (opts.PrintSpec)
        {
            foreach (var s in specs)
            {
                Console.WriteLine("Specification File: {0}", opts.SpecFile);
                Console.WriteLine(new string('=', 80));

                Console.WriteLine(s.ToString());

                Console.WriteLine("-- Spec End");
                Console.WriteLine(new string('=', 80));
            }
        }

        // Try to find spec for provided binary, no leading '.'
        var binExtension = Path.GetExtension(opts.BinaryFile)?.Remove(0, 1);
        var result = string.IsNullOrEmpty(opts.SpecFile)
            ? specs.FirstOrDefault(s => s.Extensions.Contains(binExtension))
            : specs.FirstOrDefault();

        if (result == null)
        {
            Console.WriteLine("No specification file found for \"{0}\" type. Try specifying " +
                              "a spec file with the -s flag or a spec root with the -r flag.", binExtension);
        }

        return result;
    }
}