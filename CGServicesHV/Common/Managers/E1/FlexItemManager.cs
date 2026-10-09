using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class FlexItemManager
    {

        public DataTable dt;

        public FlexItemManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("FlexItemManager.GetDescription", GetDescription());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        public FlexItemManager(BusinessSystemSession session, string programId)
        {
            dt = getDataTable(session, programId);
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("FlexItemManager.GetDescription", GetDescription());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /********************************************************************************
        * Commands
        *******************************************************************************/
        public iDB2Command GetDescription()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            *
            FROM
                F585810
            WHERE
	            DRPID = ?
            ORDER BY
                DRSEQ";
            cmd.Parameters.Add(new iDB2Parameter("ProgramId", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        public DataTable getDataTable(BusinessSystemSession session, string programId)
        {
            var dt = new DataTable();
            dt.Columns.Add("DRZ58ATNM", typeof(string));
            dt.Columns.Add("DRZ58APFX", typeof(string));
            dt.Columns.Add("DRZ58APFXL", typeof(decimal));
            dt.Columns.Add("DRZ58ASFX", typeof(string));
            dt.Columns.Add("DRZ58ASFXL", typeof(decimal));
            dt.Columns.Add("DRZ58ACHS", typeof(string));
            dt.Columns.Add("DRZ58AHSV", typeof(string));
            dt.Columns.Add("DRZ58CRLF", typeof(string));
            dt.Columns.Add("DRZ58ADSPU", typeof(string));
            try
            {
                var cmd = new iDB2Command();
                cmd = (session.DbCommands.ContainsKey("FlexItemManager.GetDescription")) ? session.DbCommands["FlexItemManager.GetDescription"] : GetDescription();
                cmd.Parameters[0].Value = programId;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataRow dr = dt.NewRow();
                        dr["DRZ58ATNM"] = Data.CastDbNull.To<String>(reader["DRZ58ATNM"], String.Empty).Trim();
                        dr["DRZ58APFX"] = Data.CastDbNull.To<String>(reader["DRZ58APFX"], String.Empty).TrimEnd();
                        dr["DRZ58APFXL"] = Data.CastDbNull.To<Decimal>(reader["DRZ58APFXL"], 0);
                        dr["DRZ58ASFX"] = Data.CastDbNull.To<String>(reader["DRZ58ASFX"], String.Empty).Trim();
                        dr["DRZ58ASFXL"] = Data.CastDbNull.To<Decimal>(reader["DRZ58ASFXL"], 0);
                        dr["DRZ58ACHS"] = Data.CastDbNull.To<String>(reader["DRZ58ACHS"], String.Empty).Trim();
                        dr["DRZ58AHSV"] = Data.CastDbNull.To<String>(reader["DRZ58AHSV"], String.Empty).Trim();
                        dr["DRZ58CRLF"] = Data.CastDbNull.To<String>(reader["DRZ58CRLF"], String.Empty).Trim();
                        dr["DRZ58ADSPU"] = Data.CastDbNull.To<String>(reader["DRZ58ADSPU"], String.Empty).Trim();
                        dt.Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }

            return dt;
        }

        /********************************************************************************
        * Methods
        *******************************************************************************/
        public List<String> GetDescription(BusinessSystemSession session, string programId, ItemAttributes attributes)
        {
            var lines = new List<String>();
            var sb = new StringBuilder();
            try
            {
                var cmd = new iDB2Command();
                cmd = (session.DbCommands.ContainsKey("FlexItemManager.GetDescription")) ? session.DbCommands["FlexItemManager.GetDescription"] : GetDescription();
                cmd.Parameters[0].Value = programId;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var attName = Data.CastDbNull.To<String>(reader["DRZ58ATNM"], String.Empty).Trim();
                        var prefix = Data.CastDbNull.To<String>(reader["DRZ58APFX"], String.Empty).TrimEnd();
                        var prefixLength = Data.CastDbNull.To<Decimal>(reader["DRZ58APFXL"], 0);
                        var suffix = Data.CastDbNull.To<String>(reader["DRZ58ASFX"], String.Empty).Trim();
                        var suffixLength = Data.CastDbNull.To<Decimal>(reader["DRZ58ASFXL"], 0);
                        var useIgnoreString = Data.CastDbNull.To<String>(reader["DRZ58ACHS"], String.Empty).Trim();
                        var ignoreString = Data.CastDbNull.To<String>(reader["DRZ58AHSV"], String.Empty).Trim();
                        var endLine = Data.CastDbNull.To<String>(reader["DRZ58CRLF"], String.Empty).Trim();
                        var displayType = Data.CastDbNull.To<String>(reader["DRZ58ADSPU"], String.Empty).Trim();
                        if (attributes.Contains(attName))
                        {
                            if (attributes[attName].Value != ignoreString || useIgnoreString == "N")
                            {
                                if (displayType == "1")
                                {
                                    sb.Append(prefix);
                                    sb.Append("".PadLeft(((int) prefixLength - prefix.Length), ' '));
                                    sb.Append(attributes[attName].Desc1);
                                    sb.Append(suffix);
                                    sb.Append("".PadLeft(((int) suffixLength - suffix.Length), ' '));
                                }
                                else
                                {
                                    if (displayType == "2")
                                    {
                                        sb.Append(prefix);
                                        sb.Append("".PadLeft(((int) prefixLength - prefix.Length), ' '));
                                        sb.Append(attributes[attName].Desc2);
                                        sb.Append(suffix);
                                        sb.Append("".PadLeft(((int) suffixLength - suffix.Length), ' '));
                                    }
                                    else
                                    {
                                        sb.Append(prefix);
                                        sb.Append("".PadLeft(((int) prefixLength - prefix.Length), ' '));
                                        sb.Append(attributes[attName].Value);
                                        sb.Append(suffix);
                                        sb.Append("".PadLeft(((int) suffixLength - suffix.Length), ' '));
                                    }
                                }
                            }
                        }

                        if (endLine == "Y")
                        {
                            lines.Add(sb.ToString());
                            sb.Clear();
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                var x = e;
                sb.Append(" [ERROR IN FLEX DESC]");
                lines.Add(sb.ToString());
                //throw new ApplicationException(err.ToString());
            }

            return lines;
        }


        public List<String> GetDescription(BusinessSystemSession session, ItemAttributes attributes)
        {
            var lines = new List<String>();
            var sb = new StringBuilder();
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    var attName = (string)row["DRZ58ATNM"];
                    var prefix = (string)row["DRZ58APFX"];
                    var prefixLength = (decimal)row["DRZ58APFXL"];
                    var suffix = (string)row["DRZ58ASFX"];
                    var suffixLength = (decimal)row["DRZ58ASFXL"];
                    var useIgnoreString = (string)row["DRZ58ACHS"];
                    var ignoreString = (string)row["DRZ58AHSV"];
                    var endLine = (string)row["DRZ58CRLF"];
                    var displayType = (string)row["DRZ58ADSPU"];

                    if (attributes.Contains(attName))
                    {
                        if (attributes[attName].Value != ignoreString || useIgnoreString == "N")
                        {
                            if (displayType == "1")
                            {
                                sb.Append(prefix);
                                sb.Append("".PadLeft(((int)prefixLength - prefix.Length), ' '));
                                sb.Append(attributes[attName].Desc1);
                                sb.Append(suffix);
                                sb.Append("".PadLeft(((int)suffixLength - suffix.Length), ' '));
                            }
                            else
                            {
                                if (displayType == "2")
                                {
                                    sb.Append(prefix);
                                    sb.Append("".PadLeft(((int)prefixLength - prefix.Length), ' '));
                                    sb.Append(attributes[attName].Desc2);
                                    sb.Append(suffix);
                                    sb.Append("".PadLeft(((int)suffixLength - suffix.Length), ' '));
                                }
                                else
                                {
                                    sb.Append(prefix);
                                    sb.Append("".PadLeft(((int)prefixLength - prefix.Length), ' '));
                                    sb.Append(attributes[attName].Value);
                                    sb.Append(suffix);
                                    sb.Append("".PadLeft(((int)suffixLength - suffix.Length), ' '));
                                }
                            }
                        }
                    }
                    if (endLine == "Y")
                    {
                        lines.Add(sb.ToString());
                        sb.Clear();
                    }
                }
            }
            catch (Exception)
            {
                sb.Append(" [ERROR IN FLEX DESC]");
                lines.Add(sb.ToString());
                //throw new ApplicationException(err.ToString());
            }
            return lines;
        }



    }
}
