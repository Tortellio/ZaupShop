using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ZaupShop
{
    public class CommandSell : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "sell";

        public string Help => "Allows you to sell items to the shop from your inventory.";

        public string Syntax => "[v] <name or id> [amount]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.Sell((UnturnedPlayer)playerid, msg);
        }
    }
}