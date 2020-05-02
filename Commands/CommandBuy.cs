using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using Steamworks;

namespace ZaupShop
{
    public class CommandBuy : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "buy";

        public string Help => "Allows you to buy items from the shop.";

        public string Syntax => "[v.]<name or id> [amount] [25 | 50 | 75 | 100]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.Buy((UnturnedPlayer)playerid, msg);
        }
    }
}