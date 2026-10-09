using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class CrateManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CrateManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"CrateManager.GetCrate", GetCrate()},
                {"CrateManager.GetCrates", GetCrates()},
                {"CrateManager.GetCratesDesc", GetCratesDesc()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /******************************************************************
        * Commands
        ******************************************************************/
        public iDB2Command GetCrate()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG                          
                    FROM 
	                    F580220
                    WHERE
                        CRDOCO > 0 AND
                        CRDOCO = ? AND
                        CRDCTO = ? AND
                        CRKCOO = ? AND
                        CRLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetCrates()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG                                  
                    FROM 
	                    F580220
                    WHERE 
                        CRDOCO = ? AND
                        CRDCTO = ? AND
                        CRKCOO = ? AND
                        CRLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetCratesDesc()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG,
                        DRDL01                              
                    FROM 
                     F580220
                    INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                    ON DRSY = '58' AND
                        DRRT = 'CT' AND
                        TRIM(DRKY) = CRZ58CRTY
                    WHERE
                        CRDOCO > 0 AND
                        CRDOCO = ? AND
                        CRDCTO = ? AND
                        CRKCOO = ? AND
                        CRLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetCratesDescDynamic(List<string> filters)
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
                    SELECT 
                        CRDOCO,
                        CRDCTO,
                        CRKCOO,
                        CRLNID,
                        CRUORG,
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG,
                        DRDL01
                    FROM 
	                    F580220
                    INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                    ON DRSY = '58' AND
                        DRRT = 'CT' AND
                        TRIM(DRKY) = CRZ58CRTY
                    {filterString}"
            };
            return cmd;
        }

        public Crate GetCrate(BusinessSystemSession e1Session, decimal orderNum, string orderType, string companyNum, decimal orderLineNum)
        {
            var crate = new Crate();
            try
            {
                //var cmd = (e1Session.DbCommands.ContainsKey("CrateManager.GetCrate")) ? e1Session.DbCommands["CrateManager.GetCrate"] : GetCrate();
                var cmd = e1Session.DbCommands["CrateManager.GetCrate"];
                cmd.Parameters["OrderNum"].Value = orderNum;
                cmd.Parameters["OrderType"].Value = orderType;
                cmd.Parameters["CompanyNum"].Value = companyNum;
                cmd.Parameters["OrderLineNum"].Value = orderLineNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        crate = new Crate
                        {
                            OrderNum = Data.CastDbNull.To<decimal>(reader["CRDOCO"], 0),
                            OrderType = Data.CastDbNull.To(reader["CRDCTO"], string.Empty).Trim(),
                            OrderCompany = Data.CastDbNull.To(reader["CRKCOO"], string.Empty).Trim(),
                            OrderLineNum = Data.CastDbNull.To<decimal>(reader["CRLNID"], 0),
                            OrderQuantity = Data.CastDbNull.To<decimal>(reader["CRUORG"], 0),
                            CrateId = Data.CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
                            CrateType = Data.CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim(),
                            Quantity = Data.CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0),
                            Message = Data.CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim(),
                            Description = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim()
                        };
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return crate;
        }

        //public F580220s GetCratesDescDynamic(BusinessSystemSession e1Session, MTOScheduleLines mtoScheduleLines)
        //{
        //    var f580220s = new F580220s();
        //    var filters = new List<string>();
        //    try
        //    {
        //        var inString = new StringBuilder();
        //        foreach (var line in mtoScheduleLines)
        //        {
        //            inString.Append(line.WorkOrder.RelatedOrderNum + ",");
        //        }
        //        inString.Length -= 1;
        //        // Related Orders
        //        filters.Add("CRDOCO IN (" + inString + ") \r\n");
        //        var cmd = GetCratesDescDynamic(filters);
        //        cmd.Connection = e1Session.DbConnection;
        //        Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
        //        Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
        //        Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
        //        using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
        //        while (reader.Read())
        //        {
        //            var f580220 = new F580220
        //            {
        //                CRDOCO = Data.CastDbNull.To<decimal>(reader["CRDOCO"], 0),
        //                CRDCTO = Data.CastDbNull.To(reader["CRDCTO"], string.Empty).Trim(),
        //                CRKCOO = Data.CastDbNull.To(reader["CRKCOO"], string.Empty).Trim(),
        //                CRLNID = Data.CastDbNull.To<decimal>(reader["CRLNID"], 0),
        //                CRUORG = Data.CastDbNull.To<decimal>(reader["CRUORG"], 0),
        //                CRZ58CRID = Data.CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
        //                CRZ58CRTY = Data.CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim(),
        //                CRZ58CRQ = Data.CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0),
        //                CRZ58CRMSG = Data.CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim(),
        //                DRDL01 = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim()
        //            };
        //            f580220s.Add(f580220);
        //        }
        //        reader.Close();
        //        Logger.Trace("ROWS: " + f580220s.Count);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
        //        throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
        //    }
        //    return f580220s;
        //}
    }
}
