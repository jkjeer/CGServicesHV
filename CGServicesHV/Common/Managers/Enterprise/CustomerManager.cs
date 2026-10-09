using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class CustomerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CustomerManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"CustomerManager.GetCustomer", GetCustomer()},
                {"CustomerManager.GetLegacyCustomer", GetLegacyCustomer()},
                {"CustomerManager.GetCustomerAddress", GetCustomerAddress()},
                {"CustomerManager.GetCustomerItemReference", GetCustomerItemReference()},
                {"CustomerManager.GetParentCustomerNum", GetParentCustomerNum()},
                {"CustomerManager.GetPlantAddressNum", GetPlantAddressNum()},
                {"CustomerManager.GetTerms", GetTerms()},
                {"CustomerManager.GetTaxRate", GetTaxRate()}
            };
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        private static iDB2Command GetCustomersDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                     SELECT  F0101.ABAN8 
                            ,F0101.ABALKY 
                            ,F0101.ABTAX 
                            ,F0101.ABALPH 
                            ,F0101.ABDC 
                            ,F0101.ABMCU 
                            ,F0101.ABAT1 
                            ,F0005.DRDL01
                            ,F0101.ABTAXC 
                            ,F0101.ABAT2 
                            ,F0101.ABAT3 
                            ,F0101.ABAT4 
                            ,F0101.ABAT5 
                            ,F0101.ABATP 
                            ,F0101.ABATR 
                            ,F0101.ABATPR 
                            ,F0101.ABATE 
                            ,F0101.ABAN81 
                            ,F0101.ABAC05 
                            ,F0101.ABAC06 
                            ,F0101.ABAC08 
                            ,F0101.ABAC10 
                            ,F0101.ABAC13 
                            ,F0101.ABAC16 
                            ,F0101.ABAC18 
                            ,F0101.ABAC26 
                            ,F0101.ABAC29 
                            ,F0101.ABAC30 
                            ,F0116.ALADD1 
                            ,F0116.ALADD2 
                            ,F0116.ALADD3 
                            ,F0116.ALADD4 
                            ,F0116.ALADDZ 
                            ,F0116.ALCTY1 
                            ,F0116.ALCOUN 
                            ,F0116.ALADDS 
                            ,F0116.ALCTR
                            ,F0116.ALEFTB
                            ,F0401.A6UPMJ 
                            ,F0401.A6UPMT 
                            ,F0101.ABUPMJ 
                            ,F0101.ABUPMT 
                            ,F0116.ALUPMJ 
                            ,F0116.ALUPMT 
                            ,F0115.WPAR1
                            ,F0115.WPPH1
                            ,COALESCE(MAPA8,ABAN8) AS MAPA8
                            ,ROW_NUMBER() OVER ( PARTITION BY F0116.ALAN8 ORDER BY F0116.ALEFTB ) AS RowNumber
                    FROM    F0401 F0401
                            INNER JOIN F0101 F0101 ON F0101.ABAN8 = F0401.A6AN8 
                            LEFT OUTER JOIN ( SELECT * 
                                                FROM (
		                                                SELECT   ALAN8 
                                                                ,ALADD1 
                                                                ,ALADD2 
                                                                ,ALADD3 
                                                                ,ALADD4 
                                                                ,ALADDZ 
                                                                ,ALCTY1 
                                                                ,ALCOUN 
                                                                ,ALADDS 
                                                                ,ALCTR
                                                                ,ALEFTB
                                                                ,ALUPMJ 
                                                                ,ALUPMT 
		                                                        ,ROW_NUMBER() OVER ( PARTITION BY ALAN8 ORDER BY ALEFTB ) AS RowNumber
		                                                FROM F0116 
	                                                ) x WHERE RowNumber = 1 	
                                            ) F0116 ON F0116.ALAN8 = F0101.ABAN8 
	                        LEFT OUTER JOIN F0115 F0115 ON WPAN8 = ABAN8 AND
				                                     WPIDLN = 0 AND
				                                     WPRCK7 = 1 AND
				                                     WPCNLN = 0
	                        LEFT OUTER JOIN F0150 ON MAAN8 = ABAN8
                            LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 F0005 ON DRSY = '01' AND
                                                                                                      DRRT = 'ST' AND
                                                                                                      TRIM(DRKY) = ABAT1
                    {filterString}"
            };
            return cmd;
        }

        public iDB2Command GetCustomer()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    ABAN8,
                        ABALPH,
                        ABALKY,
                        ABAC30,
                        ABAC29,
                        ABAT1              
                    FROM 
	                    F0101
                    WHERE 
                        ABAN8 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AddressNum"].Precision = 8;
            cmd.Parameters["AddressNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetCustomers()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    ABAN8,
                        ABALPH,
                        ABALKY,
                        ABAC30,
                        ABAC29,
                        ABAT1                
                    FROM 
	                    F0101
                    WHERE 
                        ABAT1 = 'C'"
            };
            return cmd;
        }

        public iDB2Command GetLegacyCustomer()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    ABAN8,
                        ABALPH,
                        ABALKY,
                        ABAC30,
                        ABAC29,
                        ABAT1                
                    FROM 
	                    F0101
                    WHERE 
                        ABALKY = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("LegacyNum", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static Customers GetCustomers(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, decimal addressNum = 0)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var customers = new Customers();
            var filters = new List<string>();
            try
            {
                filters.Add("RowNumber = 1 AND \r\n");
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("A6UPMJ", "A6UPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString6 = E1TimeManager.GetE1DateTimeQueryString("MAUPMJ", "MAUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + " OR " + queryString6 + ")" + " AND \r\n");
                }

                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetCustomersDynamic(filters);
                }
                else
                {
                    cmd = GetCustomersDynamic(filters);
                }

                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                //using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                //{
                //    //while (reader.Read())
                //    //{
                //    //    customer.CustomerNum = addressNum;
                //    //    customer.LegacyCustomerNum = Data.CastDbNull.To(reader["ABALKY"], string.Empty).Trim();
                //    //    customer.CustomerType = Data.CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                //    //    customer.CustomerName = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                //    //    var abac30 = Data.CastDbNull.To(reader["ABAC30"], string.Empty).Trim();
                //    //    customer.IsPlant = (abac30 != string.Empty & customer.CustomerType == "F");
                //    //    if (customer.IsPlant)
                //    //    {
                //    //        customer.PlantNum = abac30;
                //    //        customer.PlantCode = e1Session.PlantMap.GetAttributesByNum(abac30).Code;
                //    //        customer.ShortCompanyNum = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                //    //        customer.CompanyNum = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim().PadLeft(5).Replace(' ', '0');
                //    //    }
                //    //    if (includeAddress)
                //    //    {
                //    //        customer.Address = GetCustomerAddress(e1Session, addressNum);
                //    //    }
                //    //}
                //    reader.Close();
                //    Logger.Trace("ROWS: " + customers.Count);
                //}
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return customers;
        }

        public Customer GetCustomer(BusinessSystemSession e1Session, decimal addressNum, bool includeAddress = false, SqlDmlResponse sqlResponse = null)
        {
            var customer = new Customer();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetCustomer")) ? e1Session.DbCommands["CustomerManager.GetCustomer"] : GetCustomer();
                cmd.Parameters["AddressNum"].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        customer.CustomerNum = addressNum;
                        customer.LegacyCustomerNum = Data.CastDbNull.To(reader["ABALKY"], string.Empty).Trim();
                        customer.CustomerType = Data.CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                        customer.CustomerName = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        var abac30 = Data.CastDbNull.To(reader["ABAC30"], string.Empty).Trim();
                        customer.IsPlant = (abac30 != string.Empty & customer.CustomerType == "F");
                        if (customer.IsPlant)
                        {
                            customer.PlantNum = abac30;
                            customer.PlantCode = PlantManager.GetPlant(abac30).PlantCode;
                            customer.ShortCompanyNum = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                            customer.CompanyNum = Data.CastDbNull.To(reader["ABAC29"], string.Empty).Trim().PadLeft(5).Replace(' ', '0');
                        }

                        if (includeAddress)
                        {
                            customer.Address = GetCustomerAddress(e1Session, addressNum);
                        }
                    }

                    reader.Close();
                    sqlResponse.RowsAffected = 1;
                    sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                    sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return customer;
        }

        public Customers GetCustomers(BusinessSystemSession e1Session, bool includeAddress = false)
        {
            var customers = new Customers();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetCustomers")) ? e1Session.DbCommands["CustomerManager.GetCustomers"] : GetCustomers();
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var customer = new Customer
                        {
                            CustomerNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0),
                            LegacyCustomerNum = Data.CastDbNull.To(reader["ABALKY"], string.Empty).Trim(),
                            CustomerName = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim(),
                            PlantNum = Data.CastDbNull.To(reader["ABAC30"], string.Empty).Trim()
                        };
                        customer.IsPlant = (customer.PlantNum != string.Empty);
                        if (includeAddress)
                        {
                            customer.Address = GetCustomerAddress(e1Session, customer.CustomerNum);
                        }

                        customers.Add(customer);
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return customers;
        }

        public Customer GetLegacyCustomer(BusinessSystemSession e1Session, string legacyNum, bool includeAddress = false)
        {
            var customer = new Customer();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetLegacyCustomer")) ? e1Session.DbCommands["CustomerManager.GetLegacyCustomer"] : GetLegacyCustomer();
                cmd.Parameters["LegacyNum"].Value = legacyNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        customer.CustomerNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        customer.LegacyCustomerNum = legacyNum;
                        customer.CustomerName = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        customer.PlantNum = Data.CastDbNull.To(reader["ABAC30"], string.Empty).Trim();
                        customer.IsPlant = (customer.PlantNum != string.Empty);
                        if (includeAddress)
                        {
                            customer.Address = GetCustomerAddress(e1Session, customer.CustomerNum);
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return customer;
        }

        public iDB2Command GetCustomerItemReference()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    @"SELECT
                    IVCITM,
                    IVXRT,
                    IVDSC1,
                    IVDSC2
                FROM
                    F4104 
                    INNER JOIN F0101
                    ON ABAN8 = IVAN8
                WHERE
                    IVAN8 = ? AND
                    IVITM = ? AND
                    IVEFTJ <= ? AND
                    IVEXDJ >= ? AND
                    IVXRT = 'C' AND
                    ABAT1 = 'C'";
                cmd.Parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
                cmd.Parameters["AddressNum"].Precision = 8;
                cmd.Parameters["AddressNum"].Scale = 0;
                cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
                cmd.Parameters["ItemNum"].Precision = 8;
                cmd.Parameters["ItemNum"].Scale = 0;
                cmd.Parameters.Add(new iDB2Parameter("EffectiveStartDateId", iDB2DbType.iDB2Decimal));
                cmd.Parameters["EffectiveStartDateId"].Precision = 6;
                cmd.Parameters["EffectiveStartDateId"].Scale = 0;
                cmd.Parameters.Add(new iDB2Parameter("EffectiveEndDateId", iDB2DbType.iDB2Decimal));
                cmd.Parameters["EffectiveEndDateId"].Precision = 6;
                cmd.Parameters["EffectiveEndDateId"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return cmd;
        }

        public DiscreteItemReference GetCustomerItemReference(BusinessSystemSession e1Session, decimal addressNum, decimal itemNum, SqlDmlResponse sqlResponse = null)
        {
            var itemReference = new DiscreteItemReference();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetCustomerItemReference")) ? e1Session.DbCommands["CustomerManager.GetCustomerItemReference"] : GetCustomerItemReference();
                cmd.Parameters[0].Value = addressNum;
                cmd.Parameters[1].Value = itemNum;
                var dateId = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters[2].Value = dateId;
                cmd.Parameters[3].Value = dateId;
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        //itemReference.AddressNum = addressNum;
                        itemReference.CustomerItemNum = Data.CastDbNull.To(reader["IVCITM"], string.Empty).Trim();
                        itemReference.ReferenceType = Data.CastDbNull.To(reader["IVXRT"], string.Empty).Trim();
                        itemReference.Description1 = Data.CastDbNull.To(reader["IVDSC1"], string.Empty).Trim();
                        itemReference.Description2 = Data.CastDbNull.To(reader["IVDSC2"], string.Empty).Trim();
                    }

                    reader.Close();
                    sqlResponse.RowsAffected = 1;
                    sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                    sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                    sqlResponse.Name = "GetCustomerItemReference";
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return itemReference;
        }

        public iDB2Command GetCustomerAddress()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    @"SELECT
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

        public Address GetCustomerAddress(BusinessSystemSession e1Session, decimal addressNum, SqlDmlResponse sqlResponse = null)
        {
            var address = new Address();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetCustomerAddress")) ? e1Session.DbCommands["CustomerManager.GetCustomerAddress"] : GetCustomerAddress();
                cmd.Parameters[0].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        address.AddressNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        address.Name = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                        address.Address1 = Data.CastDbNull.To(reader["ALADD1"], string.Empty).Trim();
                        address.Address2 = Data.CastDbNull.To(reader["ALADD2"], string.Empty).Trim();
                        address.Address3 = Data.CastDbNull.To(reader["ALADD3"], string.Empty).Trim();
                        address.Address4 = Data.CastDbNull.To(reader["ALADD4"], string.Empty).Trim();
                        address.City = Data.CastDbNull.To(reader["ALCTY1"], string.Empty).Trim();
                        address.State = Data.CastDbNull.To(reader["ALADDS"], string.Empty).Trim();
                        address.PostalCode = Data.CastDbNull.To(reader["ALADDZ"], string.Empty).Trim();
                        address.Country = Data.CastDbNull.To(reader["ALCTR"], string.Empty).Trim();
                    }

                    reader.Close();
                    sqlResponse.RowsAffected = 1;
                    sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                    sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return address;
        }

        public iDB2Command GetParentCustomerNum()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT 
	                    MAPA8          
                    FROM 
	                    F0150
                    WHERE 
                        MAAN8 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AddressNum"].Precision = 8;
            cmd.Parameters["AddressNum"].Scale = 0;
            return cmd;
        }

        public decimal GetParentCustomerNum(BusinessSystemSession e1Session, decimal addressNum)
        {
            decimal returnNum = 0;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetParentCustomerNum")) ? e1Session.DbCommands["CustomerManager.GetParentCustomerNum"] : GetParentCustomerNum();
                cmd.Parameters[0].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        returnNum = Data.CastDbNull.To<decimal>(reader["MAPA8"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return returnNum;
        }

        private static iDB2Command GetCustomerByAddressNum()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT 
	                ABAN8,
                    ABALKY,
                    ABALPH          
                FROM 
	                F0101
                WHERE 
                    ABAT1 = 'F' AND
                    ABAN8 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
            return cmd;
        }

        public static string GetCustomerByAddressNum(BusinessSystemSession e1Session, decimal addressNum)
        {
            string custName = "";
            try
            {
                var cmd = GetCustomerByAddressNum();
                cmd.Parameters[0].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        custName = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return custName;
        }

        public iDB2Command GetPlantAddressNum()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT 
	                ABAN8,
                    ABALKY,
                    ABALPH          
                FROM 
	                F0101
                WHERE 
                    ABAT1 = 'F' AND
                    ABAC30 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 3));
            return cmd;
        }

        public decimal GetPlantAddressNum(BusinessSystemSession e1Session, string plantNum)
        {
            decimal returnNum = 0;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetPlantAddressNum")) ? e1Session.DbCommands["CustomerManager.GetPlantAddressNum"] : GetPlantAddressNum();
                cmd.Parameters[0].Value = plantNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        returnNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return returnNum;
        }

        public Plant GetPlant(BusinessSystemSession e1Session, string plantNum, bool includeAddress = false)
        {
            var plant = new Plant();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetPlantAddressNum")) ? e1Session.DbCommands["CustomerManager.GetPlantAddressNum"] : GetPlantAddressNum();
                cmd.Parameters[0].Value = plantNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        plant.PlantNum = plantNum;
                        plant.LegacyAddressNum = Data.CastDbNull.To(reader["ABALKY"], string.Empty).Trim();
                        plant.AddressNum = Data.CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        plant.Name = Data.CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return plant;
        }

        /******************************************************************
        * Get Terms
        ******************************************************************/
        public iDB2Command GetTerms()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                SELECT
                    AITRAR
                FROM
                    F03012
                WHERE
                    AIAN8 = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AddressNum"].Precision = 8;
            cmd.Parameters["AddressNum"].Scale = 0;
            return cmd;
        }

        public string GetTerms(BusinessSystemSession e1Session, decimal addressNum)
        {
            string terms = "";
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetTerms")) ? e1Session.DbCommands["CustomerManager.GetTerms"] : GetTerms();
                cmd.Parameters[0].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        terms = Data.CastDbNull.To(reader["AITRAR"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return terms;
        }

        /******************************************************************
        * Get Terms
        ******************************************************************/
        public iDB2Command GetTaxRate()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                SELECT
                    AITXA1
                FROM
                    F03012
                WHERE
                    AIAN8 = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AddressNum"].Precision = 8;
            cmd.Parameters["AddressNum"].Scale = 0;
            return cmd;
        }

        public string GetTaxRate(BusinessSystemSession e1Session, decimal addressNum)
        {
            string terms = "";
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CustomerManager.GetTaxRate")) ? e1Session.DbCommands["CustomerManager.GetTaxRate"] : GetTaxRate();
                cmd.Parameters[0].Value = addressNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        terms = Data.CastDbNull.To(reader["AITXA1"], string.Empty).Trim();
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return terms;
        }
    }
}
