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
    public class RackTypeDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public RackTypeDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant)
        {
            var response = new ProcessResponse("Rack Type Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new RackTypeTable();

            if (!response.HasErrors())
            {
                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetReader(session, plant.PlantNum);
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

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
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
            }
            response.Close();
            return response;
        }

        /********************************************************************************
        * EVENTS
        ********************************************************************************/
        private static iDB2Command GetRackTypes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT *
                    FROM (
                            SELECT
                                P.RTZ58RT,
                                P.RTMCU,
                                P.RTAN8,
                                P.RTSHAN,
                                P.RTZ58RTD,
                                P.RTZ58RTST,
                                P.RTZ58CRT,
                                P.RTZ58SHP,
                                P.RTZ58RTWT,
                                P.RTZ58WTMN,
                                P.RTZ58WTMX,
                                P.RTZ58WTUM,
                                P.RTGHET,
                                P.RTGWID,
                                P.RTGDEP,
                                P.RTZ58LMDC,
                                P.RTZ58LXDC,
                                P.RTZ58HMDC,
                                P.RTZ58HXDC,
                                P.RTZ58WMDC,
                                P.RTZ58WXDC,
                                P.RTWIUM,
                                P.RTZ58RSD,
                                P.RTZ58RPS,
                                P.RTZ58RXT,
                                P.RTZ58RXR,
                                P.RTZ58RSL,
                                P.RTZ58RSH,
                                P.RTZ58RAN,
                                P.RTGCUB,
                                P.RTVUMD,
                                P.RTUPMJ,
                                P.RTTDAY
                            FROM
                                F580061 P
                            WHERE P.RTMCU = ?
                            UNION
                            SELECT
                                Z.RTZ58RT,
                                Z.RTMCU,
                                Z.RTAN8,
                                Z.RTSHAN,
                                Z.RTZ58RTD,
                                Z.RTZ58RTST,
                                Z.RTZ58CRT,
                                Z.RTZ58SHP,
                                Z.RTZ58RTWT,
                                Z.RTZ58WTMN,
                                Z.RTZ58WTMX,
                                Z.RTZ58WTUM,
                                Z.RTGHET,
                                Z.RTGWID,
                                Z.RTGDEP,
                                Z.RTZ58LMDC,
                                Z.RTZ58LXDC,
                                Z.RTZ58HMDC,
                                Z.RTZ58HXDC,
                                Z.RTZ58WMDC,
                                Z.RTZ58WXDC,
                                Z.RTWIUM,
                                Z.RTZ58RSD,
                                Z.RTZ58RPS,
                                Z.RTZ58RXT,
                                Z.RTZ58RXR,
                                Z.RTZ58RSL,
                                Z.RTZ58RSH,
                                Z.RTZ58RAN,
                                Z.RTGCUB,
                                Z.RTVUMD,
                                Z.RTUPMJ,
                                Z.RTTDAY
                            FROM
                                F580061 Z
                            WHERE Z.RTMCU = ''
                            AND Z.RTZ58RT NOT IN (  SELECT RTZ58RT
                                                    FROM F580061 
                                                    WHERE RTMCU = ?)
                    ) x
                "
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, string plantNum)
        {
            var parameters = new List<iDB2Parameter>();
            try
            {
                var cmd = GetRackTypes();
                cmd.Parameters.Add(new iDB2Parameter("MCU1", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters.Add(new iDB2Parameter("MCU2", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters["MCU1"].Value = plantNum.PadLeft(12);
                cmd.Parameters["MCU2"].Value = plantNum.PadLeft(12);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
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
