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
    public class InventoryLocationManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public InventoryLocationManager(BusinessSystemSession e1Session)
        //{
        //}

        /*********************************************************************************
         * Get Inventory Locations Dynamic
         *******************************************************************************/
        private static iDB2Command GetInventoryLocationsDynamic(IReadOnlyCollection<string> filters)
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
                        LMMCU,
                        LMLOCN,
                        LMAISL,
                        LMBIN,
                        LMLA03,
                        LMLA04,
                        LMLLDL,
                        LMSTAG,
                        LMPZON,
                        LMUPMJ,
                        LMTDAY
                    FROM
                        F4100
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static InventoryLocations GetInventoryLocations(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            string plantNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var inventoryLocations = new InventoryLocations();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                filters.Add("RTRIM(LMLOCN) <> '' AND \r\n");
                filters.Add("RTRIM(LMLLDL) <> '2' AND \r\n");
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("LMMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("LMUPMJ", "LMTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetInventoryLocationsDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetInventoryLocationsDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var inventoryLocation = new InventoryLocation
                    {
                        PlantNum = CastDbNull.To(reader["LMMCU"], string.Empty).Trim(),
                        LocationCode = CastDbNull.To(reader["LMLOCN"], string.Empty).Trim(),
                        Aisle = CastDbNull.To(reader["LMAISL"], string.Empty).Trim(),
                        Bin = CastDbNull.To(reader["LMBIN"], string.Empty).Trim(),
                        DetailLevel = CastDbNull.To(reader["LMLLDL"], string.Empty).Trim(),
                        PutawayZone = CastDbNull.To(reader["LMPZON"], string.Empty).Trim(),
                        Category03 = CastDbNull.To(reader["LMLA03"], string.Empty).Trim(),
                        Category04 = CastDbNull.To(reader["LMLA04"], string.Empty).Trim(),
                        StagingFlag = CastDbNull.To(reader["LMSTAG"], string.Empty).Trim(),
                        UpdateDateId = CastDbNull.To<decimal>(reader["LMUPMJ"], 0),
                        UpdateTimeId = CastDbNull.To<decimal>(reader["LMTDAY"], 0)
                    };
                    var updateDateId = CastDbNull.To<decimal>(reader["LMUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["LMTDAY"], 0);
                    inventoryLocation.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    inventoryLocations.Add(inventoryLocation);
                }
                reader.Close();
                Logger.Trace("ROWS: " + inventoryLocations.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return inventoryLocations;
        }
    }
}
