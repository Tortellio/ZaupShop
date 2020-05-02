using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using Steamworks;

namespace ZaupShop
{
    public class CommandPremiumBuy : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "premiumbuy";

        public string Help => "Allows you to buy items from the shop with premium currency.";

        public string Syntax => "[v.]<name or id> [amount] [25 | 50 | 75 | 100]";

        public List<string> Aliases => new List<string> { "pbuy" };

        public List<string> Permissions => new List<string> { "premiumbuy" };

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.PremiumBuy(UnturnedPlayer.FromCSteamID(new CSteamID(ulong.Parse(playerid.Id))), msg);
        }
    }
}