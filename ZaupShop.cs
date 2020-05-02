using System;
using System.Linq;
using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace ZaupShop
{
    public class ZaupShop : RocketPlugin<ZaupShopConfiguration>
    {
        public DatabaseMgr ShopDB;

        public DatabaseMgrAuction AuctionDB;

        public static ZaupShop Instance;

        public delegate void PlayerShopBuy(UnturnedPlayer player, decimal amt, byte items, ushort item,
            string type = "item");

        public event PlayerShopBuy OnShopBuy;

        public delegate void PlayerShopSell(UnturnedPlayer player, decimal amt, byte items, ushort item);

        public event PlayerShopSell OnShopSell;

        public Sales sale;
        public PremiumSales premiumsale;
        #region Translations
        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                {
                    "buy_command_usage",
                    "Usage: /buy [v] <name or id> [amount]."
                },
                {
                    "pbuy_command_usage",
                    "Usage: /pbuy [v] <name or id> [amount]."
                },
                {
                    "cost_command_usage",
                    "Usage: /cost [v] <name or id>."
                },
                {
                    "pcost_command_usage",
                    "Usage: /pcost [v] <name or id>."
                },
                {
                    "limit_command_usage",
                    "Usage: /limit [v] <name or id>."
                },
                {
                    "plimit_command_usage",
                    "Usage: /plimit [v] <name or id>."
                },
                {
                    "sell_command_usage",
                    "Usage: /sell <name or id> [amount]."
                },
                {
                    "sellv_command_usage",
                    "Usage: /sellv"
                },
                {
                    "psell_command_usage",
                    "Usage: /psell <name or id> [amount]."
                },
                {
                    "psellv_command_usage",
                    "Usage: /psellv"
                },
                {
                    "shop_command_usage",
                    "Usage: /shop <add/rem/chng/buy/lim/res> [v] <itemid> <cost> <cost> is not required for rem, buy is only for items."
                },
                {
                    "pshop_command_usage",
                    "Usage: /pshop <add/rem/chng/buy/lim/res> [v] <itemid> <cost> <cost> is not required for rem, buy is only for items."
                },
                {
                    "error_giving_item",
                    "There was an error giving you {0} (ID: {1}).  You have not been charged."
                },
                {
                    "error_getting_cost",
                    "There was an error getting the cost of {0} (ID: {1})!"
                },
                {
                    "error_getting_limit",
                    "There was an error getting the limit of {0} (ID: {1})!"
                },
                {
                    "item_cost_msg",
                    "The item {0} (ID: {5}) costs {1} {2} to buy and gives {3} {4} when you sell it."
                },
                {
                    "item_limit_msg",
                    "The item {0} (ID: {3}) is limited to {1} to buy and has been bought {2} times."
                },
                {
                    "item_unlimit_msg",
                    "The item {0} (ID: {2}) is unlimited and has been bought {1} times."
                },
                {
                    "vehicle_limit_msg",
                    "The vehicle {0} (ID: {3}) is limited to {1} to buy and has been bought {2} times."
                },
                {
                    "vehicle_unlimit_msg",
                    "The vehicle {0} (ID: {2}) is unlimited and has been bought {1} times."
                },
                {
                    "vehicle_cost_msg",
                    "The vehicle {0} (ID: {5}) costs {1} {2} to buy and gives {3} {4} when you sell it."
                },
                {
                    "item_buy_msg",
                    "You have bought {5}x {0} for {1} {2}.  You now have {3} {4}."
                },
                {
                    "item_limited",
                    "You can't buy {0} (ID: {1}). This item buy limit has been reached. Status: {2}/{3}"
                },
                {
                    "vehicle_buy_msg",
                    "You have bought 1x {0} for {1} {2}.  You now have {3} {4}."
                },
                {
                    "vehicle_limited",
                    "You can't buy {0} ({1}). This vehicle buy limit has been reached. Status: {2}/{3}"
                },
                {
                    "not_enough_currency_msg",
                    "You do not have enough {0} to buy {1} {2}."
                },
                {
                    "buy_items_off",
                    "I'm sorry, but the ability to buy items is turned off."
                },
                {
                    "buy_vehicles_off",
                    "I'm sorry, but the ability to buy vehicles is turned off."
                },
                {
                    "item_not_available",
                    "I'm sorry, but {0} (ID: {1}) is not available in the shop."
                },
                {
                    "pitem_not_available",
                    "I'm sorry, but {0} (ID: {1}) is not available in the premium shop."
                },
                {
                    "vehicle_not_available",
                    "I'm sorry, but {0} (ID: {1}) is not available in the shop."
                },
                {
                    "pvehicle_not_available",
                    "I'm sorry, but {0} (ID: {1}) is not available in the premium shop."
                },
                {
                    "could_not_find",
                    "I'm sorry, I couldn't find an id for {0}."
                },
                {
                    "not_in_vehicle",
                    "You are not inside any vehicle."
                },
                {
                    "vehicle_wrong",
                    "The vehicle you are looking at is {0} (ID: {1})! Not {2} (ID: {3})."
                },
                {
                    "sell_items_off",
                    "I'm sorry, but the ability to sell items is turned off."
                },
                {
                    "sell_vehicles_off",
                    "I'm sorry, but the ability to sell vehicles is turned off."
                },
                {
                    "not_have_item_sell",
                    "I'm sorry, but you don't have any {0} (ID: {1}) to sell."
                },
                {
                    "not_have_vehicle_sell",
                    "I'm sorry, but you don't own this vehicle (ID: {0})."
                },
                {
                    "not_enough_items_sell",
                    "I'm sorry, but you don't have {0}x {1} (ID: {2}) to sell."
                },
                {
                    "not_enough_ammo_sell",
                    "I'm sorry, but you don't have enough ammo in {0} (ID: {1}) to sell."
                },
                {
                    "sold_items",
                    "You have sold {0}x {1} (ID: {6}) to the shop and receive {2} {3} in return. Your balance is now {4} {5}."
                },
                {
                    "psold_items",
                    "You have sold {0}x {1} (ID: {6}) to the premium shop and receive {2} {3} in return. Your balance is now {4} {5}."
                },
                {
                    "sold_vehicles",
                    "You have sold {0} (ID: {1}) to the shop and receive {2} {3} in return. Your balance is now {4} {5}."
                },
                {
                    "psold_vehicles",
                    "You have sold {0} (ID: {1}) to the premium shop and receive {2} {3} in return. Your balance is now {4} {5}."
                },
                {
                    "no_sell_price_set",
                    "The shop is not buying {0} (ID: {1}) right now"
                },
                {
                    "pno_sell_price_set",
                    "The premium shop is not buying {0} (ID: {1}) right now"
                },
                {
                    "no_itemid_given",
                    "An itemid is required."
                },
                {
                    "no_vehicleid_given",
                    "An vehicleid is required."
                },
                {
                    "no_cost_given",
                    "A cost is required."
                },
                {
                    "no_buyback_given",
                    "A buyback amount is required."
                },
                {
                    "no_limit_given",
                    "A limit is required."
                },
                {
                    "invalid_amt",
                    "You have entered in an invalid amount."
                },
                {
                    "v_not_provided",
                    "You must specify v for vehicle or just an item id.  Ex. /shop rem/101"
                },
                {
                    "pv_not_provided",
                    "You must specify v for vehicle or just an item id.  Ex. /pshop rem/101"
                },
                {
                    "invalid_id_given",
                    "You need to provide a valid item or vehicle id."
                },
                {
                    "invalid_cost_given",
                    "You need to provide a valid cost amount."
                },
                {
                    "invalid_buyback_given",
                    "You need to provide a valid cost amount."
                },
                {
                    "invalid_limit_given",
                    "You need to provide a valid limit amount."
                },
                {
                    "no_permission_shop_chng",
                    "You don't have permission to use the shop chng msg."
                },
                {
                    "no_permission_shop_add",
                    "You don't have permission to use the shop add msg."
                },
                {
                    "no_permission_shop_rem",
                    "You don't have permission to use the shop rem msg."
                },
                {
                    "no_permission_shop_buy",
                    "You don't have permission to use the shop buy msg."
                },
                {
                    "no_permission_shop_lim",
                    "You don't have permission to use the shop lim msg."
                },
                {
                    "no_permission_pshop_chng",
                    "You don't have permission to use the premium shop chng msg."
                },
                {
                    "no_permission_pshop_add",
                    "You don't have permission to use the premium shop add msg."
                },
                {
                    "no_permission_pshop_rem",
                    "You don't have permission to use the premium shop rem msg."
                },
                {
                    "no_permission_pshop_buy",
                    "You don't have permission to use the premium shop buy msg."
                },
                {
                    "no_permission_pshop_lim",
                    "You don't have permission to use the premium shop lim msg."
                },
                {
                    "changed",
                    "changed"
                },
                {
                    "added",
                    "added"
                },
                {
                    "changed_or_added_to_shop",
                    "You have {0} the {1} (Cost: {2}, Buyback: {3}, Limit: {4}) to the shop."
                },
                {
                    "changed_or_added_to_pshop",
                    "You have {0} the {1} (Cost: {2}, Buyback: {3}, Limit: {4}) to the premium shop."
                },
                {
                    "changed_limit_to_shop",
                    "You have {0} the {1} with limit {2}x to the shop."
                },
                {
                    "changed_limit_to_pshop",
                    "You have {0} the {1} with limit {2}x to the premium shop."
                },
                {
                    "error_adding_or_changing",
                    "There was an error adding/changing {0}! Usage: /shop add [v] <name | id> <cost> <buyback> <limit>."
                },
                {
                    "error_limiting",
                    "There was an error limiting {0}!"
                },
                {
                    "removed_from_shop",
                    "You have removed the {0} from the shop."
                },
                {
                    "removed_from_pshop",
                    "You have removed the {0} from the premium shop."
                },
                {
                    "not_in_shop_to_remove",
                    "{0} wasn't in the shop, so couldn't be removed."
                },
                {
                    "not_in_pshop_to_remove",
                    "{0} wasn't in the premium shop, so couldn't be removed."
                },
                {
                    "not_in_shop_to_reset",
                    "{0} wasn't in the shop, so couldn't be reseted."
                },
                {
                    "not_in_pshop_to_reset",
                    "{0} wasn't in the premium shop, so couldn't be reseted."
                },
                {
                    "not_in_shop_to_set_buyback",
                    "{0} isn't in the shop so can't set a buyback price."
                },
                {
                    "set_buyback_price",
                    "You set the buyback price for {0} to {1} in the shop."
                },
                {
                    "invalid_shop_command",
                    "You entered an invalid shop command."
                },
                {
                    "removed_from_pshop",
                    "You have removed the {0} from the premium shop."
                },
                {
                    "not_in_pshop_to_remove",
                    "{0} wasn't in the premium shop, so couldn't be removed."
                },
                {
                    "not_in_pshop_to_set_buyback",
                    "{0} isn't in the premium shop so can't set a buyback price."
                },
                {
                    "pset_buyback_price",
                    "You set the buyback price for {0} to {1} in the premium shop."
                },
                {
                    "invalid_pshop_command",
                    "You entered an invalid premium shop command."
                },
                {
                    "reset_from_shop",
                    "You have reset the {0} from the shop."
                },
                {
                    "reset_from_pshop",
                    "You have reset the {0} from the premium shop."
                },
                {"shop_disabled", "Item and Vehicle shop are disabled!"},
                {"pshop_disabled", "Premium Item and Vehicle shop are disabled!"},
                {"sale_start", "The sale is starting in {0} seconds!"},
                {"sale_started", "The sale has started and will end in {0} minutes! Everything in the shop is now on sale!"},
                {"sale_command","The next sale will start in {0} Minutes"},
                {"sale_end","Sales have ended, all price are back to normal"},
                {"sale_ending", "Sale is ending in {0} {1}"},
                {"psale_start", "The premium sale is starting in {0} seconds!"},
                {"psale_started", "The premium sale has started and will end in {0} minutes! Everything in the shop is now on sale!"},
                {"psale_command","The next premium sale will start in {0} Minutes"},
                {"psale_end","Premium Sales have ended, all price are back to normal"},
                {"psale_ending", "Premium Sale is ending in {0} {1}"},
                {"auction_command_usage","/auction add, /auction list, /auction find, /auction cancel"},
                {"auction_addcommand_usage","/auction <Item Name or ID> <Price>"},
                {"auction_addcommand_usage2","Missing <Price> Parameter"},
                {"auction_disabled","Auction is disabled."},
                {"not_have_item_auction", "You do not have any {0} for auctioning."},
                {"auction_item_notinshop","The item you are auctioning is not available from the shop."},
                {"auction_item_mag_ammo","Unable to auction magazines or ammo!"},
                {"auction_item_succes","You have placed {0} on auction for {1} {2}"},
                {"auction_item_failed","Fail to place item on auction"},
                {"auction_unequip_item","Please de-equip {0} first before auctioning"},
                {"auction_buycommand_usage","/auction buy <ID 0 - 9 ...>"},
                {"auction_addcommand_idnotexist","Auction ID does not exist!"},
                {"auction_buy_msg","You got item {0} for {1} {2}.  You now have {3} {4}."},
                {"auction_notexist","Auction ID does not exist"},
                {"auction_notown","You do not own that auction!"},
                {"auction_cancelled","You have remove Auction {0}"},
                {"auction_cancelcommand_usage","/auction cancel [Auction ID]"},
                {"auction_findcommand_usage","/auction find [Item Name or ID]"},
                {"auction_find_invalid","Invalid Item ID or Item Name"},
                {"auction_find_failed","No item found with that ID/Name"},
            };
        #endregion
        protected override void Load()
        {
            Instance = this;
            ShopDB = new DatabaseMgr();
            AuctionDB = new DatabaseMgrAuction();
            if ((Configuration.Instance.ItemSaleEnable || Configuration.Instance.VehicleSaleEnable) && (Configuration.Instance.CanBuyItems || Configuration.Instance.CanBuyVehicles))
            {
                sale = new Sales();
                sale.ResetSale();
            }
            if ((Configuration.Instance.PremiumItemSaleEnable || Configuration.Instance.PremiumVehicleSaleEnable) && (Configuration.Instance.CanBuyPremiumItems || Configuration.Instance.CanBuyPremiumVehicles))
            {
                premiumsale = new PremiumSales();
                premiumsale.ResetPremiumSale();
            }
        }

        protected override void Unload()
        {
            ShopDB = null;
            Instance = null;
        }
        #region Buy
        public bool Buy(UnturnedPlayer playerid, string[] components)
        {
            string message;
            bool v = false;
            if (components.Contains("v")) v = true;
            if (components.Length == 0 || (components.Length == 1 && v == true))
            { 
                message = Instance.Translate("buy_command_usage");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            byte amttobuy = 1;

            ushort id;
            switch (v)
            {
                case true:
                    if (!Instance.Configuration.Instance.CanBuyVehicles)
                    {
                        message = Instance.Translate("buy_vehicles_off");
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find(EAssetType.VEHICLE);

                        var vAsset = array.Cast<VehicleAsset>()
                            .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(components[1].ToLower()) == true);

                        if (vAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[1]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }

                        id = vAsset.id;
                        name = vAsset.vehicleName;
                    }

                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }

                    if (!playerid.HasPermission(Configuration.Instance.VehicleLimitBypassPermission))
                    {
                        var limit = Instance.ShopDB.GetVehicleLimit(id);
                        var bought = Instance.ShopDB.GetVehicleBought(id);
                        if ((bought + amttobuy > limit || bought == limit) && limit != 0)
                        {
                            message = Instance.Translate("vehicle_limited", name, id, bought.ToString(), limit.ToString());
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }
                    }

                    var cost = Instance.ShopDB.GetVehicleCost(id);
                    if (Configuration.Instance.VehicleSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.VehicleSalePercentage) / 100.00);
                        if (Instance.sale.salesStart == true)
                            cost = decimal.Round((cost * (Convert.ToDecimal(1.00) - saleprice)), 2);
                    }

                    var balance = Uconomy.Instance.Database.GetBalance(playerid.CSteamID.ToString());
                    if (cost <= 0m)
                    {
                        message = Instance.Translate("vehicle_not_available", name, id);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (balance < cost)
                    {
                        message = Instance.Translate("not_enough_currency_msg",
                            Uconomy.Instance.Configuration.Instance.MoneyName, "1", name);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (!playerid.GiveVehicle(id))
                    {
                        message = Instance.Translate("error_giving_item", name, id);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    Instance.ShopDB.IncreaseVehicleBought(id, amttobuy);

                    var newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), cost * -1);
                    message = Instance.Translate("vehicle_buy_msg", name, cost,
                        Uconomy.Instance.Configuration.Instance.MoneyName, newbal,
                        Uconomy.Instance.Configuration.Instance.MoneyName);
                    Instance.OnShopBuy?.Invoke(playerid, cost, 1, id, "vehicle");
                    playerid.Player.gameObject.SendMessage("ZaupShopOnBuy",
                        new object[] { playerid, cost, amttobuy, id, "vehicle" }, SendMessageOptions.DontRequireReceiver);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/F27XYPh.png");
                    Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", -cost, "Bought (V) 1x " + name + " (" + id + ")");
                    return true;
                case false:
                    if (!Instance.Configuration.Instance.CanBuyItems)
                    {
                        message = Instance.Translate("buy_items_off");
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        var array = Assets.find(EAssetType.ITEM);
                        var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                            k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                        if (iAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[0]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }

                        id = iAsset.id;
                        name = iAsset.itemName;
                    }

                    if (components.Length == 2 && !byte.TryParse(components[1], out amttobuy))
                    {
                        message = Instance.Translate("invalid_amt");
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[0]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }

                    if (!playerid.HasPermission(Configuration.Instance.ItemLimitBypassPermission))
                    {
                        var limit = Instance.ShopDB.GetItemLimit(id);
                        var bought = Instance.ShopDB.GetItemBought(id);
                        if ((bought + amttobuy > limit || bought == limit) && limit != 0)
                        {
                            message = Instance.Translate("item_limited", name, id, bought.ToString(), limit.ToString());
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }
                    }

                    cost = decimal.Round(Instance.ShopDB.GetItemCost(id) * amttobuy, 2);
                    if (Configuration.Instance.ItemSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.ItemSalePercentage) / 100.00);
                        if (Instance.sale.salesStart == true)
                            cost = decimal.Round((cost * (Convert.ToDecimal(1.00) - saleprice)), 2);
                    }
                    balance = Uconomy.Instance.Database.GetBalance(playerid.CSteamID.ToString());
                    if (cost <= 0m)
                    {
                        message = Instance.Translate("item_not_available", name, id);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (balance < cost)
                    {
                        message = Instance.Translate("not_enough_currency_msg",
                            Uconomy.Instance.Configuration.Instance.MoneyName, amttobuy, name);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    Instance.ShopDB.IncreaseItemBought(id, amttobuy);

                    playerid.GiveItem(id, amttobuy);
                    newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), cost * -1);
                    message = Instance.Translate("item_buy_msg", name, cost,
                        Uconomy.Instance.Configuration.Instance.MoneyName, newbal,
                        Uconomy.Instance.Configuration.Instance.MoneyName, amttobuy);
                    Instance.OnShopBuy?.Invoke(playerid, cost, amttobuy, id);
                    playerid.Player.gameObject.SendMessage("ZaupShopOnBuy",
                        new object[] { playerid, cost, amttobuy, id, "item" }, SendMessageOptions.DontRequireReceiver);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/F27XYPh.png");
                    Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", -cost, "Bought " + amttobuy + "x " + name + " (" + id + ")");
                    return true;
                default:
                    message = Instance.Translate("buy_vehicles_off");
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
            }
        }

        public bool PremiumBuy(UnturnedPlayer playerid, string[] components)
        {
            string message;
            bool v = false;
            if (components.Contains("v")) v = true;
            if (components.Length == 0 || (components.Length == 1 && v == true))
            {
                message = Instance.Translate("pbuy_command_usage");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            byte amttobuy = 1;

            ushort id;
            switch (v)
            {
                case true:
                    if (!Instance.Configuration.Instance.CanBuyPremiumVehicles)
                    {
                        message = Instance.Translate("pbuy_vehicles_off");
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find(EAssetType.VEHICLE);

                        var vAsset = array.Cast<VehicleAsset>()
                            .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(components[1].ToLower()) == true);

                        if (vAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[1]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }

                        id = vAsset.id;
                        name = vAsset.vehicleName;
                    }

                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }

                    if (!playerid.HasPermission(Configuration.Instance.PremiumVehicleLimitBypassPermission))
                    {
                        var limit = Instance.ShopDB.GetPremiumVehicleLimit(id);
                        var bought = Instance.ShopDB.GetPremiumVehicleBought(id);
                        if ((bought + amttobuy > limit || bought == limit) && limit != 0)
                        {
                            message = Instance.Translate("vehicle_limited", name, id, bought.ToString(), limit.ToString());
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }
                    }

                    var cost = Instance.ShopDB.GetPremiumVehicleCost(id);
                    if (Configuration.Instance.PremiumVehicleSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.PremiumVehicleSalePercentage) / 100.00);
                        if (Instance.premiumsale.PremiumsalesStart == true)
                            cost = decimal.Round((cost * (Convert.ToDecimal(1.00) - saleprice)), 2);
                    }
                    var balance = Uconomy.Instance.Database.GetPremiumBalance(playerid.CSteamID.ToString());
                    if (cost <= 0m)
                    {
                        message = Instance.Translate("pvehicle_not_available", name, id);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (balance < cost)
                    {
                        message = Instance.Translate("not_enough_currency_msg",
                            Uconomy.Instance.Configuration.Instance.PremiumMoneyName, "1", name);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (!playerid.GiveVehicle(id))
                    {
                        message = Instance.Translate("error_giving_item", name, id);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    Instance.ShopDB.IncreasePremiumVehicleBought(id, amttobuy);

                    var newbal = Uconomy.Instance.Database.IncreasePremiumBalance(playerid.CSteamID.ToString(), cost * -1);
                    message = Instance.Translate("vehicle_buy_msg", name, cost,
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName, newbal,
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName);
                    Instance.OnShopBuy?.Invoke(playerid, cost, 1, id, "vehicle");
                    playerid.Player.gameObject.SendMessage("ZaupShopOnBuy",
                        new object[] { playerid, cost, amttobuy, id, "vehicle" }, SendMessageOptions.DontRequireReceiver);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/F27XYPh.png");
                    Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", -cost, "Premium Bought (V) 1x " + name + " (" + id + ")");
                    return true;
                case false:
                    if (!Instance.Configuration.Instance.CanBuyPremiumItems)
                    {
                        message = Instance.Translate("pbuy_items_off");
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        var array = Assets.find(EAssetType.ITEM);
                        var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                            k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                        if (iAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[0]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }

                        id = iAsset.id;
                        name = iAsset.itemName;
                    }

                    if (components.Length == 2 && !byte.TryParse(components[1], out amttobuy))
                    {
                        message = Instance.Translate("invalid_amt");
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[0]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }

                    if (!playerid.HasPermission(Configuration.Instance.PremiumItemLimitBypassPermission))
                    {
                        var limit = Instance.ShopDB.GetPremiumItemLimit(id);
                        var bought = Instance.ShopDB.GetPremiumItemBought(id);
                        if ((bought + amttobuy > limit || bought == limit) && limit != 0)
                        {
                            message = Instance.Translate("item_limited", name, id, bought.ToString(), limit.ToString());
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return false;
                        }
                    }

                    cost = decimal.Round(Instance.ShopDB.GetPremiumItemCost(id) * amttobuy, 2);
                    if (Configuration.Instance.PremiumItemSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.PremiumItemSalePercentage) / 100.00);
                        if (Instance.premiumsale.PremiumsalesStart == true)
                            cost = decimal.Round((cost * (Convert.ToDecimal(1.00) - saleprice)), 2);
                    }
                    balance = Uconomy.Instance.Database.GetPremiumBalance(playerid.CSteamID.ToString());
                    if (cost <= 0m)
                    {
                        message = Instance.Translate("pitem_not_available", name, id);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    if (balance < cost)
                    {
                        message = Instance.Translate("not_enough_currency_msg",
                            Uconomy.Instance.Configuration.Instance.PremiumMoneyName, amttobuy, name);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return false;
                    }

                    Instance.ShopDB.IncreasePremiumItemBought(id, amttobuy);

                    playerid.GiveItem(id, amttobuy);
                    newbal = Uconomy.Instance.Database.IncreasePremiumBalance(playerid.CSteamID.ToString(), cost * -1);
                    message = Instance.Translate("item_buy_msg", name, cost,
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName, newbal,
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName, amttobuy);
                    Instance.OnShopBuy?.Invoke(playerid, cost, amttobuy, id);
                    playerid.Player.gameObject.SendMessage("ZaupShopOnBuy",
                        new object[] { playerid, cost, amttobuy, id, "item" }, SendMessageOptions.DontRequireReceiver);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/F27XYPh.png");
                    Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", -cost, "Premium Bought " + amttobuy + "x " + name + " (" + id + ")");
                    return true;
                default:
                    message = Instance.Translate("pbuy_command_usage");
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
            }
        }
        #endregion
        #region Cost
        public void Cost(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || components.Length == 1 &&
                (components[0].Trim() == string.Empty || components[0].Trim() == "v"))
            {
                message = Instance.Translate("cost_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            if (components.Length == 2 && (components[0] != "v" || components[1].Trim() == string.Empty))
            {
                message = Instance.Translate("cost_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            ushort id;
            switch (components[0])
            {
                case "v":
                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find(EAssetType.VEHICLE);

                        var vAsset = array.Cast<VehicleAsset>()
                            .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(components[1].ToLower()) == true);

                        if (vAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[1]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = vAsset.id;
                        name = vAsset.vehicleName;
                    }

                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }

                    var cost = Instance.ShopDB.GetVehicleCost(id);
                    var buyback = Instance.ShopDB.GetVehicleBuyPrice(id);
                    if (Configuration.Instance.VehicleSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.VehicleSalePercentage) / 100.00);
                        if (Instance.sale.salesStart == true)
                            cost = (cost * (Convert.ToDecimal(1.00) - saleprice));
                    }
                    message = Instance.Translate("vehicle_cost_msg", name, cost.ToString(),
                        Uconomy.Instance.Configuration.Instance.MoneyName, buyback.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName, id.ToString());
                    if (cost <= 0m) message = Instance.Translate("error_getting_cost", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
                default:
                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        var array = Assets.find(EAssetType.ITEM);
                        var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                            k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                        if (iAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[0]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = iAsset.id;
                        name = iAsset.itemName;
                    }

                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[0]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }

                    cost = Instance.ShopDB.GetItemCost(id);
                    var bbp = Instance.ShopDB.GetItemBuyPrice(id);
                    if (Configuration.Instance.ItemSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.ItemSalePercentage) / 100.00);
                        if (Instance.sale.salesStart == true)
                            cost = (cost * (Convert.ToDecimal(1.00) - saleprice));
                    }
                    message = Instance.Translate("item_cost_msg", name, cost.ToString(),
                        Uconomy.Instance.Configuration.Instance.MoneyName, bbp.ToString(),
                        Uconomy.Instance.Configuration.Instance.MoneyName, id);
                    if (cost <= 0m) message = Instance.Translate("error_getting_cost", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
            }
        }

        public void PremiumCost(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || components.Length == 1 &&
                (components[0].Trim() == string.Empty || components[0].Trim() == "v"))
            {
                message = Instance.Translate("pcost_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            if (components.Length == 2 && (components[0] != "v" || components[1].Trim() == string.Empty))
            {
                message = Instance.Translate("pcost_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            ushort id;
            switch (components[0])
            {
                case "v":
                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find(EAssetType.VEHICLE);

                        var vAsset = array.Cast<VehicleAsset>()
                            .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(components[1].ToLower()) == true);

                        if (vAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[1]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = vAsset.id;
                        name = vAsset.vehicleName;
                    }

                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }

                    var cost = Instance.ShopDB.GetPremiumVehicleCost(id);
                    var buyback = Instance.ShopDB.GetPremiumVehicleBuyPrice(id);
                    if (Configuration.Instance.PremiumVehicleSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.PremiumVehicleSalePercentage) / 100.00);
                        if (Instance.premiumsale.PremiumsalesStart == true)
                            cost = (cost * (Convert.ToDecimal(1.00) - saleprice));
                    }
                    message = Instance.Translate("vehicle_cost_msg", name, cost.ToString(),
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName, buyback.ToString(), Uconomy.Instance.Configuration.Instance.PremiumMoneyName, id.ToString());
                    if (cost <= 0m) message = Instance.Translate("error_getting_cost", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
                default:
                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        var array = Assets.find(EAssetType.ITEM);
                        var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                            k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                        if (iAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[0]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = iAsset.id;
                        name = iAsset.itemName;
                    }

                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[0]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }

                    cost = Instance.ShopDB.GetPremiumItemCost(id);
                    var bbp = Instance.ShopDB.GetPremiumItemBuyPrice(id);
                    if (Configuration.Instance.PremiumItemSaleEnable)
                    {
                        decimal saleprice = Convert.ToDecimal(Convert.ToDouble(Configuration.Instance.PremiumItemSalePercentage) / 100.00);
                        if (Instance.premiumsale.PremiumsalesStart == true)
                            cost = (cost * (Convert.ToDecimal(1.00) - saleprice));
                    }
                    message = Instance.Translate("item_cost_msg", name, cost.ToString(),
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName, bbp.ToString(),
                        Uconomy.Instance.Configuration.Instance.PremiumMoneyName, id);
                    if (cost <= 0m) message = Instance.Translate("error_getting_cost", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
            }
        }
        #endregion
        #region Limit
        public void Limit(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || components.Length == 1 &&
                (components[0].Trim() == string.Empty || components[0].Trim() == "v"))
            {
                message = Instance.Translate("limit_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            if (components.Length == 2 && (components[0] != "v" || components[1].Trim() == string.Empty))
            {
                message = Instance.Translate("limit_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            ushort id;
            switch (components[0])
            {
                case "v":
                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find(EAssetType.VEHICLE);

                        var vAsset = array.Cast<VehicleAsset>()
                            .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(components[1].ToLower()) == true);

                        if (vAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[1]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = vAsset.id;
                        name = vAsset.vehicleName;
                    }

                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }

                    var limit = Instance.ShopDB.GetVehicleLimit(id);
                    var bought = Instance.ShopDB.GetVehicleBought(id);
                    message = Instance.Translate("vehicle_limit_msg", name, limit.ToString(),
                        bought.ToString());
                    if (limit == 0) message = Instance.Translate("vehicle_unlimit_msg", name, bought.ToString());
                    if (limit < 0) message = Instance.Translate("error_getting_limit", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
                default:
                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        var array = Assets.find(EAssetType.ITEM);
                        var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                            k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                        if (iAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[0]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = iAsset.id;
                        name = iAsset.itemName;
                    }

                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[0]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }

                    limit = Instance.ShopDB.GetItemLimit(id);
                    bought = Instance.ShopDB.GetItemBought(id);
                    message = Instance.Translate("item_limit_msg", name, limit.ToString(), bought.ToString(), id);
                    if (limit == 0) message = Instance.Translate("item_unlimit_msg", name, bought.ToString(), id);
                    if (limit < 0) message = Instance.Translate("error_getting_limit", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
            }
        }

        public void PremiumLimit(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0 || components.Length == 1 &&
                (components[0].Trim() == string.Empty || components[0].Trim() == "v"))
            {
                message = Instance.Translate("plimit_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            if (components.Length == 2 && (components[0] != "v" || components[1].Trim() == string.Empty))
            {
                message = Instance.Translate("plimit_command_usage");
                // We are going to print how to use
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return;
            }

            ushort id;
            switch (components[0])
            {
                case "v":
                    string name = null;
                    if (!ushort.TryParse(components[1], out id))
                    {
                        var array = Assets.find(EAssetType.VEHICLE);

                        var vAsset = array.Cast<VehicleAsset>()
                            .FirstOrDefault(k => k?.vehicleName?.ToLower().Contains(components[1].ToLower()) == true);

                        if (vAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[1]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = vAsset.id;
                        name = vAsset.vehicleName;
                    }

                    if (Assets.find(EAssetType.VEHICLE, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[1]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((VehicleAsset)Assets.find(EAssetType.VEHICLE, id)).vehicleName;
                    }

                    var limit = Instance.ShopDB.GetPremiumVehicleLimit(id);
                    var bought = Instance.ShopDB.GetPremiumVehicleBought(id);
                    message = Instance.Translate("vehicle_limit_msg", name, limit.ToString(),
                        bought.ToString());
                    if (limit == 0) message = Instance.Translate("vehicle_unlimit_msg", name, bought.ToString());
                    if (limit < 0) message = Instance.Translate("error_getting_limit", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
                default:
                    name = null;
                    if (!ushort.TryParse(components[0], out id))
                    {
                        var array = Assets.find(EAssetType.ITEM);
                        var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                            k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                        if (iAsset == null)
                        {
                            message = Instance.Translate("could_not_find", components[0]);
                            UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                            return;
                        }

                        id = iAsset.id;
                        name = iAsset.itemName;
                    }

                    if (Assets.find(EAssetType.ITEM, id) == null)
                    {
                        message = Instance.Translate("could_not_find", components[0]);
                        UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                        return;
                    }
                    else if (name == null && id != 0)
                    {
                        name = ((ItemAsset)Assets.find(EAssetType.ITEM, id)).itemName;
                    }

                    limit = Instance.ShopDB.GetPremiumItemLimit(id);
                    bought = Instance.ShopDB.GetPremiumItemBought(id);
                    message = Instance.Translate("item_limit_msg", name, limit.ToString(), bought.ToString(), id);
                    if (limit == 0) message = Instance.Translate("item_unlimit_msg", name, bought.ToString(), id);
                    if (limit < 0) message = Instance.Translate("error_getting_limit", name, id);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/ClG436y.png");
                    break;
            }
        }
        #endregion
        #region Sell
        public bool Sell(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0)
            {
                message = Instance.Translate("sell_command_usage");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            byte amttosell = 1;
            if (components.Length > 1)
                if (!byte.TryParse(components[1], out amttosell))
                {
                    message = Instance.Translate("invalid_amt");
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
                }

            var amt = amttosell;
            if (!Instance.Configuration.Instance.CanSellItems)
            {
                message = Instance.Translate("sell_items_off");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }
            string name = null;
            if (!ushort.TryParse(components[0], out var id))
            {
                var array = Assets.find(EAssetType.ITEM);
                var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                    k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                if (iAsset == null)
                {
                    message = Instance.Translate("could_not_find", components[0]);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
                }

                id = iAsset.id;
                name = iAsset.itemName;
            }
            if (id == 0)
            {
                message = Instance.Translate("could_not_find", components[0]);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var vAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);

            if (vAsset == null)
            {
                message = Instance.Translate("could_not_find", components[0]);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (name == null) name = vAsset.itemName;

            // Get how many they have
            if (playerid.Inventory.has(id) == null)
            {
                message = Instance.Translate("not_have_item_sell", name);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var list = playerid.Inventory.search(id, true, true);
            if (list.Count == 0 || vAsset.amount == 1 && list.Count < amttosell)
            {
                message = Instance.Translate("not_enough_items_sell", amttosell.ToString(), name);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (vAsset.amount > 1)
            {
                var ammomagamt = 0;
                foreach (var ins in list) ammomagamt += ins.jar.item.amount;
                if (ammomagamt < amttosell)
                {
                    message = Instance.Translate("not_enough_ammo_sell", name);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
                }
            }

            // We got this far, so let's buy back the items and give them money.
            // Get cost per item.  This will be whatever is set for most items, but changes for ammo and magazines.
            var price = Instance.ShopDB.GetItemBuyPrice(id);
            if (price <= 0.00m)
            {
                message = Instance.Translate("no_sell_price_set", name);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            byte quality = 100;
            decimal peritemprice = 0;
            decimal addmoney = 0;
            switch (vAsset.amount)
            {
                case 1:
                    // These are single items, not ammo or magazines
                    while (amttosell > 0)
                    {
                        if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                            playerid.Player.equipment.dequip();
                        if (Instance.Configuration.Instance.QualityCounts)
                            quality = list[0].jar.item.durability;
                        peritemprice = decimal.Round(price * (quality / 100.0m), 2);
                        addmoney += peritemprice;
                        playerid.Inventory.removeItem(list[0].page,
                            playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                        list.RemoveAt(0);
                        amttosell--;
                    }

                    break;
                default:
                    // This is ammo or magazines
                    var amttosell1 = amttosell;
                    while (amttosell > 0)
                    {
                        if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                            playerid.Player.equipment.dequip();
                        if (list[0].jar.item.amount >= amttosell)
                        {
                            var left = (byte)(list[0].jar.item.amount - amttosell);
                            list[0].jar.item.amount = left;
                            playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, left);
                            amttosell = 0;
                            if (left == 0)
                            {
                                playerid.Inventory.removeItem(list[0].page,
                                    playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                list.RemoveAt(0);
                            }
                        }
                        else
                        {
                            amttosell -= list[0].jar.item.amount;
                            playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, 0);
                            playerid.Inventory.removeItem(list[0].page,
                                playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                            list.RemoveAt(0);
                        }
                    }

                    peritemprice = decimal.Round(price * (amttosell1 / (decimal)vAsset.amount), 2);
                    addmoney += peritemprice;
                    break;
            }

            var balance = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), addmoney);
            message = Instance.Translate("sold_items", amt, name, addmoney,
                Uconomy.Instance.Configuration.Instance.MoneyName, balance,
                Uconomy.Instance.Configuration.Instance.MoneyName, id);
            Instance.OnShopSell?.Invoke(playerid, addmoney, amt, id);
            playerid.Player.gameObject.SendMessage("ZaupShopOnSell", new object[] { playerid, addmoney, amt, id},
                SendMessageOptions.DontRequireReceiver);
            UnturnedChat.Say(playerid, message, "https://i.imgur.com/V88RHfN.png");
            Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", addmoney, "Sold " + amt + "x " + name + " (" + id + ")");
            return true;
        }
        public bool SellVehicle(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length != 0)
            {
                message = Instance.Translate("sellv_command_usage");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (!Instance.Configuration.Instance.CanSellVehicles)
            {
                message = Instance.Translate("sellv_vehicles_off");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (playerid.IsInVehicle == false)
            {
                message = Instance.Translate("not_in_vehicle");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (playerid.CurrentVehicle.lockedOwner != playerid.CSteamID)
            {
                message = Instance.Translate("not_have_vehicle_sell");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var addmoney = Instance.ShopDB.GetVehicleBuyPrice(playerid.CurrentVehicle.asset.id);
            if (addmoney <= 0m)
            {
                message = Instance.Translate("vehicle_not_available", playerid.CurrentVehicle.asset.vehicleName, playerid.CurrentVehicle.asset.id.ToString());
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var vehicle = playerid.CurrentVehicle;
            VehicleManager.askVehicleDestroy(playerid.CurrentVehicle);

            ushort amt = 1;
            Instance.ShopDB.IncreaseVehicleBought(vehicle.asset.id, -amt);
            decimal balance = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), addmoney);
            message = Instance.Translate("sold_vehicles", vehicle.asset.vehicleName, vehicle.asset.id.ToString(), addmoney.ToString(),
                Uconomy.Instance.Configuration.Instance.MoneyName, balance.ToString(),
                Uconomy.Instance.Configuration.Instance.MoneyName);
            Instance.OnShopSell?.Invoke(playerid, addmoney, (byte)amt, vehicle.asset.id);
            playerid.Player.gameObject.SendMessage("ZaupShopOnSell", new object[] { playerid, addmoney, amt, vehicle.asset.id},
                SendMessageOptions.DontRequireReceiver);
            UnturnedChat.Say(playerid, message, "https://i.imgur.com/V88RHfN.png");
            Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", addmoney, "Sold (V) 1x " + vehicle.asset.vehicleName + " (" + vehicle.asset.id + ")");
            return true;
        }

        public bool PremiumSell(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length == 0)
            {
                message = Instance.Translate("psell_command_usage");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            byte amttosell = 1;
            if (components.Length > 1)
                if (!byte.TryParse(components[1], out amttosell))
                {
                    message = Instance.Translate("invalid_amt");
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
                }

            var amt = amttosell;
            if (!Instance.Configuration.Instance.CanSellPremiumItems)
            {
                message = Instance.Translate("psell_items_off");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }
            string name = null;
            if (!ushort.TryParse(components[0], out var id))
            {
                var array = Assets.find(EAssetType.ITEM);
                var iAsset = array.Cast<ItemAsset>().FirstOrDefault(k =>
                    k?.itemName?.ToLower().Contains(components[0].ToLower()) == true);

                if (iAsset == null)
                {
                    message = Instance.Translate("could_not_find", components[0]);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
                }

                id = iAsset.id;
                name = iAsset.itemName;
            }
            if (id == 0)
            {
                message = Instance.Translate("could_not_find", components[0]);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var vAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);

            if (vAsset == null)
            {
                message = Instance.Translate("could_not_find", components[0]);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (name == null) name = vAsset.itemName;

            // Get how many they have
            if (playerid.Inventory.has(id) == null)
            {
                message = Instance.Translate("not_have_item_sell", name);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var list = playerid.Inventory.search(id, true, true);
            if (list.Count == 0 || vAsset.amount == 1 && list.Count < amttosell)
            {
                message = Instance.Translate("not_enough_items_sell", amttosell.ToString(), name);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (vAsset.amount > 1)
            {
                var ammomagamt = 0;
                foreach (var ins in list) ammomagamt += ins.jar.item.amount;
                if (ammomagamt < amttosell)
                {
                    message = Instance.Translate("not_enough_ammo_sell", name);
                    UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                    return false;
                }
            }

            // We got this far, so let's buy back the items and give them money.
            // Get cost per item.  This will be whatever is set for most items, but changes for ammo and magazines.
            var price = Instance.ShopDB.GetPremiumItemBuyPrice(id);
            if (price <= 0.00m)
            {
                message = Instance.Translate("pno_sell_price_set", name);
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            byte quality = 100;
            decimal peritemprice = 0;
            decimal addmoney = 0;
            switch (vAsset.amount)
            {
                case 1:
                    // These are single items, not ammo or magazines
                    while (amttosell > 0)
                    {
                        if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                            playerid.Player.equipment.dequip();
                        if (Instance.Configuration.Instance.QualityCounts)
                            quality = list[0].jar.item.durability;
                        peritemprice = decimal.Round(price * (quality / 100.0m), 2);
                        addmoney += peritemprice;
                        playerid.Inventory.removeItem(list[0].page,
                            playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                        list.RemoveAt(0);
                        amttosell--;
                    }

                    break;
                default:
                    // This is ammo or magazines
                    var amttosell1 = amttosell;
                    while (amttosell > 0)
                    {
                        if (playerid.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                            playerid.Player.equipment.dequip();
                        if (list[0].jar.item.amount >= amttosell)
                        {
                            var left = (byte)(list[0].jar.item.amount - amttosell);
                            list[0].jar.item.amount = left;
                            playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, left);
                            amttosell = 0;
                            if (left == 0)
                            {
                                playerid.Inventory.removeItem(list[0].page,
                                    playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                list.RemoveAt(0);
                            }
                        }
                        else
                        {
                            amttosell -= list[0].jar.item.amount;
                            playerid.Inventory.sendUpdateAmount(list[0].page, list[0].jar.x, list[0].jar.y, 0);
                            playerid.Inventory.removeItem(list[0].page,
                                playerid.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                            list.RemoveAt(0);
                        }
                    }

                    peritemprice = decimal.Round(price * (amttosell1 / (decimal)vAsset.amount), 2);
                    addmoney += peritemprice;
                    break;
            }

            var balance = Uconomy.Instance.Database.IncreasePremiumBalance(playerid.CSteamID.ToString(), addmoney);
            message = Instance.Translate("psold_items", amt, name, addmoney,
                Uconomy.Instance.Configuration.Instance.PremiumMoneyName, balance,
                Uconomy.Instance.Configuration.Instance.PremiumMoneyName, id);
            Instance.OnShopSell?.Invoke(playerid, addmoney, amt, id);
            playerid.Player.gameObject.SendMessage("ZaupShopOnSell", new object[] { playerid, addmoney, amt, id},
                SendMessageOptions.DontRequireReceiver);
            UnturnedChat.Say(playerid, message, "https://i.imgur.com/V88RHfN.png");
            Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", addmoney, "Premium Sold " + amt + "x " + name + " (" + id + ")");
            return true;
        }
        public bool PremiumSellVehicle(UnturnedPlayer playerid, string[] components)
        {
            string message;
            if (components.Length != 0)
            {
                message = Instance.Translate("psellv_command_usage");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (!Instance.Configuration.Instance.CanSellPremiumVehicles)
            {
                message = Instance.Translate("psellv_vehicles_off");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (playerid.CurrentVehicle == null)
            {
                message = Instance.Translate("not_in_vehicle");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            if (playerid.CurrentVehicle.lockedOwner != playerid.CSteamID)
            {
                message = Instance.Translate("not_have_vehicle_sell");
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var addmoney = Instance.ShopDB.GetPremiumVehicleBuyPrice(playerid.CurrentVehicle.asset.id);
            if (addmoney <= 0m)
            {
                message = Instance.Translate("vehicle_not_available", playerid.CurrentVehicle.asset.vehicleName, playerid.CurrentVehicle.asset.id.ToString());
                UnturnedChat.Say(playerid, message, "https://i.imgur.com/FeIvao9.png");
                return false;
            }

            var vehicle = playerid.CurrentVehicle;
            VehicleManager.askVehicleDestroy(playerid.CurrentVehicle);

            ushort amt = 1;
            Instance.ShopDB.IncreasePremiumVehicleBought(vehicle.asset.id, -amt);
            var balance = Uconomy.Instance.Database.IncreasePremiumBalance(playerid.CSteamID.ToString(), addmoney);
            message = Instance.Translate("sold_vehicles", vehicle.asset.vehicleName, vehicle.asset.id.ToString(), addmoney.ToString(),
                Uconomy.Instance.Configuration.Instance.PremiumMoneyName, balance.ToString(),
                Uconomy.Instance.Configuration.Instance.PremiumMoneyName);
            Instance.OnShopSell?.Invoke(playerid, addmoney, (byte)amt, vehicle.asset.id);
            playerid.Player.gameObject.SendMessage("ZaupShopOnSell", new object[] { playerid, addmoney, amt, vehicle.asset.id},
                SendMessageOptions.DontRequireReceiver);
            UnturnedChat.Say(playerid, message, "https://i.imgur.com/V88RHfN.png");
            Uconomy.Instance.Database.AddHistory(playerid.CSteamID, "ZaupShop", addmoney, "Premium Sold (V) 1x " + vehicle.asset.vehicleName + " (" + vehicle.asset.id + ")");
            return true;
        }
        #endregion

        public void FixedUpdate()
        {
            if ((Configuration.Instance.ItemSaleEnable || Configuration.Instance.VehicleSaleEnable) && (Configuration.Instance.CanBuyItems || Configuration.Instance.CanBuyVehicles)) sale.CheckSale();
            if ((Configuration.Instance.PremiumItemSaleEnable || Configuration.Instance.PremiumVehicleSaleEnable) && (Configuration.Instance.CanBuyPremiumItems || Configuration.Instance.CanBuyPremiumVehicles)) premiumsale.CheckPremiumSale();
        }
    }
}