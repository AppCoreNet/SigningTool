using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands
{
    public class SnCommand : ICommand
    {
        public string Name => "sn";

        public string Description => "Assembly strong naming commands.";

        public void Configure(CommandLineApplication cmd)
        {
        }

        public int Execute(CommandLineApplication cmd)
        {
            cmd.ShowRootCommandFullNameAndVersion();
            cmd.ShowHelp();
            return 0;
        }
    }
}