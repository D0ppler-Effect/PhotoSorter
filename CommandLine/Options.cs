using CommandLine;

namespace PhotoSorter.CommandLine
{
    public class Options
    {
        [Option(Required = true)]
        public string Source { get; set; }

        [Option(Required = true)]
        public string Target { get; set; }
    }
}