// Decompiled with JetBrains decompiler
// Type: CGServicesHV.Common.Managers.Process.SalesLineDownloadManager
// Assembly: CGServicesHV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 85F30AF5-AC07-435F-A3C1-52D64835746D
// Assembly location: \\CGD1intsvcpd\c$\inetpub\wwwroot\CGServicesHVE1\bin\CGServicesHV.dll

using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Managers.E1;
using CGServicesHV.Common.Managers.Enterprise;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Types.ShopFloor;
using CGServicesHV.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CGServicesHV.Common.Managers.Process
{
    public class SalesLineDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(
          BusinessSystemSession session,
          Dictionary<string, SqlConnection> sqlCns,
          Plant plant,
          TimeSpan timeSpan,
          bool useHighWaterMark,
          Decimal salesOrderNum,
          List<string> nextStatusCodes)
        {
            ProcessResponse processResponse = new ProcessResponse("Sales Line Download", false, (string)null);
            processResponse.Plant = plant;
            processResponse.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (KeyValuePair<string, SqlConnection> sqlCn in sqlCns)
                processResponse.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            SalesLineTable salesLineTable = new SalesLineTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark mark = new HighWaterMark();
            ProcessStep step1 = new ProcessStep("Resolve Run Span");
            try
            {
                mark = useHighWaterMark ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "SalesLine") : (HighWaterMark)null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, timeSpan, E1TimeManager.GetE1TimeStamp(session, (string)null));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception ex)
            {
                step1.Errors.Add(new CGServicesHV.Common.Types.Response.Error(ex, true));
            }
            finally
            {
                step1.Done();
                processResponse.AddStep(step1);
            }
            if (!processResponse.HasErrors())
            {
                ProcessStep step2 = new ProcessStep("Get Source Data");
                try
                {
                    IDataReader salesLine = SalesLineDownloadManager.GetSalesLine(session, processRunSpan, plant.PlantNum, salesOrderNum, nextStatusCodes);
                    salesLineTable.Add(salesLine, plant);
                    step2.RowsAffected = salesLineTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    step2.Errors.Add(new CGServicesHV.Common.Types.Response.Error(ex, true));
                }
                finally
                {
                    step2.Done();
                    processResponse.AddStep(step2);
                }
            }
            if (!processResponse.HasErrors() && salesLineTable.Rows.Count > 0)
            {
                ProcessStep step2 = new ProcessStep("Bulk Copy");
                SqlBulkCopyResponse bulkCopyResponse = new SqlBulkCopyResponse();
                try
                {
                    if (salesLineTable.Rows.Count > 0)
                        bulkCopyResponse = BulkCopyManager.BulkTable(salesLineTable.ExtendedProperties[(object)"Target"].ToString(), (DataTable)salesLineTable, sqlCns["PLANT.INTEGRATION"], (SqlTransaction)null);
                }
                catch (Exception ex)
                {
                    step2.Errors.Add(new CGServicesHV.Common.Types.Response.Error(ex, true));
                }
                finally
                {
                    step2.RowsAffected = (int)bulkCopyResponse.RowsCopied;
                    step2.SqlBulkCopyResponses.Add(bulkCopyResponse);
                    step2.Done();
                    processResponse.RowsAffected += step2.RowsAffected;
                    processResponse.AddStep(step2);
                }
            }
            if (!processResponse.HasErrors() & useHighWaterMark)
            {
                ProcessStep step2 = new ProcessStep("Update High Water Mark");
                try
                {
                    if (salesLineTable.Rows.Count > 0)
                        step2.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"], plant, mark, salesLineTable.MaxDate.AddHours((double)-plant.TimeOffset)));
                }
                catch (Exception ex)
                {
                    step2.Errors.Add(new CGServicesHV.Common.Types.Response.Error(ex, true));
                }
                finally
                {
                    step2.Done();
                    processResponse.AddStep(step2);
                }
            }
            processResponse.Close(false);
            return processResponse;
        }

        public static IDataReader GetSalesLine(
          BusinessSystemSession session,
          ProcessRunSpan processRunSpan,
          string plantNum,
          Decimal salesOrderNum,
          List<string> nextStatusCodes)
        {
            List<string> filters = new List<string>();
            List<iDB2Parameter> source = new List<iDB2Parameter>();
            try
            {
                filters.Add("SDMCU = ? AND \r\n");
                source.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                source.Last<iDB2Parameter>().Value = (object)plantNum.PadLeft(12);
                if (!processRunSpan.IsEmpty)
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("SDUPMJ", "SDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                if (salesOrderNum > Decimal.Zero)
                {
                    filters.Add("SDDOCO = ? AND \r\n");
                    source.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    source.Last<iDB2Parameter>().Precision = (byte)8;
                    source.Last<iDB2Parameter>().Scale = (byte)0;
                    source.Last<iDB2Parameter>().Value = (object)salesOrderNum;
                }
                if (nextStatusCodes.Any<string>())
                {
                    string str = "";
                    foreach (string nextStatusCode in nextStatusCodes)
                    {
                        str += "?,";
                        source.Add(new iDB2Parameter("NXTR", iDB2DbType.iDB2Graphic, 3));
                        source.Last<iDB2Parameter>().Value = (object)nextStatusCode;
                    }
                    filters.Add("SDNXTR IN (" + str.Substring(0, str.Length - 1) + ") AND \r\n");
                }
                iDB2Command salesLine = SalesLineDownloadManager.GetSalesLine(filters);
                for (int index = 0; index < 3; ++index)
                {
                    foreach (iDB2Parameter iDb2Parameter in source)
                        salesLine.Parameters.Add(iDb2Parameter);
                }
                salesLine.Connection = session.DbConnection;
                SalesLineDownloadManager.Logger.Debug(Logging.DbCmdString(salesLine));
                return (IDataReader)salesLine.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static iDB2Command GetSalesLine(List<string> filters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string filter in filters)
                stringBuilder.Append(filter);
            iDB2Command iDb2Command = new iDB2Command();
            iDb2Command.CommandType = CommandType.Text;
            iDb2Command.CommandText = $@"
                SELECT
                    SDKCOO    ,
                    SDDOCO    ,
                    SDDCTO    ,
                    SDLNID    ,
                    SDRORN    ,
                    SDRCTO    ,
                    SDRLLN    ,
                    SDTRDJ    ,
                    SDDRQJ    ,
                    SDOPDJ    ,
                    SDCNDJ    ,
                    SDADDJ    ,
                    SDIVD     ,
                    SDITM     ,
                    SDLNTY    ,
                    SDNXTR    ,
                    SDLTTR    ,
                    SDAN8     ,
                    SDSHAN    ,
                    SDUORG    ,
                    SDUNCS    ,
                    SDECST    ,
                    SDUPRC    ,
                    SDAEXP    ,
                    SDDOC     ,
                    SDGLC     ,
                    SDRCD     ,
                    SDURCD    ,
                    SDVR01    ,
                    SDPSN     ,
                    SDUPMJ    ,
                    SDTDAY    ,
                    SDUSER    ,
                    SDPID     ,
                    SDDSC1    ,
                    SDDSC2    ,
                    SDPROV    ,
                    ATZ58PT   ,
                    ATZ58CSQNO,
                    ATZ58GSQNO,
                    ATZ58CP2  ,
                    ATZ58CP3  ,
                    ATZ58CP4  ,
                    ATZ5840460,
                    ATZ58USF  ,
                    ATZ58BSF  ,
                    ATZ58ZPO  ,
                    ATUPMJ    ,
                    ATTDAY    ,
                    IBSTKT
                FROM
                        F4211
                LEFT OUTER JOIN
                        F580032
                ON
                        ATKCOO = SDKCOO
                AND ATDOCO = SDDOCO
                AND ATDCTO = SDDCTO
                AND ATLNID = SDLNID
                LEFT OUTER JOIN
                        F4102
                ON
                        IBMCU = SDMCU
                AND IBITM = SDITM
                WHERE
                        {stringBuilder}
                            1 = 1
                UNION ALL
                SELECT
                    SDKCOO,
                    SDDOCO,
                    SDDCTO,
                    SDLNID,
                    SDRORN,
                    SDRCTO,
                    SDRLLN,
                    SDTRDJ,
                    SDDRQJ,
                    SDOPDJ,
                    SDCNDJ,
                    SDADDJ,
                    SDIVD,
                    SDITM,
                    SDLNTY,
                    SDNXTR,
                    SDLTTR,
                    SDAN8,
                    SDSHAN,
                    SDUORG,
                    SDUNCS,
                    SDECST,
                    SDUPRC,
                    SDAEXP,
                    SDDOC,
                    SDGLC,
                    SDRCD,
                    SDURCD,
                    SDVR01,
                    SDPSN,
                    SDUPMJ,
                    SDTDAY,
                    SDUSER,
                    SDPID,
                    SDDSC1,
                    SDDSC2,
                    SDPROV,
                    ATZ58PT,
                    ATZ58CSQNO,
                    ATZ58GSQNO,
                    ATZ58CP2,
                    ATZ58CP3,
                    ATZ58CP4,
                    ATZ5840460,
                    ATZ58USF,
                    ATZ58BSF,
                    ATZ58ZPO,
                    ATUPMJ,
                    ATTDAY,
                    IBSTKT
                FROM
                        F42119
                LEFT OUTER JOIN
                        F580032
                ON
                        ATKCOO = SDKCOO
                AND ATDOCO = SDDOCO
                AND ATDCTO = SDDCTO
                AND ATLNID = SDLNID
                LEFT OUTER JOIN
                        F4102
                ON
                        IBMCU = SDMCU
                AND IBITM = SDITM
                WHERE
                        {stringBuilder}
                            1 = 1
                FOR READ ONLY
            ";
            return iDb2Command;
        }

        public static IDataReader GetSalesLineArchive(
          BusinessSystemSession session,
          ProcessRunSpan processRunSpan,
          string plantNum,
          Decimal salesOrderNum,
          List<string> nextStatusCodes)
        {
            List<string> filters = new List<string>();
            List<iDB2Parameter> source = new List<iDB2Parameter>();
            try
            {
                filters.Add("SDMCU = ? AND \r\n");
                source.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                source.Last<iDB2Parameter>().Value = (object)plantNum.PadLeft(12);
                if (!processRunSpan.IsEmpty)
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("SDUPMJ", "SDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                if (salesOrderNum > Decimal.Zero)
                {
                    filters.Add("SDDOCO = ? AND \r\n");
                    source.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    source.Last<iDB2Parameter>().Precision = (byte)8;
                    source.Last<iDB2Parameter>().Scale = (byte)0;
                    source.Last<iDB2Parameter>().Value = (object)salesOrderNum;
                }
                if (nextStatusCodes.Any<string>())
                {
                    string str = "";
                    foreach (string nextStatusCode in nextStatusCodes)
                    {
                        str += "?,";
                        source.Add(new iDB2Parameter("NXTR", iDB2DbType.iDB2Graphic, 3));
                        source.Last<iDB2Parameter>().Value = (object)nextStatusCode;
                    }
                    filters.Add("SDNXTR IN (" + str.Substring(0, str.Length - 1) + ") AND \r\n");
                }
                iDB2Command salesLineArchive = SalesLineDownloadManager.GetSalesLineArchive(filters);
                foreach (iDB2Parameter iDb2Parameter in source)
                    salesLineArchive.Parameters.Add(iDb2Parameter);
                salesLineArchive.Connection = session.DbConnection;
                SalesLineDownloadManager.Logger.Debug(Logging.DbCmdString(salesLineArchive));
                return (IDataReader)salesLineArchive.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static iDB2Command GetSalesLineArchive(List<string> filters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string filter in filters)
                stringBuilder.Append(filter);
            iDB2Command iDb2Command = new iDB2Command();
            iDb2Command.CommandType = CommandType.Text;
            iDb2Command.CommandText = string.Format("\r\n                    SDKCOO,\r\n\t                SDDOCO,\r\n\t                SDDCTO,\r\n\t                SDLNID,\r\n\t                SDRORN,\r\n\t                SDRCTO,\r\n                    SDRLLN,\r\n\t                SDTRDJ,\r\n\t                SDDRQJ,\r\n\t                SDCNDJ,\r\n                    SDADDJ,\r\n\t                SDIVD,\r\n\t                SDITM,\r\n\t                SDLNTY,\r\n\t                SDNXTR,\r\n\t                SDLTTR,\r\n\t                SDAN8,\r\n\t                SDSHAN,\r\n\t                SDUORG,\r\n\t                SDUNCS,\r\n\t                SDECST,\r\n\t                SDUPRC,\r\n\t                SDAEXP,\r\n\t                SDDOC,\r\n\t                SDGLC,\r\n\t                SDRCD,\r\n                    SDURCD,\r\n\t                SDVR01,\r\n                    SDPSN,\r\n\t                SDUPMJ,\r\n\t                SDTDAY,\r\n                    SDUSER,\r\n                    SDPID,\r\n                    SDDSC1,\r\n                    SDDSC2,\r\n\t                ATZ58PT,\r\n\t                ATZ58CSQNO,\r\n\t                ATZ58GSQNO,\r\n\t                ATZ58CP2,\r\n\t                ATZ58CP3,\r\n\t                ATZ58CP4,\r\n\t                ATZ5840460,                    \r\n                    ATZ58USF,\r\n                    ATZ58BSF,\r\n\t                ATUPMJ,\r\n\t                ATTDAY,\r\n                    IBSTKT\r\n\t            WHERE \r\n                    {0}\r\n                    1=1\r\n                FOR READ ONLY", (object)stringBuilder);
            return iDb2Command;
        }
    }
}
