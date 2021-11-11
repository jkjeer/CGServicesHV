using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;
// ReSharper disable InconsistentNaming


namespace CGServicesHV.Common.Managers.E1
{
    public class F580030Manager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F580030Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F580030Manager.Insert", Insert()},
                {"F580030Manager.Exists", Exists()},
                {"F580030Manager.UKIDExists", UKIDExists()},
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * INSERT STATEMENT
         ********************************************************************************/
        public iDB2Command Insert()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                INSERT INTO F580030 (
                SFUKID,
                SFEDUS,
                SFEDBT,
                SFEDTN,
                SFEDLN,
                SFEDCT,
                SFTYTN,
                SFEDFT,
                SFEDDT,
                SFDRIN,
                SFEDDL,
                SFEDSP,
                SFPNID,
                SFTNAC,
                SFMMCU,
                SFACDT,
                SFSTTM,
                SFDTMS,
                SFLITM,
                SFLOTN,
                SFZ58SN,
                SFDOCO,
                SFDCTO,
                SFMCU,
                SFZ58SCN,
                SFZ58SCSN,
                SFANP,
                SFANPA,
                SFSFT,
                SFZ58PCR,
                SFZ58RCD,
                SFUORG,
                SFSOCN,
                SFLOCN,
                SFZ58RID,
                SFZ58RT,
                SFZ58GWT,
                SFZ58GWTT,
                SFZ58IGLN,
                SFZ58UNID,
                SFZ58UID,
                SFTKNR,
                SFZ58PT,
                SFZ58L1CLN,
                SFZ58BSF,
                SFZ5850000,
                SFLOCATION,
                SFZ58LDOCO,
                SFZ58LNID,
                SFZ58FLAG,
                SFZ58LWSTN,
                SFJFLG,
                SFFLAG,
                SFUSER,
                SFUPMJ,
                SFUPMT,
                SFPID,
                SFZ58RCP,
                SFZ58RCC,
                SFZ58IP01,
                SFZ58GT,
                SFZ58HST,
                SFZ58AID)
                VALUES(
                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                ?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("SFUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFUKID"].Precision = 15;
            cmd.Parameters["SFUKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("SFEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["SFEDLN"].Precision = 7;
            cmd.Parameters["SFEDLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFTYTN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("SFEDFT", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFEDDT"].Precision = 6;
            cmd.Parameters["SFEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFDRIN", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFEDDL", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFEDDL"].Precision = 5;
            cmd.Parameters["SFEDDL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFPNID", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFTNAC", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFMMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SFACDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFACDT"].Precision = 6;
            cmd.Parameters["SFACDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFSTTM", iDB2DbType.iDB2Graphic, 24));
            cmd.Parameters.Add(new iDB2Parameter("SFDTMS", iDB2DbType.iDB2Graphic, 24));
            cmd.Parameters.Add(new iDB2Parameter("SFLITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SFLOTN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SFDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFDOCO"].Precision = 8;
            cmd.Parameters["SFDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFZ58SCN"].Precision = 5;
            cmd.Parameters["SFZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SCSN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFZ58SCSN"].Precision = 5;
            cmd.Parameters["SFZ58SCSN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFANP", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFANP"].Precision = 8;
            cmd.Parameters["SFANP"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFANPA", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFANPA"].Precision = 8;
            cmd.Parameters["SFANPA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFSFT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58PCR", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RCD", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFUORG", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFUORG"].Precision = 15;
            cmd.Parameters["SFUORG"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("SFSOCN", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFSOCN"].Precision = 15;
            cmd.Parameters["SFSOCN"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("SFLOCN", iDB2DbType.iDB2Graphic, 20));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58RID"].Precision = 15;
            cmd.Parameters["SFZ58RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58GWT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58GWT"].Precision = 15;
            cmd.Parameters["SFZ58GWT"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58GWTT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58GWTT"].Precision = 15;
            cmd.Parameters["SFZ58GWTT"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58IGLN", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58UNID", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58UID", iDB2DbType.iDB2Graphic, 14));
            cmd.Parameters.Add(new iDB2Parameter("SFTKNR", iDB2DbType.iDB2Graphic, 20));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58PT", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58L1CLN", iDB2DbType.iDB2Decimal, 1));
            cmd.Parameters["SFZ58L1CLN"].Precision = 1;
            cmd.Parameters["SFZ58L1CLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58BSF", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58BSF"].Precision = 15;
            cmd.Parameters["SFZ58BSF"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ5850000", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["SFZ5850000"].Precision = 7;
            cmd.Parameters["SFZ5850000"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFLOCATION", iDB2DbType.iDB2Graphic, 51));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58LDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFZ58LDOCO"].Precision = 8;
            cmd.Parameters["SFZ58LDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58LNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFZ58LNID"].Precision = 6;
            cmd.Parameters["SFZ58LNID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58FLAG", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58LWSTN", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SFJFLG", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFFLAG", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFUPMJ"].Precision = 6;
            cmd.Parameters["SFUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFUPMT"].Precision = 6;
            cmd.Parameters["SFUPMT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RCP", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RCC", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58IP01", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58GT", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58HST", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58AID", iDB2DbType.iDB2Graphic, 15));
            return cmd;
        }

        /*********************************************************************************
         * SAVE RECORD
         ********************************************************************************/
        public SqlDmlResponse Save(BusinessSystemSession session, iDB2Transaction txn, F580030 rec)
        {
            var response = new SqlDmlResponse();
            var cmd = (session.DbCommands.ContainsKey("F580030Manager.Insert"))
                ? session.DbCommands["F580030Manager.Insert"]
                : Insert();
            cmd.Connection = session.DbConnection;
            if (txn != null)
            {
                cmd.Transaction = txn;
            }
            try
            {
                cmd.Parameters["SFUKID"].Value = rec.SFUKID;
                cmd.Parameters["SFEDUS"].Value = rec.SFEDUS;
                cmd.Parameters["SFEDBT"].Value = rec.SFEDBT;
                cmd.Parameters["SFEDTN"].Value = rec.SFEDTN;
                cmd.Parameters["SFEDLN"].Value = rec.SFEDLN;
                cmd.Parameters["SFEDCT"].Value = rec.SFEDCT;
                cmd.Parameters["SFTYTN"].Value = rec.SFTYTN;
                cmd.Parameters["SFEDFT"].Value = rec.SFEDFT;
                cmd.Parameters["SFEDDT"].Value = rec.SFEDDT;
                cmd.Parameters["SFDRIN"].Value = rec.SFDRIN;
                cmd.Parameters["SFEDDL"].Value = rec.SFEDDL;
                cmd.Parameters["SFEDSP"].Value = rec.SFEDSP;
                cmd.Parameters["SFPNID"].Value = rec.SFPNID;
                cmd.Parameters["SFTNAC"].Value = rec.SFTNAC;
                cmd.Parameters["SFMMCU"].Value = rec.SFMMCU;
                cmd.Parameters["SFACDT"].Value = rec.SFACDT;
                cmd.Parameters["SFSTTM"].Value = rec.SFSTTM;
                cmd.Parameters["SFDTMS"].Value = rec.SFDTMS;
                cmd.Parameters["SFLITM"].Value = rec.SFLITM;
                cmd.Parameters["SFLOTN"].Value = rec.SFLOTN;
                cmd.Parameters["SFZ58SN"].Value = rec.SFZ58SN;
                cmd.Parameters["SFDOCO"].Value = rec.SFDOCO;
                cmd.Parameters["SFDCTO"].Value = rec.SFDCTO;
                cmd.Parameters["SFMCU"].Value = rec.SFMCU;
                cmd.Parameters["SFZ58SCN"].Value = rec.SFZ58SCN;
                cmd.Parameters["SFZ58SCSN"].Value = rec.SFZ58SCSN;
                cmd.Parameters["SFANP"].Value = rec.SFANP;
                cmd.Parameters["SFANPA"].Value = rec.SFANPA;
                cmd.Parameters["SFSFT"].Value = rec.SFSFT;
                cmd.Parameters["SFZ58PCR"].Value = rec.SFZ58PCR;
                cmd.Parameters["SFZ58RCD"].Value = rec.SFZ58RCD;
                cmd.Parameters["SFUORG"].Value = rec.SFUORG;
                cmd.Parameters["SFSOCN"].Value = rec.SFSOCN;
                cmd.Parameters["SFLOCN"].Value = rec.SFLOCN;
                cmd.Parameters["SFZ58RID"].Value = rec.SFZ58RID;
                cmd.Parameters["SFZ58RT"].Value = rec.SFZ58RT;
                cmd.Parameters["SFZ58GWT"].Value = rec.SFZ58GWT;
                cmd.Parameters["SFZ58GWTT"].Value = rec.SFZ58GWTT;
                cmd.Parameters["SFZ58IGLN"].Value = rec.SFZ58IGLN;
                cmd.Parameters["SFZ58UNID"].Value = rec.SFZ58UNID;
                cmd.Parameters["SFZ58UID"].Value = rec.SFZ58UID;
                cmd.Parameters["SFTKNR"].Value = rec.SFTKNR;
                cmd.Parameters["SFZ58PT"].Value = rec.SFZ58PT;
                cmd.Parameters["SFZ58L1CLN"].Value = rec.SFZ58L1CLN;
                cmd.Parameters["SFZ58BSF"].Value = rec.SFZ58BSF;
                cmd.Parameters["SFZ5850000"].Value = rec.SFZ5850000;
                cmd.Parameters["SFLOCATION"].Value = rec.SFLOCATION;
                cmd.Parameters["SFZ58LDOCO"].Value = rec.SFZ58LDOCO;
                cmd.Parameters["SFZ58LNID"].Value = rec.SFZ58LNID;
                cmd.Parameters["SFZ58FLAG"].Value = rec.SFZ58FLAG;
                cmd.Parameters["SFZ58LWSTN"].Value = rec.SFZ58LWSTN;
                cmd.Parameters["SFJFLG"].Value = rec.SFJFLG;
                cmd.Parameters["SFFLAG"].Value = rec.SFFLAG;
                cmd.Parameters["SFUSER"].Value = rec.SFUSER;
                cmd.Parameters["SFUPMJ"].Value = rec.SFUPMJ;
                cmd.Parameters["SFUPMT"].Value = rec.SFUPMT;
                cmd.Parameters["SFPID"].Value = rec.SFPID;
                cmd.Parameters["SFZ58RCP"].Value = rec.SFZ58RCP;
                cmd.Parameters["SFZ58RCC"].Value = rec.SFZ58RCC;
                cmd.Parameters["SFZ58IP01"].Value = rec.SFZ58IP01;
                cmd.Parameters["SFZ58GT"].Value = rec.SFZ58GT;
                cmd.Parameters["SFZ58HST"].Value = rec.SFZ58HST;
                cmd.Parameters["SFZ58AID"].Value = rec.SFZ58AID;
                response = new SqlDmlResponse();
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                int rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                Logger.Info($"Rows: {rows}");
                response.EndTime = DateTime.Now.ToLocalTime();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
                response.Message = e.Message;
            }
            return response;
        }

        /*********************************************************************************
         * SAVE COLLECTION
         ********************************************************************************/
        public SqlDmlResponses Save(BusinessSystemSession session, iDB2Transaction txn, F580030s recs)
        {
            var responses = new SqlDmlResponses();
            var cmd = session.DbCommands["F580030Manager.Insert"];
            cmd.Connection = session.DbConnection;
            cmd.Transaction = txn;
            try
            {
                foreach (F580030 rec in recs)
                {
                    var response = new SqlDmlResponse();
                    try
                    {
                        cmd.Parameters["SFUKID"].Value = rec.SFUKID;
                        cmd.Parameters["SFEDUS"].Value = rec.SFEDUS;
                        cmd.Parameters["SFEDBT"].Value = rec.SFEDBT;
                        cmd.Parameters["SFEDTN"].Value = rec.SFEDTN;
                        cmd.Parameters["SFEDLN"].Value = rec.SFEDLN;
                        cmd.Parameters["SFEDCT"].Value = rec.SFEDCT;
                        cmd.Parameters["SFTYTN"].Value = rec.SFTYTN;
                        cmd.Parameters["SFEDFT"].Value = rec.SFEDFT;
                        cmd.Parameters["SFEDDT"].Value = rec.SFEDDT;
                        cmd.Parameters["SFDRIN"].Value = rec.SFDRIN;
                        cmd.Parameters["SFEDDL"].Value = rec.SFEDDL;
                        cmd.Parameters["SFEDSP"].Value = rec.SFEDSP;
                        cmd.Parameters["SFPNID"].Value = rec.SFPNID;
                        cmd.Parameters["SFTNAC"].Value = rec.SFTNAC;
                        cmd.Parameters["SFMMCU"].Value = rec.SFMMCU;
                        cmd.Parameters["SFACDT"].Value = rec.SFACDT;
                        cmd.Parameters["SFSTTM"].Value = rec.SFSTTM;
                        cmd.Parameters["SFDTMS"].Value = rec.SFDTMS;
                        cmd.Parameters["SFLITM"].Value = rec.SFLITM;
                        cmd.Parameters["SFLOTN"].Value = rec.SFLOTN;
                        cmd.Parameters["SFZ58SN"].Value = rec.SFZ58SN;
                        cmd.Parameters["SFDOCO"].Value = rec.SFDOCO;
                        cmd.Parameters["SFDCTO"].Value = rec.SFDCTO;
                        cmd.Parameters["SFMCU"].Value = rec.SFMCU;
                        cmd.Parameters["SFZ58SCN"].Value = rec.SFZ58SCN;
                        cmd.Parameters["SFZ58SCSN"].Value = rec.SFZ58SCSN;
                        cmd.Parameters["SFANP"].Value = rec.SFANP;
                        cmd.Parameters["SFANPA"].Value = rec.SFANPA;
                        cmd.Parameters["SFSFT"].Value = rec.SFSFT;
                        cmd.Parameters["SFZ58PCR"].Value = rec.SFZ58PCR;
                        cmd.Parameters["SFZ58RCD"].Value = rec.SFZ58RCD;
                        cmd.Parameters["SFUORG"].Value = rec.SFUORG;
                        cmd.Parameters["SFSOCN"].Value = rec.SFSOCN;
                        cmd.Parameters["SFLOCN"].Value = rec.SFLOCN;
                        cmd.Parameters["SFZ58RID"].Value = rec.SFZ58RID;
                        cmd.Parameters["SFZ58RT"].Value = rec.SFZ58RT;
                        cmd.Parameters["SFZ58GWT"].Value = rec.SFZ58GWT;
                        cmd.Parameters["SFZ58GWTT"].Value = rec.SFZ58GWTT;
                        cmd.Parameters["SFZ58IGLN"].Value = rec.SFZ58IGLN;
                        cmd.Parameters["SFZ58UNID"].Value = rec.SFZ58UNID;
                        cmd.Parameters["SFZ58UID"].Value = rec.SFZ58UID;
                        cmd.Parameters["SFTKNR"].Value = rec.SFTKNR;
                        cmd.Parameters["SFZ58PT"].Value = rec.SFZ58PT;
                        cmd.Parameters["SFZ58L1CLN"].Value = rec.SFZ58L1CLN;
                        cmd.Parameters["SFZ58BSF"].Value = rec.SFZ58BSF;
                        cmd.Parameters["SFZ5850000"].Value = rec.SFZ5850000;
                        cmd.Parameters["SFLOCATION"].Value = rec.SFLOCATION;
                        cmd.Parameters["SFZ58LDOCO"].Value = rec.SFZ58LDOCO;
                        cmd.Parameters["SFZ58LNID"].Value = rec.SFZ58LNID;
                        cmd.Parameters["SFZ58FLAG"].Value = rec.SFZ58FLAG;
                        cmd.Parameters["SFZ58LWSTN"].Value = rec.SFZ58LWSTN;
                        cmd.Parameters["SFJFLG"].Value = rec.SFJFLG;
                        cmd.Parameters["SFFLAG"].Value = rec.SFFLAG;
                        cmd.Parameters["SFUSER"].Value = rec.SFUSER;
                        cmd.Parameters["SFUPMJ"].Value = rec.SFUPMJ;
                        cmd.Parameters["SFUPMT"].Value = rec.SFUPMT;
                        cmd.Parameters["SFPID"].Value = rec.SFPID;
                        cmd.Parameters["SFZ58RCP"].Value = rec.SFZ58RCP;
                        cmd.Parameters["SFZ58RCC"].Value = rec.SFZ58RCC;
                        cmd.Parameters["SFZ58IP01"].Value = rec.SFZ58IP01;
                        cmd.Parameters["SFZ58GT"].Value = rec.SFZ58GT;
                        cmd.Parameters["SFZ58HST"].Value = rec.SFZ58HST;
                        cmd.Parameters["SFZ58AID"].Value = rec.SFZ58AID;
                        response.Init(cmd);
                        Logger.Info(Logging.DbCmdString(cmd));
                        int rows = cmd.ExecuteNonQuery();
                        response.RowsAffected = rows;
                        response.EndTime = DateTime.Now.ToLocalTime();
                        responses.Add(response);
                        Logger.Info($"Rows: {rows}");
                    }
                    catch (Exception e)
                    {
                        throw new Exception($@"Plant: {rec.SFMMCU} SFZ58UID: {rec.SFZ58UID} SFEDCT: {rec.SFEDCT} WO: {rec.SFDOCO} SER: {rec.SFLOTN} SFUKID: {rec.SFUKID}  {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($@"Plant: {recs.First().SFMMCU}  SFZ58UID: {recs.First().SFZ58UID} WO: {recs.First().SFDOCO}  SER: {recs.First().SFLOTN}  SFUKID: {recs.First().SFUKID}  {e.Message}");
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
                COUNT(*) RECS 
            FROM 
                F580030
            WHERE
                SFMMCU = ? AND
                SFZ58UID = ? AND
                SFEDCT = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("UID", iDB2DbType.iDB2Graphic, 14));
            cmd.Parameters.Add(new iDB2Parameter("EDCT", iDB2DbType.iDB2Graphic, 2));
            return cmd;
        }

        public bool Exists(BusinessSystemSession session, string mcu, string uid, string edct)
        {
            bool val = false;
            try
            {
                var cmd = session.DbCommands["F580030Manager.Exists"];
                cmd.Connection = session.DbConnection;
                cmd.Parameters["MCU"].Value = mcu.PadLeft(12);
                cmd.Parameters["UID"].Value = uid;
                cmd.Parameters["EDCT"].Value = edct;
                Logger.Info(Logging.DbCmdString(cmd));
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
                throw new FaultException(e.Message);
            }

            return val;
        }

        public iDB2Command UKIDExists()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                COUNT(*) RECS 
            FROM 
                F580030
            WHERE
                SFUKID = ?";
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;
            return cmd;
        }

        public bool UKIDExists(BusinessSystemSession session, decimal ukid)
        {
            bool val = false;
            try
            {
                var cmd = session.DbCommands["F580030Manager.UKIDExists"];
                cmd.Connection = session.DbConnection;
                cmd.Parameters["UKID"].Value = ukid;
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

        public static SqlDmlResponse InsertScheduleRebox(BusinessSystemSession session, string plantNum, decimal scheduleNum, long sfukid, long sfedbt, decimal scheduleDateE1)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = InsertScheduleBackoutCmd();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["SFUKID"].Value = sfukid;
                cmd.Parameters["SFEDUS"].Value = "SVCSOA";
                cmd.Parameters["SFEDBT"].Value = sfedbt.ToString();
                cmd.Parameters["SFEDTN"].Value = "1".PadLeft(10, '0').PadRight(22);  //"";
                cmd.Parameters["SFEDLN"].Value = 1000;
                cmd.Parameters["SFEDCT"].Value = "BD";
                cmd.Parameters["SFTYTN"].Value = "CTTO";
                cmd.Parameters["SFEDFT"].Value = "CTBD";
                cmd.Parameters["SFEDDT"].Value = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters["SFDRIN"].Value = "1";
                cmd.Parameters["SFEDDL"].Value = 0;
                cmd.Parameters["SFEDSP"].Value = "";
                cmd.Parameters["SFPNID"].Value = "Rebox";
                cmd.Parameters["SFTNAC"].Value = "01";
                cmd.Parameters["SFMMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["SFACDT"].Value = scheduleDateE1;
                cmd.Parameters["SFSTTM"].Value = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                cmd.Parameters["SFDTMS"].Value = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                cmd.Parameters["SFZ58SCN"].Value = scheduleNum;
                cmd.Parameters["SFUSER"].Value = "SVCSOA";
                cmd.Parameters["SFUPMJ"].Value = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters["SFUPMT"].Value = Utilities.Common.CastToE1Time(DateTime.Now);
                response = new SqlDmlResponse();
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                int rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
                response.RowsAffected = rowCount;
                response.EndTime = DateTime.Now.ToLocalTime();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
                response.Message = e.Message;
            }
            return response;
        }

        public static SqlDmlResponse InsertScheduleBackout(BusinessSystemSession session, string plantNum, decimal scheduleNum, long sfukid, long sfedbt, decimal scheduleDateE1)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = InsertScheduleBackoutCmd();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["SFUKID"].Value = sfukid;
                cmd.Parameters["SFEDUS"].Value = "SVCSOA";
                cmd.Parameters["SFEDBT"].Value = sfedbt.ToString();
                cmd.Parameters["SFEDTN"].Value = "1".PadLeft(10, '0').PadRight(22);  //"";
                cmd.Parameters["SFEDLN"].Value = 1000;
                cmd.Parameters["SFEDCT"].Value = "BX";
                cmd.Parameters["SFTYTN"].Value = "CGMTO";
                cmd.Parameters["SFEDFT"].Value = "CGBX";
                cmd.Parameters["SFEDDT"].Value = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters["SFDRIN"].Value = "1";
                cmd.Parameters["SFEDDL"].Value = 0;
                cmd.Parameters["SFEDSP"].Value = "";
                cmd.Parameters["SFPNID"].Value = "Backout";
                cmd.Parameters["SFTNAC"].Value = "01";
                cmd.Parameters["SFMMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["SFACDT"].Value = scheduleDateE1;
                cmd.Parameters["SFSTTM"].Value = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                cmd.Parameters["SFDTMS"].Value = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                cmd.Parameters["SFZ58SCN"].Value = scheduleNum;
                cmd.Parameters["SFUSER"].Value = "SVCSOA";
                cmd.Parameters["SFUPMJ"].Value = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters["SFUPMT"].Value = Utilities.Common.CastToE1Time(DateTime.Now);
                response = new SqlDmlResponse();
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                int rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
                response.RowsAffected = rowCount;
                response.EndTime = DateTime.Now.ToLocalTime();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
                response.Message = e.Message;
            }
            return response;
        }

        private static iDB2Command InsertScheduleBackoutCmd()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                INSERT INTO F580030 (
                SFUKID,
                SFEDUS,
                SFEDBT,
                SFEDTN,
                SFEDLN,
                SFEDCT,
                SFTYTN,
                SFEDFT,
                SFEDDT,
                SFDRIN,
                SFEDDL,
                SFEDSP,
                SFPNID,
                SFTNAC,
                SFMMCU,
                SFACDT,
                SFSTTM,
                SFDTMS,
                SFZ58SCN,
                SFUSER,
                SFUPMJ,
                SFUPMT)
                VALUES(
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,
                ?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("SFUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFUKID"].Precision = 15;
            cmd.Parameters["SFUKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("SFEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["SFEDLN"].Precision = 7;
            cmd.Parameters["SFEDLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFTYTN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("SFEDFT", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFEDDT"].Precision = 6;
            cmd.Parameters["SFEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFDRIN", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFEDDL", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFEDDL"].Precision = 5;
            cmd.Parameters["SFEDDL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFPNID", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFTNAC", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFMMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SFACDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFACDT"].Precision = 6;
            cmd.Parameters["SFACDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFSTTM", iDB2DbType.iDB2Graphic, 24));
            cmd.Parameters.Add(new iDB2Parameter("SFDTMS", iDB2DbType.iDB2Graphic, 24));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFZ58SCN"].Precision = 5;
            cmd.Parameters["SFZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFUPMJ"].Precision = 6;
            cmd.Parameters["SFUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFUPMT"].Precision = 6;
            cmd.Parameters["SFUPMT"].Scale = 0;
            return cmd;
        }
    }
}
