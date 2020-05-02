using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;

namespace ZaupShop
{
    public class CommandSale : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }
        public string Name
        {
            get
            {
                return "sale";
            }
        }
        public string Help
        {
            get
            {
                return "Allows you to put items on sale in your shop.";
            }
        }
        public string Syntax
        {
            get
            {
                return "sale";
            }
        }
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }
        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "sale.*", "sale.start", "sale.stop", "sale" };
            }
        }
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if ((!ZaupShop.Instance.Configuration.Instance.CanBuyItems || !ZaupShop.Instance.Configuration.Instance.CanBuyVehicles) || (!ZaupShop.Instance.Configuration.Instance.ItemSaleEnable || !ZaupShop.Instance.Configuration.Instance.VehicleSaleEnable))
            {
                UnturnedChat.Say(caller, ZaupShop.Instance.Translate("shop_disable"), "https://i.imgur.com/FeIvao9.png");
                return;
            }
            bool console = (caller == null) ? true : false;
            if (command.Length == 0)
            {
                ZaupShop.Instance.sale.MsgSale(caller);
            }
            if (command.Length == 1)
            {
                switch (command[0])
                {
                    case "stop":
                        if (caller.HasPermission("sale.*") || caller.HasPermission("sale.stop") || caller.HasPermission("*"))
                        {
                            ZaupShop.Instance.sale.ResetSale();
                            if (!console)
                                UnturnedChat.Say(caller, "You have stop the sales.", "https://i.imgur.com/3gOMlxE.png");
                            else
                                Logger.Log("Sale have stop");
                            UnturnedChat.Say(ZaupShop.Instance.Translate("sale_end"), "https://i.imgur.com/3gOMlxE.png");
                        }
                        break;
                    case "start":
                        if (caller.HasPermission("sale.*") || caller.HasPermission("sale.start") || caller.HasPermission("*"))
                        {
                            if (!ZaupShop.Instance.sale.salesStart)
                            {
                                ZaupShop.Instance.sale.StartSale();
                                if (!console)
                                    UnturnedChat.Say(caller, "You have started the sale!", "https://i.imgur.com/3gOMlxE.png");
                                else
                                    Logger.Log("Sales have started");
                                UnturnedChat.Say(ZaupShop.Instance.Translate("sale_started", ZaupShop.Instance.Configuration.Instance.SaleTime), "https://i.imgur.com/3gOMlxE.png");
                            }
                            else
                            {
                                if (!console)
                                    UnturnedChat.Say(caller, "Sales is still ongoing!", "https://i.imgur.com/3gOMlxE.png");
                                else
                                    Logger.Log("Sales is still ongoing!");
                                return;
                            }
                        }
                        break;
                    default:
                        UnturnedChat.Say(caller, "/sale - To Check Sale time, /sale stop - To stop or reset sale", "https://i.imgur.com/FeIvao9.png");
                        break;
                }
            }
        }
    }
}