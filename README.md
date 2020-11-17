# UnityBuildMonitor
Very simple utility, polling log file each second until the Unity process exits.
Useful for Windows build pipelines.

## How to use?
Place this utility next to the *Unity.exe* or append Unity path to the *PATH* environment vaiable.
Just replace *Unity.exe* for *UnityBuildMonitor.exe* in your pipeline commands.
Utility respects *-logFile* unless console output is specified (which obviously does not work on Windows).