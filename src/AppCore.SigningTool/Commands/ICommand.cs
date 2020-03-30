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

        int Execute(CommandLineApplication cmd);
    }

    /// <summary>
    /// Represents a child command.
    /// </summary>
    /// <typeparam name="TParent">The parent command type.</typeparam>
    public interface ICommand<TParent> : ICommand
        where TParent : ICommand
    {
    }
}