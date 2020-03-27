using AppCore.SigningTool.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Extensions
{
    public static class CommandLineApplicationExtensions
    {
        public static CommandLineApplication Command<TCommand>(this CommandLineApplication app)
            where TCommand : ICommand, new()
        {
            var cmd = new TCommand();
            return app.Command(
                cmd.Name,
                c =>
                {
                    c.Description = cmd.Description;
                    cmd.Configure(c);
                    c.OnExecute(() => cmd.OnExecute(c));
                });
        }
    }
}