# UnrealUtil

Utility for Unreal projects. 

# Commands

The only command currently available is `extract`, which allows the user to extract a standalone Unreal application (Slate, more than likely) into its own directory with the minimum required dependencies. This

This is useful if you've built a slate application you need to distribute to team members who don't have access to the engine source. Art or QA tools, for example.

The usage is fairly simple. 

`UnrealUtil.exe extract --target-file {UE-source}\Engine\Binaries\Win64\UnrealInsights.target --destination C:/SomeFolder --engine-path {UE-source}`

for example `extract -t F:\UE_5.1\Engine\Binaries\Win64\UnrealInsights.target -d F:\Tests -e F:\UE_5.1`

You can also have it create a symlink of your executable with `--symlink {dir}`, however admin privileges is required on windows

`extract help` for more info.