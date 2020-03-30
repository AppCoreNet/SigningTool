using System;
using System.Collections.Generic;
using AppCore.SigningTool.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.SigningTool.Extensions
{
    public static class CommandLineApplicationExtensions
    {
        public static void RegisterCommands(this CommandLineApplication app, IServiceProvider serviceProvider)
        {
            IEnumerable<ICommand> commands = serviceProvider.GetServices<ICommand>();
            foreach (ICommand command in commands)
            {
                RegisterCommand(app, serviceProvider, command);
            }
        }

        private static void RegisterCommands(CommandLineApplication app, IServiceProvider serviceProvider, Type parentType)
        {
            Type cmdType = typeof(ICommand<>).MakeGenericType(parentType);
            IEnumerable<object> commands = serviceProvider.GetServices(cmdType);
            foreach (ICommand command in commands)
            {
                RegisterCommand(app, serviceProvider, command);
            }
        }

        private static void RegisterCommand(CommandLineApplication app, IServiceProvider serviceProvider, ICommand cmd)
        {
            CommandLineApplication childApp = app.Command(
                cmd.Name,
                c =>
                {
                    c.Description = cmd.Description;
                    cmd.Configure(c);
                    c.OnExecute(() => cmd.Execute(c));
                });

            RegisterCommands(childApp, serviceProvider, cmd.GetType());
        }
    }
}