using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Process
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
                {"GlassItemDownloadManager.GetWeight", GetWeight()},
                {"GlassItemDownloadManager.GetCost", GetCost()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum, List<string> stockTypes)
        {
            var response = new ProcessResponse("Glass Item Download Manager");
            var table = new GlassItemTable();

            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "GlassItem") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {
                /********************************************************/
                var step2a = new ProcessStep("Get Events");
                try
                {
                    table = GetEvents(session, sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan, itemNum, stockTypes);
                    step2a.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.Done();
                    response.ProcessSteps.Add(step2a);
                }

                var step2c = new ProcessStep("Get Item Definition");
                try
                {
                    GetItemDefinition(session, plant, table);
                    step2c.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2c.Errors.Add(new Error(e));
                }
                finally
                {
                    step2c.Done();
                    response.ProcessSteps.Add(step2c);
                }

                var step2e = new ProcessStep("Get Cost");
                try
                {
                    GetCost(session, plant, table);
                    step2e.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2e.Errors.Add(new Error(e));
                }
                finally
                {
                    step2e.Done();
                    response.ProcessSteps.Add(step2e);
                }

                var step2f = new ProcessStep("Get Customer Part");
                try
                {
                    GetCustomerPart(session, plant, table);
                    step2f.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2f.Errors.Add(new Error(e));
                }
                finally
                {
                    step2f.Done();
                    response.ProcessSteps.Add(step2f);
                }

                var step2g = new ProcessStep("Get Primary Location");
                try
                {
                    GetPrimaryLocation(session, plant, table);
                    step2g.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2g.Errors.Add(new Error(e));
                }
                finally
                {
                    step2g.Done();
                    response.ProcessSteps.Add(step2g);
                }

                var step2h = new ProcessStep("Get Weight");
                try
                {
                    GetWeight(session, plant, table);
                    step2h.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2h.Errors.Add(new Error(e));
                }
                finally
                {
                    step2h.Done();
                    response.ProcessSteps.Add(step2h);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse =
                                BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                    table, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            if (table.MaxDate > processRunSpan.EndDateTime)
                            {
                                table.MaxDate = processRunSpan.EndDateTime;
                            }
                            step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"],
                                plant, highWaterMark, table.MaxDate.AddMinutes(-plant.BusinessSystemOffset)));
                        }
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Close();
            return response;
        }

        /******************************************************************
         * Events
         ******************************************************************/
        public iDB2Command GetEvents(List<string> filters)
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
            SELECT * FROM (
            SELECT 
	            IBMCU,
	            IBITM,
                IBUPMJ,
                IBTDAY,                
                IMUPMJ,
                IMTDAY,
                COALESCE(IEUPMJ, 0) AS IEUPMJ,
                COALESCE(IETDAY, 0) AS IETDAY,
                COALESCE(UMUPMJ, 0) AS UMUPMJ,
                COALESCE(UMTDAY, 0) AS UMTDAY,
                COALESCE(IVUPMJ, 0) AS IVUPMJ,
                COALESCE(IVTDAY, 0) AS IVTDAY,
                ROW_NUMBER() OVER (PARTITION BY IBMCU, IBITM ORDER BY IBUPMJ DESC, IBTDAY DESC) AS RN
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
                {0} IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA')) A
            WHERE A.RN = 1
            FOR READ ONLY", filterString);
            return cmd;
        }

        public GlassItemTable GetEvents(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, List<string> stockTypes)
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
                    filters.Add("IBMCU = ? AND ");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IBITM = ? AND ");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (stockTypes.Any())
                {
                    string strInClause = "";
                    foreach (string stockType in stockTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND ");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("IMUPMJ", "IMTDAY", span.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("IEUPMJ", "IETDAY", span.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("UMUPMJ", "UMTDAY", span.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("IVUPMJ", "IVTDAY", span.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND ");
                }

                cmd = GetEvents(filters);
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
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
                    dr["IMDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IMUPMJ"], 0), CastDbNull.To<decimal>(reader["IMTDAY"], 0));
                    var ibdate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["IBTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    var imdate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IMUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["IMTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    var iedate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IEUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["IETDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    var umdate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["UMUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["UMTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    var ivdate = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IVUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["IVTDAY"], 0)).AddMinutes(plant.BusinessSystemOffset);
                    var maxDate = new DateTime[] { ibdate, imdate, iedate, umdate, ivdate }.Max();
                    dr["TransactionDate"] = maxDate;
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                IBANPL,
                IBBUYR,
                IBSRP1,
                IBSRP2,
                IBSRP3,
                IBSRP4,
                IBSRP5,
                IBSRP6,
                IBSRP7,
                IBSRP8,
                IBSRP9,
                IBSRP0,
                IBPRP1,
                IBPRP2,
                IBPRP3,
                IBPRP4,
                IBPRP5,
                IBPRP6,
                IBPRP7,
                IBPRP8,
                IBPRP9,
                IBPRP0,
                IBROPI,
                IBROQI,
                IBRQMX,
                IBRQMN,
                IBSAFE, 
                IBUPMJ,
                IBTDAY,
	            AB1.ABALPH AS ANPLALPH,
	            AB2.ABALPH AS VENDALPH,
                IA1.IADVAL AS Z58USF,
                IA2.IADVAL AS Z58BSF   
            FROM 
	            F4102 
	            INNER JOIN F4101 
	            ON IMITM = IBITM
	            LEFT OUTER JOIN F0101 AB1
	            ON AB1.ABAN8 = IBANPL
	            LEFT OUTER JOIN F0101 AB2
	            ON AB2.ABAN8 = IBVEND
                LEFT OUTER JOIN F580002 IA1
                ON IA1.IAITM = IMITM and
                    IA1.IAZ58ATNM = 'Z58USF'
                LEFT OUTER JOIN F580002 IA2
                ON IA2.IAITM = IMITM and
                    IA2.IAZ58ATNM = 'Z58BSF'
            WHERE
                IBMCU = ? AND
                IBITM = ?
            FOR READ ONLY";
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
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    _logger.Debug(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["Plant"] = CastDbNull.To(reader["IBMCU"], String.Empty).Trim();
                        dr["UnitType"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
                        dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
                        dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
                        dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                        dr["GlCategory"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
                        dr["MasterPlanningCode"] = CastDbNull.To<string>(reader["IBPRP4"], String.Empty).Trim();
                        dr["CustomerNum"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
                        dr["CustomerName"] = CastDbNull.To<string>(reader["ANPLALPH"], String.Empty).Trim();
                        dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                        dr["SupplierNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
                        dr["SupplierName"] = CastDbNull.To<string>(reader["VENDALPH"], String.Empty).Trim();
                        dr["ActualSqft"] = Data.ParseStringToDecimal(CastDbNull.To<string>(reader["Z58USF"], String.Empty).Trim(), 4);
                        dr["BillableSqft"] = Data.ParseStringToDecimal(CastDbNull.To<string>(reader["Z58BSF"], String.Empty).Trim(), 4);
                        dr["ReorderInputPoint"] = CastDbNull.To<decimal>(reader["IBROPI"], 0) / 1000;
                        dr["ReorderInputQty"] = CastDbNull.To<decimal>(reader["IBROQI"], 0) / 1000;
                        dr["ReorderMinQty"] = CastDbNull.To<decimal>(reader["IBRQMN"], 0) / 1000;
                        dr["ReorderMaxQty"] = CastDbNull.To<decimal>(reader["IBRQMX"], 0) / 1000;
                        dr["SafetyStockSupply"] = CastDbNull.To<decimal>(reader["IBSAFE"], 0) / 1000;
                        dr["VendorNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
                        dr["PlannerNum"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
                        dr["BuyerNum"] = CastDbNull.To<decimal>(reader["IBBUYR"], 0);
                        dr["SRP1Code"] = CastDbNull.To<string>(reader["IBSRP1"], String.Empty).Trim();
                        dr["SRP2Code"] = CastDbNull.To<string>(reader["IBSRP2"], String.Empty).Trim();
                        dr["SRP3Code"] = CastDbNull.To<string>(reader["IBSRP3"], String.Empty).Trim();
                        dr["SRP4Code"] = CastDbNull.To<string>(reader["IBSRP4"], String.Empty).Trim();
                        dr["SRP5Code"] = CastDbNull.To<string>(reader["IBSRP5"], String.Empty).Trim();
                        dr["SRP6Code"] = CastDbNull.To<string>(reader["IBSRP6"], String.Empty).Trim();
                        dr["SRP7Code"] = CastDbNull.To<string>(reader["IBSRP7"], String.Empty).Trim();
                        dr["SRP8Code"] = CastDbNull.To<string>(reader["IBSRP8"], String.Empty).Trim();
                        dr["SRP9Code"] = CastDbNull.To<string>(reader["IBSRP9"], String.Empty).Trim();
                        dr["SRP0Code"] = CastDbNull.To<string>(reader["IBSRP0"], String.Empty).Trim();
                        dr["PRP1Code"] = CastDbNull.To<string>(reader["IBPRP1"], String.Empty).Trim();
                        dr["PRP2Code"] = CastDbNull.To<string>(reader["IBPRP2"], String.Empty).Trim();
                        dr["PRP3Code"] = CastDbNull.To<string>(reader["IBPRP3"], String.Empty).Trim();
                        dr["PRP4Code"] = CastDbNull.To<string>(reader["IBPRP4"], String.Empty).Trim();
                        dr["PRP5Code"] = CastDbNull.To<string>(reader["IBPRP5"], String.Empty).Trim();
                        dr["PRP6Code"] = CastDbNull.To<string>(reader["IBPRP6"], String.Empty).Trim();
                        dr["PRP7Code"] = CastDbNull.To<string>(reader["IBPRP7"], String.Empty).Trim();
                        dr["PRP8Code"] = CastDbNull.To<string>(reader["IBPRP8"], String.Empty).Trim();
                        dr["PRP9Code"] = CastDbNull.To<string>(reader["IBPRP9"], String.Empty).Trim();
                        dr["PRP0Code"] = CastDbNull.To<string>(reader["IBPRP0"], String.Empty).Trim();
                        dr["IBDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0), CastDbNull.To<decimal>(reader["IBTDAY"], 0));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
		        IVAN8 = ?
            FOR READ ONLY";
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
                    try
                    {
                        cmd.Parameters[0].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                        cmd.Parameters[1].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                        cmd.Parameters[2].Value = (decimal)dr["ItemNum"];
                        cmd.Parameters[3].Value = (decimal)dr["CustomerNum"];
                        IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                        while (reader.Read())
                        {
                            dr["CustomerPartNum"] = CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim();
                            dr["IVDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IVUPMJ"], 0), CastDbNull.To<decimal>(reader["IVTDAY"], 0));
                        }

                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                LIITM = ?
            FOR READ ONLY";
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
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["PrimaryLocation"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
                        dr["PrimaryLocationFull"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty);
                        dr["LIDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0), CastDbNull.To<decimal>(reader["LITDAY"], 0));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                SUM(IECSL) IS NOT NULL
            FOR READ ONLY";
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
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["CostPerEa"] = Math.Round(CastDbNull.To<decimal>(reader["IECSL"], 0), 4);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
		        UMRUM = 'LB'
            FOR READ ONLY";
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
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["LbsPerEa"] = Math.Round(CastDbNull.To<decimal>(reader["UMCONV"], 0), 7);
                        dr["UMDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["UMUPMJ"], 0), CastDbNull.To<decimal>(reader["UMTDAY"], 0));
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
