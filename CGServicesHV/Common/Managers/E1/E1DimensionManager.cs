using System;
using System.Collections.Generic;
using System.Data;
//using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Utilities;
using NLog;

// ReSharper disable InconsistentNaming

namespace CGServicesHV.Common.Managers.E1
{
    public class E1DimensionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
     * Dimension Attributes
     *******************************************************************************/
        public ItemAttributes GetDimensionAttributes(BusinessSystemSession session, ItemAttributes atts)
        {
            var itemAtts = new ItemAttributes();
            //EdiAttribute ediAtt;
            string[] decSegments = { "Z581BDEC", "Z581LDEC", "Z581RDEC", "Z581TDEC", "Z5811DEC", "Z5812DEC" };
            string[] mmSegments = { "Z581BMIM", "Z581LMIM", "Z581RMIM", "Z581TMIM", "Z5811MIM", "Z5812MIM" };
            string[] baseSegments = { "Z58L1BDM", "Z58L1LDM", "Z58L1RDM", "Z58L1TDM", "Z58L11DM", "Z58L12DM" };
            string[] numSegments = { "Z58L1BNM", "Z58L1LNM", "Z58L1RNM", "Z58L1TNM", "Z58L11NM", "Z58L12NM" };
            string[] denSegments = { "Z58L1BDN", "Z58L1LDN", "Z58L1RDN", "Z58L1TDN", "Z58L11DN", "Z58L12DN" };
            string[] frcSegments = { "Z5895774", "Z5895775", "Z5895776", "Z5895777", "Z5895778", "Z5895779" };
            try
            {

                // decimals
                if (atts.Contains("Z581BDEC"))
                {
                    for (int i = 0; i < decSegments.Length; i++)
                    {
                        if (atts.Contains(decSegments[i]))
                        {
                            var decValue = atts.GetDecimalAttributeValue(decSegments[i]);
                            var mmValue = Convert.ToDouble(decValue) * 25.400;

                            // Decimal Segment
                            var itemAtt1 = new ItemAttribute();
                            itemAtt1.Name = decSegments[i];
                            itemAtt1.Value = decValue.ToString();
                            itemAtts.Add(itemAtt1);

                            // Millimeter Segment
                            var itemAtt2 = new ItemAttribute();
                            itemAtt2.Name = mmSegments[i];
                            itemAtt2.Value = mmValue.ToString();
                            itemAtts.Add(itemAtt2);

                            // Base Segment
                            var itemAtt3 = new ItemAttribute();
                            itemAtt3.Name = baseSegments[i];
                            itemAtt3.Value = Math.Floor(Convert.ToDouble(decValue)).ToString();
                            itemAtts.Add(itemAtt3);

                            // Base - Fractional
                            var frcDecimal = decValue - Math.Floor(decValue);
                            var fracsDict = GetFractionalValues(session, frcDecimal);

                            // Numerator Segment
                            var itemAtt4 = new ItemAttribute();
                            itemAtt4.Name = numSegments[i];
                            itemAtt4.Value = Convert.ToDecimal(fracsDict["FDZ58NUMR"]).ToString();
                            itemAtts.Add(itemAtt4);

                            // Denominator
                            var itemAtt5 = new ItemAttribute();
                            itemAtt5.Name = denSegments[i];
                            itemAtt5.Value = Convert.ToDecimal(fracsDict["FDZ58DENM"]).ToString();
                            itemAtts.Add(itemAtt5);

                        }
                    }
                }
                else
                {
                    // Millimeters
                    if (atts.Contains("Z581BMIM"))
                    {
                        for (int i = 0; i < mmSegments.Length; i++)
                        {
                            if (atts.Contains(mmSegments[i]))
                            {
                                var mmValue = atts.GetDecimalAttributeValue(mmSegments[i]);
                                var decValue = Math.Round((mmValue / (decimal)25.400), 6);

                                // Decimal Segment
                                var itemAtt1 = new ItemAttribute();
                                itemAtt1.Name = decSegments[i];
                                itemAtt1.Value = decValue.ToString();
                                itemAtts.Add(itemAtt1);

                                // Millimeter Segment
                                var itemAtt2 = new ItemAttribute();
                                itemAtt2.Name = mmSegments[i];
                                itemAtt2.Value = mmValue.ToString();
                                itemAtts.Add(itemAtt2);

                                // Base Segment
                                var itemAtt3 = new ItemAttribute();
                                itemAtt3.Name = baseSegments[i];
                                itemAtt3.Value = Math.Floor(Convert.ToDouble(decValue)).ToString();
                                itemAtts.Add(itemAtt3);

                                // Base - Fractional
                                var frcDecimal = decValue - Math.Floor(decValue);
                                var fracsDict = GetFractionalValues(session, frcDecimal);

                                // Numerator Segment
                                var itemAtt4 = new ItemAttribute();
                                itemAtt4.Name = numSegments[i];
                                itemAtt4.Value = Convert.ToDecimal(fracsDict["FDZ58NUMR"]).ToString();
                                itemAtts.Add(itemAtt4);

                                // Denominator
                                var itemAtt5 = new ItemAttribute();
                                itemAtt5.Name = denSegments[i];
                                itemAtt5.Value = Convert.ToDecimal(fracsDict["FDZ58DENM"]).ToString();
                                itemAtts.Add(itemAtt5);

                            }
                        }
                    }
                    // base, num, den
                    else
                    {
                        if (atts.Contains("Z58L1BDM"))
                        {
                            for (int i = 0; i < baseSegments.Length; i++)
                            {
                                if (atts.Contains(baseSegments[i]))
                                {
                                    var decValue = atts.GetDecimalAttributeValue(baseSegments[i]);

                                    if (atts.Contains(numSegments[i]) || itemAtts.Contains(denSegments[i]))
                                    {
                                        var fracsDict1 = GetFractionalValues(session, atts.GetDecimalAttributeValue(numSegments[i]), atts.GetDecimalAttributeValue(denSegments[i]));
                                        decValue = decValue + (decimal)fracsDict1["FDZ58DECE"];
                                    }
                                    else
                                    {
                                        if (atts.Contains(frcSegments[i]))
                                        {
                                            var fracsDict2 = GetFractionalValues(session, atts.GetStringAttributeValue(frcSegments[i]));
                                            decValue = decValue + (decimal)fracsDict2["FDZ58DECE"];
                                        }
                                    }

                                    var mmValue = Convert.ToDouble(decValue) * 25.400;

                                    // Decimal Segment
                                    var itemAtt1 = new ItemAttribute();
                                    itemAtt1.Name = decSegments[i];
                                    itemAtt1.Value = decValue.ToString();
                                    itemAtts.Add(itemAtt1);

                                    // Millimeter Segment
                                    var itemAtt2 = new ItemAttribute();
                                    itemAtt2.Name = mmSegments[i];
                                    itemAtt2.Value = mmValue.ToString();
                                    itemAtts.Add(itemAtt2);

                                    // Base Segment
                                    var itemAtt3 = new ItemAttribute();
                                    itemAtt3.Name = baseSegments[i];
                                    itemAtt3.Value = Math.Floor(Convert.ToDouble(decValue)).ToString();
                                    itemAtts.Add(itemAtt3);

                                    // Base - Fractional
                                    var frcDecimal = decValue - Math.Floor(decValue);
                                    var fracsDict = GetFractionalValues(session, frcDecimal);

                                    // Numerator Segment
                                    var itemAtt4 = new ItemAttribute();
                                    itemAtt4.Name = numSegments[i];
                                    itemAtt4.Value = Convert.ToDecimal(fracsDict["FDZ58NUMR"]).ToString();
                                    itemAtts.Add(itemAtt4);

                                    // Denominator
                                    var itemAtt5 = new ItemAttribute();
                                    itemAtt5.Name = denSegments[i];
                                    itemAtt5.Value = Convert.ToDecimal(fracsDict["FDZ58DENM"]).ToString();
                                    itemAtts.Add(itemAtt5);

                                }
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return itemAtts;
        }

        private static iDB2Command GetFractionalByDecimal()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
                FDZ58DECE,
                FDZ58FRAC,
                FDZ58NUMR,
                FDZ58DENM
            FROM
                F580067
            WHERE
                FDZ58LODR <= ? AND
                FDZ58HIDR >= ?");
            cmd.Parameters.Add(new iDB2Parameter("Fractional1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Fractional1"].Precision = 15;
            cmd.Parameters["Fractional1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Fractional2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Fractional2"].Precision = 8;
            cmd.Parameters["Fractional2"].Scale = 0;
            return cmd;
        }

        public static Dictionary<string, object> GetFractionalValues(BusinessSystemSession session, decimal fractional)
        {
            var dict = new Dictionary<string, object>();
            try
            {
                iDB2Command cmd = GetFractionalByDecimal();
                cmd.Parameters[0].Value = Decimal.Round(fractional, 3) * 1000000;
                cmd.Parameters[1].Value = Decimal.Round(fractional, 3) * 1000000;
                cmd.Connection = session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        dict.Add("FDZ58DECE", Data.CastDbNull.To<Decimal>(reader["FDZ58DECE"], 0) / 1000000);
                        dict.Add("FDZ58FRAC", Data.CastDbNull.To<String>(reader["FDZ58FRAC"], String.Empty).Trim());
                        dict.Add("FDZ58NUMR", Data.CastDbNull.To<Decimal>(reader["FDZ58NUMR"], 0));
                        dict.Add("FDZ58DENM", Data.CastDbNull.To<Decimal>(reader["FDZ58DENM"], 0));
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return dict;
        }

        public iDB2Command GetFractionalByInteger()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
                FDZ58DECE,
                FDZ58FRAC,
                FDZ58NUMR,
                FDZ58DENM
            FROM
                F580067
            WHERE
                FDZ58NUMR = ? AND
                FDZ58DENM = ?");
            cmd.Parameters.Add(new iDB2Parameter("Z58NUMR", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58NUMR"].Precision = 2;
            cmd.Parameters["Z58NUMR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Z58DENM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58DENM"].Precision = 2;
            cmd.Parameters["Z58DENM"].Scale = 0;
            return cmd;
        }

        public Dictionary<string, object> GetFractionalValues(BusinessSystemSession session, decimal numer, decimal denom)
        {
            var dict = new Dictionary<string, object>();
            try
            {
                iDB2Command cmd = (session.DbCommands.ContainsKey("ItemAttributeManager.GetFractionalByInteger"))
                    ? session.DbCommands["ItemAttributeManager.GetFractionalByInteger"]
                    : GetFractionalByInteger();
                cmd.Parameters[0].Value = numer;
                cmd.Parameters[1].Value = denom;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        dict.Add("FDZ58DECE", Data.CastDbNull.To<Decimal>(reader["FDZ58DECE"], 0) / 1000000);
                        dict.Add("FDZ58FRAC", Data.CastDbNull.To<String>(reader["FDZ58FRAC"], String.Empty).Trim());
                        dict.Add("FDZ58NUMR", Data.CastDbNull.To<Decimal>(reader["FDZ58NUMR"], 0));
                        dict.Add("FDZ58DENM", Data.CastDbNull.To<Decimal>(reader["FDZ58DENM"], 0));
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return dict;
        }

        public iDB2Command GetFractionalByString()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
                FDZ58DECE,
                FDZ58FRAC,
                FDZ58NUMR,
                FDZ58DENM
            FROM
                F580067
            WHERE
                FDZ58FRAC = ?");
            cmd.Parameters.Add(new iDB2Parameter("Z58FRAC", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        public Dictionary<string, object> GetFractionalValues(BusinessSystemSession session, string frcValue)
        {
            var dict = new Dictionary<string, object>();
            try
            {
                iDB2Command cmd = (session.DbCommands.ContainsKey("ItemAttributeManager.GetFractionalByString"))
                    ? session.DbCommands["ItemAttributeManager.GetFractionalByString"]
                    : GetFractionalByString();
                cmd.Parameters[0].Value = frcValue;
                cmd.Connection = session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        dict.Add("FDZ58DECE", Data.CastDbNull.To<Decimal>(reader["FDZ58DECE"], 0) / 1000000);
                        dict.Add("FDZ58FRAC", Data.CastDbNull.To<String>(reader["FDZ58FRAC"], String.Empty).Trim());
                        dict.Add("FDZ58NUMR", Data.CastDbNull.To<Decimal>(reader["FDZ58NUMR"], 0));
                        dict.Add("FDZ58DENM", Data.CastDbNull.To<Decimal>(reader["FDZ58DENM"], 0));
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return dict;
        }
    }
}
