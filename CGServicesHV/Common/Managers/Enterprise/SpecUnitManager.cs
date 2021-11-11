using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CGServices.Common.Managers.E1;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServices.Common.Managers.Enterprise
{
    public class SpecUnitManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ItemAttributeManager _itemAttMgr;
        private readonly FlexItemManager _flexMgr;
        private readonly GlassUnitManager _glassUnitMgr;

        public SpecUnitManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpecUnitManager.GetRelatedOrders", GetRelatedOrders()},
                {"SpecUnitManager.GetPurchasedLot", GetPurchasedLot()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttMgr = new ItemAttributeManager(session);
            _flexMgr = new FlexItemManager(session, "P5842300");
            _glassUnitMgr = new GlassUnitManager(session);
        }

        /*********************************************************************************
         * Get Configs
         *******************************************************************************/
        public iDB2Command GetMtoUnits(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
                *
            FROM (
	            SELECT
                    CMCFGID,
                    CCITM,
                    CMUPMJ,
                    CMTDAY,
                    ROWNUMBER() OVER (PARTITION BY CMCFGID, CCITM ORDER BY CMUPMJ DESC, CMTDAY DESC) AS ROWNUM
	            FROM 
                    F3201
                    INNER JOIN F3215
                    ON  CMCFGID = CCCFGID AND
                        CCCFGCID = 1
	            WHERE 
                    {0}
                    CMDOCO > 0) A
            WHERE
                A.ROWNUM = 1", filterString.ToString());
            return cmd;
        }

        public iDB2Command GetMtoXrefs(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
	            SELECT
                    CMCFGID,
                    CCITM,
                    CMDOCO,
                    CMDCTO,
                    CMLNID,
                    CMUPMJ,
                    CMTDAY,
	            FROM 
                    F3201
                    INNER JOIN F3215
                    ON  CMCFGID = CCCFGID AND
                        CCCFGCID = 1
	            WHERE 
                    {0}
                    CMDOCO > 0", filterString.ToString());
            return cmd;
        }


        public SpecUnits GetMtoUnits(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal configId, List<string> docTypeList )
        {
            var ids = new SpecUnits();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    if (plant.PlantCode == "MZCG")
                    {
                        filters.Add("((CMEMCU = '          32') OR (CMEMCU = '          05') OR (CMEMCU = '          04')) AND \r\n");
                    }
                    else
                    {
                        filters.Add("CMEMCU = ? AND\r\n");
                        parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                        parameters.Last().Value = plant.PlantNum.PadLeft(12);
                    }
                }

                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("CMUPMJ", "CMTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }

                // Doc Types
                if (docTypeList.Any())
                {
                    string strInClause = "";
                    foreach (string docType in docTypeList)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
                        parameters.Last().Value = docType;
                    }

                    filters.Add("CMDCTO IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Config ID
                if (configId > 0)
                {
                    filters.Add("CMCFGID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = configId;
                }

                //// Order Num
                //if (orderNum > 0)
                //{
                //    filters.Add("CMDOCO = ? AND \r\n");
                //    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                //    parameters.Last().Precision = 8;
                //    parameters.Last().Scale = 0;
                //    parameters.Last().Value = orderNum;
                //}

                //// Order Type
                //if (!String.IsNullOrEmpty(orderType))
                //{
                //    filters.Add("CMDCTO = ? AND\r\n");
                //    parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
                //    parameters.Last().Value = orderType;
                //}

                //// Order Line Num
                //if (orderLineNum > 0)
                //{
                //    filters.Add("CMLNID = ? AND \r\n");
                //    parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
                //    parameters.Last().Precision = 6;
                //    parameters.Last().Scale = 0;
                //    parameters.Last().Value = orderLineNum;
                //}

                cmd = GetMtoUnits(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                _logger.Info(Logging.DbCmdString(cmd));

                _logger.Info("Start Get Ids");
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var id = new SpecUnit();
                        id.ConfigurationId = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                        id.ItemNum = Data.CastDbNull.To<decimal>(reader["CCITM"], 0);
                        id.TransactionDate = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["CMUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["CMTDAY"], 0));
                        ids.Add(id);
                    }

                    reader.Close();
                }

                _logger.Info("Start Item Attributes");
                foreach (SpecUnit id in ids)
                {
                    id.ItemAttributes = _itemAttMgr.GetConfiguredItemAttributes(session, id.ConfigurationId);
                }

                _logger.Info("Start Flex Descriptions");
                foreach (SpecUnit id in ids)
                {
                    id.FlexDescriptions = _flexMgr.GetDescription(session, id.ItemAttributes);
                }

                _logger.Info("Start Glass Items");
                foreach (SpecUnit id in ids)
                {
                    id.GlassItem = _glassUnitMgr.GetGlassUnit(session, id.ItemAttributes);
                }

                _logger.Info("Start Related Orders");
                foreach (SpecUnit id in ids)
                {
                    id.OrderIds = GetRelatedOrders(session, id.ConfigurationId);
                    //id.Orders = GetRelatedOrders(session, id.Id);
                }
                
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }

            return ids;
        }

        public iDB2Command GetMtsUnits(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT 
	            *
            FROM (
	            SELECT
		            IAITM,
		            IAUPMJ,
		            IATDAY,
		            ROW_NUMBER() OVER (PARTITION BY IAITM ORDER BY IAUPMJ DESC, IATDAY DESC) AS ROWNUM
	            FROM 
		            F4102
		            INNER JOIN F580002
		            ON IAITM = IBITM
	            WHERE 
                    {0}
                    IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ')) A
            WHERE
	            A.ROWNUM = 1", filterString.ToString());
            return cmd;
        }

        public SpecUnits GetMtsUnits(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var ids = new SpecUnits();
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

                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("IAUPMJ", "IATDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IAITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                cmd = GetMtsUnits(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Info(Logging.DbCmdString(cmd));

                _logger.Info("Start MTS Units");
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var id = new SpecUnit();
                        id.ConfigurationId = 0;
                        id.ItemNum = Data.CastDbNull.To<decimal>(reader["IAITM"], 0);
                        id.TransactionDate = E1TimeManager.CastToDateTime(Data.CastDbNull.To<decimal>(reader["IAUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["IATDAY"], 0));
                        ids.Add(id);
                    }

                    reader.Close();
                }

                _logger.Info("Start Item Attributes");
                foreach (SpecUnit id in ids)
                {
                    id.ItemAttributes = _itemAttMgr.GetDiscreteItemAttributes(session, id.ItemNum);
                }
                _logger.Info("Start Flex Descriptions");
                foreach (SpecUnit id in ids)
                {
                    id.FlexDescriptions = _flexMgr.GetDescription(session, id.ItemAttributes);
                }
                _logger.Info("Start Glass Items");
                foreach (SpecUnit id in ids)
                {
                    id.GlassItem = _glassUnitMgr.GetGlassUnit(session, id.ItemAttributes);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return ids;
        }


        /*********************************************************************************
         * Related Orders
         *******************************************************************************/
        public iDB2Command GetRelatedOrders()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
	        SELECT 
                CMEMCU,
                CMKCOO,
                CMDOCO,
                CMDCTO,
                CMLNID
	        FROM 
                F3201
	        WHERE
                CMDOCO > 0 AND 
                CMCFGID = ?";
            cmd.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID"].Precision = 15;
            cmd.Parameters["CFGID"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetPurchasedLot()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
	        SELECT 
                PDLOTN
	        FROM 
                F4311
	        WHERE
                PDDOCO = ? AND 
                PDDCTO = ? AND
                PDLNID = ?";
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNID"].Precision = 6;
            cmd.Parameters["LNID"].Scale = 0;
            return cmd;
        }

        public List<string> GetRelatedOrdersOLD(BusinessSystemSession session, decimal id)
        {
            var orders = new List<string>();
            try
            {
                var cmd = session.DbCommands["SpecUnitManager.GetRelatedOrders"];
                cmd.Parameters[0].Value = id;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var plantNum = CastDbNull.To(reader["CMEMCU"], string.Empty).Trim();
                        var docNum = CastDbNull.To<decimal>(reader["CMDOCO"], 0);
                        var docType = CastDbNull.To(reader["CMDCTO"], string.Empty).Trim();
                        var docLineNum = CastDbNull.To<decimal>(reader["CMLNID"], 0);
                        orders.Add("[" + plantNum + "] " + docNum + "-" + docType + "-" + docLineNum);
                        // LOTS
                        if (docType == "WO")
                        {
                            orders.Add("[" + plantNum + "] " + docNum + "-" + "LOT");
                        }

                        if (docType == "OT" || docType == "OG")
                        {
                            var cmd1 = session.DbCommands["SpecUnitManager.GetPurchasedLot"];
                            cmd1.Parameters[0].Value = docNum;
                            cmd1.Parameters[1].Value = docType;
                            cmd1.Parameters[2].Value = docLineNum;
                            cmd1.Connection = session.DbConnection;
                            using (IDataReader reader1 = cmd1.ExecuteReader(CommandBehavior.SingleRow))
                            {
                                while (reader1.Read())
                                {
                                    var lotNum = CastDbNull.To(reader1["PDLOTN"], string.Empty).Trim();
                                    orders.Add("[" + plantNum + "] " + lotNum + "-" + "LOT");
                                }

                                reader1.Close();
                            }
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return orders;
        }


        public OrderIds GetRelatedOrders(BusinessSystemSession session, decimal id)
        {
            var orderIds = new OrderIds();
            try
            {
                var cmd = session.DbCommands["SpecUnitManager.GetRelatedOrders"];
                cmd.Parameters[0].Value = id;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var orderId = new OrderId();
                        orderId.DocumentNum = CastDbNull.To<decimal>(reader["CMDOCO"], 0);
                        orderId.DocumentType = CastDbNull.To<string>(reader["CMDCTO"], string.Empty).Trim();
                        orderId.DocumentLineNum = CastDbNull.To<decimal>(reader["CMLNID"], 0);
                        orderId.Plant = CastDbNull.To<string>(reader["CMEMCU"], string.Empty).Trim();
                        orderId.Company = CastDbNull.To<string>(reader["CMKCOO"], string.Empty).Trim();
                        orderId.Description = orderId.Plant + "-" + orderId.DocumentNum + "-" + orderId.DocumentType + "-" + orderId.DocumentLineNum;

                        // LOT
                        orderId.LotNum = "";
                        if (orderId.DocumentType == "WO")
                        {
                            orderId.LotNum = CastDbNull.To<decimal>(reader["CMDOCO"], 0).ToString();
                        }

                        if (orderId.DocumentType == "OT" || orderId.DocumentType == "OG")
                        {
                            var cmd1 = session.DbCommands["SpecUnitManager.GetPurchasedLot"];
                            cmd1.Parameters[0].Value = orderId.DocumentNum;
                            cmd1.Parameters[1].Value = orderId.DocumentType;
                            cmd1.Parameters[2].Value = orderId.DocumentLineNum;
                            cmd1.Connection = session.DbConnection;
                            using (IDataReader reader1 = cmd1.ExecuteReader(CommandBehavior.SingleRow))
                            {
                                while (reader1.Read())
                                {
                                    orderId.LotNum = CastDbNull.To<string>(reader1["PDLOTN"], String.Empty).Trim();
                                }

                                reader1.Close();
                            }
                        }

                        orderIds.Add(orderId);

                        //// LOT
                        //if (orderId.DocumentType == "WO")
                        //{
                        //    var lot = new OrderId();
                        //    lot.Plant = orderId.Plant;
                        //    lot.LotNum = CastDbNull.To<decimal>(reader["CMDOCO"], 0).ToString();
                        //    lot.DocumentType = "LOT";
                        //    lot.Description = orderId.Plant + "-" + lot.LotNum + "-" + lot.DocumentType;
                        //    orderIds.Add(lot);
                        //}
                        //if (orderId.DocumentType == "OT" || orderId.DocumentType == "OG")
                        //{
                        //    var cmd1 = session.DbCommands["SpecUnitManager.GetPurchasedLot"];
                        //    cmd1.Parameters[0].Value = orderId.DocumentNum;
                        //    cmd1.Parameters[1].Value = orderId.DocumentType;
                        //    cmd1.Parameters[2].Value = orderId.DocumentLineNum;
                        //    cmd1.Connection = session.DbConnection;
                        //    using (IDataReader reader1 = cmd1.ExecuteReader(CommandBehavior.SingleRow);
                        //    while (reader1.Read())
                        //    {
                        //        var lot = new OrderId();
                        //        lot.Plant = orderId.Plant;
                        //        lot.LotNum = CastDbNull.To<string>(reader1["PDLOTN"], String.Empty).Trim();
                        //        lot.DocumentType = "LOT";
                        //        lot.Description = orderId.Plant + "-" + lot.LotNum + "-" + lot.DocumentType;
                        //        orderIds.Add(lot);
                        //    }
                        //    reader1.Close();
                        //}
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return orderIds;
        }

    }
}
