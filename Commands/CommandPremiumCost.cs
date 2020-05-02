using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ZaupShop
{
    public class CommandPremiumCost : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "premiumcost";

        public string Help => "Tells you the cost of a selected item.";

        public string Syntax => "[v.]<name or id>";

        public List<string> Aliases => new List<string> { "pcost" };

        public List<string> Permissions => new List<string> { "premiumcost" };

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.PremiumCost((UnturnedPlayer)playerid, msg);
        }
    }
}