using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using fr34kyn01535.Uconomy;

namespace ZaupShop
{
    public class CommandPremiumAuction : IRocketCommand
    {
        public string Name
        {
            get
            {
                return "premiumauction";
            }
        }
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }
        public string Help
        {
            get
            {
                return "Allows you to auction your items from your inventory.";
            }
        }
        public string Syntax
        {
            get
            {
                return "<name or id>";
            }
        }
        public List<string> Aliases
        {
            get { return new List<string> { "pauction" }; }
        }
        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "premiumauction" };
            }
        }
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (!ZaupShop.Instance.Configuration.Instance.AllowPremiumAuction)
            {
                UnturnedChat.Say(caller, ZaupShop.Instance.Translate("pauction_disabled"), "https://i.imgur.com/FeIvao9.png");
                return;
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length == 0)
            {
                UnturnedChat.Say(player, ZaupShop.Instance.Translate("pauction_command_usage"), "https://i.imgur.com/FeIvao9.png");
                return;
            }
            if (command.Length == 1)
            {
                switch (command[0])
                {
                    case ("add"):
                        UnturnedChat.Say(player, ZaupShop.Instance.Translate("pauction_addcommand_usage"), "https://i.imgur.com/FeIvao9.png");
                        return;
                    case ("list"):
                        string Message = "";
                        string[] ItemNameAndQuality = ZaupShop.Instance.AuctionDB.GetAllPremiumItemNameWithQuality();
                        string[] AuctionID = ZaupShop.Instance.AuctionDB.GetAllPremiumAuctionID();
                        string[] ItemPrice = ZaupShop.Instance.AuctionDB.GetAllPremiumItemPrice();
                        int count = 0;
                        for (int x = 0; x < ItemNameAndQuality.Length; x++)
                        {
                            if (x < ItemNameAndQuality.Length - 1)
                                Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.PremiumMoneyName + ", ";
                            else
                                Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.PremiumMoneyName;
                            count++;
                            if (count == 2)
                            {
                                UnturnedChat.Say(player, Message);
                                Message = "";
                                count = 0;
                            }
                        }
                        if (Message != "")
                            UnturnedChat.Say(player, Message);
                        break;
                    case ("buy"):
                        UnturnedChat.Say(player, ZaupShop.Instance.Translate("pauction_buycommand_usage"), "https://i.imgur.com/FeIvao9.png");
                        return;
                    case ("cancel"):
                        UnturnedChat.Say(player, ZaupShop.Instance.Translate("pauction_cancelcommand_usage"), "https://i.imgur.com/FeIvao9.png");
                        return;
                    case ("find"):
                        UnturnedChat.Say(player, ZaupShop.Instance.Translate("pauction_findcommand_usage"), "https://i.imgur.com/FeIvao9.png");
                        return;
                }
            }
            if (command.Length == 2)
            {
                int auctionid;
                switch (command[0])
                {
                    case ("add"):
                        UnturnedChat.Say(player, ZaupShop.Instance.Translate("pauction_addcommand_usage2"), "https://i.imgur.com/FeIvao9.png");
                        return;
                    case ("buy"):
                        if (int.TryParse(command[1], out auctionid))
                        {
                            try
                            {
                                string[] itemInfo = ZaupShop.Instance.AuctionDB.PremiumAuctionBuy(auctionid);
                                decimal balance = Uconomy.Instance.Database.GetPremiumBalance(player.Id);
                                decimal.TryParse(itemInfo[2], out decimal cost);
                                if (balance < cost)
                                {
                                    UnturnedChat.Say(player, ZaupShop.Instance.DefaultTranslations.Translate("not_enough_currency_msg", Uconomy.Instance.Configuration.Instance.PremiumMoneyName, itemInfo[1]), "https://i.imgur.com/FeIvao9.png");
                                    return;
                                }
                                player.GiveItem(ushort.Parse(itemInfo[0]), 1);
                                InventorySearch inventory = player.Inventory.has(ushort.Parse(itemInfo[0]));
                                byte index = player.Inventory.getIndex(inventory.page, inventory.jar.x, inventory.jar.y);
                                player.Inventory.updateQuality(inventory.page, index, byte.Parse(itemInfo[3]));
                                ZaupShop.Instance.AuctionDB.DeletePremiumAuction(command[1]);
                                decimal newbal = Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), (cost * -1));
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_buy_msg", itemInfo[1], cost, Uconomy.Instance.Configuration.Instance.PremiumMoneyName, newbal, Uconomy.Instance.Configuration.Instance.PremiumMoneyName), "https://i.imgur.com/F27XYPh.png");
                                decimal sellernewbalance = Uconomy.Instance.Database.IncreasePremiumBalance(itemInfo[4], (cost * 1));
                            }
                            catch
                            {
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_addcommand_idnotexist"), "https://i.imgur.com/FeIvao9.png");
                                return;
                            }

                        }
                        else
                        {
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_addcommand_usage2"), "https://i.imgur.com/FeIvao9.png");
                            return;
                        }
                        break;
                    case ("cancel"):
                        if (int.TryParse(command[1], out auctionid))
                        {
                            if (ZaupShop.Instance.AuctionDB.CheckPremiumAuctionExist(auctionid))
                            {
                                string OwnerID = ZaupShop.Instance.AuctionDB.GetPremiumOwner(auctionid);
                                if (OwnerID.Trim() == player.Id.Trim())
                                {
                                    string[] itemInfo = ZaupShop.Instance.AuctionDB.PremiumAuctionCancel(auctionid);
                                    player.GiveItem(ushort.Parse(itemInfo[0]), 1);
                                    InventorySearch inventory = player.Inventory.has(ushort.Parse(itemInfo[0]));
                                    byte index = player.Inventory.getIndex(inventory.page, inventory.jar.x, inventory.jar.y);
                                    player.Inventory.updateQuality(inventory.page, index, byte.Parse(itemInfo[1]));
                                    ZaupShop.Instance.AuctionDB.DeletePremiumAuction(auctionid.ToString());
                                    UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_cancelled", auctionid), "https://i.imgur.com/FeIvao9.png");
                                }
                                else
                                {
                                    UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_notown"), "https://i.imgur.com/FeIvao9.png");
                                    return;
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_notexist"), "https://i.imgur.com/FeIvao9.png");
                                return;
                            }
                        }
                        else
                        {
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_notexist"), "https://i.imgur.com/FeIvao9.png");
                            return;
                        }
                        break;
                    case ("find"):
                        uint ItemID;
                        if (uint.TryParse(command[1], out ItemID))
                        {
                            string[] AuctionID = ZaupShop.Instance.AuctionDB.FindPremiumItemByID(ItemID.ToString());
                            string Message = "";
                            string[] ItemNameAndQuality = ZaupShop.Instance.AuctionDB.FindAllPremiumItemNameWithQualityByID(ItemID.ToString());
                            string[] ItemPrice = ZaupShop.Instance.AuctionDB.FindAllPremiumItemPriceByID(ItemID.ToString());
                            int count = 0;
                            for (int x = 0; x < ItemNameAndQuality.Length; x++)
                            {
                                if (x < ItemNameAndQuality.Length - 1)
                                    Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.PremiumMoneyName + ", ";
                                else
                                    Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.PremiumMoneyName;
                                count++;
                                if (count == 2)
                                {
                                    UnturnedChat.Say(player, Message);
                                    Message = "";
                                    count = 0;
                                }
                            }
                            if (Message != "")
                                UnturnedChat.Say(player, Message);
                            else
                            {
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_find_failed"), "https://i.imgur.com/FeIvao9.png");
                                return;
                            }
                        }
                        else
                        {
                            Asset[] array = Assets.find(EAssetType.ITEM);
                            Asset[] array2 = array;
                            string ItemName = "";
                            for (int i = 0; i < array2.Length; i++)
                            {
                                ItemAsset vAsset = (ItemAsset)array2[i];
                                if (vAsset != null && vAsset.name != null && vAsset.name.ToLower().Contains(command[1].ToLower()))
                                {
                                    _ = vAsset.id;
                                    ItemName = vAsset.name;
                                    break;
                                }
                            }
                            if (ItemName != "")
                            {
                                string[] AuctionID = ZaupShop.Instance.AuctionDB.FindPremiumItemByName(ItemName);
                                string Message = "";
                                string[] ItemNameAndQuality = ZaupShop.Instance.AuctionDB.FindAllPremiumItemNameWithQualityByItemName(ItemName);
                                string[] ItemPrice = ZaupShop.Instance.AuctionDB.FindAllPremiumItemPriceByItemName(ItemName);
                                int count = 0;
                                for (int x = 0; x < ItemNameAndQuality.Length; x++)
                                {
                                    if (x < ItemNameAndQuality.Length - 1)
                                        Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.PremiumMoneyName + ", ";
                                    else
                                        Message += "[" + AuctionID[x] + "]: " + ItemNameAndQuality[x] + " for " + ItemPrice[x] + Uconomy.Instance.Configuration.Instance.PremiumMoneyName;
                                    count++;
                                    if (count == 2)
                                    {
                                        UnturnedChat.Say(player, Message);
                                        Message = "";
                                        count = 0;
                                    }
                                }
                                if (Message != "")
                                    UnturnedChat.Say(player, Message);
                                else
                                {
                                    UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_find_failed"), "https://i.imgur.com/FeIvao9.png");
                                    return;
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_find_failed"), "https://i.imgur.com/FeIvao9.png");
                                return;
                            }
                        }
                        break;
                }
            }
            if (command.Length > 2)
            {
                switch (command[0])
                {
                    case ("add"):
                        byte amt = 1;
                        ushort id;
                        string name = null;
                        ItemAsset vAsset = null;
                        string itemname = "";
                        for (int x = 1; x < command.Length - 1; x++)
                        {
                            itemname += command[x] + " ";
                        }
                        itemname = itemname.Trim();
                        if (!ushort.TryParse(itemname, out id))
                        {
                            Asset[] array = Assets.find(EAssetType.ITEM);
                            Asset[] array2 = array;
                            for (int i = 0; i < array2.Length; i++)
                            {
                                vAsset = (ItemAsset)array2[i];
                                if (vAsset != null && vAsset.name != null && vAsset.name.ToLower().Contains(itemname.ToLower()))
                                {
                                    id = vAsset.id;
                                    name = vAsset.name;
                                    break;
                                }
                            }
                        }
                        if (name == null && id == 0)
                        {
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("could_not_find", itemname), "https://i.imgur.com/FeIvao9.png");
                            return;
                        }
                        else if (name == null && id != 0)
                        {
                            try
                            {
                                vAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                                name = vAsset.name;
                            }
                            catch
                            {
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("item_invalid"), "https://i.imgur.com/FeIvao9.png");
                                return;
                            }
                        }
                        if (player.Inventory.has(id) == null)
                        {
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("not_have_item_auction", name), "https://i.imgur.com/FeIvao9.png");
                            return;
                        }
                        List<InventorySearch> list = player.Inventory.search(id, true, true);
                        if (vAsset.amount > 1)
                        {
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_item_mag_ammo", name), "https://i.imgur.com/FeIvao9.png");
                            return;
                        }
                        decimal price = 0.00m;
                        if (ZaupShop.Instance.Configuration.Instance.CanBuyPremiumItems || ZaupShop.Instance.Configuration.Instance.CanBuyPremiumVehicles)
                        {
                            price = ZaupShop.Instance.ShopDB.GetPremiumItemCost(id);
                            if (price <= 0.00m)
                            {
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_item_notinshop", name), "https://i.imgur.com/FeIvao9.png");
                                price = 0.00m;
                            }
                        }
                        byte quality = 100;
                        switch (vAsset.amount)
                        {
                            case 1:
                                // These are single items, not ammo or magazines
                                while (amt > 0)
                                {
                                    try
                                    {
                                        if (player.Player.equipment.checkSelection(list[0].page, list[0].jar.x, list[0].jar.y))
                                        {
                                            player.Player.equipment.dequip();
                                        }
                                    }
                                    catch
                                    {
                                        UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_unequip_item", name), "https://i.imgur.com/FeIvao9.png");
                                        return;
                                    }
                                    quality = list[0].jar.item.durability;
                                    player.Inventory.removeItem(list[0].page, player.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                    list.RemoveAt(0);
                                    amt--;
                                }
                                break;
                            default:
                                UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_item_mag_ammo", name), "https://i.imgur.com/FeIvao9.png");
                                return;
                        }
                        decimal SetPrice;
                        if (!decimal.TryParse(command[command.Length - 1], out SetPrice))
                            SetPrice = price;
                        if (ZaupShop.Instance.AuctionDB.AddPremiumAuctionItem(ZaupShop.Instance.AuctionDB.GetLastPremiumAuctionNo(), id.ToString(), name, SetPrice, price, (int)quality, player.Id))
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_item_succes", name, SetPrice, Uconomy.Instance.Configuration.Instance.PremiumMoneyName), "https://i.imgur.com/yBn85Gc.png");
                        else
                            UnturnedChat.Say(player, ZaupShop.Instance.Translate("auction_item_failed"), "https://i.imgur.com/FeIvao9.png");
                        break;
                }

            }
        }

    }

}