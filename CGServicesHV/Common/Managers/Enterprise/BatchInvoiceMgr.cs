using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class BatchInvoiceMgr
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public BatchInvoiceMgr(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"BatchInvoiceMgr.Insert", InsertCmd()}
            };

            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
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
                    INSERT INTO F03B11Z1 (
                        VJEDUS,
                        VJEDTY,
                        VJEDSQ,
                        VJEDTN,
                        VJEDCT,
                        VJEDLN,
                        VJEDTS,
                        VJEDFT,
                        VJEDDT,
                        VJEDER,
                        VJEDDL,
                        VJEDSP,
                        VJEDTC,
                        VJEDTR,
                        VJEDBT,
                        VJEDGL,
                        VJEDDH,
                        VJEDAN,
                        VJDOC,
                        VJDCT,
                        VJKCO,
                        VJSFX,
                        VJAN8,
                        VJDGJ,
                        VJDIVJ,
                        VJICUT,
                        VJICU,
                        VJDICJ,
                        VJFY,
                        VJCTRY,
                        VJPN,
                        VJCO,
                        VJGLC,
                        VJAID,
                        VJPA8,
                        VJAN8J,
                        VJPYR,
                        VJPOST,
                        VJISTR,
                        VJBALJ,
                        VJPST,
                        VJAG,
                        VJAAP,
                        VJADSC,
                        VJADSA,
                        VJATXA,
                        VJATXN,
                        VJSTAM,
                        VJCRRM,
                        VJCRCD,
                        VJCRR,
                        VJDMCD,
                        VJACR,
                        VJFAP,
                        VJCDS,
                        VJCDSA,
                        VJCTXA,
                        VJCTXN,
                        VJCTAM,
                        VJTXA1,
                        VJEXR1,
                        VJDSVJ,
                        VJGLBA,
                        VJAM,
                        VJAID2,
                        VJAM2,
                        VJMCU,
                        VJOBJ,
                        VJSUB,
                        VJSBLT,
                        VJSBL,
                        VJPTC,
                        VJDDJ,
                        VJDDNJ,
                        VJRDDJ,
                        VJRDSJ,
                        VJSMTJ,
                        VJNBRR,
                        VJRDRL,
                        VJRMDS,
                        VJCOLL,
                        VJCORC,
                        VJAFC,
                        VJRSCO,
                        VJCKNU,
                        VJODOC,
                        VJODCT,
                        VJOKCO,
                        VJOSFX,
                        VJVINV,
                        VJPO,
                        VJPDCT,
                        VJPKCO,
                        VJDCTO,
                        VJLNID,
                        VJSDOC,
                        VJSDCT,
                        VJSKCO,
                        VJSFXO,
                        VJVLDT,
                        VJCMC1,
                        VJVR01,
                        VJUNIT,
                        VJMCU2,
                        VJRMK,
                        VJALPH,
                        VJRF,
                        VJDRF,
                        VJCTL,
                        VJFNLP,
                        VJITM,
                        VJU,
                        VJUM,
                        VJALT6,
                        VJRYIN,
                        VJVDGJ,
                        VJVRE,
                        VJRP1,
                        VJRP2,
                        VJRP3,
                        VJAR01,
                        VJAR02,
                        VJAR03,
                        VJAR04,
                        VJAR05,
                        VJAR06,
                        VJAR07,
                        VJAR08,
                        VJAR09,
                        VJAR10,
                        VJURC1,
                        VJURDT,
                        VJURAT,
                        VJURAB,
                        VJURRF,
                        VJTORG,
                        VJUSER,
                        VJPID,
                        VJUPMJ,
                        VJUPMT,
                        VJJOBN,
                        VJHCRR,
                        VJHDGJ,
                        VJDIM,
                        VJDID,
                        VJDIY,
                        VJDI#,
                        VJDGM,
                        VJDGD,
                        VJDGY,
                        VJDG#,
                        VJDICM,
                        VJDICD,
                        VJDICY,
                        VJDIC#,
                        VJDSVM,
                        VJDSVD,
                        VJDSVY,
                        VJDSV#,
                        VJDDM,
                        VJDDD,
                        VJDDY,
                        VJDD#,
                        VJDDNM,
                        VJDDND,
                        VJDDNY,
                        VJDDN#,
                        VJSMTM,
                        VJSMTD,
                        VJSMTY,
                        VJSMT#,
                        VJRDDM,
                        VJRDDD,
                        VJRDDY,
                        VJRDD#,
                        VJRDSM,
                        VJRDSD,
                        VJRDSY,
                        VJRDS#,
                        VJHDGM,
                        VJHDGD,
                        VJHDGY,
                        VJHDG#,
                        VJSHPN,
                        VJDTXS,
                        VJOMOD,
                        VJCLMG,
                        VJCMGR,
                        VJATAD,
                        VJCTAD,
                        VJNRTA,
                        VJFNRT,
                        VJPRGF,
                        VJGFL1,
                        VJGFL2,
                        VJDOCO,
                        VJKCOO,
                        VJSOTF,
                        VJDTPB,
                        VJERDJ,
                        VJNETST,
                        VJRMR1)
                        VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("VJEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJEDTY", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDSQ", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJEDSQ"].Precision = 2;
            cmd.Parameters["VJEDSQ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("VJEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["VJEDLN"].Precision = 7;
            cmd.Parameters["VJEDLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJEDTS", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("VJEDFT", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJEDDT"].Precision = 6;
            cmd.Parameters["VJEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJEDER", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDDL", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["VJEDDL"].Precision = 5;
            cmd.Parameters["VJEDDL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDTC", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDTR", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("VJEDGL", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDDH", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJEDAN", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJEDAN"].Precision = 8;
            cmd.Parameters["VJEDAN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDOC", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJDOC"].Precision = 8;
            cmd.Parameters["VJDOC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VJSFX", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAN8", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJAN8"].Precision = 8;
            cmd.Parameters["VJAN8"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDGJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDGJ"].Precision = 6;
            cmd.Parameters["VJDGJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDIVJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDIVJ"].Precision = 6;
            cmd.Parameters["VJDIVJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJICUT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJICU", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJICU"].Precision = 8;
            cmd.Parameters["VJICU"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDICJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDICJ"].Precision = 6;
            cmd.Parameters["VJDICJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJFY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJFY"].Precision = 2;
            cmd.Parameters["VJFY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCTRY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJCTRY"].Precision = 2;
            cmd.Parameters["VJCTRY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJPN", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJPN"].Precision = 2;
            cmd.Parameters["VJPN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VJGLC", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("VJAID", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJPA8", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJPA8"].Precision = 8;
            cmd.Parameters["VJPA8"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJAN8J", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJAN8J"].Precision = 8;
            cmd.Parameters["VJAN8J"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJPYR", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJPYR"].Precision = 8;
            cmd.Parameters["VJPYR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJPOST", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJISTR", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJBALJ", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJPST", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJAG", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJAG"].Precision = 15;
            cmd.Parameters["VJAG"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJAAP", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJAAP"].Precision = 15;
            cmd.Parameters["VJAAP"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJADSC", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJADSC"].Precision = 15;
            cmd.Parameters["VJADSC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJADSA", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJADSA"].Precision = 15;
            cmd.Parameters["VJADSA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJATXA", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJATXA"].Precision = 15;
            cmd.Parameters["VJATXA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJATXN", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJATXN"].Precision = 15;
            cmd.Parameters["VJATXN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSTAM", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJSTAM"].Precision = 15;
            cmd.Parameters["VJSTAM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCRRM", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJCRCD", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJCRR", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCRR"].Precision = 15;
            cmd.Parameters["VJCRR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDMCD", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJACR", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJACR"].Precision = 15;
            cmd.Parameters["VJACR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJFAP", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJFAP"].Precision = 15;
            cmd.Parameters["VJFAP"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCDS", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCDS"].Precision = 15;
            cmd.Parameters["VJCDS"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCDSA", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCDSA"].Precision = 15;
            cmd.Parameters["VJCDSA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCTXA", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCTXA"].Precision = 15;
            cmd.Parameters["VJCTXA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCTXN", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCTXN"].Precision = 15;
            cmd.Parameters["VJCTXN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCTAM", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCTAM"].Precision = 15;
            cmd.Parameters["VJCTAM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJTXA1", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJEXR1", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJDSVJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDSVJ"].Precision = 6;
            cmd.Parameters["VJDSVJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJGLBA", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJAM", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJAID2", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJAM2", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VJOBJ", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("VJSUB", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJSBLT", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJSBL", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJPTC", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJDDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDDJ"].Precision = 6;
            cmd.Parameters["VJDDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDNJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDDNJ"].Precision = 6;
            cmd.Parameters["VJDDNJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJRDDJ"].Precision = 6;
            cmd.Parameters["VJRDDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDSJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJRDSJ"].Precision = 6;
            cmd.Parameters["VJRDSJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSMTJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJSMTJ"].Precision = 6;
            cmd.Parameters["VJSMTJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJNBRR", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRDRL", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRMDS", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["VJRMDS"].Precision = 3;
            cmd.Parameters["VJRMDS"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCOLL", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJCORC", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJAFC", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRSCO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJCKNU", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VJODOC", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJODOC"].Precision = 8;
            cmd.Parameters["VJODOC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJODCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJOKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VJOSFX", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJVINV", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VJPO", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJPDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJPKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VJDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJLNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJLNID"].Precision = 6;
            cmd.Parameters["VJLNID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSDOC", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJSDOC"].Precision = 8;
            cmd.Parameters["VJSDOC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJSKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VJSFXO", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJVLDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJVLDT"].Precision = 6;
            cmd.Parameters["VJVLDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCMC1", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJCMC1"].Precision = 8;
            cmd.Parameters["VJCMC1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJVR01", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VJUNIT", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VJMCU2", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VJRMK", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("VJALPH", iDB2DbType.iDB2Graphic, 40));
            cmd.Parameters.Add(new iDB2Parameter("VJRF", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJDRF", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["VJDRF"].Precision = 3;
            cmd.Parameters["VJDRF"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCTL", iDB2DbType.iDB2Graphic, 13));
            cmd.Parameters.Add(new iDB2Parameter("VJFNLP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJITM", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJITM"].Precision = 8;
            cmd.Parameters["VJITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJU", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJU"].Precision = 15;
            cmd.Parameters["VJU"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJUM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VJALT6", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRYIN", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJVDGJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJVDGJ"].Precision = 6;
            cmd.Parameters["VJVDGJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJVRE", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJRP1", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRP2", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRP3", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJAR01", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR02", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR03", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR04", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR05", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR06", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR07", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR08", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR09", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJAR10", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJURC1", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VJURDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJURDT"].Precision = 6;
            cmd.Parameters["VJURDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJURAT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJURAT"].Precision = 15;
            cmd.Parameters["VJURAT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJURAB", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJURAB"].Precision = 8;
            cmd.Parameters["VJURAB"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJURRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("VJTORG", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJUPMJ"].Precision = 6;
            cmd.Parameters["VJUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJUPMT"].Precision = 6;
            cmd.Parameters["VJUPMT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJHCRR", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJHCRR"].Precision = 15;
            cmd.Parameters["VJHCRR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJHDGJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJHDGJ"].Precision = 6;
            cmd.Parameters["VJHDGJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDIM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDIM"].Precision = 2;
            cmd.Parameters["VJDIM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDID", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDID"].Precision = 2;
            cmd.Parameters["VJDID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDIY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDIY"].Precision = 2;
            cmd.Parameters["VJDIY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDI#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDI#"].Precision = 2;
            cmd.Parameters["VJDI#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDGM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDGM"].Precision = 2;
            cmd.Parameters["VJDGM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDGD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDGD"].Precision = 2;
            cmd.Parameters["VJDGD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDGY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDGY"].Precision = 2;
            cmd.Parameters["VJDGY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDG#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDG#"].Precision = 2;
            cmd.Parameters["VJDG#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDICM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDICM"].Precision = 2;
            cmd.Parameters["VJDICM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDICD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDICD"].Precision = 2;
            cmd.Parameters["VJDICD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDICY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDICY"].Precision = 2;
            cmd.Parameters["VJDICY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDIC#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDIC#"].Precision = 2;
            cmd.Parameters["VJDIC#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDSVM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDSVM"].Precision = 2;
            cmd.Parameters["VJDSVM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDSVD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDSVD"].Precision = 2;
            cmd.Parameters["VJDSVD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDSVY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDSVY"].Precision = 2;
            cmd.Parameters["VJDSVY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDSV#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDSV#"].Precision = 2;
            cmd.Parameters["VJDSV#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDM"].Precision = 2;
            cmd.Parameters["VJDDM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDD"].Precision = 2;
            cmd.Parameters["VJDDD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDY"].Precision = 2;
            cmd.Parameters["VJDDY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDD#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDD#"].Precision = 2;
            cmd.Parameters["VJDD#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDNM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDNM"].Precision = 2;
            cmd.Parameters["VJDDNM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDND", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDND"].Precision = 2;
            cmd.Parameters["VJDDND"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDNY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDNY"].Precision = 2;
            cmd.Parameters["VJDDNY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDDN#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJDDN#"].Precision = 2;
            cmd.Parameters["VJDDN#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSMTM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJSMTM"].Precision = 2;
            cmd.Parameters["VJSMTM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSMTD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJSMTD"].Precision = 2;
            cmd.Parameters["VJSMTD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSMTY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJSMTY"].Precision = 2;
            cmd.Parameters["VJSMTY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSMT#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJSMT#"].Precision = 2;
            cmd.Parameters["VJSMT#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDDM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDDM"].Precision = 2;
            cmd.Parameters["VJRDDM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDDD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDDD"].Precision = 2;
            cmd.Parameters["VJRDDD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDDY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDDY"].Precision = 2;
            cmd.Parameters["VJRDDY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDD#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDD#"].Precision = 2;
            cmd.Parameters["VJRDD#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDSM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDSM"].Precision = 2;
            cmd.Parameters["VJRDSM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDSD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDSD"].Precision = 2;
            cmd.Parameters["VJRDSD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDSY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDSY"].Precision = 2;
            cmd.Parameters["VJRDSY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJRDS#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJRDS#"].Precision = 2;
            cmd.Parameters["VJRDS#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJHDGM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJHDGM"].Precision = 2;
            cmd.Parameters["VJHDGM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJHDGD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJHDGD"].Precision = 2;
            cmd.Parameters["VJHDGD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJHDGY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJHDGY"].Precision = 2;
            cmd.Parameters["VJHDGY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJHDG#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VJHDG#"].Precision = 2;
            cmd.Parameters["VJHDG#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJSHPN", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJSHPN"].Precision = 8;
            cmd.Parameters["VJSHPN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJDTXS", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJOMOD", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJCLMG", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJCMGR", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VJATAD", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJATAD"].Precision = 15;
            cmd.Parameters["VJATAD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJCTAD", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJCTAD"].Precision = 15;
            cmd.Parameters["VJCTAD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJNRTA", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJNRTA"].Precision = 15;
            cmd.Parameters["VJNRTA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJFNRT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VJFNRT"].Precision = 15;
            cmd.Parameters["VJFNRT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJPRGF", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJGFL1", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJGFL2", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VJDOCO"].Precision = 8;
            cmd.Parameters["VJDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJKCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VJSOTF", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJDTPB", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJDTPB"].Precision = 6;
            cmd.Parameters["VJDTPB"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJERDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VJERDJ"].Precision = 6;
            cmd.Parameters["VJERDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VJNETST", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VJRMR1", iDB2DbType.iDB2Graphic, 50));
            return cmd;
        }

        public SqlDmlResponse Save(BusinessSystemSession e1Session,  iDB2Transaction trans, BatchInvoices recs)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var response = new SqlDmlResponse
            {
                StartTime = DateTime.Now
            };
            var cmd = (e1Session.DbCommands.ContainsKey("BatchInvoiceMgr.Insert")) ? e1Session.DbCommands["BatchInvoiceMgr.Insert"] : InsertCmd();
            cmd.Connection = e1Session.DbConnection;
            cmd.Transaction = trans;
            try
            {
                foreach (var rec in recs)
                {
                    cmd.Parameters[0].Value = rec.VJEDUS;
                    cmd.Parameters[1].Value = rec.VJEDTY;
                    cmd.Parameters[2].Value = rec.VJEDSQ;
                    cmd.Parameters[3].Value = rec.VJEDTN;
                    cmd.Parameters[4].Value = rec.VJEDCT;
                    cmd.Parameters[5].Value = rec.VJEDLN;
                    cmd.Parameters[6].Value = rec.VJEDTS;
                    cmd.Parameters[7].Value = rec.VJEDFT;
                    cmd.Parameters[8].Value = rec.VJEDDT;
                    cmd.Parameters[9].Value = rec.VJEDER;
                    cmd.Parameters[10].Value = rec.VJEDDL;
                    cmd.Parameters[11].Value = rec.VJEDSP;
                    cmd.Parameters[12].Value = rec.VJEDTC;
                    cmd.Parameters[13].Value = rec.VJEDTR;
                    cmd.Parameters[14].Value = rec.VJEDBT;
                    cmd.Parameters[15].Value = rec.VJEDGL;
                    cmd.Parameters[16].Value = rec.VJEDDH;
                    cmd.Parameters[17].Value = rec.VJEDAN;
                    cmd.Parameters[18].Value = rec.VJDOC;
                    cmd.Parameters[19].Value = rec.VJDCT;
                    cmd.Parameters[20].Value = rec.VJKCO;
                    cmd.Parameters[21].Value = rec.VJSFX;
                    cmd.Parameters[22].Value = rec.VJAN8;
                    cmd.Parameters[23].Value = rec.VJDGJ;
                    cmd.Parameters[24].Value = rec.VJDIVJ;
                    cmd.Parameters[25].Value = rec.VJICUT;
                    cmd.Parameters[26].Value = rec.VJICU;
                    cmd.Parameters[27].Value = rec.VJDICJ;
                    cmd.Parameters[28].Value = rec.VJFY;
                    cmd.Parameters[29].Value = rec.VJCTRY;
                    cmd.Parameters[30].Value = rec.VJPN;
                    cmd.Parameters[31].Value = rec.VJCO;
                    cmd.Parameters[32].Value = rec.VJGLC;
                    cmd.Parameters[33].Value = rec.VJAID;
                    cmd.Parameters[34].Value = rec.VJPA8;
                    cmd.Parameters[35].Value = rec.VJAN8J;
                    cmd.Parameters[36].Value = rec.VJPYR;
                    cmd.Parameters[37].Value = rec.VJPOST;
                    cmd.Parameters[38].Value = rec.VJISTR;
                    cmd.Parameters[39].Value = rec.VJBALJ;
                    cmd.Parameters[40].Value = rec.VJPST;
                    cmd.Parameters[41].Value = rec.VJAG;
                    cmd.Parameters[42].Value = rec.VJAAP;
                    cmd.Parameters[43].Value = rec.VJADSC;
                    cmd.Parameters[44].Value = rec.VJADSA;
                    cmd.Parameters[45].Value = rec.VJATXA;
                    cmd.Parameters[46].Value = rec.VJATXN;
                    cmd.Parameters[47].Value = rec.VJSTAM;
                    cmd.Parameters[48].Value = rec.VJCRRM;
                    cmd.Parameters[49].Value = rec.VJCRCD;
                    cmd.Parameters[50].Value = rec.VJCRR;
                    cmd.Parameters[51].Value = rec.VJDMCD;
                    cmd.Parameters[52].Value = rec.VJACR;
                    cmd.Parameters[53].Value = rec.VJFAP;
                    cmd.Parameters[54].Value = rec.VJCDS;
                    cmd.Parameters[55].Value = rec.VJCDSA;
                    cmd.Parameters[56].Value = rec.VJCTXA;
                    cmd.Parameters[57].Value = rec.VJCTXN;
                    cmd.Parameters[58].Value = rec.VJCTAM;
                    cmd.Parameters[59].Value = rec.VJTXA1;
                    cmd.Parameters[60].Value = rec.VJEXR1;
                    cmd.Parameters[61].Value = rec.VJDSVJ;
                    cmd.Parameters[62].Value = rec.VJGLBA;
                    cmd.Parameters[63].Value = rec.VJAM;
                    cmd.Parameters[64].Value = rec.VJAID2;
                    cmd.Parameters[65].Value = rec.VJAM2;
                    cmd.Parameters[66].Value = rec.VJMCU;
                    cmd.Parameters[67].Value = rec.VJOBJ;
                    cmd.Parameters[68].Value = rec.VJSUB;
                    cmd.Parameters[69].Value = rec.VJSBLT;
                    cmd.Parameters[70].Value = rec.VJSBL;
                    cmd.Parameters[71].Value = rec.VJPTC;
                    cmd.Parameters[72].Value = rec.VJDDJ;
                    cmd.Parameters[73].Value = rec.VJDDNJ;
                    cmd.Parameters[74].Value = rec.VJRDDJ;
                    cmd.Parameters[75].Value = rec.VJRDSJ;
                    cmd.Parameters[76].Value = rec.VJSMTJ;
                    cmd.Parameters[77].Value = rec.VJNBRR;
                    cmd.Parameters[78].Value = rec.VJRDRL;
                    cmd.Parameters[79].Value = rec.VJRMDS;
                    cmd.Parameters[80].Value = rec.VJCOLL;
                    cmd.Parameters[81].Value = rec.VJCORC;
                    cmd.Parameters[82].Value = rec.VJAFC;
                    cmd.Parameters[83].Value = rec.VJRSCO;
                    cmd.Parameters[84].Value = rec.VJCKNU;
                    cmd.Parameters[85].Value = rec.VJODOC;
                    cmd.Parameters[86].Value = rec.VJODCT;
                    cmd.Parameters[87].Value = rec.VJOKCO;
                    cmd.Parameters[88].Value = rec.VJOSFX;
                    cmd.Parameters[89].Value = rec.VJVINV;
                    cmd.Parameters[90].Value = rec.VJPO;
                    cmd.Parameters[91].Value = rec.VJPDCT;
                    cmd.Parameters[92].Value = rec.VJPKCO;
                    cmd.Parameters[93].Value = rec.VJDCTO;
                    cmd.Parameters[94].Value = rec.VJLNID;
                    cmd.Parameters[95].Value = rec.VJSDOC;
                    cmd.Parameters[96].Value = rec.VJSDCT;
                    cmd.Parameters[97].Value = rec.VJSKCO;
                    cmd.Parameters[98].Value = rec.VJSFXO;
                    cmd.Parameters[99].Value = rec.VJVLDT;
                    cmd.Parameters[100].Value = rec.VJCMC1;
                    cmd.Parameters[101].Value = rec.VJVR01;
                    cmd.Parameters[102].Value = rec.VJUNIT;
                    cmd.Parameters[103].Value = rec.VJMCU2;
                    cmd.Parameters[104].Value = rec.VJRMK;
                    cmd.Parameters[105].Value = rec.VJALPH;
                    cmd.Parameters[106].Value = rec.VJRF;
                    cmd.Parameters[107].Value = rec.VJDRF;
                    cmd.Parameters[108].Value = rec.VJCTL;
                    cmd.Parameters[109].Value = rec.VJFNLP;
                    cmd.Parameters[110].Value = rec.VJITM;
                    cmd.Parameters[111].Value = rec.VJU;
                    cmd.Parameters[112].Value = rec.VJUM;
                    cmd.Parameters[113].Value = rec.VJALT6;
                    cmd.Parameters[114].Value = rec.VJRYIN;
                    cmd.Parameters[115].Value = rec.VJVDGJ;
                    cmd.Parameters[116].Value = rec.VJVRE;
                    cmd.Parameters[117].Value = rec.VJRP1;
                    cmd.Parameters[118].Value = rec.VJRP2;
                    cmd.Parameters[119].Value = rec.VJRP3;
                    cmd.Parameters[120].Value = rec.VJAR01;
                    cmd.Parameters[121].Value = rec.VJAR02;
                    cmd.Parameters[122].Value = rec.VJAR03;
                    cmd.Parameters[123].Value = rec.VJAR04;
                    cmd.Parameters[124].Value = rec.VJAR05;
                    cmd.Parameters[125].Value = rec.VJAR06;
                    cmd.Parameters[126].Value = rec.VJAR07;
                    cmd.Parameters[127].Value = rec.VJAR08;
                    cmd.Parameters[128].Value = rec.VJAR09;
                    cmd.Parameters[129].Value = rec.VJAR10;
                    cmd.Parameters[130].Value = rec.VJURC1;
                    cmd.Parameters[131].Value = rec.VJURDT;
                    cmd.Parameters[132].Value = rec.VJURAT;
                    cmd.Parameters[133].Value = rec.VJURAB;
                    cmd.Parameters[134].Value = rec.VJURRF;
                    cmd.Parameters[135].Value = rec.VJTORG;
                    cmd.Parameters[136].Value = rec.VJUSER;
                    cmd.Parameters[137].Value = rec.VJPID;
                    cmd.Parameters[138].Value = rec.VJUPMJ;
                    cmd.Parameters[139].Value = rec.VJUPMT;
                    cmd.Parameters[140].Value = rec.VJJOBN;
                    cmd.Parameters[141].Value = rec.VJHCRR;
                    cmd.Parameters[142].Value = rec.VJHDGJ;
                    cmd.Parameters[143].Value = rec.VJDIM;
                    cmd.Parameters[144].Value = rec.VJDID;
                    cmd.Parameters[145].Value = rec.VJDIY;
                    cmd.Parameters[146].Value = rec.VJDI_PS;
                    cmd.Parameters[147].Value = rec.VJDGM;
                    cmd.Parameters[148].Value = rec.VJDGD;
                    cmd.Parameters[149].Value = rec.VJDGY;
                    cmd.Parameters[150].Value = rec.VJDG_PS;
                    cmd.Parameters[151].Value = rec.VJDICM;
                    cmd.Parameters[152].Value = rec.VJDICD;
                    cmd.Parameters[153].Value = rec.VJDICY;
                    cmd.Parameters[154].Value = rec.VJDIC_PS;
                    cmd.Parameters[155].Value = rec.VJDSVM;
                    cmd.Parameters[156].Value = rec.VJDSVD;
                    cmd.Parameters[157].Value = rec.VJDSVY;
                    cmd.Parameters[158].Value = rec.VJDSV_PS;
                    cmd.Parameters[159].Value = rec.VJDDM;
                    cmd.Parameters[160].Value = rec.VJDDD;
                    cmd.Parameters[161].Value = rec.VJDDY;
                    cmd.Parameters[162].Value = rec.VJDD_PS;
                    cmd.Parameters[163].Value = rec.VJDDNM;
                    cmd.Parameters[164].Value = rec.VJDDND;
                    cmd.Parameters[165].Value = rec.VJDDNY;
                    cmd.Parameters[166].Value = rec.VJDDN_PS;
                    cmd.Parameters[167].Value = rec.VJSMTM;
                    cmd.Parameters[168].Value = rec.VJSMTD;
                    cmd.Parameters[169].Value = rec.VJSMTY;
                    cmd.Parameters[170].Value = rec.VJSMT_PS;
                    cmd.Parameters[171].Value = rec.VJRDDM;
                    cmd.Parameters[172].Value = rec.VJRDDD;
                    cmd.Parameters[173].Value = rec.VJRDDY;
                    cmd.Parameters[174].Value = rec.VJRDD_PS;
                    cmd.Parameters[175].Value = rec.VJRDSM;
                    cmd.Parameters[176].Value = rec.VJRDSD;
                    cmd.Parameters[177].Value = rec.VJRDSY;
                    cmd.Parameters[178].Value = rec.VJRDS_PS;
                    cmd.Parameters[179].Value = rec.VJHDGM;
                    cmd.Parameters[180].Value = rec.VJHDGD;
                    cmd.Parameters[181].Value = rec.VJHDGY;
                    cmd.Parameters[182].Value = rec.VJHDG_PS;
                    cmd.Parameters[183].Value = rec.VJSHPN;
                    cmd.Parameters[184].Value = rec.VJDTXS;
                    cmd.Parameters[185].Value = rec.VJOMOD;
                    cmd.Parameters[186].Value = rec.VJCLMG;
                    cmd.Parameters[187].Value = rec.VJCMGR;
                    cmd.Parameters[188].Value = rec.VJATAD;
                    cmd.Parameters[189].Value = rec.VJCTAD;
                    cmd.Parameters[190].Value = rec.VJNRTA;
                    cmd.Parameters[191].Value = rec.VJFNRT;
                    cmd.Parameters[192].Value = rec.VJPRGF;
                    cmd.Parameters[193].Value = rec.VJGFL1;
                    cmd.Parameters[194].Value = rec.VJGFL2;
                    cmd.Parameters[195].Value = rec.VJDOCO;
                    cmd.Parameters[196].Value = rec.VJKCOO;
                    cmd.Parameters[197].Value = rec.VJSOTF;
                    cmd.Parameters[198].Value = rec.VJDTPB;
                    cmd.Parameters[199].Value = rec.VJERDJ;
                    cmd.Parameters[200].Value = rec.VJNETST;
                    cmd.Parameters[201].Value = rec.VJRMR1;
                    var rows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = "F03B11Z1: " + e;
            }
            finally
            {
                response.EndTime = DateTime.Now;
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return response;
        }
    }
}
