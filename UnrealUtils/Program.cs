// See https://aka.ms/new-console-template for more information

using CommandLine;
using UnrealUtils.Options;

namespace UnrealUtils {

    public class Program {

        public static void Main(string[] args) {

            Parser.Default.ParseArguments<Options.Options, ExtractOptions>(args)
                .WithParsed<ExtractOptions>(Extract.Run);
        }
    }
}