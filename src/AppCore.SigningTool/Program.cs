using AppCore.SigningTool.Commands;
using AppCore.SigningTool.Extensions;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "dotnet-signtool",
                FullName = "AppCore .NET - Assembly Signing Tool for .NET Core"
            };

            app.VersionOptionFromAssemblyAttributes(typeof(Program).Assembly);
            app.HelpOption("-h|--help", true);
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            app.Command<SnCommand>();

            return app.Execute(args);
        }
    }
}
