namespace Dos
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class CommandLine
    {
        public static string Execute(string command)
        {
            string str = "";
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c " + command) {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    str = reader.ReadToEnd();
                }
                process.WaitForExit();
            }
            return str.Trim();
        }
    }
}

