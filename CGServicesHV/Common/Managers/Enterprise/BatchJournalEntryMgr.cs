using System;
using System.Collections.Generic;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class BatchJournalEntryMgr
    {
        public BatchJournalEntryMgr(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("BatchJournalEntryMgr.Insert", InsertCmd());

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
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"INSERT INTO F0911Z1 (
            VNEDUS,
            VNEDTY,
            VNEDSQ,
            VNEDTN,
            VNEDCT,
            VNEDLN,
            VNEDTS,
            VNEDFT,
            VNEDDT,
            VNEDER,
            VNEDDL,
            VNEDSP,
            VNEDTC,
            VNEDTR,
            VNEDBT,
            VNEDGL,
            VNEDAN,
            VNKCO,
            VNDCT,
            VNDOC,
            VNDGJ,
            VNJELN,
            VNEXTL,
            VNPOST,
            VNICU,
            VNICUT,
            VNDICJ,
            VNDSYJ,
            VNTICU,
            VNCO,
            VNANI,
            VNAM,
            VNAID,
            VNMCU,
            VNOBJ,
            VNSUB,
            VNSBL,
            VNSBLT,
            VNLT,
            VNPN,
            VNCTRY,
            VNFY,
            VNFQ,
            VNCRCD,
            VNCRR,
            VNHCRR,
            VNHDGJ,
            VNAA,
            VNU,
            VNUM,
            VNGLC,
            VNRE,
            VNEXA,
            VNEXR,
            VNR1,
            VNR2,
            VNR3,
            VNSFX,
            VNODOC,
            VNODCT,
            VNOSFX,
            VNPKCO,
            VNOKCO,
            VNPDCT,
            VNAN8,
            VNCN,
            VNDKJ,
            VNDKC,
            VNASID,
            VNBRE,
            VNRCND,
            VNSUMM,
            VNPRGE,
            VNTNN,
            VNALT1,
            VNALT2,
            VNALT3,
            VNALT4,
            VNALT5,
            VNALT6,
            VNALT7,
            VNALT8,
            VNALT9,
            VNALT0,
            VNALTT,
            VNALTU,
            VNALTV,
            VNALTW,
            VNALTX,
            VNALTZ,
            VNDLNA,
            VNCFF1,
            VNCFF2,
            VNASM,
            VNBC,
            VNVINV,
            VNIVD,
            VNWR01,
            VNPO,
            VNPSFX,
            VNDCTO,
            VNLNID,
            VNWY,
            VNWN,
            VNFNLP,
            VNOPSQ,
            VNJBCD,
            VNJBST,
            VNHMCU,
            VNDOI,
            VNALID,
            VNALTY,
            VNDSVJ,
            VNTORG,
            VNREG#,
            VNPYID,
            VNUSER,
            VNPID,
            VNJOBN,
            VNUPMJ,
            VNUPMT,
            VNCRRM,
            VNACR,
            VNDGM,
            VNDGD,
            VNDGY,
            VNDG#,
            VNDICM,
            VNDICD,
            VNDICY,
            VNDIC#,
            VNDSYM,
            VNDSYD,
            VNDSYY,
            VNDSY#,
            VNDKM,
            VNDKD,
            VNDKY,
            VNDK#,
            VNDSVM,
            VNDSVD,
            VNDSVY,
            VNDSV#,
            VNHDGM,
            VNHDGD,
            VNHDGY,
            VNHDG#,
            VNDKCM,
            VNDKCD,
            VNDKCY,
            VNDKC#,
            VNIVDM,
            VNIVDD,
            VNIVDY,
            VNIVD#,
            VNABR1,
            VNABR2,
            VNABR3,
            VNABR4,
            VNABT1,
            VNABT2,
            VNABT3,
            VNABT4,
            VNITM,
            VNPM01,
            VNPM02,
            VNPM03,
            VNPM04,
            VNPM05,
            VNPM06,
            VNPM07,
            VNPM08,
            VNPM09,
            VNPM10,
            VNBCRC,
            VNEXR1,
            VNTXA1,
            VNTXITM,
            VNACTB,
            VNSTAM,
            VNCTAM,
            VNAG,
            VNAGF,
            VNTKTX,
            VNDLNID,
            VNCKNU,
            VNBUPC,
            VNAHBU,
            VNEPGC,
            VNJPGC,
            VNRC5,
            VNSFXE,
            VNOFM)
            VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            cmd.Parameters.Add(new iDB2Parameter("VNEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNEDTY", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEDSQ", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNEDSQ"].Precision = 2;
            cmd.Parameters["VNEDSQ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("VNEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["VNEDLN"].Precision = 7;
            cmd.Parameters["VNEDLN"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("VNEDTS", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("VNEDFT", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNEDDT"].Precision = 6;
            cmd.Parameters["VNEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNEDER", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEDDL", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["VNEDDL"].Precision = 5;
            cmd.Parameters["VNEDDL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEDTC", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEDTR", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("VNEDGL", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEDAN", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNEDAN"].Precision = 8;
            cmd.Parameters["VNEDAN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VNDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNDOC", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNDOC"].Precision = 8;
            cmd.Parameters["VNDOC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDGJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDGJ"].Precision = 6;
            cmd.Parameters["VNDGJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNJELN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["VNJELN"].Precision = 7;
            cmd.Parameters["VNJELN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNEXTL", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNPOST", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNICU", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNICU"].Precision = 8;
            cmd.Parameters["VNICU"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNICUT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNDICJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDICJ"].Precision = 6;
            cmd.Parameters["VNDICJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSYJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDSYJ"].Precision = 6;
            cmd.Parameters["VNDSYJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNTICU", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNTICU"].Precision = 6;
            cmd.Parameters["VNTICU"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VNANI", iDB2DbType.iDB2Graphic, 29));
            cmd.Parameters.Add(new iDB2Parameter("VNAM", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNAID", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNOBJ", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("VNSUB", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNSBL", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNSBLT", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNLT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNPN", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNPN"].Precision = 2;
            cmd.Parameters["VNPN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNCTRY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNCTRY"].Precision = 2;
            cmd.Parameters["VNCTRY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNFY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNFY"].Precision = 2;
            cmd.Parameters["VNFY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNFQ", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("VNCRCD", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNCRR", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNCRR"].Precision = 15;
            cmd.Parameters["VNCRR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNHCRR", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNHCRR"].Precision = 15;
            cmd.Parameters["VNHCRR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNHDGJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNHDGJ"].Precision = 6;
            cmd.Parameters["VNHDGJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNAA", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNAA"].Precision = 15;
            cmd.Parameters["VNAA"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNU", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNU"].Precision = 15;
            cmd.Parameters["VNU"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNUM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNGLC", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("VNRE", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNEXA", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("VNEXR", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("VNR1", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNR2", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNR3", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNSFX", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNODOC", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNODOC"].Precision = 8;
            cmd.Parameters["VNODOC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNODCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNOSFX", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNPKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VNOKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("VNPDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNAN8", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNAN8"].Precision = 8;
            cmd.Parameters["VNAN8"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNCN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNDKJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDKJ"].Precision = 6;
            cmd.Parameters["VNDKJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKC", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDKC"].Precision = 6;
            cmd.Parameters["VNDKC"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNASID", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VNBRE", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNRCND", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNSUMM", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPRGE", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNTNN", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT1", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT2", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT3", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT4", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT5", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT6", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT7", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT8", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT9", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALT0", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALTT", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALTU", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALTV", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALTW", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALTX", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNALTZ", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNDLNA", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNCFF1", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNCFF2", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNASM", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNBC", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNVINV", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VNIVD", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNIVD"].Precision = 6;
            cmd.Parameters["VNIVD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNWR01", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("VNPO", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("VNPSFX", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNLNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNLNID"].Precision = 6;
            cmd.Parameters["VNLNID"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("VNWY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNWY"].Precision = 2;
            cmd.Parameters["VNWY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNWN", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNWN"].Precision = 2;
            cmd.Parameters["VNWN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNFNLP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNOPSQ", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["VNOPSQ"].Precision = 5;
            cmd.Parameters["VNOPSQ"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNJBCD", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("VNJBST", iDB2DbType.iDB2Graphic, 4));
            cmd.Parameters.Add(new iDB2Parameter("VNHMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNDOI", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDOI"].Precision = 2;
            cmd.Parameters["VNDOI"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNALID", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VNALTY", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNDSVJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDSVJ"].Precision = 6;
            cmd.Parameters["VNDSVJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNTORG", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNREG#", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNREG#"].Precision = 8;
            cmd.Parameters["VNREG#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNPYID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNPYID"].Precision = 15;
            cmd.Parameters["VNPYID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNUPMJ"].Precision = 6;
            cmd.Parameters["VNUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNUPMT"].Precision = 6;
            cmd.Parameters["VNUPMT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNCRRM", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNACR", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNACR"].Precision = 15;
            cmd.Parameters["VNACR"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNDGM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDGM"].Precision = 2;
            cmd.Parameters["VNDGM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDGD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDGD"].Precision = 2;
            cmd.Parameters["VNDGD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDGY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDGY"].Precision = 2;
            cmd.Parameters["VNDGY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDG#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDG#"].Precision = 2;
            cmd.Parameters["VNDG#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDICM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDICM"].Precision = 2;
            cmd.Parameters["VNDICM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDICD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDICD"].Precision = 2;
            cmd.Parameters["VNDICD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDICY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDICY"].Precision = 2;
            cmd.Parameters["VNDICY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDIC#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDIC#"].Precision = 2;
            cmd.Parameters["VNDIC#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSYM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSYM"].Precision = 2;
            cmd.Parameters["VNDSYM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSYD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSYD"].Precision = 2;
            cmd.Parameters["VNDSYD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSYY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSYY"].Precision = 2;
            cmd.Parameters["VNDSYY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSY#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSY#"].Precision = 2;
            cmd.Parameters["VNDSY#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKM"].Precision = 2;
            cmd.Parameters["VNDKM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKD"].Precision = 2;
            cmd.Parameters["VNDKD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKY"].Precision = 2;
            cmd.Parameters["VNDKY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDK#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDK#"].Precision = 2;
            cmd.Parameters["VNDK#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSVM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSVM"].Precision = 2;
            cmd.Parameters["VNDSVM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSVD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSVD"].Precision = 2;
            cmd.Parameters["VNDSVD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSVY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSVY"].Precision = 2;
            cmd.Parameters["VNDSVY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDSV#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDSV#"].Precision = 2;
            cmd.Parameters["VNDSV#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNHDGM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNHDGM"].Precision = 2;
            cmd.Parameters["VNHDGM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNHDGD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNHDGD"].Precision = 2;
            cmd.Parameters["VNHDGD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNHDGY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNHDGY"].Precision = 2;
            cmd.Parameters["VNHDGY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNHDG#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNHDG#"].Precision = 2;
            cmd.Parameters["VNHDG#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKCM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKCM"].Precision = 2;
            cmd.Parameters["VNDKCM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKCD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKCD"].Precision = 2;
            cmd.Parameters["VNDKCD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKCY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKCY"].Precision = 2;
            cmd.Parameters["VNDKCY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNDKC#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNDKC#"].Precision = 2;
            cmd.Parameters["VNDKC#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNIVDM", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNIVDM"].Precision = 2;
            cmd.Parameters["VNIVDM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNIVDD", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNIVDD"].Precision = 2;
            cmd.Parameters["VNIVDD"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNIVDY", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNIVDY"].Precision = 2;
            cmd.Parameters["VNIVDY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNIVD#", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNIVD#"].Precision = 2;
            cmd.Parameters["VNIVD#"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNABR1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNABR2", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNABR3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNABR4", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNABT1", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNABT2", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNABT3", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNABT4", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNITM", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNITM"].Precision = 8;
            cmd.Parameters["VNITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNPM01", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM02", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM03", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM04", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM05", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM06", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM07", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM08", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM09", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNPM10", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNBCRC", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNEXR1", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("VNTXA1", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNTXITM", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["VNTXITM"].Precision = 8;
            cmd.Parameters["VNTXITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNACTB", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("VNSTAM", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNSTAM"].Precision = 15;
            cmd.Parameters["VNSTAM"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNCTAM", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNCTAM"].Precision = 15;
            cmd.Parameters["VNCTAM"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNAG", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNAG"].Precision = 15;
            cmd.Parameters["VNAG"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNAGF", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["VNAGF"].Precision = 15;
            cmd.Parameters["VNAGF"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("VNTKTX", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNDLNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["VNDLNID"].Precision = 6;
            cmd.Parameters["VNDLNID"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("VNCKNU", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("VNBUPC", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("VNAHBU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("VNEPGC", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNJPGC", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("VNRC5", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["VNRC5"].Precision = 5;
            cmd.Parameters["VNRC5"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNSFXE", iDB2DbType.iDB2Decimal, 2));
            cmd.Parameters["VNSFXE"].Precision = 2;
            cmd.Parameters["VNSFXE"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("VNOFM", iDB2DbType.iDB2Graphic, 1));
            return cmd;
        }

        public SqlDmlResponse Save(BusinessSystemSession e1Session, iDB2Transaction trans, BatchJournalEntrys recs)
        {
            var response = new SqlDmlResponse();
            response.StartTime = DateTime.Now;
            var cmd = (e1Session.DbCommands.ContainsKey("BatchJournalEntryMgr.Insert")) ? e1Session.DbCommands["BatchJournalEntryMgr.Insert"] : InsertCmd();
            cmd.Connection = e1Session.DbConnection;
            cmd.Transaction = trans;
            try
            {
                foreach (BatchJournalEntry rec in recs)
                {
                    cmd.Parameters[0].Value = rec.VNEDUS;
                    cmd.Parameters[1].Value = rec.VNEDTY;
                    cmd.Parameters[2].Value = rec.VNEDSQ;
                    cmd.Parameters[3].Value = rec.VNEDTN;
                    cmd.Parameters[4].Value = rec.VNEDCT;
                    cmd.Parameters[5].Value = rec.VNEDLN;
                    cmd.Parameters[6].Value = rec.VNEDTS;
                    cmd.Parameters[7].Value = rec.VNEDFT;
                    cmd.Parameters[8].Value = rec.VNEDDT;
                    cmd.Parameters[9].Value = rec.VNEDER;
                    cmd.Parameters[10].Value = rec.VNEDDL;
                    cmd.Parameters[11].Value = rec.VNEDSP;
                    cmd.Parameters[12].Value = rec.VNEDTC;
                    cmd.Parameters[13].Value = rec.VNEDTR;
                    cmd.Parameters[14].Value = rec.VNEDBT;
                    cmd.Parameters[15].Value = rec.VNEDGL;
                    cmd.Parameters[16].Value = rec.VNEDAN;
                    cmd.Parameters[17].Value = rec.VNKCO;
                    cmd.Parameters[18].Value = rec.VNDCT;
                    cmd.Parameters[19].Value = rec.VNDOC;
                    cmd.Parameters[20].Value = rec.VNDGJ;
                    cmd.Parameters[21].Value = rec.VNJELN;
                    cmd.Parameters[22].Value = rec.VNEXTL;
                    cmd.Parameters[23].Value = rec.VNPOST;
                    cmd.Parameters[24].Value = rec.VNICU;
                    cmd.Parameters[25].Value = rec.VNICUT;
                    cmd.Parameters[26].Value = rec.VNDICJ;
                    cmd.Parameters[27].Value = rec.VNDSYJ;
                    cmd.Parameters[28].Value = rec.VNTICU;
                    cmd.Parameters[29].Value = rec.VNCO;
                    cmd.Parameters[30].Value = rec.VNANI;
                    cmd.Parameters[31].Value = rec.VNAM;
                    cmd.Parameters[32].Value = rec.VNAID;
                    cmd.Parameters[33].Value = rec.VNMCU;
                    cmd.Parameters[34].Value = rec.VNOBJ;
                    cmd.Parameters[35].Value = rec.VNSUB;
                    cmd.Parameters[36].Value = rec.VNSBL;
                    cmd.Parameters[37].Value = rec.VNSBLT;
                    cmd.Parameters[38].Value = rec.VNLT;
                    cmd.Parameters[39].Value = rec.VNPN;
                    cmd.Parameters[40].Value = rec.VNCTRY;
                    cmd.Parameters[41].Value = rec.VNFY;
                    cmd.Parameters[42].Value = rec.VNFQ;
                    cmd.Parameters[43].Value = rec.VNCRCD;
                    cmd.Parameters[44].Value = rec.VNCRR;
                    cmd.Parameters[45].Value = rec.VNHCRR;
                    cmd.Parameters[46].Value = rec.VNHDGJ;
                    cmd.Parameters[47].Value = rec.VNAA;
                    cmd.Parameters[48].Value = rec.VNU;
                    cmd.Parameters[49].Value = rec.VNUM;
                    cmd.Parameters[50].Value = rec.VNGLC;
                    cmd.Parameters[51].Value = rec.VNRE;
                    cmd.Parameters[52].Value = rec.VNEXA;
                    cmd.Parameters[53].Value = rec.VNEXR;
                    cmd.Parameters[54].Value = rec.VNR1;
                    cmd.Parameters[55].Value = rec.VNR2;
                    cmd.Parameters[56].Value = rec.VNR3;
                    cmd.Parameters[57].Value = rec.VNSFX;
                    cmd.Parameters[58].Value = rec.VNODOC;
                    cmd.Parameters[59].Value = rec.VNODCT;
                    cmd.Parameters[60].Value = rec.VNOSFX;
                    cmd.Parameters[61].Value = rec.VNPKCO;
                    cmd.Parameters[62].Value = rec.VNOKCO;
                    cmd.Parameters[63].Value = rec.VNPDCT;
                    cmd.Parameters[64].Value = rec.VNAN8;
                    cmd.Parameters[65].Value = rec.VNCN;
                    cmd.Parameters[66].Value = rec.VNDKJ;
                    cmd.Parameters[67].Value = rec.VNDKC;
                    cmd.Parameters[68].Value = rec.VNASID;
                    cmd.Parameters[69].Value = rec.VNBRE;
                    cmd.Parameters[70].Value = rec.VNRCND;
                    cmd.Parameters[71].Value = rec.VNSUMM;
                    cmd.Parameters[72].Value = rec.VNPRGE;
                    cmd.Parameters[73].Value = rec.VNTNN;
                    cmd.Parameters[74].Value = rec.VNALT1;
                    cmd.Parameters[75].Value = rec.VNALT2;
                    cmd.Parameters[76].Value = rec.VNALT3;
                    cmd.Parameters[77].Value = rec.VNALT4;
                    cmd.Parameters[78].Value = rec.VNALT5;
                    cmd.Parameters[79].Value = rec.VNALT6;
                    cmd.Parameters[80].Value = rec.VNALT7;
                    cmd.Parameters[81].Value = rec.VNALT8;
                    cmd.Parameters[82].Value = rec.VNALT9;
                    cmd.Parameters[83].Value = rec.VNALT0;
                    cmd.Parameters[84].Value = rec.VNALTT;
                    cmd.Parameters[85].Value = rec.VNALTU;
                    cmd.Parameters[86].Value = rec.VNALTV;
                    cmd.Parameters[87].Value = rec.VNALTW;
                    cmd.Parameters[88].Value = rec.VNALTX;
                    cmd.Parameters[89].Value = rec.VNALTZ;
                    cmd.Parameters[90].Value = rec.VNDLNA;
                    cmd.Parameters[91].Value = rec.VNCFF1;
                    cmd.Parameters[92].Value = rec.VNCFF2;
                    cmd.Parameters[93].Value = rec.VNASM;
                    cmd.Parameters[94].Value = rec.VNBC;
                    cmd.Parameters[95].Value = rec.VNVINV;
                    cmd.Parameters[96].Value = rec.VNIVD;
                    cmd.Parameters[97].Value = rec.VNWR01;
                    cmd.Parameters[98].Value = rec.VNPO;
                    cmd.Parameters[99].Value = rec.VNPSFX;
                    cmd.Parameters[100].Value = rec.VNDCTO;
                    cmd.Parameters[101].Value = rec.VNLNID;
                    cmd.Parameters[102].Value = rec.VNWY;
                    cmd.Parameters[103].Value = rec.VNWN;
                    cmd.Parameters[104].Value = rec.VNFNLP;
                    cmd.Parameters[105].Value = rec.VNOPSQ;
                    cmd.Parameters[106].Value = rec.VNJBCD;
                    cmd.Parameters[107].Value = rec.VNJBST;
                    cmd.Parameters[108].Value = rec.VNHMCU;
                    cmd.Parameters[109].Value = rec.VNDOI;
                    cmd.Parameters[110].Value = rec.VNALID;
                    cmd.Parameters[111].Value = rec.VNALTY;
                    cmd.Parameters[112].Value = rec.VNDSVJ;
                    cmd.Parameters[113].Value = rec.VNTORG;
                    cmd.Parameters[114].Value = rec.VNREG_PS;
                    cmd.Parameters[115].Value = rec.VNPYID;
                    cmd.Parameters[116].Value = rec.VNUSER;
                    cmd.Parameters[117].Value = rec.VNPID;
                    cmd.Parameters[118].Value = rec.VNJOBN;
                    cmd.Parameters[119].Value = rec.VNUPMJ;
                    cmd.Parameters[120].Value = rec.VNUPMT;
                    cmd.Parameters[121].Value = rec.VNCRRM;
                    cmd.Parameters[122].Value = rec.VNACR;
                    cmd.Parameters[123].Value = rec.VNDGM;
                    cmd.Parameters[124].Value = rec.VNDGD;
                    cmd.Parameters[125].Value = rec.VNDGY;
                    cmd.Parameters[126].Value = rec.VNDG_PS;
                    cmd.Parameters[127].Value = rec.VNDICM;
                    cmd.Parameters[128].Value = rec.VNDICD;
                    cmd.Parameters[129].Value = rec.VNDICY;
                    cmd.Parameters[130].Value = rec.VNDIC_PS;
                    cmd.Parameters[131].Value = rec.VNDSYM;
                    cmd.Parameters[132].Value = rec.VNDSYD;
                    cmd.Parameters[133].Value = rec.VNDSYY;
                    cmd.Parameters[134].Value = rec.VNDSY_PS;
                    cmd.Parameters[135].Value = rec.VNDKM;
                    cmd.Parameters[136].Value = rec.VNDKD;
                    cmd.Parameters[137].Value = rec.VNDKY;
                    cmd.Parameters[138].Value = rec.VNDK_PS;
                    cmd.Parameters[139].Value = rec.VNDSVM;
                    cmd.Parameters[140].Value = rec.VNDSVD;
                    cmd.Parameters[141].Value = rec.VNDSVY;
                    cmd.Parameters[142].Value = rec.VNDSV_PS;
                    cmd.Parameters[143].Value = rec.VNHDGM;
                    cmd.Parameters[144].Value = rec.VNHDGD;
                    cmd.Parameters[145].Value = rec.VNHDGY;
                    cmd.Parameters[146].Value = rec.VNHDG_PS;
                    cmd.Parameters[147].Value = rec.VNDKCM;
                    cmd.Parameters[148].Value = rec.VNDKCD;
                    cmd.Parameters[149].Value = rec.VNDKCY;
                    cmd.Parameters[150].Value = rec.VNDKC_PS;
                    cmd.Parameters[151].Value = rec.VNIVDM;
                    cmd.Parameters[152].Value = rec.VNIVDD;
                    cmd.Parameters[153].Value = rec.VNIVDY;
                    cmd.Parameters[154].Value = rec.VNIVD_PS;
                    cmd.Parameters[155].Value = rec.VNABR1;
                    cmd.Parameters[156].Value = rec.VNABR2;
                    cmd.Parameters[157].Value = rec.VNABR3;
                    cmd.Parameters[158].Value = rec.VNABR4;
                    cmd.Parameters[159].Value = rec.VNABT1;
                    cmd.Parameters[160].Value = rec.VNABT2;
                    cmd.Parameters[161].Value = rec.VNABT3;
                    cmd.Parameters[162].Value = rec.VNABT4;
                    cmd.Parameters[163].Value = rec.VNITM;
                    cmd.Parameters[164].Value = rec.VNPM01;
                    cmd.Parameters[165].Value = rec.VNPM02;
                    cmd.Parameters[166].Value = rec.VNPM03;
                    cmd.Parameters[167].Value = rec.VNPM04;
                    cmd.Parameters[168].Value = rec.VNPM05;
                    cmd.Parameters[169].Value = rec.VNPM06;
                    cmd.Parameters[170].Value = rec.VNPM07;
                    cmd.Parameters[171].Value = rec.VNPM08;
                    cmd.Parameters[172].Value = rec.VNPM09;
                    cmd.Parameters[173].Value = rec.VNPM10;
                    cmd.Parameters[174].Value = rec.VNBCRC;
                    cmd.Parameters[175].Value = rec.VNEXR1;
                    cmd.Parameters[176].Value = rec.VNTXA1;
                    cmd.Parameters[177].Value = rec.VNTXITM;
                    cmd.Parameters[178].Value = rec.VNACTB;
                    cmd.Parameters[179].Value = rec.VNSTAM;
                    cmd.Parameters[180].Value = rec.VNCTAM;
                    cmd.Parameters[181].Value = rec.VNAG;
                    cmd.Parameters[182].Value = rec.VNAGF;
                    cmd.Parameters[183].Value = rec.VNTKTX;
                    cmd.Parameters[184].Value = rec.VNDLNID;
                    cmd.Parameters[185].Value = rec.VNCKNU;
                    cmd.Parameters[186].Value = rec.VNBUPC;
                    cmd.Parameters[187].Value = rec.VNAHBU;
                    cmd.Parameters[188].Value = rec.VNEPGC;
                    cmd.Parameters[189].Value = rec.VNJPGC;
                    cmd.Parameters[190].Value = rec.VNRC5;
                    cmd.Parameters[191].Value = rec.VNSFXE;
                    cmd.Parameters[192].Value = rec.VNOFM;
                    int rows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = "F0911Z1: " + e.ToString();
            }
            finally
            {
                response.EndTime = DateTime.Now;
            }
            return response;
        }
    }
}
