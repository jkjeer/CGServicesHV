using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class F580200Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F580200Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F580200Manager.Insert", Insert()},
                {"F580200Manager.Exists", Exists()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        public iDB2Command Insert()
        {
            var cmd = new iDB2Command
            {
            CommandType = CommandType.Text,
            CommandText = @"
                INSERT INTO F580200 (
                    BIEDUS,
                    BIEDBT,
                    BIEDTN,
                    BIEDLN,
                    BIEDDT,
                    BIEDSP,
                    BIDOCO,
                    BIDCTO,
                    BILNID,
                    BIZ58UNID,
                    BIZ58BXCR,
                    BIZ58SNUM,
                    BIZ58RT,
                    BIZ58BOX,
                    BIZ58BXRO,
                    BIZ58SD,
                    BIZ58STK,
                    BIZ58ROW,
                    BIZ58SL,
                    BIZ58IGLN,
                    BIZ58SCN,
                    BIZ58RNSQ,
                    BIZ58ONSK,
                    BIZ58L1R,
                    BIZ58L1SLN,
                    BIZ58L2R,
                    BIZ58L2SLN,
                    BIZ58L3SLN,
                    BIZ58L3R,
                    BIURCD,
                    BIURDT,
                    BIURAT,
                    BIURAB,
                    BIURRF,
                    BIUSER,
                    BIPID,
                    BIJOBN,
                    BIUPMJ,
                    BITDAY,
                    BIZ58RID,
                    BIZ58SCHDJ,
                    BIMCU,
                    BIKCOO,
                    BIAN8,
                    BISHAN,
                    BIITM,
                    BILITM,
                    BIAITM,
                    BIRKCO,
                    BITRDJ,
                    BIRORN,
                    BIRCTO,
                    BIRLLN,
                    BIITWT,
                    BIWTUM,
                    BITRQTY,
                    BIUM,
                    BICFGID,
                    BIUKID)
                    VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                            ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                            ?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("BIEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("BIEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("BIEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["BIEDLN"].Precision = 7;
            cmd.Parameters["BIEDLN"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIEDDT"].Precision = 6;
            cmd.Parameters["BIEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIDOCO"].Precision = 8;
            cmd.Parameters["BIDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BILNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BILNID"].Precision = 6;
            cmd.Parameters["BILNID"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58UNID", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58BXCR", iDB2DbType.iDB2Decimal, 1));
            cmd.Parameters["BIZ58BXCR"].Precision = 1;
            cmd.Parameters["BIZ58BXCR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SNUM", iDB2DbType.iDB2Graphic, 14));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58RT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58BOX", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["BIZ58BOX"].Precision = 5;
            cmd.Parameters["BIZ58BOX"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58BXRO", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SD", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58STK", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58STK"].Precision = 3;
            cmd.Parameters["BIZ58STK"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58ROW", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58ROW"].Precision = 3;
            cmd.Parameters["BIZ58ROW"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SL", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58SL"].Precision = 3;
            cmd.Parameters["BIZ58SL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58IGLN", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["BIZ58SCN"].Precision = 5;
            cmd.Parameters["BIZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58RNSQ", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["BIZ58RNSQ"].Precision = 5;
            cmd.Parameters["BIZ58RNSQ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58ONSK", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L1R", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L1SLN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58L1SLN"].Precision = 3;
            cmd.Parameters["BIZ58L1SLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L2R", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L2SLN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58L2SLN"].Precision = 3;
            cmd.Parameters["BIZ58L2SLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L3SLN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58L3SLN"].Precision = 3;
            cmd.Parameters["BIZ58L3SLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L3R", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIURCD", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIURDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIURDT"].Precision = 6;
            cmd.Parameters["BIURDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIURAT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIURAT"].Precision = 15;
            cmd.Parameters["BIURAT"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("BIURAB", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIURAB"].Precision = 8;
            cmd.Parameters["BIURAB"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIURRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("BIUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIUPMJ"].Precision = 6;
            cmd.Parameters["BIUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BITDAY", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BITDAY"].Precision = 6;
            cmd.Parameters["BITDAY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58RID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIZ58RID"].Precision = 15;
            cmd.Parameters["BIZ58RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SCHDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIZ58SCHDJ"].Precision = 6;
            cmd.Parameters["BIZ58SCHDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("BIKCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("BIAN8", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIAN8"].Precision = 8;
            cmd.Parameters["BIAN8"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BISHAN", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BISHAN"].Precision = 8;
            cmd.Parameters["BISHAN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIITM", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIITM"].Precision = 8;
            cmd.Parameters["BIITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BILITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("BIAITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("BIRKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("BITRDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BITRDJ"].Precision = 6;
            cmd.Parameters["BITRDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIRORN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("BIRCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIRLLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["BIRLLN"].Precision = 7;
            cmd.Parameters["BIRLLN"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIITWT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIITWT"].Precision = 15;
            cmd.Parameters["BIITWT"].Scale = 4;
            cmd.Parameters.Add(new iDB2Parameter("BIWTUM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BITRQTY", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BITRQTY"].Precision = 15;
            cmd.Parameters["BITRQTY"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIUM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BICFGID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BICFGID"].Precision = 15;
            cmd.Parameters["BICFGID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIUKID"].Precision = 15;
            cmd.Parameters["BIUKID"].Scale = 0;
            return cmd;
        }

        /*********************************************************************************
         * SAVE COLLECTION
         ********************************************************************************/
        public SqlDmlResponses Save(BusinessSystemSession session, iDB2Transaction txn, F580200s recs)
        {
            var responses = new SqlDmlResponses();
            var cmd = session.DbCommands["F580200Manager.Insert"];
            cmd.Connection = session.DbConnection;
            cmd.Transaction = txn;
            try
            {
                foreach (F580200 rec in recs)
                {
                    var response = new SqlDmlResponse();
                    try
                    {
                        cmd.Parameters["BIEDUS"].Value = rec.BIEDUS;
                        cmd.Parameters["BIEDBT"].Value = rec.BIEDBT;
                        cmd.Parameters["BIEDTN"].Value = rec.BIEDTN;
                        cmd.Parameters["BIEDLN"].Value = rec.BIEDLN;
                        cmd.Parameters["BIEDDT"].Value = rec.BIEDDT;
                        cmd.Parameters["BIEDSP"].Value = rec.BIEDSP;
                        cmd.Parameters["BIDOCO"].Value = rec.BIDOCO;
                        cmd.Parameters["BIDCTO"].Value = rec.BIDCTO;
                        cmd.Parameters["BILNID"].Value = rec.BILNID;
                        cmd.Parameters["BIZ58UNID"].Value = rec.BIZ58UNID;
                        cmd.Parameters["BIZ58BXCR"].Value = rec.BIZ58BXCR;
                        cmd.Parameters["BIZ58SNUM"].Value = rec.BIZ58SNUM;
                        cmd.Parameters["BIZ58RT"].Value = rec.BIZ58RT;
                        cmd.Parameters["BIZ58BOX"].Value = rec.BIZ58BOX;
                        cmd.Parameters["BIZ58BXRO"].Value = rec.BIZ58BXRO;
                        cmd.Parameters["BIZ58SD"].Value = rec.BIZ58SD;
                        cmd.Parameters["BIZ58STK"].Value = rec.BIZ58STK;
                        cmd.Parameters["BIZ58ROW"].Value = rec.BIZ58ROW;
                        cmd.Parameters["BIZ58SL"].Value = rec.BIZ58SL;
                        cmd.Parameters["BIZ58IGLN"].Value = rec.BIZ58IGLN;
                        cmd.Parameters["BIZ58SCN"].Value = rec.BIZ58SCN;
                        cmd.Parameters["BIZ58RNSQ"].Value = rec.BIZ58RNSQ;
                        cmd.Parameters["BIZ58ONSK"].Value = rec.BIZ58ONSK;
                        cmd.Parameters["BIZ58L1R"].Value = rec.BIZ58L1R;
                        cmd.Parameters["BIZ58L1SLN"].Value = rec.BIZ58L1SLN;
                        cmd.Parameters["BIZ58L2R"].Value = rec.BIZ58L2R;
                        cmd.Parameters["BIZ58L2SLN"].Value = rec.BIZ58L2SLN;
                        cmd.Parameters["BIZ58L3SLN"].Value = rec.BIZ58L3SLN;
                        cmd.Parameters["BIZ58L3R"].Value = rec.BIZ58L3R;
                        cmd.Parameters["BIURCD"].Value = rec.BIURCD;
                        cmd.Parameters["BIURDT"].Value = rec.BIURDT;
                        cmd.Parameters["BIURAT"].Value = rec.BIURAT;
                        cmd.Parameters["BIURAB"].Value = rec.BIURAB;
                        cmd.Parameters["BIURRF"].Value = rec.BIURRF;
                        cmd.Parameters["BIUSER"].Value = rec.BIUSER;
                        cmd.Parameters["BIPID"].Value = rec.BIPID;
                        cmd.Parameters["BIJOBN"].Value = rec.BIJOBN;
                        cmd.Parameters["BIUPMJ"].Value = rec.BIUPMJ;
                        cmd.Parameters["BITDAY"].Value = rec.BITDAY;
                        cmd.Parameters["BIZ58RID"].Value = rec.BIZ58RID;
                        cmd.Parameters["BIZ58SCHDJ"].Value = rec.BIZ58SCHDJ;
                        cmd.Parameters["BIMCU"].Value = rec.BIMCU;
                        cmd.Parameters["BIKCOO"].Value = rec.BIKCOO;
                        cmd.Parameters["BIAN8"].Value = rec.BIAN8;
                        cmd.Parameters["BISHAN"].Value = rec.BISHAN;
                        cmd.Parameters["BIITM"].Value = rec.BIITM;
                        cmd.Parameters["BILITM"].Value = rec.BILITM;
                        cmd.Parameters["BIAITM"].Value = rec.BIAITM;
                        cmd.Parameters["BIRKCO"].Value = rec.BIRKCO;
                        cmd.Parameters["BITRDJ"].Value = rec.BITRDJ;
                        cmd.Parameters["BIRORN"].Value = rec.BIRORN;
                        cmd.Parameters["BIRCTO"].Value = rec.BIRCTO;
                        cmd.Parameters["BIRLLN"].Value = rec.BIRLLN;
                        cmd.Parameters["BIITWT"].Value = rec.BIITWT;
                        cmd.Parameters["BIWTUM"].Value = rec.BIWTUM;
                        cmd.Parameters["BITRQTY"].Value = rec.BITRQTY;
                        cmd.Parameters["BIUM"].Value = rec.BIUM;
                        cmd.Parameters["BICFGID"].Value = rec.BICFGID;
                        cmd.Parameters["BIUKID"].Value = rec.BIUKID;
                        response.Init(cmd);
                        int rows = cmd.ExecuteNonQuery();
                        response.RowsAffected = rows;
                        response.EndTime = DateTime.Now.ToLocalTime();
                        responses.Add(response);
                    }
                    catch (Exception e)
                    {
                        response.Errors.Add(new Error(e));
                        throw new Exception($"BIUKID: {rec.BIUKID} TRACE: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return responses;
        }

        /*********************************************************************************
         * EXISTS
         ********************************************************************************/
        public iDB2Command Exists()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                COUNT(*) AS RECS 
            FROM 
                F580200
            WHERE
                BIMCU = ? AND
                BIDOCO = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            return cmd;
        }

        public bool Exists(BusinessSystemSession session, Plant plant, decimal workOrderNum)
        {
            bool val = false;
            try
            {
                var cmd = session.DbCommands["F580200Manager.Exists"];
                cmd.Connection = session.DbConnection;
                cmd.Parameters["MCU"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["DOCO"].Value = workOrderNum;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        if (CastDbNull.To(reader["RECS"], 0) > 0)
                        {
                            val = true;
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return val;
        }

    }
}
