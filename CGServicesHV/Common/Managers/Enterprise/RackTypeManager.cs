using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using CGServices.Common.Managers.E1;
using NLog;

namespace CGServices.Common.Managers.Enterprise
{
    public class RackTypeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RackTypeManager(BusinessSystemSession e1Session)
        //{
        //}

        /*********************************************************************************
         * Get Rack Types Dynamic
         *******************************************************************************/
        private static iDB2Command GetRackTypesDynamic(IReadOnlyCollection<string> filters)
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
                        RTZ58RT,
                        RTMCU,
                        RTAN8,
                        RTSHAN,
                        RTZ58RTD,
                        RTZ58RTST,
                        RTZ58CRT,
                        RTZ58SHP,
                        RTZ58RTWT,
                        RTZ58WTMN,
                        RTZ58WTMX,
                        RTZ58WTUM,
                        RTGHET,
                        RTGWID,
                        RTGDEP,
                        RTZ58LMDC,
                        RTZ58LXDC,
                        RTZ58HMDC,
                        RTZ58HXDC,
                        RTZ58WMDC,
                        RTZ58WXDC,
                        RTWIUM,
                        RTZ58RSD,
                        RTZ58RPS,
                        RTZ58RXT,
                        RTZ58RXR,
                        RTZ58RSL,
                        RTZ58RSH,
                        RTZ58RAN,
                        RTGCUB,
                        RTVUMD,
                        RTUPMJ,
                        RTTDAY
                    FROM
                        F580061
                    {filterString}"
            };
            return cmd;
        }

        private static iDB2Command GetRackTypesDynamicPlant(IReadOnlyCollection<string> filters)
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

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static RackTypes GetRackTypes(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string plantNum, string rackTypeCode = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var rackTypes = new RackTypes();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Rack Type Code
                if (!string.IsNullOrEmpty(rackTypeCode))
                {
                    filters.Add("RTZ58RT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("RackTypeCode", iDB2DbType.iDB2Graphic, 5));
                    parameters.Last().Value = rackTypeCode;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RTUPMJ", "RTTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        cmd = GetRackTypesDynamicPlant(filters);
                        cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        cmd.Parameters.Add(new iDB2Parameter("PlantNum2", iDB2DbType.iDB2Graphic, 12));
                        cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                        cmd.Parameters["PlantNum2"].Value = plantNum.PadLeft(12);
                    }
                    else
                    {
                        cmd = GetRackTypesDynamic(filters);
                    }
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                    {
                        cmd = GetRackTypesDynamicPlant(filters);
                        cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                        cmd.Parameters.Add(new iDB2Parameter("PlantNum2", iDB2DbType.iDB2Graphic, 12));
                        cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                        cmd.Parameters["PlantNum2"].Value = plantNum.PadLeft(12);
                    }
                    else
                    {
                        cmd = GetRackTypesDynamic(filters);
                    }
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rackType = new RackType
                    {
                        RackTypeCode = CastDbNull.To(reader["RTZ58RT"], string.Empty).Trim(),
                        CostCenter = CastDbNull.To(reader["RTMCU"], string.Empty).Trim(),
                        AddressNum = CastDbNull.To<decimal>(reader["RTAN8"], 0),
                        AddressShipToNum = CastDbNull.To<decimal>(reader["RTSHAN"], 0),
                        RackTypeName = CastDbNull.To(reader["RTZ58RTD"], string.Empty).Trim(),
                        RackTypeStatus = CastDbNull.To(reader["RTZ58RTST"], string.Empty).Trim(),
                        CorpFlag = CastDbNull.To(reader["RTZ58CRT"], string.Empty).Trim(),
                        ShippableFlag = CastDbNull.To(reader["RTZ58SHP"], string.Empty).Trim(),
                        TareWeight = CastDbNull.To<decimal>(reader["RTZ58RTWT"], 0) / 100,
                        MinTotalWeight = CastDbNull.To<decimal>(reader["RTZ58WTMN"], 0),
                        MaxTotalWeight = CastDbNull.To<decimal>(reader["RTZ58WTMX"], 0),
                        WeightUom = CastDbNull.To(reader["RTZ58WTUM"], string.Empty).Trim(),
                        GrossHeight = CastDbNull.To<decimal>(reader["RTGHET"], 0) / 100,
                        GrossWidth = CastDbNull.To<decimal>(reader["RTGWID"], 0) / 100,
                        GrossDepth = CastDbNull.To<decimal>(reader["RTGDEP"], 0) / 100,
                        GlassMinLength = CastDbNull.To<decimal>(reader["RTZ58LMDC"], 0) / 1000000,
                        GlassMaxLength = CastDbNull.To<decimal>(reader["RTZ58LXDC"], 0) / 1000000,
                        GlassMinHeight = CastDbNull.To<decimal>(reader["RTZ58HMDC"], 0) / 1000000,
                        GlassMaxHeight = CastDbNull.To<decimal>(reader["RTZ58HXDC"], 0) / 1000000,
                        GlassMinWidth = CastDbNull.To<decimal>(reader["RTZ58WMDC"], 0) / 1000000,
                        GlassMaxWidth = CastDbNull.To<decimal>(reader["RTZ58WXDC"], 0) / 1000000,
                        DimensionUom = CastDbNull.To(reader["RTWIUM"], string.Empty).Trim(),
                        RackSides = CastDbNull.To<decimal>(reader["RTZ58RSD"], 0),
                        RackPositions = CastDbNull.To<decimal>(reader["RTZ58RPS"], 0),
                        RackStacks = CastDbNull.To<decimal>(reader["RTZ58RXT"], 0),
                        RackRows = CastDbNull.To<decimal>(reader["RTZ58RXR"], 0),
                        RackSlots = CastDbNull.To<decimal>(reader["RTZ58RSL"], 0),
                        ShapeFlag = CastDbNull.To(reader["RTZ58RSH"], string.Empty).Trim(),
                        AnnealedFlag = CastDbNull.To(reader["RTZ58RAN"], string.Empty).Trim(),
                        GrossCubicDimensions = CastDbNull.To<decimal>(reader["RTGCUB"], 0) / 100,
                        VolumeUom = CastDbNull.To(reader["RTVUMD"], string.Empty).Trim()
                    };
                    var updateDateId = CastDbNull.To<decimal>(reader["RTUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["RTTDAY"], 0);
                    rackType.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    rackTypes.Add(rackType);
                }
                reader.Close();
                Logger.Trace("ROWS: " + rackTypes.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rackTypes;
        }
    }
}
