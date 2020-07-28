using System;
using fr34kyn01535.Uconomy;
using I18N.West;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace ZaupShop
{
    public class DatabaseMgr
    {
        internal DatabaseMgr()
        {
            new CP1250();
            CheckSchema();
        }
        internal void CheckSchema()
        {
            try
            {
                var res = ExecuteQuery(true,
                $"show tables like '{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}'");

                if (res == null)
                    ExecuteQuery(false,
                        $"CREATE TABLE `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` (`id` int(6) NOT NULL,`itemname` varchar(32) NOT NULL,`cost` decimal(15,2) NOT NULL DEFAULT '0.00',`buyback` decimal(15,2) NOT NULL DEFAULT '0.00',`limit` int(6) NOT NULL DEFAULT '0',`bought` int(10) NOT NULL DEFAULT '0',PRIMARY KEY (`id`))");

                res = ExecuteQuery(true,
                    $"show tables like '{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}'");

                if (res == null)
                    ExecuteQuery(false,
                        $"CREATE TABLE `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` (`id` int(6) NOT NULL,`vehiclename` varchar(32) NOT NULL,`cost` decimal(15,2) NOT NULL DEFAULT '0.00',`buyback` decimal(15,2) NOT NULL DEFAULT '0.00',`limit` int(6) NOT NULL DEFAULT '0',`bought` int(10) NOT NULL DEFAULT '0',PRIMARY KEY (`id`))");

                res = ExecuteQuery(true,
                    $"show columns from `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` like 'buyback'");

                if (res == null)
                    ExecuteQuery(false,
                        $"ALTER TABLE `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` ADD `buyback` decimal(15,2) NOT NULL DEFAULT '0.00', ADD `limit` int(6) NOT NULL DEFAULT '0', ADD `bought` int(10) NOT NULL DEFAULT '0'");

                res = ExecuteQuery(true,
                    $"show columns from `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` like 'limit'");

                if (res == null)
                    ExecuteQuery(false,
                        $"ALTER TABLE `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` ADD `limit` int(6) NOT NULL DEFAULT '0', ADD `bought` int(10) NOT NULL DEFAULT '0';");

                res = ExecuteQuery(true,
                    $"show tables like '{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}'");

                if (res == null)
                    ExecuteQuery(false,
                        $"CREATE TABLE `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` (`id` int(6) NOT NULL,`itemname` varchar(32) NOT NULL,`cost` decimal(15,2) NOT NULL DEFAULT '20.00',`buyback` decimal(15,2) NOT NULL DEFAULT '0.00',`limit` int(6) NOT NULL DEFAULT '0',`bought` int(10) NOT NULL DEFAULT '0',PRIMARY KEY (`id`))");

                res = ExecuteQuery(true,
                    $"show tables like '{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}'");

                if (res == null)
                    ExecuteQuery(false,
                        $"CREATE TABLE `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` (`id` int(6) NOT NULL,`vehiclename` varchar(32) NOT NULL,`cost` decimal(15,2) NOT NULL DEFAULT '100.00',`buyback` decimal(15,2) NOT NULL DEFAULT '0.00',`limit` int(6) NOT NULL DEFAULT '0',`bought` int(10) NOT NULL DEFAULT '0',PRIMARY KEY (`id`))");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "MySQL Database configurations are invalid!");
            }
        }
        public bool IsItemPriceExist(int itemId)
        {
            var scalar = ExecuteQuery(true,
                $"SELECT * FROM `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` WHERE id = @id;",
                new MySqlParameter("@id", itemId));
            return scalar != null;
        }
        public bool IsVehiclePriceExist(int vehicleId)
        {
            var scalar = ExecuteQuery(true,
                $"SELECT * FROM `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` WHERE id = @id;",
                new MySqlParameter("@id", vehicleId));
            return scalar != null;
        }
        public bool AddItem(int id, string name, decimal cost, decimal buyback, int limit, bool change)
        {
            var affected = ExecuteQuery(false,
                change
                    ? $"update `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` set itemname=@name, cost='{cost}', buyback='{buyback}', `limit`='{limit}' where id='{id}';"
                    : $"Insert into `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` (`id`, `itemname`, `cost`, `buyback`, `limit`) VALUES ('{id}', @name, '{cost}', '{buyback}', '{limit}');",
                new MySqlParameter("@name", name));

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool AddPremiumItem(int id, string name, decimal cost, decimal buyback, int limit, bool change)
        {
            var affected = ExecuteQuery(false,
                change
                    ? $"update `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` set itemname=@name, cost='{cost}', buyback='{buyback}', `limit`='{limit}' where id='{id}';"
                    : $"Insert into `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` (`id`, `itemname`, `cost`, `buyback`, `limit`) VALUES ('{id}', @name, '{cost}', '{buyback}', '{limit}');",
                new MySqlParameter("@name", name));

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool AddVehicle(int id, string name, decimal cost, decimal buyback, int limit, bool change)
        {
            var affected = ExecuteQuery(false,
                change
                    ? $"update `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` set vehiclename=@name, cost='{cost}', buyback='{buyback}', `limit`='{limit}' where id='{id}';"
                    : $"Insert into `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` (`id`, `vehiclename`, `cost`, `buyback`, `limit`) VALUES ('{id}', @name, '{cost}', '{buyback}', '{limit}');",
                new MySqlParameter("@name", name));

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool AddPremiumVehicle(int id, string name, decimal cost, decimal buyback, int limit, bool change)
        {
            var affected = ExecuteQuery(false,
                change
                    ? $"update `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` set vehiclename=@name, cost='{cost}', buyback='{buyback}', `limit`='{limit}' where id='{id}';"
                    : $"Insert into `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` (`id`, `vehiclename`, `cost`, `buyback`, `limit`) VALUES ('{id}', @name, '{cost}', '{buyback}', '{limit}');",
                new MySqlParameter("@name", name));

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public decimal GetItemCost(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `cost` from `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public decimal GetPremiumItemCost(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `cost` from `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public decimal GetVehicleCost(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `cost` from `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public decimal GetPremiumVehicleCost(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `cost` from `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public bool DeleteItem(int id)
        {
            var affected = ExecuteQuery(false,
                $"delete from `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool DeletePremiumItem(int id)
        {
            var affected = ExecuteQuery(false,
                $"delete from `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool DeleteVehicle(int id)
        {
            var affected = ExecuteQuery(false,
                $"delete from `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool DeletePremiumVehicle(int id)
        {
            var affected = ExecuteQuery(false,
                $"delete from `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetVehicleBuyPrice(int id, decimal cost)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` set `buyback`='{cost}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetPremiumVehicleBuyPrice(int id, decimal cost)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` set `buyback`='{cost}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetVehicleBuyLimit(int id, int limit)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` set `limit`='{limit}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetPremiumVehicleBuyLimit(int id, int limit)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` set `limit`='{limit}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetBuyPrice(int id, decimal cost)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` set `buyback`='{cost}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetPremiumBuyPrice(int id, decimal cost)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` set `buyback`='{cost}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetBuyLimit(int id, int limit)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` set `limit`='{limit}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool SetPremiumBuyLimit(int id, int limit)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` set `limit`='{limit}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool IncreaseItemBought(int id, int amount)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` set `bought`=`bought`+'{amount}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool IncreasePremiumItemBought(int id, int amount)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` set `bought`=`bought`+'{amount}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool IncreaseVehicleBought(int id, int amount)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` set `bought`=`bought`+'{amount}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool IncreasePremiumVehicleBought(int id, int amount)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` set `bought`=`bought`+'{amount}' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool ResetItemBought(int id)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` set `bought`='0' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool ResetPremiumItemBought(int id)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` set `bought`='0' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool ResetVehicleBought(int id)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` set `bought`='0' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public bool ResetPremiumVehicleBought(int id)
        {
            var affected = ExecuteQuery(false,
                $"update `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` set `bought`='0' where id='{id}';");

            if (affected == null) return false;

            int.TryParse(affected.ToString(), out var rows);

            return rows > 0;
        }

        public decimal GetItemBuyPrice(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `buyback` from `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public decimal GetPremiumItemBuyPrice(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `buyback` from `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public decimal GetVehicleBuyPrice(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `buyback` from `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public decimal GetPremiumVehicleBuyPrice(int id)
        {
            var num = new decimal(0);
            var obj = ExecuteQuery(true,
                $"select `buyback` from `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) decimal.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetItemLimit(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `limit` from `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetPremiumItemLimit(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `limit` from `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetVehicleLimit(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `limit` from `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetPremiumVehicleLimit(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `limit` from `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetItemBought(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `bought` from `{ZaupShop.Instance.Configuration.Instance.ItemShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetPremiumItemBought(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `bought` from `{ZaupShop.Instance.Configuration.Instance.PremiumItemShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetVehicleBought(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `bought` from `{ZaupShop.Instance.Configuration.Instance.VehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }

        public int GetPremiumVehicleBought(int id)
        {
            int num = -1;
            var obj = ExecuteQuery(true,
                $"select `bought` from `{ZaupShop.Instance.Configuration.Instance.PremiumVehicleShopTableName}` where `id` = '{id}';");

            if (obj != null) int.TryParse(obj.ToString(), out num);

            return num;
        }
        private MySqlConnection CreateConnection()
        {
            MySqlConnection mySqlConnection = null;
            try
            {
                if (ZaupShop.Instance.Configuration.Instance.DatabasePort == 0)
                    ZaupShop.Instance.Configuration.Instance.DatabasePort = 3306;
                mySqlConnection = new MySqlConnection(
                    $"SERVER={ZaupShop.Instance.Configuration.Instance.DatabaseAddress};DATABASE={ZaupShop.Instance.Configuration.Instance.DatabaseName};UID={ZaupShop.Instance.Configuration.Instance.DatabaseUsername};PASSWORD={ZaupShop.Instance.Configuration.Instance.DatabasePassword};PORT={ZaupShop.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }

            return mySqlConnection;
        }

        /// <summary>
        /// Executes a MySql query.
        /// </summary>
        /// <param name="isScalar">If the query is expected to return a value.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="parameters">The MySqlParameters to be added to the command.</param>
        /// <returns>The value if isScalar is true, null otherwise.</returns>
        public object ExecuteQuery(bool isScalar, string query, params MySqlParameter[] parameters)
        {
            object result = null;

            using (var connection = CreateConnection())
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText = query;

                    foreach (var parameter in parameters)
                        command.Parameters.Add(parameter);

                    connection.Open();
                    result = isScalar ? command.ExecuteScalar() : command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }
    }
}