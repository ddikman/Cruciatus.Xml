using System;
using System.Diagnostics;
using System.Linq;
using CommandLine;

namespace Cruciatus.Xml.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
                return;

            if (options.Debug)
                Debugger.Launch();

            if (string.IsNullOrEmpty(options.TestXPath))
                Console.WriteLine(new XmlRetriever(options.ProcessId).Parse());
            else
                Console.WriteLine(new XPathTester(options.ProcessId).Find(options.TestXPath));
        }
    }

    internal class CruciatusWrapper
    {
    }
}
