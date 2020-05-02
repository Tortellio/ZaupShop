using fr34kyn01535.Uconomy;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;
using System.Data;

namespace ZaupShop
{
    public class DatabaseMgrAuction
    {
        // The base code for this class comes from Uconomy itself.
        internal DatabaseMgrAuction()
        {
            new I18N.West.CP1250(); //Workaround for database encoding issues with mono
            CheckSchema();
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (ZaupShop.Instance.Configuration.Instance.DatabasePort == 0) ZaupShop.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4}; Convert Zero Datetime=True;", ZaupShop.Instance.Configuration.Instance.DatabaseAddress, ZaupShop.Instance.Configuration.Instance.DatabaseName, ZaupShop.Instance.Configuration.Instance.DatabaseUsername, ZaupShop.Instance.Configuration.Instance.DatabasePassword, ZaupShop.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public bool AddAuctionItem(int id, string itemid, string itemname, decimal price, decimal shopprice, int quality, string sellerID)
        {
            bool added = false;
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "Insert into `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` (`id`,`itemid`,`ItemName`,`Price`,`ShopPrice`,`Quality`,`SellerID`) Values('" + id.ToString() + "', '" + itemid + "', '" + itemname + "', '" + price + "', '" + shopprice + "', '" + quality + "', '" + sellerID + "')";
                Connection.Open();
                Command.ExecuteNonQuery();
                Connection.Close();
                added = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return added;
        }
        public bool AddPremiumAuctionItem(int id, string itemid, string itemname, decimal price, decimal shopprice, int quality, string sellerID)
        {
            bool added = false;
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "Insert into `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` (`id`,`itemid`,`ItemName`,`Price`,`ShopPrice`,`Quality`,`SellerID`) Values('" + id.ToString() + "', '" + itemid + "', '" + itemname + "', '" + price + "', '" + shopprice + "', '" + quality + "', '" + sellerID + "')";
                Connection.Open();
                Command.ExecuteNonQuery();
                Connection.Close();
                added = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return added;
        }
        public bool CheckAuctionExist(int id)
        {
            bool exist = false;
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `id` = " + id + "";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                    exist = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return exist;
        }
        public bool CheckPremiumAuctionExist(int id)
        {
            bool exist = false;
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `id` = " + id + "";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                    exist = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return exist;
        }
        public int GetLastAuctionNo()
        {
            DataTable dt = new DataTable();
            int AuctionNo = 0;
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    if (dt.Rows[i].ItemArray[0].ToString() != i.ToString())
                    {
                        AuctionNo = i;
                        return AuctionNo;
                    }
                }
                AuctionNo = dt.Rows.Count;

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionNo;
        }
        public int GetLastPremiumAuctionNo()
        {
            DataTable dt = new DataTable();
            int AuctionNo = 0;
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    if (dt.Rows[i].ItemArray[0].ToString() != i.ToString())
                    {
                        AuctionNo = i;
                        return AuctionNo;
                    }
                }
                AuctionNo = dt.Rows.Count;

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionNo;
        }
        public string[] GetAllItemNameWithQuality()
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "`";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }
        public string[] GetAllPremiumItemNameWithQuality()
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "`";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }
        public string[] FindAllItemNameWithQualityByID(string ItemID)
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `itemid` = " + ItemID + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `itemid` = " + ItemID + "";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }
        public string[] FindAllPremiumItemNameWithQualityByID(string ItemID)
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `itemid` = " + ItemID + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `itemid` = " + ItemID + "";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }
        public string[] FindAllItemNameWithQualityByItemName(string Itemname)
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `ItemName` = " + Itemname + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `ItemName` = " + Itemname + "";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }
        public string[] FindAllPremiumItemNameWithQualityByItemName(string Itemname)
        {
            DataTable dt = new DataTable();
            DataTable quality = new DataTable();
            string[] ItemName = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `ItemName` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `ItemName` = " + Itemname + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                command.CommandText = "select `Quality` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `ItemName` = " + Itemname + "";
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(quality);
                connection.Close();
                ItemName = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemName[i] = dt.Rows[i].ItemArray[0].ToString() + "(" + quality.Rows[i].ItemArray[0].ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemName;
        }
        public string[] GetAllAuctionID()
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }
        public string[] GetAllPremiumAuctionID()
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }
        public string[] FindAllItemPriceByID(string ItemID)
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `itemid` = " + ItemID + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }
        public string[] FindAllPremiumItemPriceByID(string ItemID)
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `itemid` = " + ItemID + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }
        public string[] FindAllItemPriceByItemName(string ItemName)
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `ItemName` = " + ItemName + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }
        public string[] FindAllPremiumItemPriceByItemName(string ItemName)
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `ItemName` = " + ItemName + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }
        public string[] GetAllItemPrice()
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }
        public string[] GetAllPremiumItemPrice()
        {
            DataTable dt = new DataTable();
            string[] ItemPrice = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `Price` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "`";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                ItemPrice = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    ItemPrice[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemPrice;
        }
        public string[] AuctionBuy(int auctionID)
        {
            DataTable dt = new DataTable();
            string[] ItemInfo = new string[5];
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `itemid`,`ItemName`, `Price`, `Quality`, `SellerID` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `id` = " + auctionID.ToString() + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Columns.Count); i++)
                {
                    ItemInfo[i] = dt.Rows[0].ItemArray[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemInfo;
        }
        public string[] PremiumAuctionBuy(int auctionID)
        {
            DataTable dt = new DataTable();
            string[] ItemInfo = new string[5];
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `itemid`,`ItemName`, `Price`, `Quality`, `SellerID` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `id` = " + auctionID.ToString() + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Columns.Count); i++)
                {
                    ItemInfo[i] = dt.Rows[0].ItemArray[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemInfo;
        }
        public string[] AuctionCancel(int auctionID)
        {
            DataTable dt = new DataTable();
            string[] ItemInfo = new string[5];
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `itemid`, `Quality` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `id` = " + auctionID.ToString() + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Columns.Count); i++)
                {
                    ItemInfo[i] = dt.Rows[0].ItemArray[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemInfo;
        }
        public string[] PremiumAuctionCancel(int auctionID)
        {
            DataTable dt = new DataTable();
            string[] ItemInfo = new string[5];
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `itemid`, `Quality` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `id` = " + auctionID.ToString() + "";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < (dt.Columns.Count); i++)
                {
                    ItemInfo[i] = dt.Rows[0].ItemArray[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return ItemInfo;
        }
        public void DeleteAuction(string auctionID)
        {
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "delete from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `id` = '" + auctionID + "'";
                Connection.Open();
                object obj = Command.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }
        public void DeletePremiumAuction(string auctionID)
        {
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "delete from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `id` = '" + auctionID + "'";
                Connection.Open();
                object obj = Command.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }
        public string GetOwner(int auctionID)
        {
            string ID = "";
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select `SellerID` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `id` = '" + auctionID + "'";
                Connection.Open();
                object obj = Command.ExecuteScalar();
                if (obj != null)
                    ID = obj.ToString().Trim();
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
            return ID;
        }
        public string GetPremiumOwner(int auctionID)
        {
            string ID = "";
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select `SellerID` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `id` = '" + auctionID + "'";
                Connection.Open();
                object obj = Command.ExecuteScalar();
                if (obj != null)
                    ID = obj.ToString().Trim();
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
            return ID;
        }
        public string[] FindItemByID(string ItemID)
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `itemid` = '" + ItemID + "'";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }
        public string[] FindPremiumItemByID(string ItemID)
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `itemid` = '" + ItemID + "'";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }
        public string[] FindItemByName(string ItemName)
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` where `ItemName` = '" + ItemName + "'";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }
        public string[] FindPremiumItemByName(string ItemName)
        {
            DataTable dt = new DataTable();
            string[] AuctionID = { };
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` where `ItemName` = '" + ItemName + "'";
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);
                connection.Close();
                AuctionID = new string[dt.Rows.Count];
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    AuctionID[i] = dt.Rows[i].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return AuctionID;
        }
        internal void CheckSchema()
        {
            try
            {
                MySqlConnection Connection = CreateConnection();
                MySqlCommand Command = Connection.CreateCommand();
                Connection.Open();
                object test;
                if (ZaupShop.Instance.Configuration.Instance.AllowAuction)
                {
                    Command.CommandText = "show tables like '" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "'";
                    test = Command.ExecuteScalar();
                    if (test == null)
                    {
                        Command.CommandText = "CREATE TABLE `" + ZaupShop.Instance.Configuration.Instance.ItemAuctionTableName + "` (`id` int(6) NOT NULL,`itemid` int(7) NOT NULL,`ItemName` varchar(56) NOT NULL,`Price` decimal(15,2) NOT NULL DEFAULT '20.00',`ShopPrice` decimal(15,2) NOT NULL DEFAULT '0.00', `Quality` int(3) NOT NULL DEFAULT '50', `SellerID` varchar(20) NOT NULL, PRIMARY KEY (`id`))";
                        Command.ExecuteNonQuery();
                    }
                }
                if (ZaupShop.Instance.Configuration.Instance.AllowPremiumAuction)
                {
                    Command.CommandText = "show tables like '" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "'";
                    test = Command.ExecuteScalar();
                    if (test == null)
                    {
                        Command.CommandText = "CREATE TABLE `" + ZaupShop.Instance.Configuration.Instance.PremiumItemAuctionTableName + "` (`id` int(6) NOT NULL,`itemid` int(7) NOT NULL,`ItemName` varchar(56) NOT NULL,`Price` decimal(15,2) NOT NULL DEFAULT '20.00',`ShopPrice` decimal(15,2) NOT NULL DEFAULT '0.00', `Quality` int(3) NOT NULL DEFAULT '50', `SellerID` varchar(20) NOT NULL, PRIMARY KEY (`id`))";
                        Command.ExecuteNonQuery();
                    }
                }
                Connection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }
    }
}