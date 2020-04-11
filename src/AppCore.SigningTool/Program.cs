using AppCore.SigningTool.Commands;
using AppCore.SigningTool.Commands.Sn;
using AppCore.SigningTool.Extensions;
using AppCore.SigningTool.StrongName;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.SigningTool
{
    class Program
    {
        static int Main(string[] args)
        {
            ServiceProvider serviceProvider = CreateServiceProvider();

            var app = new CommandLineApplication
            {
                FullName = "AppCore .NET - Assembly Signing Tool for .NET Core",
                Name = "dotnet-signtool"
            };

            app.MakeSuggestionsInErrorMessage = true;
            app.VersionOptionFromAssemblyAttributes(typeof(Program).Assembly);
            app.HelpOption("-h|--help", true);
            app.OnExecute(
                () =>
                {
                    app.ShowHelp();
                    return 0;
                });

            app.RegisterCommands(serviceProvider);

            try
            {
                return app.Execute(args);
            }
            catch (UnrecognizedCommandParsingException error)
            {
                app.Error.WriteLine(error.Message + ".");
                app.Out.WriteLine("Did you mean '{0}' ?", string.Join(' ', error.NearestMatches));
            }
            catch (CommandParsingException error)
            {
                app.Error.WriteLine(error.Message);
            }

            return ExitCodes.UnrecognizedCommandOrArgument;
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IStrongNameKeyGenerator, StrongNameKeyGenerator>();
            services.AddSingleton<IStrongNameKeyLoader, StrongNameKeyLoader>();
            services.AddSingleton<IStrongNameSigner, StrongNameSigner>();

            services.AddSingleton<ICommand, SnCommand>();
            services.AddSingleton<ICommand<SnCommand>, SnCreateKeyCommand>();
            services.AddSingleton<ICommand<SnCommand>, SnExportPublicKeyCommand>();
            services.AddSingleton<ICommand<SnCommand>, SnShowKeyCommand>();
            services.AddSingleton<ICommand<SnCommand>, SnSignCommand>();
        }
    }
}
