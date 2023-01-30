using System.Security.Principal;
using CommandLine;
using Newtonsoft.Json;
using UnrealUtils.Models;

namespace UnrealUtils.Options; 

[Verb("extract", HelpText = "")]
public class ExtractOptions {
    [Option('t', "target-file", Required = true, HelpText = "The path to the .target file of the standalone application you wish to extract")]
    public string TargetFilePath { get; set; }
            
    [Option('d', "destination", Required = true, HelpText = "The destination where the Unreal files should be copies to")]
    public string DestinationPath { get; set; }
    
    [Option('e', "engine-path", Required = true, HelpText = "The base path where your engine is located. Expects to point to the parent of your \\Engine directory")]
    public string EnginePath { get; set; }
    
    [Option('s', "symlink", HelpText = "Pass where to create a symlink to the executable standalone app")]
    public string CreateSymlinkPath { get; set; }
}

/// <summary>
/// Extraction option class - Extracts standalone application out of Unreal with minimal dependencies.
/// </summary>
public class Extract {

    private static string _internationalizationContentPath = "$(EngineDir)\\Content\\Internationalization";
    
    public static void Run(ExtractOptions options) {
        Console.WriteLine("Starting extraction of Unreal standalone application...");
        if (!Validate(options))
            return;

        var unrealTarget = ReadTargetFile(options.TargetFilePath);
        var enginePath = Path.GetFullPath(options.EnginePath);
        var destinationFolder = Path.Join(Path.GetFullPath(options.DestinationPath), unrealTarget.TargetName);
        
        CreateDirectories(unrealTarget, destinationFolder);
        CopyFiles(unrealTarget, destinationFolder, enginePath);
        
        var intDirRoot = _internationalizationContentPath.Replace("$(EngineDir)", Path.Join(enginePath,"Engine"));
        CopyDirectory(intDirRoot, destinationFolder, enginePath);
        
        if (!string.IsNullOrEmpty(options.CreateSymlinkPath)) {
            try {
                var symlinkExe = Path.Join(Path.GetFullPath(options.CreateSymlinkPath), $"{unrealTarget.TargetName}.exe");
                var executableOrigin = Path.Join(enginePath, unrealTarget.Launch.Replace("$(EngineDir)", "Engine"));
                File.CreateSymbolicLink(symlinkExe, executableOrigin);

                Console.WriteLine($"Symlink to {executableOrigin} successfully created at {symlinkExe}");
            }
            catch {
                Console.WriteLine("Failed to create symlink!");
                Environment.Exit((int)ExitCodes.GeneralFailure);
            }

        }
        
        Console.WriteLine("Successfully copied files");
        Environment.Exit((int)ExitCodes.Success);
    }
    
    /// <summary>
    /// Validate options provided and exit application if validation fails
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static bool Validate(ExtractOptions options) {
        
        if (!string.IsNullOrEmpty(options.CreateSymlinkPath)) {
#if _WINDOWS
            var winPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (winPrincipal.IsInRole(WindowsBuiltInRole.Administrator)) return true;
            
            Console.WriteLine("Unable to create a symlink on Windows unless running as Admin!\n Cancelling operation.");
            Environment.Exit((int)ExitCodes.GeneralFailure);
#endif
            
        }

        return true;
    }
    
    /// <summary>
    /// Parses Unreal .target file and returns a populated UnrealTarget object
    /// </summary>
    private static UnrealTarget ReadTargetFile(string targetFile) {

        if (!Path.Exists(targetFile) || Path.GetExtension(targetFile) != ".target") {
            Console.WriteLine($"Provided target file path {targetFile} is invalid");
            Environment.Exit((int)ExitCodes.InvalidFile);
        }
        using var sr = new StreamReader(targetFile);

        var jsonString = sr.ReadToEnd();
        var unrealTarget = JsonConvert.DeserializeObject<UnrealTarget>(jsonString);

        if (unrealTarget == null) {
            Console.WriteLine("Deserialized .target file resulted in a null or invalid object.\nPlease make sure your .target file is valid.");
            Environment.Exit((int)ExitCodes.GeneralFailure);
        }

        return unrealTarget;
    }

    /// <summary>
    /// Creates all directories listed in .target file
    /// </summary>
    private static void CreateDirectories(UnrealTarget unrealTarget, string destination) {
        Console.WriteLine("Creating directories...");
        
        var directoriesToCreate = new List<string>();
        directoriesToCreate.AddRange(unrealTarget.GetAllDirectories()
            .Select(x =>
                Path.GetDirectoryName(x.Path.Replace("$(EngineDir)", "Engine"))!)
            .Distinct());
        
        foreach (var dir in directoriesToCreate) {
            Directory.CreateDirectory(Path.Join(destination, dir));
        }

    }

    /// <summary>
    /// Copies required dependencies to the provided destination folder
    /// </summary>
    /// <param name="unrealTarget"></param>
    /// <param name="destinationFolder"></param>
    /// <param name="enginePath"></param>
    private static void CopyFiles(UnrealTarget unrealTarget, string destinationFolder, string enginePath) {
        Console.WriteLine("Copying files...");
        var filesToCopy = new List<string>(unrealTarget.RuntimeDependencies.Select(x => x.Path));
        filesToCopy.AddRange(unrealTarget.BuildProducts.Select(x => x.Path)); ;
        
        foreach (var fileRaw in filesToCopy) {

            var file = fileRaw.Replace("$(EngineDir)", "Engine");
            var fileDest = Path.Join(destinationFolder, file);
            var filePath = Path.Join(enginePath, file);
            
            File.Copy(filePath, fileDest, overwrite: true);
        }

    }
    
    /// <summary>
    /// Copies an entire engine directory to a new location
    /// </summary>
    private static void CopyDirectory(string root, string dest, string engineDir) {
        // Create directories
        foreach (var directory in Directory.GetDirectories(root,"*", SearchOption.AllDirectories)) {
            var newDir = directory.Replace(engineDir, dest);
            Directory.CreateDirectory(newDir);
        }

        // Copy files
        foreach (var file in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)) {
            var newFile = file.Replace(engineDir, dest);
            File.Copy(file, newFile, overwrite: true);
        }
    }
}
