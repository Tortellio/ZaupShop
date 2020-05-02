using System.Collections.Generic;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace ZaupShop
{
    public class CommandPremiumShop : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "premiumshop";

        public string Help => "Allows admins to change, add, remove, limit, reset items/vehicles from the premium shop.";

        public string Syntax => "<add | rem | chng | buy | lim | res> [v] <itemid> <cost | amount> [buyback price | limit amount]";

        public List<string> Aliases => new List<string> { "pshop" };

        public List<string> Permissions => new List<string> { "pshop.*", "pshop.add", "pshop.rem", "pshop.chng", "pshop.buy", "pshop.lim", "pshop.res" };

        public void Execute(IRocketPlayer caller, string[] msg)
        {
            var console = caller is ConsolePlayer;
            string[] permnames = { "pshop.*", "pshop.add", "pshop.rem", "pshop.chng", "pshop.buy", "pshop.lim", "pshop.res" };
            bool[] perms = { false, false, false, false, false, false, false };
            var anyuse = false;
            string message;
            foreach (var s in caller.GetPermissions())
                switch (s.Name)
                {
                    case "pshop.*":
                        perms[0] = true;
                        anyuse = true;
                        break;
                    case "pshop.add":
                        perms[1] = true;
                        anyuse = true;
                        break;
                    case "pshop.rem":
                        perms[2] = true;
                        anyuse = true;
                        break;
                    case "pshop.chng":
                        perms[3] = true;
                        anyuse = true;
                        break;
                    case "pshop.buy":
                        perms[4] = true;
                        anyuse = true;
                        break;
                    case "pshop.lim":
                        perms[5] = true;
                        anyuse = true;
                        break;
                    case "pshop.res":
                        perms[6] = true;
                        anyuse = true;
                        break;
                    case "*":
                        perms[0] = true;
                        perms[1] = true;
                        perms[2] = true;
                        perms[3] = true;
                        perms[4] = true;
                        perms[5] = true;
                        perms[6] = true;
                        anyuse = true;
                        break;
                }

            if (console || ((UnturnedPlayer)caller).IsAdmin)
            {
                perms[0] = true;
                perms[1] = true;
                perms[2] = true;
                perms[3] = true;
                perms[4] = true;
                perms[5] = true;
                perms[6] = true;
                anyuse = true;
            }

            if (!anyuse)
            {
                // Assume this is a player
                UnturnedChat.Say(caller, "You don't have permission to use the /pshop command.", "https://i.imgur.com/FeIvao9.png");
                return;
            }

            if (msg.Length == 0)
            {
                message = ZaupShop.Instance.Translate("pshop_command_usage");
                // We are going to print how to use
                SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                return;
            }

            if (msg.Length == 1)
            {
                message = ZaupShop.Instance.Translate("no_itemid_given");
                SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                return;
            }

            else if (msg.Length >= 2)
            {
                var change = false;
                var pass = false;

                // All basic checks complete.  Let's get down to business.
                bool success;
                switch (msg[0])
                {
                    case "chng":
                        if (!perms[3] && !perms[0])
                        {
                            message = ZaupShop.Instance.Translate("no_permission_pshop_chng");
                            SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                            return;
                        }

                        change = true;
                        pass = true;
                        goto case "add";
                    case "add":
                        if (!pass)
                            if (!perms[1] && !perms[0])
                            {
                                message = ZaupShop.Instance.Translate("no_permission_pshop_add");
                                SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                return;
                            }

                        var ac = pass
                            ? ZaupShop.Instance.Translate("changed")
                            : ZaupShop.Instance.Translate("added");

                        switch (msg[1])
                        {
                            case "v":
                                ushort id = 0;
                                if (msg.Length <= 2 || msg.Length >= 7)
                                {
                                    message = "Usage: /pshop add v <id | name> <cost> [buyback] [limit]";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }

                                if (msg.Length >= 3)
                                {
                                    if (!ushort.TryParse(msg[2], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "v"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (msg.Length == 3)
                                    {
                                        message = ZaupShop.Instance.Translate("no_cost_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                decimal cost = 0.00m;
                                if (msg.Length >= 4)
                                {
                                    if (!decimal.TryParse(msg[3], out cost))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_cost_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                decimal buyback = 0.00m;
                                if (msg.Length >= 5)
                                {
                                    if (!decimal.TryParse(msg[4], out buyback))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_buyback_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                int limit = 0;
                                if (msg.Length == 6)
                                {
                                    if (!int.TryParse(msg[5], out limit))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_limit_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var va = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
                                message = ZaupShop.Instance.Translate("changed_or_added_to_pshop", ac, va.vehicleName,
                                    cost, buyback, limit);
                                success = ZaupShop.Instance.ShopDB.AddPremiumVehicle(id, va.vehicleName,
                                    cost, buyback, limit, change);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("error_adding_or_changing", va.vehicleName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                            default:
                                id = 0;
                                if (msg.Length <= 1 || msg.Length >= 6)
                                {
                                    message = "Usage: /shop add <id | name> <cost> [buyback] [limit]";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                if (msg.Length >= 2)
                                {
                                    if (!ushort.TryParse(msg[1], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "i"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (msg.Length == 2)
                                    {
                                        message = ZaupShop.Instance.Translate("no_cost_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                cost = 0.00m;
                                if (msg.Length >= 3)
                                {
                                    if (!decimal.TryParse(msg[2], out cost))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_cost_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                buyback = 0.00m;
                                if (msg.Length >= 4)
                                {
                                    if (!decimal.TryParse(msg[3], out buyback))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_buyback_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                limit = 0;
                                if (msg.Length == 5)
                                {
                                    if (!int.TryParse(msg[4], out limit))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_limit_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                var ia = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                                message = ZaupShop.Instance.Translate("changed_or_added_to_pshop", ac, ia.itemName,
                                    cost, buyback, limit);
                                success = ZaupShop.Instance.ShopDB.AddPremiumItem(id, ia.itemName, cost, buyback, limit,
                                    change);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("error_adding_or_changing", ia.itemName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                        }

                        break;
                    case "rem":
                        if (!perms[2] && !perms[0])
                        {
                            message = ZaupShop.Instance.Translate("no_permission_pshop_rem");
                            SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                            return;
                        }

                        switch (msg[1])
                        {
                            case "v":
                                if (msg.Length <= 2 || msg.Length >= 4)
                                {
                                    message = "Usage: /pshop rem v <id | name>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                ushort id = 0;
                                if (msg.Length == 3)
                                {
                                    if (!ushort.TryParse(msg[2], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "v"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var va = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
                                message = ZaupShop.Instance.Translate("removed_from_pshop", va.vehicleName);
                                success = ZaupShop.Instance.ShopDB.DeletePremiumVehicle(id);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("not_in_pshop_to_remove", va.vehicleName);
                                SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                break;
                            default:
                                if (msg.Length <= 1 || msg.Length >= 3)
                                {
                                    message = "Usage: /pshop rem <id | name>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                id = 0;
                                if (msg.Length == 2)
                                {
                                    if (!ushort.TryParse(msg[1], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "i"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var ia = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                                message = ZaupShop.Instance.Translate("removed_from_shop", ia.itemName);
                                success = ZaupShop.Instance.ShopDB.DeleteItem(id);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("not_in_shop_to_remove", ia.itemName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                        }
                        break;
                        
                    case "buy":

                        if (!perms[4] && !perms[0])
                        {
                            message = ZaupShop.Instance.Translate("no_permission_pshop_buy");
                            SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                            return;
                        }
                        switch (msg[1])
                        {
                            case "v":
                                if (msg.Length <= 2 || msg.Length >= 5)
                                {
                                    message = "Usage: /pshop buy v <id | name> <buyback amount>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                ushort id = 0;
                                if (msg.Length >= 3)
                                {
                                    if (!ushort.TryParse(msg[2], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "v"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (msg.Length == 3)
                                    {
                                        message = ZaupShop.Instance.Translate("no_buyback_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                decimal buyback = 0.00m;
                                if (msg.Length == 4)
                                {
                                    if (!decimal.TryParse(msg[3], out buyback))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                var iab = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
                                message = ZaupShop.Instance.Translate("set_buyback_price", iab.vehicleName, buyback.ToString());
                                success = ZaupShop.Instance.ShopDB.SetPremiumVehicleBuyPrice(id, buyback);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("not_in_pshop_to_set_buyback", iab.vehicleName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                            default:
                                if (msg.Length <= 1 || msg.Length >= 4)
                                {
                                    message = "Usage: /pshop buy <id | name> <buyback amount>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                id = 0;
                                if (msg.Length >= 2)
                                {
                                    if (!ushort.TryParse(msg[1], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "i"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (msg.Length == 2)
                                    {
                                        message = ZaupShop.Instance.Translate("no_buyback_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                buyback = 0.00m;
                                if (msg.Length == 3)
                                {
                                    if (!decimal.TryParse(msg[2], out buyback))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                iab = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
                                message = ZaupShop.Instance.Translate("set_buyback_price", iab.vehicleName, buyback.ToString());
                                success = ZaupShop.Instance.ShopDB.SetPremiumBuyPrice(id, buyback);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("not_in_pshop_to_set_buyback", iab.vehicleName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                        }
                        break;
                    case "lim":
                        if (!perms[5] && !perms[0])
                        {
                            message = ZaupShop.Instance.Translate("no_permission_pshop_lim");
                            SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                            return;
                        }

                        ac = true
                            ? ZaupShop.Instance.Translate("changed")
                            : ZaupShop.Instance.Translate("added");
                        switch (msg[1])
                        {
                            case "v":
                                if (msg.Length <= 2 || msg.Length >= 5)
                                {
                                    message = "Usage: /pshop lim v <id | name> <limit amount>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                ushort id = 0;
                                if (msg.Length >= 3)
                                {
                                    if (!ushort.TryParse(msg[2], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "v"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (msg.Length == 3)
                                    {
                                        message = ZaupShop.Instance.Translate("no_limit_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                int limit = 0;
                                if (msg.Length == 4)
                                {
                                    if (!int.TryParse(msg[3], out limit))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var va = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
                                message = ZaupShop.Instance.Translate("changed_limit_to_pshop", ac, va.vehicleName,
                                    limit);
                                success = ZaupShop.Instance.ShopDB.SetPremiumVehicleBuyLimit(id,
                                    limit);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("error_limiting", va.vehicleName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                            default:
                                if (msg.Length <= 1 || msg.Length >= 4)
                                {
                                    message = "Usage: /shop buy <id | name> <limit amount>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                id = 0;
                                if (msg.Length >= 2)
                                {
                                    if (!ushort.TryParse(msg[1], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "i"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (msg.Length == 2)
                                    {
                                        message = ZaupShop.Instance.Translate("no_limit_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }
                                limit = 0;
                                if (msg.Length == 3)
                                {
                                    if (!int.TryParse(msg[2], out limit))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var ia = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                                message = ZaupShop.Instance.Translate("changed_limit_to_pshop", ac, ia.itemName,
                                    limit);
                                success = ZaupShop.Instance.ShopDB.SetPremiumBuyLimit(id, limit);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("error_limiting", ia.itemName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                        }
                        break;
                    case "res":
                        if (!perms[6] && !perms[0])
                        {
                            message = ZaupShop.Instance.Translate("no_permission_pshop_res");
                            SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                            return;
                        }

                        switch (msg[1])
                        {
                            case "v":
                                if (msg.Length <= 2 || msg.Length >= 4)
                                {
                                    message = "Usage: /shop res v <id | name>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                ushort id = 0;
                                if (msg.Length == 3)
                                {
                                    if (!ushort.TryParse(msg[2], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "v"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var va = (VehicleAsset)Assets.find(EAssetType.VEHICLE, id);
                                message = ZaupShop.Instance.Translate("reset_from_pshop", va.vehicleName);
                                success = ZaupShop.Instance.ShopDB.ResetPremiumVehicleBought(id);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("not_in_pshop_to_reset", va.vehicleName);
                                SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                break;
                            default:
                                if (msg.Length <= 1 || msg.Length >= 3)
                                {
                                    message = "Usage: /shop res <id | name>";
                                    SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                    return;
                                }
                                id = 0;
                                if (msg.Length == 2)
                                {
                                    if (!ushort.TryParse(msg[1], out id))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                    if (!IsAsset(id, "i"))
                                    {
                                        message = ZaupShop.Instance.Translate("invalid_id_given");
                                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                                        return;
                                    }
                                }

                                var ia = (ItemAsset)Assets.find(EAssetType.ITEM, id);
                                message = ZaupShop.Instance.Translate("reset_from_pshop", ia.itemName);
                                success = ZaupShop.Instance.ShopDB.ResetPremiumItemBought(id);
                                if (!success)
                                    message = ZaupShop.Instance.Translate("not_in_pshop_to_reset", ia.itemName);
                                SendMessage(caller, message, "https://i.imgur.com/6EoLIWM.png", console);
                                break;
                        }
                        break;
                    default:
                        // We shouldn't get this, but if we do send an error.
                        message = "Usage: /pshop <add|chng|buy|rem|lim|res> [v] <id | name> [cost | buyback | limit] [buyback] [limit]";

                        SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                        return;
                }
            }
            else
            {
                message = "Usage: /shop <add|chng|buy|rem|lim|res> [v] <id | name> [cost | buyback | limit] [buyback] [limit]";
                SendMessage(caller, message, "https://i.imgur.com/FeIvao9.png", console);
                return;
            }
        }

        private bool IsAsset(ushort id, string type)
        {
            // Check for valid Item/Vehicle Id.
            switch (type)
            {
                case "i":
                    return Assets.find(EAssetType.ITEM, id) != null;
                case "v":
                    return Assets.find(EAssetType.VEHICLE, id) != null;
                default:
                    return false;
            }
        }

        private void SendMessage(IRocketPlayer caller, string message, string iconURL, bool console)
        {
            if (console)
                Logger.Log(message);
            else
                UnturnedChat.Say(caller, message, iconURL);
        }
    }
}