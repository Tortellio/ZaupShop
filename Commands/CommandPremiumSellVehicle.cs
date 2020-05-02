using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ZaupShop
{
    public class CommandPremiumSellVehicle : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "premiumsellvehicle";

        public string Help => "Allows you to sell premium vehicle to the shop.";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "psellv" };

        public List<string> Permissions => new List<string> { "premiumsellvehicle" };

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.PremiumSellVehicle((UnturnedPlayer)playerid, msg);
        }
    }
}