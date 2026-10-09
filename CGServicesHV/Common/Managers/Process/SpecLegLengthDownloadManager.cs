using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using CGServicesHV.Common.Types.ShopFloor;
using Microsoft.Data.SqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;


namespace CGServicesHV.Common.Managers.Process
{
    public class SpecLegLengthDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant)
        {
            var response = new ProcessResponse("Spec Leg Length Download Manager");
            response.Plant = plant;
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var table = new SpecLegLengthTable();
            var step2 = new ProcessStep("Get Source Data");
            try
            {
                var reader = GetReader(sqlCns["CORP.INTEGRATION"]);
                table.Add(reader, plant);
                Logger.Debug($@"Rows: {table.Rows.Count}");
                step2.RowsAffected = table.Rows.Count;
                response.RowsAffected = table.Rows.Count;
            }
            catch (Exception e)
            {
                step2.Errors.Add(new Error(e));
            }
            step2.Done();
            response.AddStep(step2);

            /********************************************************/
            if (table.Rows.Count > 0)
            {
                var step3 = new ProcessStep("Bulk Copy");
                var bulkResponse = new SqlBulkCopyResponse();
                try
                {
                    bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"], null, 100000);
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
            response.Close();
            return response;
        }

        /********************************************************************************
        * EVENTS
        ********************************************************************************/
        private static SqlCommand GetReader()
        {
            var cmd = new SqlCommand()
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    select [LegLengthId]
                          ,[Increment]
                          ,[Inch]
                          ,[MinInch]
                          ,[MaxInch]
                          ,[Millimeter]
                          ,[MinMillimeter]
                          ,[MaxMillimeter]
                          ,[FractionalBase]
                          ,[FractionalNumerator]
                          ,[FractionalDenominator]
                          ,[FractionalDescription]
                          ,[WrittenDate]
                      FROM [EBS].[SpecLegLength]
                "
            };
            return cmd;
        }

        public static IDataReader GetReader(SqlConnection cnCorpInt)
        {
            try
            {
                var cmd = GetReader();
                cmd.Connection = cnCorpInt;
                Logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
