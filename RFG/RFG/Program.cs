using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Random File Generator
namespace RFG
{
  class Program
  {
    static void Main(string[] args)
    {
      int fileCount = -1;
      int minSize = -1;
      int maxSize = -1;
      bool randomize = false;
      string fileNameTemplate = "";
      string outputDirectory = "";

      var p = new OptionSet() {
   	    { "n=|number=", "Number of files",            (int n) => {fileCount = n; }},
        { "min=|minsize=", "Minimum file size in bytes", (int n) => {minSize = n;}},
        { "max=|maxsize=", "Maximum file size in bytes", (int n) => {maxSize = n;}},
        { "r|randomdata", "Toggle entering random data", r => {randomize = true; }},
        { "f=|filename=", "Allows file name template, {0} will be replaced by a sequence number", (string f) => {fileNameTemplate = f;}}, 
        { "o=|output=", "Target directory. Default: cwd/out/", (string d) => {outputDirectory = d;}}
        };

      p.Parse(args);

      // Sanitize User Input
      if (fileCount < 1) fileCount = 100;
      if (minSize < 0) minSize = 0; // Empty file
      if (maxSize < 0 || maxSize > 1024*1024*100) maxSize = 1024 * 1024; // Larger than 0 and max 100MB
      if (string.IsNullOrEmpty(fileNameTemplate)) fileNameTemplate = "random{0:X}.bin";
      if (string.IsNullOrEmpty(outputDirectory)) outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "out");

      Console.WriteLine(@"The program has been started with the following options: 
Number of files: {0}
Size between: {1}-{2}
Generating random data: {3}
Output directory: {4}", fileCount, minSize, maxSize, randomize ? "YES" : "NO", outputDirectory);

      if (AskQuestion("Are these settings correct? [Y/n]", true))
      {
        if (!Directory.Exists(outputDirectory) && AskQuestion(string.Format("Directory [{0}] does not exist, do you want to create it? [y/N]", outputDirectory), false))
        {
          Directory.CreateDirectory(outputDirectory);
        }
        Console.Write('\n');
        if (Directory.Exists(outputDirectory))
        {
          Random r = new Random();
          int cTop = Console.CursorTop;
          string sProgressFormat = "{0}/" + fileCount;
          for (int i = 0; i < fileCount; )
          {
            string path = Path.Combine(outputDirectory, string.Format(fileNameTemplate, i));
            if (File.Exists(path)) continue;

            Console.Write(sProgressFormat, ++i);
            Console.SetCursorPosition(0, cTop);

            int iSize = r.Next(minSize, maxSize);
            byte[] bytes = new byte[iSize];
            if (randomize) r.NextBytes(bytes);
            File.WriteAllBytes(path, bytes);
          }

          Console.Write('\n');
        }
        else
        {
          WriteInColor("Output directory does not exist, exiting now.", ConsoleColor.Red);
        }
      }
      Console.WriteLine("Finished. Press any key to close app.");
      Console.ReadKey();

    }

    private static bool AskQuestion(string sQuestion, bool bDefault)
    {
      Console.Write(sQuestion + " ");

      while (true)
      {
        ConsoleKeyInfo cki = Console.ReadKey();
        if (cki.Key == ConsoleKey.Y || cki.Key == ConsoleKey.J)
          return true;
        if (cki.Key == ConsoleKey.N)
          return false;
        if (cki.Key == ConsoleKey.Enter)
          return bDefault;
      }
    }

    private static void WriteLineInColor(string sText, ConsoleColor c)
    {
      WriteInColor(sText + Environment.NewLine, c);
    }

    private static void WriteInColor(string sText, ConsoleColor c)
    {
      ConsoleColor cBackup = Console.ForegroundColor;
      Console.ForegroundColor = c;
      Console.Write(sText);
      Console.ForegroundColor = cBackup;
    }
  }
}
