using CommandLine;
using CommandLine.Text;

namespace Cruciatus.Xml.CLI
{
    class Options
    {
        [Option('p', "process", HelpText = "process id to get xml for.")]
        public int ProcessId { get; set; }

        [Option('d', "debug", HelpText = "launches the windows debugger on program start.")]
        public bool Debug { get; set; }

        [Option('t', "test", HelpText = "xpath to test for finding an element in the application.")]
        public string TestXPath { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return
                "Extracts the Winium.Cruciatus xml from an application.\r\n\r\n"
                + HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
