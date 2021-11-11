using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.E1;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class ItemAttributeCache
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public Dictionary<decimal, ItemAttributes> Cache { get; set; }

        public ItemAttributeCache()
        {
            Cache = new Dictionary<decimal, ItemAttributes>();
        }

        /********************************************************************************
        * Configured Item Attributes from MTOSchedule
        *******************************************************************************/
        public iDB2Command GetConfiguredItemAttributesBySchedule()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 120,
                CommandText = $@"
                    SELECT
                        CM.CMDOCO AS DOCO,
	                    C.AMZ58ATNM AS ATNM,
                        A.KZSGVL AS SGVL,
                        U.DRDL01 AS DL01,
                        U.DRDL02 AS DL02,
                        U.DRSPHD AS SPHD
                    FROM
			            F5834A11
                        INNER JOIN F3201 CM
				        ON  CM.CMDOCO = S5DOCO AND
				            CM.CMKCOO = '*'
                        INNER JOIN F3211 A
                        ON  CM.CMCFGID = A.KZCFGID
                        INNER JOIN ( SELECT
                                        KZCFGID ,
                                        KZPEL ,
                                        MAX(KZCFGCID) AS KZCFGCID
                                    FROM
                                        F3211
                                    GROUP BY
                                        KZCFGID ,
                                        KZPEL
                                    ) B
                        ON  B.KZCFGID = A.KZCFGID AND
                            A.KZPEL = B.KZPEL AND
                            A.KZCFGCID = B.KZCFGCID
                        INNER JOIN F580001 C
                        ON  C.AMPEL = A.KZPEL  
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U
                        ON  U.DRSY = C.AMSY AND
                            U.DRRT = C.AMRT AND
                            TRIM(U.DRKY) = A.KZSGVL  
                    WHERE
                        A.KZSGVL <> '' AND
                        NOT (C.AMCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000')) AND
                        S5MCU = ? AND
				        S5Z58SCN = ? AND
				        S5Z58SCD =  ?
                    ORDER BY
                        CM.CMDOCO"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ScheduleNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleNum"].Precision = 5;
            cmd.Parameters["ScheduleNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("ScheduleDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleDateId"].Precision = 6;
            cmd.Parameters["ScheduleDateId"].Scale = 0;
            return cmd;
        }

        [DataContract(Namespace = "")]
        private class ItemAttributeRaw
        {
            public decimal DocNum { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Desc1 { get; set; }
            public string Desc2 { get; set; }
            public string Desc3 { get; set; }
        }

        private class ItemAttributesRaw : List<ItemAttributeRaw>
        {
        }

        public int LoadCacheFromSchedule(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            try
            {
                var cmd = GetConfiguredItemAttributesBySchedule();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleDateId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var attributesRaw = new ItemAttributesRaw();
                    while (reader.Read())
                    {
                        var attributeRaw = new ItemAttributeRaw
                        {
                            DocNum = (decimal) reader["DOCO"],
                            Name = reader["ATNM"].ToString().Trim(),
                            Value = reader["SGVL"].ToString().Trim(),
                            Desc1 = reader["DL01"].ToString().Trim(),
                            Desc2 = reader["DL02"].ToString().Trim(),
                            Desc3 = reader["SPHD"].ToString().Trim()
                        };
                        attributesRaw.Add(attributeRaw);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + attributesRaw.Count);
                    var nameGroups = from n in attributesRaw
                        group n by n.DocNum
                        into g
                        select new
                        {
                            Keys = g.Key,
                            Values = g
                        };
                    foreach (var g in nameGroups)
                    {
                        var attbs = new ItemAttributes();
                        foreach (var attb in g.Values.Select(x => new ItemAttribute
                        {
                            Name = x.Name,
                            Value = x.Value,
                            Desc1 = x.Desc1,
                            Desc2 = x.Desc2,
                            Desc3 = x.Desc3
                        }).Where(attb => !attbs.Contains(attb.Name)))
                        {
                            attbs.Add(attb);
                        }

                        Cache.Add(g.Keys, attbs);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return Cache.Count;
        }
    }
}
