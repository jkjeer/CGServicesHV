using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using IBM.Data.DB2.iSeries;
using CGServices.Common.Managers.E1;
using NLog;

namespace CGServices.Common.Managers.Enterprise
{
    public class SupplierManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SupplierManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //    {"SupplierManager.GetSupplier", GetSupplier()}
            };
            //foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            //{
            //    e1Session.DbCommands.Add(keyName, hash[keyName]);
            //}
        }

        private static iDB2Command GetSuppliersDynamic(IReadOnlyCollection<string> filters)
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
                     SELECT  F0101.ABAN8 
                            ,F0101.ABALKY 
                            ,F0101.ABTAX 
                            ,F0101.ABALPH 
                            ,F0101.ABDC 
                            ,F0101.ABMCU 
                            ,F0101.ABAT1 
                            ,F0005.DRDL01
                            ,F0101.ABTAXC 
                            ,F0101.ABAT2 
                            ,F0101.ABAT3 
                            ,F0101.ABAT4 
                            ,F0101.ABAT5 
                            ,F0101.ABATP 
                            ,F0101.ABATR 
                            ,F0101.ABATPR 
                            ,F0101.ABATE 
                            ,F0101.ABAN81 
                            ,F0101.ABAC05 
                            ,F0101.ABAC06 
                            ,F0101.ABAC08 
                            ,F0101.ABAC10 
                            ,F0101.ABAC13 
                            ,F0101.ABAC16 
                            ,F0101.ABAC18 
                            ,F0101.ABAC26 
                            ,F0101.ABAC29 
                            ,F0101.ABAC30 
                            ,F0116.ALADD1 
                            ,F0116.ALADD2 
                            ,F0116.ALADD3 
                            ,F0116.ALADD4 
                            ,F0116.ALADDZ 
                            ,F0116.ALCTY1 
                            ,F0116.ALCOUN 
                            ,F0116.ALADDS 
                            ,F0116.ALCTR
                            ,F0116.ALEFTB
                            ,F0401.A6UPMJ 
                            ,F0401.A6UPMT 
                            ,F0101.ABUPMJ 
                            ,F0101.ABUPMT 
                            ,F0116.ALUPMJ 
                            ,F0116.ALUPMT 
                            ,F0115.WPAR1
                            ,F0115.WPPH1
                            ,COALESCE(MAPA8,ABAN8) AS MAPA8
                            ,ROW_NUMBER() OVER ( PARTITION BY F0116.ALAN8 ORDER BY F0116.ALEFTB ) AS RowNumber
                    FROM    F0401 F0401
                            INNER JOIN F0101 F0101 ON F0101.ABAN8 = F0401.A6AN8 
                            LEFT OUTER JOIN ( SELECT * 
                                                FROM (
		                                                SELECT   ALAN8 
                                                                ,ALADD1 
                                                                ,ALADD2 
                                                                ,ALADD3 
                                                                ,ALADD4 
                                                                ,ALADDZ 
                                                                ,ALCTY1 
                                                                ,ALCOUN 
                                                                ,ALADDS 
                                                                ,ALCTR
                                                                ,ALEFTB
                                                                ,ALUPMJ 
                                                                ,ALUPMT 
		                                                        ,ROW_NUMBER() OVER ( PARTITION BY ALAN8 ORDER BY ALEFTB ) AS RowNumber
		                                                FROM F0116 
	                                                ) x WHERE RowNumber = 1 	
                                            ) F0116 ON F0116.ALAN8 = F0101.ABAN8 
	                        LEFT OUTER JOIN F0115 F0115 ON WPAN8 = ABAN8 AND
				                                     WPIDLN = 0 AND
				                                     WPRCK7 = 1 AND
				                                     WPCNLN = 0
	                        LEFT OUTER JOIN F0150 ON MAAN8 = ABAN8
                            LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 F0005 ON DRSY = '01' AND
                                                                                                      DRRT = 'ST' AND
                                                                                                      TRIM(DRKY) = ABAT1
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static Suppliers GetSuppliers(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan,
            decimal addressNum = 0)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var suppliers = new Suppliers();
            var filters = new List<string>();
            try
            {
                filters.Add("RowNumber = 1 AND \r\n");
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("A6UPMJ", "A6UPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString6 = E1TimeManager.GetE1DateTimeQueryString("MAUPMJ", "MAUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + " OR " + queryString6 + ")" + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetSuppliersDynamic(filters);
                }
                else
                {
                    cmd = GetSuppliersDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                //while (reader.Read())
                //{
                //    customer.CustomerNum = addressNum;
                //    customer.LegacyCustomerNum = CastDbNull.To(reader["ABALKY"], string.Empty).Trim();
                //    customer.CustomerType = CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                //    customer.CustomerName = CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                //    var abac30 = CastDbNull.To(reader["ABAC30"], string.Empty).Trim();
                //    customer.IsPlant = (abac30 != string.Empty & customer.CustomerType == "F");
                //    if (customer.IsPlant)
                //    {
                //        customer.PlantNum = abac30;
                //        customer.PlantCode = e1Session.PlantMap.GetAttributesByNum(abac30).Code;
                //        customer.ShortCompanyNum = CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                //        customer.CompanyNum = CastDbNull.To(reader["ABAC29"], string.Empty).Trim().PadLeft(5).Replace(' ', '0');
                //    }
                //    if (includeAddress)
                //    {
                //        customer.Address = GetCustomerAddress(e1Session, addressNum);
                //    }
                //}
                reader.Close();
                Logger.Trace("ROWS: " + suppliers.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return suppliers;
        }
    }
}
