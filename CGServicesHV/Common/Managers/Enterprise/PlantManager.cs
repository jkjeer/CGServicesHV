using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
using Plant = CGServicesHV.Common.Types.Enterprise.Plant;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class PlantManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PlantManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"PlantManager.GetPlantAddress", GetPlantAddress()},
                {"PlantManager.GetPlantAddresses", GetPlantAddresses()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /******************************************************************
        * Plant
        ******************************************************************/
        public static Types.Enterprise.Plant GetPlant(string plantCode)
        {
            Types.Enterprise.Plant plant;
            try
            {
                var plants = GetPlantCache();
                plant = plants.Find(t => t.PlantNum == plantCode || string.Equals(t.PlantCode, plantCode, StringComparison.CurrentCultureIgnoreCase) || t.NumericValue.ToString() == plantCode);
                if (plant == null)
                {
                    throw new Exception($"Error: Plant {plantCode} not found.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Plant: {plantCode} {e.Message}: {e.StackTrace}");
            }
            return plant;
        }

        public static Plants GetPlants(string plantCode)
        {
            var plants = new Plants();
            try
            {
                var plantItemCache = GetPlantItemCache();
                var plantList = plantCode.ToUpper().Split(',').ToList();
                foreach (var plantItem in plantList)
                {
                    var plant = plantItemCache.Where(t => t.GroupCode == string.Empty)
                        .Where(s => string.Equals(s.Plant.PlantNum, plantItem, StringComparison.CurrentCultureIgnoreCase) || string.Equals(s.Plant.PlantCode, plantItem, StringComparison.CurrentCultureIgnoreCase) || string.Equals(s.Plant.NumericValue.ToString(), plantItem, StringComparison.CurrentCultureIgnoreCase)).Select(z => z.Plant).FirstOrDefault();
                    if (plant != null)
                    {
                        plants.Add(plant);
                    }
                    else
                    {
                        var plantItems = plantItemCache.Where(t => t.GroupCode == plantItem).Select(x => x.Plant).ToList();
                        plants.AddRange(plantItems);
                    }
                    if (!plants.Any())
                    {
                        throw new Exception($"Error: Plant {plantCode} not found.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Plant: {plantCode} {e.Message}: {e.StackTrace}");
            }
            return plants;
        }

        /******************************************************************
        * Cache
        ******************************************************************/
        public static PlantItems GetPlantItemCache()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains("PlantItems"))
                return (PlantItems)cache.Get("PlantItems");
            else
            {
                var cn = new SqlConnection();
                PlantItems plantItems;
                try
                {
                    cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    cn.Open();
                    plantItems = GetPlantItems(cn);

                    // Store data in the cache
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    //cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                    cache.Add("PlantItems", plantItems, cacheItemPolicy);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }
                return plantItems;
            }
        }

        public static SqlCommand GetPlantItems()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                SELECT
                    PG.GROUP_CD,
	                PT.PLANT_NUMERIC_CD,
	                PT.PLANT_ALPHA_CD,
	                PT.PLANT_NAME,
                    PT.PLANT_ADDRESS_NAME,
	                PT.PLANT_NO,
	                PT.TIME_ZONE_CD,
	                PT.TIME_ZONE_OFFSET,
                    PT.TIME_ZONE_NAME,
                    PT.CUSTOMER_NO,
                    PT.COMPANY_CD,
                    PT.SHORT_COMPANY_CD
                FROM
                    SVCCG.PLANT_GROUP PG
                    INNER JOIN SVCCG.PLANT_GROUP_XREF PGX
                    ON  PGX.GROUP_CD = PG.GROUP_CD
	                INNER JOIN SVCCG.PLANT PT
	                ON PT.PLANT_ALPHA_CD = PGX.PLANT_ALPHA_CD
                UNION
                SELECT
	                '' AS GROUP_CD,
                    PLANT_NUMERIC_CD,
	                PLANT_ALPHA_CD,
	                PLANT_NAME,
                    PLANT_ADDRESS_NAME,
	                PLANT_NO,
	                TIME_ZONE_CD,
	                TIME_ZONE_OFFSET,
                    TIME_ZONE_NAME,
                    CUSTOMER_NO,
                    COMPANY_CD,
                    SHORT_COMPANY_CD
                FROM
                    SVCCG.PLANT
            ";
            return cmd;
        }

        public static PlantItems GetPlantItems(SqlConnection cn)
        {
            var plantItems = new PlantItems();
            try
            {
                var cmd = GetPlantItems();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var plantItem = new PlantItem
                        {
                            GroupCode = CastDbNull.To(reader["GROUP_CD"], string.Empty).Trim().ToUpper()
                        };
                        var plant = new Types.Enterprise.Plant
                        {
                            PlantNum = CastDbNull.To(reader["PLANT_NUMERIC_CD"], string.Empty).Trim(),
                            PlantCode = CastDbNull.To(reader["PLANT_ALPHA_CD"], string.Empty).Trim().ToUpper(),
                            Name = CastDbNull.To(reader["PLANT_NAME"], string.Empty).Trim(),
                            NumericValue = CastDbNull.To(reader["PLANT_NO"], 0),
                            AddressNum = CastDbNull.To<decimal>(reader["CUSTOMER_NO"], 0),
                            AddressName = CastDbNull.To(reader["PLANT_ADDRESS_NAME"], string.Empty).Trim(),
                            TimeOffset = CastDbNull.To(reader["TIME_ZONE_OFFSET"], 0),
                            TimeZone = CastDbNull.To(reader["TIME_ZONE_CD"], string.Empty).Trim(),
                            TimeZoneName = CastDbNull.To(reader["TIME_ZONE_NAME"], string.Empty).Trim(),
                            CompanyNum = CastDbNull.To(reader["COMPANY_CD"], string.Empty).Trim(),
                            ShortCompanyNum = CastDbNull.To(reader["SHORT_COMPANY_CD"], string.Empty).Trim()
                        };
                        plantItem.Plant = plant;
                        plantItems.Add(plantItem);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return plantItems;
        }

        /******************************************************************
        * Plant - Address
        ******************************************************************/
        public iDB2Command GetPlantAddress()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        ABAN8,
                        ABAT1,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR
                    FROM 
                        F0101
	                    LEFT OUTER JOIN F0116 
                        ON  ALAN8 = ABAN8 AND
						    ALEFTB = ABEFTB
                    WHERE 
                        ABAN8 = ?";
                cmd.Parameters.Add(new iDB2Parameter("AddressBookNum", iDB2DbType.iDB2Decimal));
                cmd.Parameters["AddressBookNum"].Precision = 8;
                cmd.Parameters["AddressBookNum"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public iDB2Command GetPlantAddresses()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        ABAN8,
                        ABAT1,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR
                    FROM 
                        F0101
	                    LEFT OUTER JOIN F0116 
                        ON  ALAN8 = ABAN8 AND
						    ALEFTB = ABEFTB
                    WHERE 
                        ABAT1 = 'F'";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public Addresses GetPlantAddresses(BusinessSystemSession e1Session, decimal addressNum)
        {
            var addresses = new Addresses();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetPlantAddresses")) ? e1Session.DbCommands["GetPlantAddresses"] : GetPlantAddresses();
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        var address = new Address();
                        address.AddressNum = CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        address.Name = CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        address.Address1 = CastDbNull.To(reader["ALADD1"], string.Empty).Trim();
                        address.Address2 = CastDbNull.To(reader["ALADD2"], string.Empty).Trim();
                        address.Address3 = CastDbNull.To(reader["ALADD3"], string.Empty).Trim();
                        address.Address4 = CastDbNull.To(reader["ALADD4"], string.Empty).Trim();
                        address.City = CastDbNull.To(reader["ALCTY1"], string.Empty).Trim();
                        address.State = CastDbNull.To(reader["ALADDS"], string.Empty).Trim();
                        address.PostalCode = CastDbNull.To(reader["ALADDZ"], string.Empty).Trim();
                        address.Country = CastDbNull.To(reader["ALCTR"], string.Empty).Trim();
                        addresses.Add(address);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + addresses.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return addresses;
        }

        public Address GetPlantAddress(BusinessSystemSession e1Session, decimal addressNum)
        {
            var address = new Address();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetPlantAddress")) ? e1Session.DbCommands["GetPlantAddress"] : GetPlantAddress();
                cmd.Parameters[0].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        address.AddressNum = CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        address.Name = CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        address.Address1 = CastDbNull.To(reader["ALADD1"], string.Empty).Trim();
                        address.Address2 = CastDbNull.To(reader["ALADD2"], string.Empty).Trim();
                        address.Address3 = CastDbNull.To(reader["ALADD3"], string.Empty).Trim();
                        address.Address4 = CastDbNull.To(reader["ALADD4"], string.Empty).Trim();
                        address.City = CastDbNull.To(reader["ALCTY1"], string.Empty).Trim();
                        address.State = CastDbNull.To(reader["ALADDS"], string.Empty).Trim();
                        address.PostalCode = CastDbNull.To(reader["ALADDZ"], string.Empty).Trim();
                        address.Country = CastDbNull.To(reader["ALCTR"], string.Empty).Trim();
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return address;
        }

        /******************************************************************
        * DEPRECATED
        ******************************************************************/
        public static Plants GetPlantCache()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains("Plants"))
                return (Plants)cache.Get("Plants");
            else
            {
                var cn = new SqlConnection();
                var plants = new Plants();
                try
                {
                    cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    cn.Open();
                    plants = GetPlantsFromDb(cn);

                    // Store data in the cache
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    //cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                    cache.Add("Plants", plants, cacheItemPolicy);
                }
                catch (Exception e)
                {
                    var err = new Error(e);
                }
                finally
                {
                    cn.Close();
                }
                return plants;
            }
        }

        public static PlantGroups GetPlantGroupCache()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains("PlantGroups"))
                return (PlantGroups)cache.Get("PlantGroups");
            else
            {
                var cn = new SqlConnection();
                var plantGroups = new PlantGroups();
                try
                {
                    cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    cn.Open();
                    var plantGroupItems = GetPlantGroupsFromDb(cn);

                    var plantCache = GetPlantCache();
                    foreach (var plantGroupItem in plantGroupItems.GroupBy(x => x.GroupCode))
                    {
                        var plantGroup = new PlantGroup
                        {
                            GroupCode = plantGroupItem.Key
                        };
                        foreach (var pg in plantGroupItem)
                        {
                            var p = plantCache.Find(t => t.PlantNum == pg.PlantNumericCode);
                            plantGroup.Plants.Add(p);
                        }
                        plantGroups.Add(plantGroup);
                    }

                    // Store data in the cache
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                    //cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                    cache.Add("PlantGroups", plantGroups, cacheItemPolicy);
                }
                catch (Exception e)
                {
                    var err = new Error(e);
                }
                finally
                {
                    cn.Close();
                }
                return plantGroups;
            }
        }

        public static SqlCommand GetPlantsFromDb()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        PLANT_NUMERIC_CD, 
                        PLANT_ALPHA_CD, 
                        PLANT_NAME, 
                        PLANT_NO, 
                        CUSTOMER_NO, 
                        TIME_ZONE_CD, 
                        TIME_ZONE_OFFSET,
                        COMPANY_CD,
                        SHORT_COMPANY_CD            
                    FROM 
	                    SVCCG.PLANT
                    "
            };
            return cmd;
        }

        public static Plants GetPlantsFromDb(SqlConnection cn)
        {
            var plants = new Plants();
            try
            {
                var cmd = GetPlantsFromDb();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var plant = new Types.Enterprise.Plant();
                        plant.PlantNum = CastDbNull.To(reader["PLANT_NUMERIC_CD"], string.Empty).Trim();
                        plant.PlantCode = CastDbNull.To(reader["PLANT_ALPHA_CD"], string.Empty).Trim();
                        plant.Name = CastDbNull.To(reader["PLANT_NAME"], string.Empty).Trim();
                        plant.NumericValue = CastDbNull.To(reader["PLANT_NO"], 0);
                        plant.AddressNum = CastDbNull.To<decimal>(reader["CUSTOMER_NO"], 0);
                        plant.TimeOffset = CastDbNull.To(reader["TIME_ZONE_OFFSET"], 0);
                        plant.TimeZone = CastDbNull.To(reader["TIME_ZONE_CD"], string.Empty).Trim();
                        plant.CompanyNum = CastDbNull.To(reader["COMPANY_CD"], string.Empty).Trim();
                        plant.ShortCompanyNum = CastDbNull.To(reader["SHORT_COMPANY_CD"], string.Empty).Trim();
                        plants.Add(plant);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return plants;
        }

        public static SqlCommand GetPlantGroupsFromDb()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            PG.GROUP_CD,
                PGX.PLANT_NUMERIC_CD
            FROM
                SVCCG.PLANT_GROUP PG
	            INNER JOIN SVCCG.PLANT_GROUP_XREF PGX
	            ON PGX.GROUP_CD = PG.GROUP_CD
            ORDER BY 1,2";
            return cmd;
        }

        public static List<PlantGroupItem> GetPlantGroupsFromDb(SqlConnection cn)
        {
            var items = new List<PlantGroupItem>();
            try
            {
                var cmd = GetPlantGroupsFromDb();
                cmd.Connection = cn;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {

                    while (reader.Read())
                    {
                        var item = new PlantGroupItem();
                        item.GroupCode = CastDbNull.To(reader["GROUP_CD"], string.Empty).Trim();
                        item.PlantNumericCode = CastDbNull.To(reader["PLANT_NUMERIC_CD"], string.Empty).Trim();
                        items.Add(item);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + " JobName: " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return items;
        }
    }
}