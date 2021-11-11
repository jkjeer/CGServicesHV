using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
    public class RackTypeKeyDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RackTypeKeyDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"RackTypeDownloadManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public static ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant)
        {
            var response = new ProcessResponse("RackTypeKey Download");
            var rackTypeKeyTable = new RackTypeKeyTable();
            //var responseTime1 = new ResponseTime(response.ProcessName, "Get", e1Session.DbConnection.ConnectionString);
            var reader = GetReader(e1Session, plant.PlantNum);
            //response.ResponseTimes.Add(responseTime1.Done());
            rackTypeKeyTable.Add(reader);
            if (rackTypeKeyTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(rackTypeKeyTable.ExtendedProperties["Target"].ToString(), rackTypeKeyTable, cnSFInt);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                //response.ResponseTimes.Add(responseTime2.Done());
            }
            return response;
        }

        private static iDB2Command GetRackTypeKeysDynamicPlant(IReadOnlyCollection<string> filters)
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
                    {filterString}"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession e1Session, string plantNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var filters = new List<string>();
            try
            {
                var cmd = GetRackTypeKeysDynamicPlant(filters);
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    cmd.Parameters.Add(new iDB2Parameter("PlantNum2", iDB2DbType.iDB2Graphic, 12));
                    cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                    cmd.Parameters["PlantNum2"].Value = plantNum.PadLeft(12);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
