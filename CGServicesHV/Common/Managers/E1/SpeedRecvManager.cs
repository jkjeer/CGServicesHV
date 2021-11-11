using System;
using System.Collections.Generic;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Response;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.E1
{
    public class SpeedRecvManager
    {
        public SpeedRecvManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpeedRecvManager.Insert", InsertCmd()}
            };

            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /******************************************************************
        * Commands
        ******************************************************************/
        public iDB2Command InsertCmd()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO F580074 (
                        SRZ58ASN,
                        SREDLN,
                        SREDSP,
                        SRDSE1,
                        SREDOC,
                        SREDCT,
                        SRMCU,
                        SRZ58RID,
                        SRZ58SN,
                        SRLOT1,
                        SRZ58RT,
                        SRITM,
                        SRLITM,
                        SRDSC1,
                        SRDSC2,
                        SRPQOH,
                        SRUOM,
                        SRDOCO,
                        SRDCTO,
                        SRKCOO,
                        SRSFXO,
                        SRLNID,
                        SRLOTN,
                        SRLOCN,
                        SRZ58SD,
                        SRZ58PS,
                        SRZ58STK,
                        SRZ58ROW,
                        SRZ58SL,
                        SRMMCU,
                        SRLODN,
                        SRBOLN,
                        SRRECDATE,
                        SRURCD,
                        SRURDT,
                        SRURAT,
                        SRURAB,
                        SRURRF,
                        SRUSER,
                        SRPID,
                        SRJOBN,
                        SRUPMJ,
                        SRUPMT)
                        VALUES( ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("SRZ58ASN", iDB2DbType.iDB2Graphic, 20));
            cmd.Parameters.Add(new iDB2Parameter("SREDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["SREDLN"].Precision = 7;
            cmd.Parameters["SREDLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SREDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SRDSE1", iDB2DbType.iDB2Graphic, 40));
            cmd.Parameters.Add(new iDB2Parameter("SREDOC", iDB2DbType.iDB2Decimal, 9));
            cmd.Parameters["SREDOC"].Precision = 9;
            cmd.Parameters["SREDOC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SREDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SRMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SRZ58RID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SRZ58RID"].Precision = 15;
            cmd.Parameters["SRZ58RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRZ58SN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SRLOT1", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SRZ58RT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SRITM", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SRITM"].Precision = 8;
            cmd.Parameters["SRITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRLITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SRDSC1", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SRDSC2", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SRPQOH", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SRPQOH"].Precision = 15;
            cmd.Parameters["SRPQOH"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRUOM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SRDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SRDOCO"].Precision = 8;
            cmd.Parameters["SRDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SRKCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SRSFXO", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("SRLNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SRLNID"].Precision = 6;
            cmd.Parameters["SRLNID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRLOTN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SRLOCN", iDB2DbType.iDB2Graphic, 20));
            cmd.Parameters.Add(new iDB2Parameter("SRZ58SD", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SRZ58PS", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["SRZ58PS"].Precision = 3;
            cmd.Parameters["SRZ58PS"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRZ58STK", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["SRZ58STK"].Precision = 3;
            cmd.Parameters["SRZ58STK"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRZ58ROW", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["SRZ58ROW"].Precision = 3;
            cmd.Parameters["SRZ58ROW"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRZ58SL", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["SRZ58SL"].Precision = 3;
            cmd.Parameters["SRZ58SL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRMMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SRLODN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["SRLODN"].Precision = 3;
            cmd.Parameters["SRLODN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRBOLN", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SRBOLN"].Precision = 8;
            cmd.Parameters["SRBOLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRRECDATE", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SRRECDATE"].Precision = 6;
            cmd.Parameters["SRRECDATE"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRURCD", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SRURDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SRURDT"].Precision = 6;
            cmd.Parameters["SRURDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRURAT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SRURAT"].Precision = 15;
            cmd.Parameters["SRURAT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRURAB", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SRURAB"].Precision = 8;
            cmd.Parameters["SRURAB"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRURRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SRUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SRPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SRJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SRUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SRUPMJ"].Precision = 6;
            cmd.Parameters["SRUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SRUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SRUPMT"].Precision = 6;
            cmd.Parameters["SRUPMT"].Scale = 0;
            return cmd;
        }

        public SqlDmlResponse Save(BusinessSystemSession e1Session, iDB2Transaction trans, SpeedRecvItems recs)
        {
            var response = new SqlDmlResponse();
            var cmd = (e1Session.DbCommands.ContainsKey("SpeedRecvManager.Insert")) ? e1Session.DbCommands["SpeedRecvManager.Insert"] : InsertCmd();
            cmd.Connection = e1Session.DbConnection;
            response.ConnectionString = cmd.Connection.ConnectionString;
            response.Statement = cmd.CommandText;
            response.HandlerClass = cmd.GetType().ToString();
            cmd.Transaction = trans;
            try
            {
                foreach (SpeedRecvItem rec in recs)
                {
                    cmd.Parameters[0].Value = rec.SRZ58ASN;
                    cmd.Parameters[1].Value = rec.SREDLN;
                    cmd.Parameters[2].Value = rec.SREDSP;
                    cmd.Parameters[3].Value = rec.SRDSE1;
                    cmd.Parameters[4].Value = rec.SREDOC;
                    cmd.Parameters[5].Value = rec.SREDCT;
                    cmd.Parameters[6].Value = rec.SRMCU;
                    cmd.Parameters[7].Value = rec.SRZ58RID;
                    cmd.Parameters[8].Value = rec.SRZ58SN;
                    cmd.Parameters[9].Value = rec.SRLOT1;
                    cmd.Parameters[10].Value = rec.SRZ58RT;
                    cmd.Parameters[11].Value = rec.SRITM;
                    cmd.Parameters[12].Value = rec.SRLITM;
                    cmd.Parameters[13].Value = rec.SRDSC1;
                    cmd.Parameters[14].Value = rec.SRDSC2;
                    cmd.Parameters[15].Value = rec.SRPQOH;
                    cmd.Parameters[16].Value = rec.SRUOM;
                    cmd.Parameters[17].Value = rec.SRDOCO;
                    cmd.Parameters[18].Value = rec.SRDCTO;
                    cmd.Parameters[19].Value = rec.SRKCOO;
                    cmd.Parameters[20].Value = rec.SRSFXO;
                    cmd.Parameters[21].Value = rec.SRLNID;
                    cmd.Parameters[22].Value = rec.SRLOTN;
                    cmd.Parameters[23].Value = rec.SRLOCN;
                    cmd.Parameters[24].Value = rec.SRZ58SD;
                    cmd.Parameters[25].Value = rec.SRZ58PS;
                    cmd.Parameters[26].Value = rec.SRZ58STK;
                    cmd.Parameters[27].Value = rec.SRZ58ROW;
                    cmd.Parameters[28].Value = rec.SRZ58SL;
                    cmd.Parameters[29].Value = rec.SRMMCU;
                    cmd.Parameters[30].Value = rec.SRLODN;
                    cmd.Parameters[31].Value = rec.SRBOLN;
                    cmd.Parameters[32].Value = rec.SRRECDATE;
                    cmd.Parameters[33].Value = rec.SRURCD;
                    cmd.Parameters[34].Value = rec.SRURDT;
                    cmd.Parameters[35].Value = rec.SRURAT;
                    cmd.Parameters[36].Value = rec.SRURAB;
                    cmd.Parameters[37].Value = rec.SRURRF;
                    cmd.Parameters[38].Value = rec.SRUSER;
                    cmd.Parameters[39].Value = rec.SRPID;
                    cmd.Parameters[40].Value = rec.SRJOBN;
                    cmd.Parameters[41].Value = rec.SRUPMJ;
                    cmd.Parameters[42].Value = rec.SRUPMT;
                    int rows = cmd.ExecuteNonQuery();
                }
                response.RowsAffected = recs.Count;
            }
            catch (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = "F580074: " + e.ToString();
            }
            finally
            {
                response.EndTime = DateTime.Now;
            }
            return response;
        }
    }
}
