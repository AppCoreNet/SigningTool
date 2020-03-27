using AppCore.SigningTool.Commands.Sn;
using AppCore.SigningTool.Extensions;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands
{
    public class SnCommand : ICommand
    {
        public string Name => "sn";

        public string Description => "Assembly strong naming commands.";

        public void Configure(CommandLineApplication cmd)
        {
            cmd.Command<SnCreateKeyCommand>()
               .Command<SnSignCommand>();
        }

        public int OnExecute(CommandLineApplication cmd)
        {
            cmd.ShowRootCommandFullNameAndVersion();
            cmd.ShowHelp();
            return 0;
        }
    }
}