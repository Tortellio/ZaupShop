using Rocket.API;

namespace ZaupShop
{
    public class ZaupShopConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public int DatabasePort;
        public string ItemShopTableName;
        public string PremiumItemShopTableName;
        public string VehicleShopTableName;
        public string PremiumVehicleShopTableName;
        public string ItemAuctionTableName;
        public string PremiumItemAuctionTableName;
       /* public string VehicleAuctionTableName;
        public string PremiumVehicleAuctionTableName;*/
        public string ItemLimitBypassPermission;
        public string VehicleLimitBypassPermission;
        public string PremiumItemLimitBypassPermission;
        public string PremiumVehicleLimitBypassPermission;
        public bool CanBuyItems;
        public bool CanBuyVehicles;
        public bool CanSellItems;
        public bool CanSellVehicles;
        public bool QualityCounts;
        public bool AllowAuction;
        public bool ItemSaleEnable;
        public bool VehicleSaleEnable;
        public int ItemSalePercentage;
        public int VehicleSalePercentage;
        public int MinNextSaleTime;
        public int MaxNextSaleTime;
        public int SaleTime;
        public bool CanBuyPremiumItems;
        public bool CanBuyPremiumVehicles;
        public bool CanSellPremiumItems;
        public bool CanSellPremiumVehicles;
        public bool PremiumQualityCounts;
        public bool AllowPremiumAuction;
        public bool PremiumItemSaleEnable;
        public bool PremiumVehicleSaleEnable;
        public int PremiumItemSalePercentage;
        public int PremiumVehicleSalePercentage;
        public int MinNextPremiumSaleTime;
        public int MaxNextPremiumSaleTime;
        public int PremiumSaleTime;

        public void LoadDefaults()
        {
            DatabaseAddress = "127.0.0.1";
            DatabaseUsername = "unturned";
            DatabasePassword = "123456";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            ItemShopTableName = "uconomy_itemshop";
            PremiumItemShopTableName = "uconomy_premiumitemshop";
            VehicleShopTableName = "uconomy_vehicleshop";
            PremiumVehicleShopTableName = "uconomy_premiumvehicleshop";
            ItemAuctionTableName = "uconomy_itemauction";
            PremiumItemAuctionTableName = "uconomy_premiumitemauction";
            ItemLimitBypassPermission = "buy.itemlimitbypass";
            VehicleLimitBypassPermission = "buy.vehiclelimitbypass";
            PremiumItemLimitBypassPermission = "buy.premiumitemlimitbypass";
            PremiumVehicleLimitBypassPermission = "buy.premiumvehiclelimitbypass";
            CanBuyItems = true;
            CanBuyVehicles = true;
            CanSellItems = true;
            CanSellVehicles = true;
            QualityCounts = true;
            AllowAuction = true;
            ItemSaleEnable = true;
            VehicleSaleEnable = true;
            ItemSalePercentage = 15;
            VehicleSalePercentage = 15;
            MinNextSaleTime = 600;
            MaxNextSaleTime = 1200;
            SaleTime = 3;
            CanBuyPremiumItems = true;
            CanBuyPremiumVehicles = true;
            CanSellPremiumItems = true;
            CanSellPremiumVehicles = true;
            PremiumQualityCounts = true;
            AllowPremiumAuction = true;
            PremiumItemSaleEnable = true;
            PremiumVehicleSaleEnable = true;
            PremiumItemSalePercentage = 15;
            PremiumVehicleSalePercentage = 15;
            MinNextPremiumSaleTime = 600;
            MaxNextPremiumSaleTime = 1200;
            PremiumSaleTime = 3;
        }
    }
}