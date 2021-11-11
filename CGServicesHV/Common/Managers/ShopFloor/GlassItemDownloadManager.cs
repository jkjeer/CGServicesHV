using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CGServices.Common.Managers.E1;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Types.ShopFloor;
using CGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServices.Common.Managers.ShopFloor
{
    public class GlassItemDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GlassItemDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GlassItemDownloadManager.GetItemDefinition", GetItemDefinition()},
                {"GlassItemDownloadManager.GetCustomerPart", GetCustomerPart()},
                {"GlassItemDownloadManager.GetPrimaryLocation", GetPrimaryLocation()},
                {"GlassItemDownloadManager.GetQuantity", GetQuantity()},
                {"GlassItemDownloadManager.GetWeight", GetWeight()},
                {"GlassItemDownloadManager.GetCost", GetCost()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, string stockTypes)
        {
            var response = new ProcessResponse("Glass Item Download Manager");
            var table = new GlassItemTable();

            var step1 = new ProcessStep("Get Definition Keys");
            try
            {
                table = GetDefinitionKeys(session, cn, plant, span, itemNum, stockTypes);
                step1.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.ProcessSteps.Add(step1);
            }

            if (!span.IsEmpty)
            {
                var step2 = new ProcessStep("Get Balance Keys");
                try
                {
                    var table1 = GetBalanceKeys(session, cn, plant, span, itemNum, stockTypes);
                    step2.RowsAffected = table1.Rows.Count;
                    foreach (DataRow dr1 in table1.Rows)
                    {
                        DataRow dr = table.NewRow();
                        dr["ITEM_NUM"] = dr1["ITEM_NUM"];
                        table.Rows.Add(dr);
                    }
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                finally
                {
                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }
            }

            var step3 = new ProcessStep("Get Item Definition");
            try
            {
                GetItemDefinition(session, plant, table);
                step3.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step3.Errors.Add(new Error(e));
            }
            finally
            {
                step3.Done();
                response.ProcessSteps.Add(step3);
            }

            var step4 = new ProcessStep("Get Quantity");
            try
            {
                GetQuantity(session, plant, table);
                step4.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step4.Errors.Add(new Error(e));
            }
            finally
            {
                step4.Done();
                response.ProcessSteps.Add(step4);
            }

            var step5 = new ProcessStep("Get Cost");
            try
            {
                GetCost(session, plant, table);
                step5.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step5.Errors.Add(new Error(e));
            }
            finally
            {
                step5.Done();
                response.ProcessSteps.Add(step5);
            }

            var step6 = new ProcessStep("Get Customer Part");
            try
            {
                GetCustomerPart(session, plant, table);
                step6.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step6.Errors.Add(new Error(e));
            }
            finally
            {
                step6.Done();
                response.ProcessSteps.Add(step6);
            }

            var step7 = new ProcessStep("Get Primary Location");
            try
            {
                GetPrimaryLocation(session, plant, table);
                step7.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step7.Errors.Add(new Error(e));
            }
            finally
            {
                step7.Done();
                response.ProcessSteps.Add(step7);
            }

            var step8 = new ProcessStep("Get Weight");
            try
            {
                GetWeight(session, plant, table);
                step8.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step8.Errors.Add(new Error(e));
            }
            finally
            {
                step8.Done();
                response.ProcessSteps.Add(step8);
            }

            if (table.Rows.Count > 0)
            {
                var step9 = new ProcessStep("Bulk Copy");
                try
                {
                    var woBulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                    step9.SqlBulkCopyResponses.Add(woBulkResponse);
                    step9.RowsAffected = (int)woBulkResponse.RowsCopied;
                }
                catch (Exception e)
                {
                    step9.Errors.Add(new Error(e));
                }
                finally
                {
                    step9.Done();
                    response.ProcessSteps.Add(step9);
                }

                if (span.HighWaterMark != null)
                {
                    var step10 = new ProcessStep("Set High Water Mark");
                    try
                    {
                        span.HighWaterMark.EnterpriseDateTime = table.MaxDate;
                        step10.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, span.HighWaterMark));
                        step10.RowsAffected = step10.SqlDmlResponses[0].RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step10.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step10.Done();
                        response.ProcessSteps.Add(step10);
                    }
                }

            }
            return response;
        }

        /******************************************************************
         * Definition Keys
         ******************************************************************/
        public iDB2Command GetDefinitionKeys(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT 
	            IBMCU,
	            IBITM
            FROM 
                F4102
	            INNER JOIN F4101 
	            ON	IMITM = IBITM
	            LEFT OUTER JOIN F30026
	            ON	IEITM = IBITM AND
		            IEMMCU = IBMCU
	            LEFT OUTER JOIN F41002
	            ON	UMITM = IBITM AND
                    UMMCU = IBMCU
	            LEFT OUTER JOIN F4104
	            ON	IVXRT = 'C' AND
                    IVITM = IBITM AND
                    IVAN8 = IBANPL
            WHERE 
                {0} IBGLPT LIKE 'GL%'
            GROUP BY
                IBMCU,
                IBITM", filterString);
            return cmd;
        }

        public GlassItemTable GetDefinitionKeys(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, string stockTypes)
        {
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("IBMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IBITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (!String.IsNullOrEmpty(stockTypes))
                {
                    var stockTypesList = stockTypes.Split(',').ToList();
                    string strInClause = "";
                    foreach (string stockType in stockTypesList)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("IMUPMJ", "IMTDAY", span.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("IEUPMJ", "IETDAY", span.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("UMUPMJ", "UMTDAY", span.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("IVUPMJ", "IVTDAY", span.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                }

                cmd = GetDefinitionKeys(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["ITEM_NUM"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return table;
        }

        /******************************************************************
          * Balance Keys
          ******************************************************************/
        public iDB2Command GetBalanceKeys(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT 
	            LIMCU,
	            LIITM
            FROM 
                F41021
	            INNER JOIN F4102
	            ON	LIMCU = IBMCU AND
                    LIITM = IBITM
            WHERE 
                {0} LIGLPT LIKE 'GL%'
            GROUP BY
                LIMCU,
                LIITM", filterString);
            return cmd;
        }

        public GlassItemTable GetBalanceKeys(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, string stockTypes)
        {
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("LIMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("LIITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (!String.IsNullOrEmpty(stockTypes))
                {
                    var stockTypesList = stockTypes.Split(',').ToList();
                    string strInClause = "";
                    foreach (string stockType in stockTypesList)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("LIUPMJ", "LITDAY", span.E1TimeIdSpan);
                    filters.Add(queryString1 + " AND \r\n");
                }

                cmd = GetBalanceKeys(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["ITEM_NUM"] = CastDbNull.To<decimal>(reader["LIITM"], 0);
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return table;
        }

        /******************************************************************
         * Item Definition
         ******************************************************************/
        public static iDB2Command GetItemDefinition()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT 
	            IBMCU,
	            IBITM,
	            IMSRP4,
	            IMDSC1,
	            IMDSC2,
	            IMUOM1,
	            IBSTKT,
	            IBGLPT,
	            IBVEND,
	            AB2.ABALPH AS VENDALPH,
	            IBANPL,
	            AB1.ABALPH AS ANPLALPH,
	            IMUPMJ,
	            IMTDAY,
	            IBUPMJ,
	            IBTDAY
            FROM 
	            F4102 
	            INNER JOIN F4101 
	            ON IMITM = IBITM
	            LEFT OUTER JOIN F0101 AB1
	            ON AB1.ABAN8 = IBANPL
	            LEFT OUTER JOIN F0101 AB2
	            ON AB2.ABAN8 = IBVEND
            WHERE
                IBMCU = ? AND
                IBITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetItemDefinition(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetItemDefinition"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["ITEM_NUM"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["UNIT_TYPE_CD"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
                            dr["DESCRIPTION1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
                            dr["DESCRIPTION2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
                            dr["STOCK_TYPE_CD"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                            dr["GL_CAT_CD"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
                            dr["CUSTOMER_NUM"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
                            dr["CUSTOMER_NAME"] = CastDbNull.To<string>(reader["ANPLALPH"], String.Empty).Trim();
                            dr["PRIMARY_UOM_CD"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                            dr["SUPPLIER_NUM"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
                            dr["SUPPLIER_NAME"] = CastDbNull.To<string>(reader["VENDALPH"], String.Empty).Trim();

                            var imTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IMUPMJ"], 0), CastDbNull.To<decimal>(reader["IMTDAY"], 0));
                            dr["UPDATE_DATE"] = imTime;

                            var ibTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0));
                            if (ibTime > imTime)
                            {
                                dr["UPDATE_DATE"] = ibTime;
                            }

                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * Customer Part
         ******************************************************************/
        public static iDB2Command GetCustomerPart()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT 
                IVCITM,
                IVUPMJ,
                IVTDAY
            FROM 
                F4104
	        WHERE  
		        IVXRT = 'C' AND
		        IVEXDJ >= ? AND
		        IVEFTJ <= ? AND
                IVITM = ? AND
		        IVAN8 = ?";
            cmd.Parameters.Add(new iDB2Parameter("EXDJ", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EXDJ"].Precision = 6;
            cmd.Parameters["EXDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("EFTJ", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EFTJ"].Precision = 6;
            cmd.Parameters["EFTJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AN8"].Precision = 8;
            cmd.Parameters["AN8"].Scale = 0;
            return cmd;
        }

        public void GetCustomerPart(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetCustomerPart"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                    cmd.Parameters[1].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                    cmd.Parameters[2].Value = (decimal) dr["ITEM_NUM"];
                    cmd.Parameters[3].Value = (decimal) dr["CUSTOMER_NUM"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["CUSTOMER_PART_NUM"] = CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim();
                            var ivTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IVUPMJ"], 0), CastDbNull.To<decimal>(reader["IVTDAY"], 0));
                            if (ivTime > (DateTime) dr["UPDATE_DATE"])
                            {
                                dr["UPDATE_DATE"] = ivTime;
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * Primary Location
         ******************************************************************/
        public static iDB2Command GetPrimaryLocation()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                LILOCN, 
                LIUPMJ,
                LITDAY
            FROM
                F41021
            WHERE
                LIPBIN = 'P' AND
                LIMCU = ? AND
                LIITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetPrimaryLocation(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetPrimaryLocation"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["ITEM_NUM"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["PRIMARY_LOCATION_CD"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
                            var liTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0), CastDbNull.To<decimal>(reader["LITDAY"], 0));
                            if (liTime > (DateTime) dr["UPDATE_DATE"])
                            {
                                dr["UPDATE_DATE"] = liTime;
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * Quantity
         ******************************************************************/
        public static iDB2Command GetQuantity()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                SUM(LIPQOH / 1000) AS LIPQOH,
                MAX(LIUPMJ*1000000+LITDAY) AS LIUPMJTDAY
            FROM
                F41021
            WHERE
                LIPQOH != 0 AND
                LIMCU = ? AND
                LIITM = ?
			HAVING 
                SUM(LIPQOH) IS NOT NULL";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetQuantity(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetQuantity"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["ITEM_NUM"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        dr["QTY_ON_HAND"] = 0;
                        while (reader.Read())
                        {
                            dr["QTY_ON_HAND"] = CastDbNull.To<decimal>(reader["LIPQOH"], 0);
                            var ts = CastDbNull.To<decimal>(reader["LIUPMJTDAY"], 0);
                            var liTime = E1TimeManager.CastToDateTime(Convert.ToDecimal(ts.ToString().Substring(0, 6)), Convert.ToDecimal(ts.ToString().Substring(6, 6)));
                            if (liTime > (DateTime) dr["UPDATE_DATE"])
                            {
                                dr["UPDATE_DATE"] = liTime;
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * Cost
         ******************************************************************/
        public static iDB2Command GetCost()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                SUM(IECSL / 10000) AS IECSL,
                MAX(IEUPMJ*1000000+IETDAY) AS IEUPMJTDAY
            FROM
                F30026
            WHERE
                IELEDG = '07' AND
                IEMMCU = ? AND
                IEITM = ?
            HAVING
                SUM(IECSL) IS NOT NULL";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetCost(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetCost"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["ITEM_NUM"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["COST_PER_EA"] = CastDbNull.To<decimal>(reader["IECSL"], 0);
                            var ts = CastDbNull.To<decimal>(reader["IEUPMJTDAY"], 0);
                            var ieTime = E1TimeManager.CastToDateTime(Convert.ToDecimal(ts.ToString().Substring(0, 6)), Convert.ToDecimal(ts.ToString().Substring(6, 6)));
                            if (ieTime > (DateTime) dr["UPDATE_DATE"])
                            {
                                dr["UPDATE_DATE"] = ieTime;
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * Cost
         ******************************************************************/
        public static iDB2Command GetWeight()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                UMCONV / 10000000 AS UMCONV,
                UMUPMJ,
                UMTDAY
            FROM
                F41002
            WHERE
		        UMMCU = ? AND
                UMITM = ? AND
		        UMUM = 'EA' AND
		        UMRUM = 'LB'";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetWeight(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetWeight"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal) dr["ITEM_NUM"];
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                        {
                            dr["LBS_PER_EA"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                            var umTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["UMUPMJ"], 0), CastDbNull.To<decimal>(reader["UMTDAY"], 0));
                            if (umTime > (DateTime) dr["UPDATE_DATE"])
                            {
                                dr["UPDATE_DATE"] = umTime;
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }
}
