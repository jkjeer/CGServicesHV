using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class RackTypeKeyManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*********************************************************************************
         * Get Rack Types Dynamic
         *******************************************************************************/
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
        public static RackTypes GetRackTypeKeys(BusinessSystemSession e1Session, string plantNum = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var rackTypes = new RackTypes();
            var filters = new List<string>();
            try
            {
                var cmd = GetRackTypesDynamicPlant(filters);
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    cmd.Parameters.Add(new iDB2Parameter("PlantNum2", iDB2DbType.iDB2Graphic, 12));
                    cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                    cmd.Parameters["PlantNum2"].Value = plantNum.PadLeft(12);
                }

                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rackType = new RackType
                        {
                            RackTypeCode = Data.CastDbNull.To(reader["RTZ58RT"], string.Empty).Trim(),
                            CostCenter = Data.CastDbNull.To(reader["RTMCU"], string.Empty).Trim(),
                            AddressNum = Data.CastDbNull.To<decimal>(reader["RTAN8"], 0),
                            AddressShipToNum = Data.CastDbNull.To<decimal>(reader["RTSHAN"], 0),
                            RackTypeName = Data.CastDbNull.To(reader["RTZ58RTD"], string.Empty).Trim(),
                            RackTypeStatus = Data.CastDbNull.To(reader["RTZ58RTST"], string.Empty).Trim(),
                            CorpFlag = Data.CastDbNull.To(reader["RTZ58CRT"], string.Empty).Trim(),
                            ShippableFlag = Data.CastDbNull.To(reader["RTZ58SHP"], string.Empty).Trim(),
                            TareWeight = Data.CastDbNull.To<decimal>(reader["RTZ58RTWT"], 0) / 100,
                            MinTotalWeight = Data.CastDbNull.To<decimal>(reader["RTZ58WTMN"], 0),
                            MaxTotalWeight = Data.CastDbNull.To<decimal>(reader["RTZ58WTMX"], 0),
                            WeightUom = Data.CastDbNull.To(reader["RTZ58WTUM"], string.Empty).Trim(),
                            GrossHeight = Data.CastDbNull.To<decimal>(reader["RTGHET"], 0) / 100,
                            GrossWidth = Data.CastDbNull.To<decimal>(reader["RTGWID"], 0) / 100,
                            GrossDepth = Data.CastDbNull.To<decimal>(reader["RTGDEP"], 0) / 100,
                            GlassMinLength = Data.CastDbNull.To<decimal>(reader["RTZ58LMDC"], 0) / 1000000,
                            GlassMaxLength = Data.CastDbNull.To<decimal>(reader["RTZ58LXDC"], 0) / 1000000,
                            GlassMinHeight = Data.CastDbNull.To<decimal>(reader["RTZ58HMDC"], 0) / 1000000,
                            GlassMaxHeight = Data.CastDbNull.To<decimal>(reader["RTZ58HXDC"], 0) / 1000000,
                            GlassMinWidth = Data.CastDbNull.To<decimal>(reader["RTZ58WMDC"], 0) / 1000000,
                            GlassMaxWidth = Data.CastDbNull.To<decimal>(reader["RTZ58WXDC"], 0) / 1000000,
                            DimensionUom = Data.CastDbNull.To(reader["RTWIUM"], string.Empty).Trim(),
                            RackSides = Data.CastDbNull.To<decimal>(reader["RTZ58RSD"], 0),
                            RackPositions = Data.CastDbNull.To<decimal>(reader["RTZ58RPS"], 0),
                            RackStacks = Data.CastDbNull.To<decimal>(reader["RTZ58RXT"], 0),
                            RackRows = Data.CastDbNull.To<decimal>(reader["RTZ58RXR"], 0),
                            RackSlots = Data.CastDbNull.To<decimal>(reader["RTZ58RSL"], 0),
                            ShapeFlag = Data.CastDbNull.To(reader["RTZ58RSH"], string.Empty).Trim(),
                            AnnealedFlag = Data.CastDbNull.To(reader["RTZ58RAN"], string.Empty).Trim(),
                            GrossCubicDimensions = Data.CastDbNull.To<decimal>(reader["RTGCUB"], 0) / 100,
                            VolumeUom = Data.CastDbNull.To(reader["RTVUMD"], string.Empty).Trim()
                        };
                        var updateDateId = Data.CastDbNull.To<decimal>(reader["RTUPMJ"], 0);
                        var updateTimeId = Data.CastDbNull.To<decimal>(reader["RTTDAY"], 0);
                        rackType.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                        rackTypes.Add(rackType);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + rackTypes.Count);
                }
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
