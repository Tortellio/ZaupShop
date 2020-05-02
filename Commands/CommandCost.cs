using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ZaupShop
{
    public class CommandCost : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "cost";

        public string Help => "Tells you the cost of a selected item.";

        public string Syntax => "[v.]<name or id>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.Cost((UnturnedPlayer) playerid, msg);
        }
    }
}