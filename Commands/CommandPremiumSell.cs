using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ZaupShop
{
    public class CommandPremiumSell : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "premiumsell";

        public string Help => "Allows you to sell items to the shop from your inventory for premium currency.";

        public string Syntax => "<name or id> [amount]";

        public List<string> Aliases => new List<string> { "psell" };

        public List<string> Permissions => new List<string> { "premiumsell" };

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.PremiumSell((UnturnedPlayer)playerid, msg);
        }
    }
}