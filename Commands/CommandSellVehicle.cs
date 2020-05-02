using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ZaupShop
{
    public class CommandSellVehicle : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "sellvehicle";

        public string Help => "Allows you to sell vehicle to the shop.";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "sellv" };

        public List<string> Permissions => new List<string> { "sellvehicle" };

        public void Execute(IRocketPlayer playerid, string[] msg)
        {
            ZaupShop.Instance.SellVehicle((UnturnedPlayer)playerid, msg);
        }
    }
}