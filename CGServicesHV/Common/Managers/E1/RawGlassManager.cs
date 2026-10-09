using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.E1
{
    public class RawGlassManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        /*********************************************************************************
        * Get F580220s Dynamic
        *******************************************************************************/
        private static iDB2Command GetRawGlassDynamic(IReadOnlyCollection<string> filters)
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
                        F3111.WMCPIT
                       ,F4102.IBMCU
                       ,F4102.IBGLPT 
                       ,F3111.WMDOCO 
                       ,F3111.WMDCTO 
                       ,F3111.WMDSC1 
                       ,F3111.WMDSC2 
                       ,F3111.WMLOCN 
                       ,F3111.WMUORG
                       ,F3111.WMTRQT
                       ,F580014.CRZ58L1TK 
                       ,F580014.CRZ58L1HT 
                       ,F580014.CRZ58L1CL 
                       ,F580014.CRZ58L11C 
                       ,F580014.CRZ58L12C 
                       ,F580014.CRZ58L1PX 
                       ,F580014.CRZ58L1BDM 
                       ,F580014.CRZ58L1BNM 
                       ,F580014.CRZ58L1BDN 
                       ,F580014.CRZ58L1LDM 
                       ,F580014.CRZ58L1LNM 
                       ,F580014.CRZ58L1LDN 
                       ,F580014.CRZ58L1RDM 
                       ,F580014.CRZ58L1RNM 
                       ,F580014.CRZ58L1RDN 
                       ,F580014.CRZ58L1TDM 
                       ,F580014.CRZ58L1TNM 
                       ,F580014.CRZ58L1TDN 
                       ,F580014.CRZ58SHAPE 
                       ,F580021.S7EDBT 
                       ,F580021.S7ITM 
                       ,F580021.S7Z58SCN 
                       ,F3111.WMOPSQ
                    FROM
                        F3111 F3111
                        INNER JOIN F4102 F4102 ON F3111.WMCPIT = F4102.IBITM
                                                  AND F3111.WMMCU = F4102.IBMCU
                        INNER JOIN F580014 F580014 ON F4102.IBITM = F580014.CRITM
                        INNER JOIN F580021 F580021 ON F3111.WMDOCO = F580021.S7DOCO
                        INNER JOIN PYDTA.F580020 F580020 ON F580020.S6EDBT = F580021.S7EDBT
                                                            AND F580020.S6MCU = F580021.S7MCU
                                                            AND F580020.S6Z58SCN = F580021.S7Z58SCN
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
        * Methods
        ********************************************************************************/
        public RawGlassItems GetRawGlassItems(BusinessSystemSession e1Session, string plantNum = null, decimal scheduleNum = 0, decimal scheduleDateE1 = 0, string batchNum = null)
        {
            var rawGlassItems = new RawGlassItems();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Part Type
                filters.Add("F4102.IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC') AND \r\n");
                // MTS Items
                filters.Add("F580020.S6Z58SFS IN ('3') AND \r\n");
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("S7MCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 6));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }

                // MTOSchedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("S7Z58SCN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }

                // Batch Num
                if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                {
                    filters.Add("S7EDBT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                    parameters.Last().Value = batchNum;
                }

                // MTOSchedule Date
                if (scheduleDateE1 > 0)
                {
                    filters.Add("S7Z58SCD = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleDateE1;
                }

                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND", "");
                var cmd = GetRawGlassDynamic(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        var rawGlassItem = new RawGlassItem
                        {
                            WMCPIT = Data.CastDbNull.To(reader["WMCPIT"], string.Empty).Trim(),
                            IBMCU = Data.CastDbNull.To(reader["IBMCU"], string.Empty).Trim(),
                            IBGLPT = Data.CastDbNull.To(reader["IBGLPT"], string.Empty).Trim(),
                            WMDOCO = Data.CastDbNull.To<decimal>(reader["WMDOCO"], 0),
                            WMDCTO = Data.CastDbNull.To(reader["WMDCTO"], string.Empty).Trim(),
                            WMDSC1 = Data.CastDbNull.To(reader["WMDSC1"], string.Empty).Trim(),
                            WMDSC2 = Data.CastDbNull.To(reader["WMDSC2"], string.Empty).Trim(),
                            WMLOCN = Data.CastDbNull.To(reader["WMLOCN"], string.Empty).Trim(),
                            WMUORG = Data.CastDbNull.To<decimal>(reader["WMUORG"], 0),
                            WMTRQT = Data.CastDbNull.To<decimal>(reader["WMTRQT"], 0),
                            CRZ58L1TK = Data.CastDbNull.To<decimal>(reader["CRZ58L1TK"], 0),
                            CRZ58L1HT = Data.CastDbNull.To(reader["CRZ58L1HT"], string.Empty).Trim(),
                            CRZ58L1CL = Data.CastDbNull.To(reader["CRZ58L1CL"], string.Empty).Trim(),
                            CRZ58L11C = Data.CastDbNull.To(reader["CRZ58L11C"], string.Empty).Trim(),
                            CRZ58L12C = Data.CastDbNull.To(reader["CRZ58L12C"], string.Empty).Trim(),
                            CRZ58L1PX = Data.CastDbNull.To(reader["CRZ58L1PX"], string.Empty).Trim(),
                            CRZ58L1BDM = Data.CastDbNull.To<decimal>(reader["CRZ58L1BDM"], 0),
                            CRZ58L1BNM = Data.CastDbNull.To<decimal>(reader["CRZ58L1BNM"], 0),
                            CRZ58L1BDN = Data.CastDbNull.To<decimal>(reader["CRZ58L1BDN"], 0),
                            CRZ58L1LDM = Data.CastDbNull.To<decimal>(reader["CRZ58L1LDM"], 0),
                            CRZ58L1LNM = Data.CastDbNull.To<decimal>(reader["CRZ58L1LNM"], 0),
                            CRZ58L1LDN = Data.CastDbNull.To<decimal>(reader["CRZ58L1LDN"], 0),
                            CRZ58L1RDM = Data.CastDbNull.To<decimal>(reader["CRZ58L1RDM"], 0),
                            CRZ58L1RNM = Data.CastDbNull.To<decimal>(reader["CRZ58L1RNM"], 0),
                            CRZ58L1RDN = Data.CastDbNull.To<decimal>(reader["CRZ58L1RDN"], 0),
                            CRZ58L1TDM = Data.CastDbNull.To<decimal>(reader["CRZ58L1TDM"], 0),
                            CRZ58L1TNM = Data.CastDbNull.To<decimal>(reader["CRZ58L1TNM"], 0),
                            CRZ58L1TDN = Data.CastDbNull.To<decimal>(reader["CRZ58L1TDN"], 0),
                            CRZ58SHAPE = Data.CastDbNull.To(reader["CRZ58SHAPE"], string.Empty).Trim(),
                            S7EDBT = Data.CastDbNull.To(reader["S7EDBT"], string.Empty).Trim(),
                            S7ITM = Data.CastDbNull.To<decimal>(reader["S7ITM"], 0),
                            S7Z58SCN = Data.CastDbNull.To<decimal>(reader["S7Z58SCN"], 0),
                            WMOPSQ = Data.CastDbNull.To<decimal>(reader["WMOPSQ"], 0)
                        };
                        rawGlassItems.Add(rawGlassItem);
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return rawGlassItems;
        }
    }
}
