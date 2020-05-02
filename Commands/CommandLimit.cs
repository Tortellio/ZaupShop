using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ZaupShop
{
    public class CommandLimit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "limit";

        public string Help => "Tells you the limit of a selected item.";

        public string Syntax => "[v.]<name or id>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.Limit((UnturnedPlayer)playerid, msg);
        }
    }
}