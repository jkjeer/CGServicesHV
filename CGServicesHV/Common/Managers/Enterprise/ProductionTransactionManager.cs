using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class ProductionTransactionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void LoadCommands(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GetProductionTransactionRejectsByWorkOrder", GetProductionTransactionRejectsByWorkOrder()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public iDB2Command GetProductionTransactionRejectsByWorkOrder()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        SFUKID,
                        SFMMCU, 
                        SFMCU,
                        MCDL01,
                        SFDOCO,
                        SFDCTO,
                        SFEDCT,
                        SFTYTN, 
                        SFSTTM, 
                        SFPNID, 
                        SFLOTN, 
                        SFZ58SN, 
                        SFZ58SCN,
                        SFUORG, 
                        SFSOCN, 
                        SFZ58RID, 
                        SFZ58UID, 
                        SFZ58RCP, 
                        SFZ58RCC, 
                        SFZ58L1CLN 
                    FROM 
	                    F580030
                        INNER JOIN F0006
                        ON MCMCU = SFMCU
                    WHERE 
                        SFMMCU = ? AND
                        SFDOCO = ? AND
                        SFSOCN > 0"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            return cmd;
        }

        public ProductionTransactions GetProductionTransactionRejectsByWorkOrder(BusinessSystemSession e1Session, string plantNum, decimal orderNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var transactions = new ProductionTransactions();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetProductionTransactionRejectsByWorkOrder")) ? e1Session.DbCommands["GetProductionTransactionRejectsByWorkOrder"] : GetProductionTransactionRejectsByWorkOrder();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = orderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var tran = new ProductionTransaction
                        {
                            TransactionId = Data.CastDbNull.To<decimal>(reader["SFUKID"], 0),
                            TransactionDate = Utilities.Common.CastSttmToDateTime(Data.CastDbNull.To(reader["SFSTTM"], string.Empty).Trim()),
                            PlantNum = Data.CastDbNull.To(reader["SFMMCU"], string.Empty).Trim(),
                            WorkCenter = Data.CastDbNull.To(reader["SFMCU"], string.Empty).Trim(),
                            WorkCenterDesc = Data.CastDbNull.To(reader["MCDL01"], string.Empty).Trim(),
                            OrderNum = Data.CastDbNull.To<decimal>(reader["SFDOCO"], 0),
                            OrderType = Data.CastDbNull.To(reader["SFDCTO"], string.Empty).Trim(),
                            UnitId = Data.CastDbNull.To(reader["SFLOTN"], string.Empty).Trim(),
                            LiteNum = Data.CastDbNull.To<decimal>(reader["SFZ58L1CLN"], 0),
                            RackId = Data.CastDbNull.To<decimal>(reader["SFZ58RID"], 0),
                            DocumentType = Data.CastDbNull.To(reader["SFEDCT"], string.Empty).Trim(),
                            TransactionType = Data.CastDbNull.To(reader["SFTYTN"], string.Empty).Trim(),
                            RejectReason = Data.CastDbNull.To(reader["SFZ58RCC"], string.Empty).Trim(),
                            RejectQuantity = Data.CastDbNull.To<decimal>(reader["SFSOCN"], 0) / 1000
                        };
                        transactions.Add(tran);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + transactions.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return transactions;
        }
    }
}
