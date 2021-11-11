using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.E1
{
    public class F580030AManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public F580030AManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F580030AManager.Insert", Insert()}
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
                    INSERT INTO F580030A (
                        QAUKID,
                        QAEDSP,
	                    QAEDCT,
	                    QAMCU,
	                    QAEDBT,
	                    QAZ58SCN,
	                    QACRTU,
	                    QACPGM,
	                    QAJOBN,
	                    QACRTDT,
	                    QACRTM,
	                    QAUPMB,
	                    QAUPGM,
	                    QAMJOB,
                        QAUPMJ,
                        QAUPMT)
                    VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("QAUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["QAUKID"].Precision = 15;
            cmd.Parameters["QAUKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("QAEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("QAMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("QAEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("QAZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["QAZ58SCN"].Precision = 5;
            cmd.Parameters["QAZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QACRTU", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QACPGM", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QACRTDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QACRTDT"].Precision = 6;
            cmd.Parameters["QACRTDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QACRTM", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QACRTM"].Precision = 6;
            cmd.Parameters["QACRTM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAUPMB", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAUPGM", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAMJOB", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QAUPMJ"].Precision = 6;
            cmd.Parameters["QAUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QAUPMT"].Precision = 6;
            cmd.Parameters["QAUPMT"].Scale = 0;
            return cmd;
        }

        public SqlDmlResponse Save(BusinessSystemSession session, iDB2Transaction e1Trans, F580030A record)
        {
            SqlDmlResponse response;
            var cmd = (session.DbCommands.ContainsKey("F580030AManager.Insert")) ? session.DbCommands["F580030AManager.Insert"] : Insert();
            cmd.Connection = session.DbConnection;
            cmd.Transaction = e1Trans;
            try
            {
                cmd.Parameters["QAUKID"].Value = record.QAUKID;
                cmd.Parameters["QAEDSP"].Value = record.QAEDSP;
                cmd.Parameters["QAEDCT"].Value = record.QAEDCT;
                cmd.Parameters["QAMCU"].Value = record.QAMCU;
                cmd.Parameters["QAEDBT"].Value = record.QAEDBT;
                cmd.Parameters["QAZ58SCN"].Value = record.QAZ58SCN;
                cmd.Parameters["QACRTU"].Value = record.QACRTU;
                cmd.Parameters["QACPGM"].Value = record.QACPGM;
                cmd.Parameters["QAJOBN"].Value = record.QAJOBN;
                cmd.Parameters["QACRTDT"].Value = record.QACRTDT;
                cmd.Parameters["QACRTM"].Value = record.QACRTM;
                cmd.Parameters["QAUPMB"].Value = record.QAUPMB;
                cmd.Parameters["QAUPGM"].Value = record.QAUPGM;
                cmd.Parameters["QAMJOB"].Value = record.QAMJOB;
                cmd.Parameters["QAUPMJ"].Value = record.QAUPMJ;
                cmd.Parameters["QAUPMT"].Value = record.QAUPMT;
                response = new SqlDmlResponse();
                response.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                response.Done();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return response;
        }

        public F580030A Create(string edct, decimal ukid, decimal edbt, string mcu)
        {
            var rec = new F580030A();
            try
            {
                rec.QAUKID = ukid;
                rec.QAEDCT = edct;
                rec.QAMCU = mcu.PadLeft(12, ' ');
                rec.QAEDBT = edbt.ToString(CultureInfo.InvariantCulture);
                rec.QACRTU = "SVCSOA";
                rec.QACPGM = "SVCSOA";
                rec.QAJOBN = "SVCSOA";
                rec.QACRTDT = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                rec.QACRTM = Utilities.Common.CastToE1Time(DateTime.Now.ToLocalTime());
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return rec;
        }

        public F580030A Create(string edct, decimal ukid, decimal edbt, string mcu, decimal scheduleNumber)
        {
            //if (edct.Equals("BD") || edct.Equals("BX")) edct = "BX";
            var rec = new F580030A();
            try
            {
                rec.QAUKID = ukid;
                rec.QAEDCT = edct;
                rec.QAZ58SCN = scheduleNumber;
                rec.QAMCU = mcu.PadLeft(12, ' ');
                rec.QAEDBT = edbt.ToString(CultureInfo.InvariantCulture);
                rec.QACRTU = "SVCSOA";
                rec.QACPGM = "SVCSOA";
                rec.QAJOBN = "SVCSOA";
                rec.QACRTDT = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                rec.QACRTM = Utilities.Common.CastToE1Time(DateTime.Now.ToLocalTime());
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return rec;
        }

        public static SqlDmlResponse InsertScheduleRebox(BusinessSystemSession session, string plantNum, decimal scheduleNum, long qaukid, long sfedbt)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = InsertScheduleBackoutCmd();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["QAUKID"].Value = qaukid;
                cmd.Parameters["QAEDSP"].Value = "";
                cmd.Parameters["QAEDCT"].Value = "BD";
                cmd.Parameters["QAMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["QAEDBT"].Value = sfedbt.ToString();
                cmd.Parameters["QAZ58SCN"].Value = scheduleNum;
                cmd.Parameters["QACRTU"].Value = "SVCSOA";
                cmd.Parameters["QACPGM"].Value = "SVCSOA";
                cmd.Parameters["QAJOBN"].Value = "SVCSOA";
                cmd.Parameters["QACRTDT"].Value = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                cmd.Parameters["QACRTM"].Value = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                cmd.Parameters["QAUPMB"].Value = "SVCSOA";
                cmd.Parameters["QAUPGM"].Value = "Rebox";
                cmd.Parameters["QAMJOB"].Value = "";
                cmd.Parameters["QAUPMJ"].Value = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters["QAUPMT"].Value = Utilities.Common.CastToE1Time(DateTime.Now);
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

        public static SqlDmlResponse InsertScheduleBackout(BusinessSystemSession session, string plantNum, decimal scheduleNum, long qaukid, long sfedbt)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = InsertScheduleBackoutCmd();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["QAUKID"].Value = qaukid;
                cmd.Parameters["QAEDSP"].Value = "";
                cmd.Parameters["QAEDCT"].Value = "BX";
                cmd.Parameters["QAMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["QAEDBT"].Value = sfedbt.ToString();
                cmd.Parameters["QAZ58SCN"].Value = scheduleNum;
                cmd.Parameters["QACRTU"].Value = "SVCSOA";
                cmd.Parameters["QACPGM"].Value = "SVCSOA";
                cmd.Parameters["QAJOBN"].Value = "SVCSOA";
                cmd.Parameters["QACRTDT"].Value = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                cmd.Parameters["QACRTM"].Value = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                cmd.Parameters["QAUPMB"].Value = "SVCSOA";
                cmd.Parameters["QAUPGM"].Value = "Backout";
                cmd.Parameters["QAMJOB"].Value = "";
                cmd.Parameters["QAUPMJ"].Value = Utilities.Common.CastToE1Date(DateTime.Now);
                cmd.Parameters["QAUPMT"].Value = Utilities.Common.CastToE1Time(DateTime.Now);
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
                    INSERT INTO F580030A (
                        QAUKID,
                        QAEDSP,
	                    QAEDCT,
	                    QAMCU,
	                    QAEDBT,
	                    QAZ58SCN,
	                    QACRTU,
	                    QACPGM,
	                    QAJOBN,
	                    QACRTDT,
	                    QACRTM,
	                    QAUPMB,
	                    QAUPGM,
	                    QAMJOB,
                        QAUPMJ,
                        QAUPMT)
                    VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("QAUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["QAUKID"].Precision = 15;
            cmd.Parameters["QAUKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("QAEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("QAMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("QAEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("QAZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["QAZ58SCN"].Precision = 5;
            cmd.Parameters["QAZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QACRTU", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QACPGM", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QACRTDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QACRTDT"].Precision = 6;
            cmd.Parameters["QACRTDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QACRTM", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QACRTM"].Precision = 6;
            cmd.Parameters["QACRTM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAUPMB", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAUPGM", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAMJOB", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QAUPMJ"].Precision = 6;
            cmd.Parameters["QAUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QAUPMT"].Precision = 6;
            cmd.Parameters["QAUPMT"].Scale = 0;
            return cmd;
        }
    }
}
