using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace UnityBuildMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string LogFile = "UnityBuildMonitor.log";

            // Change logFile argument
            var argList = new List<string>(args);
            int index = argList.IndexOf("-logFile");
            if (index < 0)
            {
                argList.Add("-logFile");
                argList.Add(LogFile);
            }
            else if (index >= (argList.Count - 1))
                argList.Add(LogFile);
            else if (argList[index + 1] != "-")
                LogFile = argList[index + 1];
            else
                argList[index + 1] = LogFile;

            // Remove the log file if already exists
            if (File.Exists(LogFile))
                File.Delete(LogFile);
           
            // Start the process
            var proc = new Process();
            proc.StartInfo.FileName = "Unity.exe";            
            proc.StartInfo.Arguments = string.Join(" ", argList.Select(a => "\"" + a + "\""));            
            if (!proc.Start())
            {
                Console.Error.WriteLine("Could not start Unity process!");
                Environment.ExitCode = -1;
                return;
            }

            // Wait until the job is done dumping new text each second
            long filePos = 0;
            while (!proc.HasExited)
            {
                filePos = DumpNew(LogFile, filePos);
                System.Threading.Thread.Sleep(1000);
            }
            DumpNew(LogFile, filePos);
            Environment.ExitCode = proc.ExitCode;
            Console.Out.WriteLine("Process has finished! Execution time: " + (proc.ExitTime - proc.StartTime));

            // Remove the log file
            if (File.Exists(LogFile))
                File.Delete(LogFile);
        }

        public static long DumpNew(string path, long seek)
        {
            // If file does not exist, return old position
            if (!File.Exists(path))
                return seek;

            // Read the file
            using (var str = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                str.Seek(seek, SeekOrigin.Begin);
                using (TextReader tr = new StreamReader(str))
                {
                    Console.Write(tr.ReadToEnd());
                    return str.Position;
                }                
            }
        }
    }
}
