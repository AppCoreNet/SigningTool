using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AppCore.SigningTool.StrongName
{
    public static class Exec
    {
        private static string NetFxToolsPath =
            "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v10.0A\\bin\\NETFX 4.8 Tools\\";

        public static int Run(string program, IEnumerable<string> arguments)
        {
            var startInfo = new ProcessStartInfo(program);
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            foreach (string argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            return process.ExitCode;
        }

        public static int RunNetFxTool(string program, IEnumerable<string> arguments)
        {
            return Run(Path.Combine(NetFxToolsPath, program), arguments);
        }
    }
}