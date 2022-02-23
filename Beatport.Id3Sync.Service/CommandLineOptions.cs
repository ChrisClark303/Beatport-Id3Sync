using Beatport.Id3Sync.TagManager;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Beatport.Id3Sync.Service
{

    public class CommandLineOptions : ITagProcessorOptions
    {
        [Value(index: 0, Required = true, HelpText = "Path to folder containing audio files to process.")]
        public string SourcePath { get;set; }

        [Value(index: 0, Required = true, HelpText = "Path to folder to copy processed files to.")]
        public string OutputPath { get; set; }
    }
}
