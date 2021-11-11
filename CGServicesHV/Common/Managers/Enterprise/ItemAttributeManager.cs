using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class ItemAttributeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ItemAttributeManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"ItemAttributeManager.GetDiscreteItemAttributes", GetDiscreteItemAttributes()},
                {"ItemAttributeManager.GetConfiguredItemAttributes", GetConfiguredItemAttributes()},
                {"ItemAttributeManager.GetConfiguredItemAttributesById", GetConfiguredItemAttributesById()},
                {"ItemAttributeManager.GetArchivedItemAttributesById", GetArchivedItemAttributesById()},
                {"ItemAttributeManager.GetConfiguredIdByDocument", GetConfiguredIdByDocument()},
                {"ItemAttributeManager.GetConfiguredId", GetConfiguredId()}

            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * Commands
        *******************************************************************************/
        private iDB2Command GetDiscreteItemAttributes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
	                    IAZ58ATNM AS ATNM,
	                    IADVAL AS SGVL,
                        A.DRDL01 AS DL01,
                        A.DRDL02 AS DL02,
                        A.DRSPHD AS SPHD
                    FROM
                        F580002
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 A
                        ON A.DRSY = IASY AND
                            A.DRRT = IART AND
                            TRIM(A.DRKY) = IADVAL  
                    WHERE
	                    IAITM = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetConfiguredItemAttributes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
	                    C.AMZ58ATNM AS ATNM,
                        A.KZSGVL AS SGVL,
                        U.DRDL01 AS DL01,
                        U.DRDL02 AS DL02,
                        U.DRSPHD AS SPHD
                    FROM
                        F3201 CM
                        INNER JOIN F3211 A
                        ON CM.CMCFGID = A.KZCFGID
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
                        ON B.KZCFGID = A.KZCFGID AND
                            A.KZPEL = B.KZPEL AND
                            A.KZCFGCID = B.KZCFGCID
                        INNER JOIN F580001 C
                        ON C.AMPEL = A.KZPEL  
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U
                        ON U.DRSY = C.AMSY AND
                            U.DRRT = C.AMRT AND
                            TRIM(U.DRKY) = A.KZSGVL  
                    WHERE
                        A.KZSGVL <> '' AND
                        NOT (C.AMCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000')) AND
                        CM.CMKCOO = ? AND
                        CM.CMDOCO = ? AND
                        CM.CMDCTO = ? AND
                        CM.CMLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetConfiguredItemAttributesById()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
	                ATNM, 
	                AMSY,
	                AMRT,
	                SGVL,
                    COALESCE(U1.DRDL01, U2.DRDL01) AS DL01,
                    COALESCE(U1.DRDL02, U2.DRDL02) AS DL02,
                    COALESCE(U1.DRSPHD, U2.DRSPHD) AS SPHD
                FROM (
	                SELECT
		                C.AMZ58ATNM AS ATNM,
		                C.AMSY,
		                C.AMRT,
		                A.KZSGVL AS SGVL,
		                ROW_NUMBER() OVER (PARTITION BY A.KZCFGID, A.KZPEL ORDER BY A.KZCFGCID ASC) AS ROWNUM
	                FROM
		                F3211 A
		                INNER JOIN F580001 C
		                ON C.AMPEL = A.KZPEL  
	                WHERE
		                A.KZCFGID = ? AND
		                (A.KZSGVL != '' OR KZPEL IN (390,2070,3660)) AND
		                NOT (A.KZCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000'))
		                ) A
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U1
                    ON  U1.DRSY = A.AMSY AND
                        U1.DRRT = A.AMRT AND
                        CAST(U1.DRKY AS INT) = CAST(A.SGVL AS INT) AND
                        A.AMSY = '57' AND
                        A.AMRT = 'BR'
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U2
                    ON  U2.DRSY = A.AMSY AND
                        U2.DRRT = A.AMRT AND
                        TRIM(U2.DRKY) = TRIM(A.SGVL)
                WHERE 
	                A.ROWNUM = 1 
                ORDER BY 1";
            cmd.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID"].Precision = 15;
            cmd.Parameters["CFGID"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetArchivedItemAttributesById()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
	                ATNM, 
	                AMSY,
	                AMRT,
	                SGVL,
                    COALESCE(U1.DRDL01, U2.DRDL01) AS DL01,
                    COALESCE(U1.DRDL02, U2.DRDL02) AS DL02,
                    COALESCE(U1.DRSPHD, U2.DRSPHD) AS SPHD
                FROM (
	                SELECT
		                C.AMZ58ATNM AS ATNM,
		                C.AMSY,
		                C.AMRT,
		                A.KZSGVL AS SGVL,
		                ROW_NUMBER() OVER (PARTITION BY A.KZCFGID, A.KZPEL ORDER BY A.KZCFGCID ASC) AS ROWNUM
	                FROM
		                F5832119 A
		                INNER JOIN F580001 C
		                ON C.AMPEL = A.KZPEL  
	                WHERE
		                A.KZCFGID = ? AND
		                (A.KZSGVL != '' OR KZPEL IN (390,2070,3660)) AND
		                NOT (A.KZCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000'))
		                ) A
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U1
                    ON  U1.DRSY = A.AMSY AND
                        U1.DRRT = A.AMRT AND
                        CAST(U1.DRKY AS INT) = CAST(A.SGVL AS INT) AND
                        A.AMSY = '57' AND
                        A.AMRT = 'BR'
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U2
                    ON  U2.DRSY = A.AMSY AND
                        U2.DRRT = A.AMRT AND
                        U2.DRKY = A.SGVL
                WHERE 
	                A.ROWNUM = 1 
                ORDER BY 1";
            cmd.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["CFGID"].Precision = 15;
            cmd.Parameters["CFGID"].Scale = 0;
            return cmd;
        }
        /********************************************************************************
        * Methods
        *******************************************************************************/
        public ItemAttributes GetItemAttributes(BusinessSystemSession session, decimal itemNum = 0, decimal documentNum = 0, string documentType = null, decimal documentLine = 0, string companyCode = null, ItemAttributeCache cache = null)
        {
            var attributes = new ItemAttributes();
            try
            {
                var cmd = new iDB2Command();
                if (documentNum > 0)
                {
                    cmd = session.DbCommands["ItemAttributeManager.GetConfiguredItemAttributes"];
                    cmd.Parameters["CompanyNum"].Value = companyCode;
                    cmd.Parameters["OrderNum"].Value = documentNum;
                    cmd.Parameters["OrderType"].Value = documentType;
                    cmd.Parameters["OrderLineNum"].Value = documentLine;
                }
                else
                {
                    if (itemNum > 0)
                    {
                        cmd = session.DbCommands["ItemAttributeManager.GetDiscreteItemAttributes"];
                        cmd.Parameters[0].Value = itemNum;
                    }
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var attribute = new ItemAttribute
                        {
                            Name = reader["ATNM"].ToString().Trim(),
                            Value = reader["SGVL"].ToString().Trim(),
                            Desc1 = reader["DL01"].ToString().Trim(),
                            Desc2 = reader["DL02"].ToString().Trim(),
                            Desc3 = reader["SPHD"].ToString().Trim()
                        };
                        if (!attributes.Contains(attribute.Name))
                        {
                            attributes.Add(attribute);
                        }
                    }

                    reader.Close();
                }
                Logger.Debug("ROWS: " + attributes.Count);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return attributes;
        }

        public ItemAttributes GetDiscreteItemAttributes(BusinessSystemSession session, decimal itemNum)
        {
            var attributes = new ItemAttributes();
            try
            {
                var cmd = session.DbCommands["ItemAttributeManager.GetDiscreteItemAttributes"];
                cmd.Parameters["ItemNum"].Value = itemNum;
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var attribute = new ItemAttribute
                        {
                            Name = reader["ATNM"].ToString().Trim(),
                            Value = reader["SGVL"].ToString().Trim(),
                            Desc1 = reader["DL01"].ToString().Trim(),
                            Desc2 = reader["DL02"].ToString().Trim(),
                            Desc3 = reader["SPHD"].ToString().Trim()
                        };
                        attributes.Add(attribute);
                    }

                    reader.Close();
                }
                Logger.Info("ROWS: " + attributes.Count);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return attributes;
        }

        public ItemAttributes GetConfiguredItemAttributes(BusinessSystemSession session, decimal orderNum, string orderType, decimal orderLineNum, string companyCode)
        {
            var attributes = new ItemAttributes();
            try
            {
                var cmd = session.DbCommands["ItemAttributeManager.GetConfiguredItemAttributes"];
                cmd.Parameters["CompanyNum"].Value = companyCode;
                cmd.Parameters["OrderNum"].Value = orderNum;
                cmd.Parameters["OrderType"].Value = orderType;
                cmd.Parameters["OrderLineNum"].Value = orderLineNum;
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var attribute = new ItemAttribute
                        {
                            Name = reader["ATNM"].ToString().Trim(),
                            Value = reader["SGVL"].ToString().Trim(),
                            Desc1 = reader["DL01"].ToString().Trim(),
                            Desc2 = reader["DL02"].ToString().Trim(),
                            Desc3 = reader["SPHD"].ToString().Trim()
                        };
                        attributes.Add(attribute);
                    }

                    reader.Close();
                }
                Logger.Debug("ROWS: " + attributes.Count);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return attributes;
        }

        public ItemAttributes GetConfiguredItemAttributes(BusinessSystemSession session, decimal configId)
        {
            //Logger.Debug($"GetConfiguredItemAttributes ConfigId {configId}");
            var attributes = new ItemAttributes();
            try
            {
                var cmd = session.DbCommands["ItemAttributeManager.GetConfiguredItemAttributesById"];
                cmd.Parameters[0].Value = configId;
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var attribute = new ItemAttribute
                        {
                            Name = reader["ATNM"].ToString().Trim(),
                            Value = reader["SGVL"].ToString().Trim(),
                            Desc1 = reader["DL01"].ToString().Trim(),
                            Desc2 = reader["DL02"].ToString().Trim(),
                            Desc3 = reader["SPHD"].ToString().Trim()
                        };
                        attributes.Add(attribute);
                    }

                    reader.Close();
                }
                //Logger.Debug("ROWS: " + attributes.Count);

                if (attributes.Count == 0)
                {
                    //Logger.Debug($"GetArchivedItemAttributesById ConfigId {configId}");
                    cmd = session.DbCommands["ItemAttributeManager.GetArchivedItemAttributesById"];
                    cmd.Parameters[0].Value = configId;
                    cmd.Connection = session.DbConnection;
                    Logger.Debug(Logging.DbCmdString(cmd));
                    using (IDataReader reader1 = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader1.Read())
                        {
                            var attribute = new ItemAttribute
                            {
                                Name = reader1["ATNM"].ToString().Trim(),
                                Value = reader1["SGVL"].ToString().Trim(),
                                Desc1 = reader1["DL01"].ToString().Trim(),
                                Desc2 = reader1["DL02"].ToString().Trim(),
                                Desc3 = reader1["SPHD"].ToString().Trim()
                            };
                            attributes.Add(attribute);
                        }

                        reader1.Close();
                    }
                    //Logger.Debug("ROWS: " + attributes.Count);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return attributes;
        }

        /********************************************************************************
        * Config Id
        *******************************************************************************/
        public iDB2Command GetConfiguredIdByDocument()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
	                    MAX(CMCFGID) AS CMCFGID
                    FROM
                        F3201
                    WHERE
	                    CMKCOO = ? AND
                        CMDOCO = ? AND
                        CMDCTO = ? AND
                        CMLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetConfiguredId()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
	                    MAX(CMCFGID) AS CMCFGID
                    FROM
                        F3201
                    WHERE
                        CMDOCO > 0 AND
	                    ((CMKCOO = ? AND CMDOCO = ? AND CMDCTO = ? AND CMLNID = ?) OR
	                    (CMKCOO = ? AND CMDOCO = ? AND CMDCTO = ? AND CMLNID = ?) OR
	                    (CMKCOO = '*' AND CMDOCO = ? AND CMDCTO = 'WO'))"
            };
            cmd.Parameters.Add(new iDB2Parameter("KCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNID"].Precision = 6;
            cmd.Parameters["LNID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("PKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("DOC2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOC2"].Precision = 8;
            cmd.Parameters["DOC2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("XCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNIC", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNIC"].Precision = 6;
            cmd.Parameters["LNIC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DOC1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOC1"].Precision = 8;
            cmd.Parameters["DOC1"].Scale = 0;
            return cmd;
        }

        public decimal GetConfiguredId(BusinessSystemSession session, string kcoo, decimal doco, string dcto, decimal lnid, string pkco, decimal doc2, string xcto, decimal lnic, decimal doc1)
        {
            decimal val = 0;
            try
            {
                var cmd = session.DbCommands["ItemAttributeManager.GetConfiguredId"];
                cmd.Parameters[0].Value = kcoo;
                cmd.Parameters[1].Value = doco;
                cmd.Parameters[2].Value = dcto;
                cmd.Parameters[3].Value = lnid;
                cmd.Parameters[4].Value = pkco;
                cmd.Parameters[5].Value = doc2;
                cmd.Parameters[6].Value = xcto;
                cmd.Parameters[7].Value = lnic;
                cmd.Parameters[8].Value = doc1;
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                var i = 0;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        val = CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                        i++;
                    }

                    reader.Close();
                }
                Logger.Debug($"ROWS: {i}");
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            return val;
        }

    }
}
