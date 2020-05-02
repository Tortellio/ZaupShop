using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;

namespace ZaupShop
{
    public class CommandPremiumSale : IRocketCommand
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
                return "premiumsale";
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
                return "[start | stop]";
            }
        }
        public List<string> Aliases
        {
            get { return new List<string> { "psale" }; }
        }
        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "psale.*", "psale.start", "psale.stop", "psale" };
            }
        }
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if ((!ZaupShop.Instance.Configuration.Instance.CanBuyPremiumItems || !ZaupShop.Instance.Configuration.Instance.CanBuyPremiumVehicles) || (!ZaupShop.Instance.Configuration.Instance.PremiumItemSaleEnable || !ZaupShop.Instance.Configuration.Instance.PremiumVehicleSaleEnable))
            {
                UnturnedChat.Say(caller, ZaupShop.Instance.Translate("pshop_disable"), "https://i.imgur.com/FeIvao9.png");
                return;
            }
            bool console = (caller == null) ? true : false;
            if (command.Length == 0)
            {
                ZaupShop.Instance.premiumsale.MsgPremiumSale(caller);
            }
            if (command.Length == 1)
            {
                switch (command[0])
                {
                    case "stop":
                        if (caller.HasPermission("psale.*") || caller.HasPermission("psale.stop") || caller.HasPermission("*"))
                        {
                            ZaupShop.Instance.premiumsale.ResetPremiumSale();
                            if (!console)
                                UnturnedChat.Say(caller, "You have stop the premium sales.", "https://i.imgur.com/3gOMlxE.png");
                            else
                                Logger.Log("Premium Sale have stop");
                            UnturnedChat.Say(ZaupShop.Instance.Translate("psale_end"), "https://i.imgur.com/3gOMlxE.png");
                        }
                        break;
                    case "start":
                        if (caller.HasPermission("psale.*") || caller.HasPermission("psale.start") || caller.HasPermission("*"))
                        {
                            if (!ZaupShop.Instance.premiumsale.PremiumsalesStart)
                            {
                                ZaupShop.Instance.premiumsale.StartPremiumSale();
                                if (!console)
                                    UnturnedChat.Say(caller, "You have started the premium sale!", "https://i.imgur.com/3gOMlxE.png");
                                else
                                    Logger.Log("Premium Sales have started");
                                UnturnedChat.Say(ZaupShop.Instance.Translate("psale_started", ZaupShop.Instance.Configuration.Instance.SaleTime), "https://i.imgur.com/3gOMlxE.png");
                            }
                            else
                            {
                                if (!console)
                                    UnturnedChat.Say(caller, "Premium Sales is still ongoing!", "https://i.imgur.com/3gOMlxE.png");
                                else
                                    Logger.Log("Premium Sales is still ongoing!");
                                return;
                            }
                        }
                        break;
                    default:
                        UnturnedChat.Say(caller, "/psale - To Check Sale time, /sale stop - To stop or reset sale", "https://i.imgur.com/FeIvao9.png");
                        break;
                }
            }
        }
    }
}