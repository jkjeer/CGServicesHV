using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    public static class BillOfMaterialDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("BillOfMaterials Download Manager", plant);
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new BillOfMaterialTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.INTEGRATION"], plant, "BillOfMaterial") : null;
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
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetData(session, processRunSpan, plant);
                    table.Add(reader, plant);
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }

                step2.Done();
                response.AddStep(step2);
            }

            /********************************************************/
            if (table.Rows.Count > 0)
            {
                var step3 = new ProcessStep("Bulk Copy");
                var bulkResponse = new SqlBulkCopyResponse();
                try
                {
                    if (table.Rows.Count > 0)
                    {
                        response.RowsAffected = table.Rows.Count;
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
                        DateTime enterpriseDateTime;
                        if (table.MaxDate.AddMinutes(-plant.BusinessSystemOffset) > processRunSpan.EndDateTime)
                        {
                            enterpriseDateTime = processRunSpan.EndDateTime;
                        }
                        else
                        {
                            enterpriseDateTime = table.MaxDate.AddMinutes(-plant.BusinessSystemOffset);
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMarkDate(sqlCns["PLANT.INTEGRATION"],
                            plant, highWaterMark, enterpriseDateTime));
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

            response.Close();
            return response;
        }

        /******************************************************************
         *  Data
         ******************************************************************/

        public static iDB2Command GetData(Sql.SqlFilters filters)
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
            SELECT
                A.IXKIT,
                A.IXTBM,
                A.IXMMCU,
                A.IXITM,
                A.IXCPNT,
                A.IXSBNT,
                A.IXQNTY,
                A.IXUM,
                A.IXBQTY,
                A.IXUOM,
                A.IXFVBT,
                A.IXEFFF,
                A.IXEFFT,
                A.IXITC,
                A.IXFORQ,
                A.IXCOBY,
                A.IXCOTY,
                A.IXOPSQ,
                A.IXFTRP,
                A.IXF$RP,
                A.IXRSCP,
                A.IXSCRP,
                A.IXREWP,
                A.IXDSC1,
                A.IXLNTY,
                A.IXUSER,
                A.IXPID,
                A.IXUPMJ,
                A.IXTDAY,
                A.IXCPNB
            FROM 
	            F3002 A
            WHERE 
                EXISTS (SELECT 1 FROM F3002 B WHERE B.IXKIT = A.IXKIT AND B.IXMMCU = A.IXMMCU AND {filters})
            FOR READ ONLY";
            return cmd;
        }

        public static IDataReader GetData(BusinessSystemSession session, ProcessRunSpan processRunSpan, Plant plant)
        {
            var filters = new Sql.SqlFilters();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add(new Sql.SqlFilter("IXMMCU = ?", "AND"));
                parameters.Add(new iDB2Parameter("MMCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Dates
                if (!processRunSpan.IsEmpty)
                {
                    var dtList = new List<string[]>
                    {
                        new string[] { "IXUPMJ", "IXTDAY" }
                    };
                    filters.Add(new Sql.SqlFilter($"{E1TimeManager.GetE1DateFilterString(dtList, processRunSpan.StartDateTime, processRunSpan.EndDateTime)}", "AND"));
                }

                // Command
                var cmd = GetData(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
