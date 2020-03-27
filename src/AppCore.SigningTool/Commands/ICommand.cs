using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand
    {
        string Name { get; }

        string Description { get; }

        void Configure(CommandLineApplication cmd);

        int OnExecute(CommandLineApplication cmd);
    }
}