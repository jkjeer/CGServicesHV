using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using CGServices.Common.Managers.E1;
using NLog;

namespace CGServices.Common.Managers.Enterprise
{
    public class AddressBookManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public AddressBookManager(BusinessSystemSession e1Session)
        //{
        //}

        /*********************************************************************************
         * Get Address Book Locations Dynamic
         *******************************************************************************/
        private static iDB2Command GetAddressBookLocationsDynamic(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
	                    ABAN8,
                        ABAT1,
                        DRDL01,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR,
                        WPAR1,
                        WPPH1,
                        COALESCE(MAPA8,ABAN8) AS MAPA8,
                        ABAC05,
                        ABAC29,
                        ABUPMJ,
                        ABUPMT
                    FROM                
                        F0101
	                    LEFT OUTER JOIN F0116 
                        ON  ALAN8 = ABAN8 AND
				            ALEFTB = ABEFTB
	                    LEFT OUTER JOIN F0115 
                        ON  WPAN8 = ABAN8 AND
				            WPIDLN = 0 AND
				            WPRCK7 = 1 AND
				            WPCNLN = 0
	                    LEFT OUTER JOIN F0150 
                        ON  MAAN8 = ABAN8
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                        ON  DRSY = '01' AND
                            DRRT = 'ST' AND
                            TRIM(DRKY) = ABAT1
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Get Address Book Address Only Dynamic
         *******************************************************************************/
        private static iDB2Command GetAddressBookAddressDynamic(List<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	                SELECT 
	                    ABAN8,
                        ABAT1,
                        ABALPH,
                        ALADD1,
                        ALADD2,
                        ALADD3,
                        ALADD4,
                        ALADDZ,
                        ALCTY1,
                        ALADDS,
                        ALCTR,
                        ABAC05,
                        ABAC29,
                        ABUPMJ,
                        ABUPMT
                    FROM                
                        F0101
	                    LEFT OUTER JOIN F0116 
                        ON  ALAN8 = ABAN8 AND
				            ALEFTB = ABEFTB
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
         * Methods
         ********************************************************************************/
        public static AddressBookLocations GetAddressBookLocations(BusinessSystemSession e1Session, ProcessRunSpan processRunSpan, string[] searchTypes = null, decimal addressNum = 0)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var addressBookLocations = new AddressBookLocations();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Search Types
                if (searchTypes != null && searchTypes.Any())
                {
                    var strInClause = "";
                    foreach (var productCode in searchTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("SearchTypes", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = productCode;
                    }
                    filters.Add("ABAT1 IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }
                // Address Num
                if (addressNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = addressNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("MAUPMJ", "MAUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetAddressBookLocationsDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetAddressBookLocationsDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var location = new AddressBookLocation
                    {
                        AddressNum = CastDbNull.To<decimal>(reader["ABAN8"], 0),
                        Name = CastDbNull.To(reader["ABALPH"], string.Empty).Trim(),
                        AddressType = CastDbNull.To(reader["ABAT1"], string.Empty).Trim(),
                        AddressTypeName = CastDbNull.To(reader["DRDL01"], string.Empty).Trim(),
                        NationalAccountGroup = CastDbNull.To(reader["ABAC05"], string.Empty).Trim(),
                        Address1 = CastDbNull.To(reader["ALADD1"], string.Empty).Trim(),
                        Address2 = CastDbNull.To(reader["ALADD2"], string.Empty).Trim(),
                        Address3 = CastDbNull.To(reader["ALADD3"], string.Empty).Trim(),
                        Address4 = CastDbNull.To(reader["ALADD4"], string.Empty).Trim(),
                        PostalCode = CastDbNull.To(reader["ALADDZ"], string.Empty).Trim(),
                        City = CastDbNull.To(reader["ALCTY1"], string.Empty).Trim(),
                        State = CastDbNull.To(reader["ALADDS"], string.Empty).Trim(),
                        Country = CastDbNull.To(reader["ALCTR"], string.Empty).Trim(),
                        AreaCode = CastDbNull.To(reader["WPAR1"], string.Empty).Trim(),
                        Phone = CastDbNull.To(reader["WPPH1"], string.Empty).Trim(),
                        UpdateDateId = CastDbNull.To<decimal>(reader["ABUPMJ"], 0),
                        UpdateTimeId = CastDbNull.To<decimal>(reader["ABUPMT"], 0)
                    };
                    var parentAddressNum = CastDbNull.To<decimal>(reader["MAPA8"], 0);
                    location.ParentAddressNum = (parentAddressNum == 0) ? location.AddressNum : parentAddressNum;
                    var company = CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                    location.Company = (company == "") ? 0 : Convert.ToDecimal(company);
                    location.UpdateDateTime = Utilities.Common.CastToDateTime(location.UpdateDateId, location.UpdateTimeId);
                    addressBookLocations.Add(location);
                }
                reader.Close();
                Logger.Trace("ROWS: " + addressBookLocations.Count);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return addressBookLocations;
        }

        public Address GetAddressBookAddress(BusinessSystemSession e1Session, decimal addressNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var address = new Address();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Address Num
                if (addressNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AddressNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = addressNum;
                }
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetAddressBookAddressDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetAddressBookAddressDynamic(filters);
                }
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var counter = 0;
                while (reader.Read())
                {
                    address.AddressNum = CastDbNull.To<decimal>(reader["ABAN8"], 0);
                    address.Name = CastDbNull.To(reader["ABALPH"], string.Empty).Trim();
                    address.AddressType = CastDbNull.To(reader["ABAT1"], string.Empty).Trim();
                    address.NationalAccountGroup = CastDbNull.To(reader["ABAC05"], string.Empty).Trim();
                    address.Address1 = CastDbNull.To(reader["ALADD1"], string.Empty).Trim();
                    address.Address2 = CastDbNull.To(reader["ALADD2"], string.Empty).Trim();
                    address.Address3 = CastDbNull.To(reader["ALADD3"], string.Empty).Trim();
                    address.Address4 = CastDbNull.To(reader["ALADD4"], string.Empty).Trim();
                    address.PostalCode = CastDbNull.To(reader["ALADDZ"], string.Empty).Trim();
                    address.City = CastDbNull.To(reader["ALCTY1"], string.Empty).Trim();
                    address.State = CastDbNull.To(reader["ALADDS"], string.Empty).Trim();
                    address.Country = CastDbNull.To(reader["ALCTR"], string.Empty).Trim();
                    address.UpdateDateId = CastDbNull.To<decimal>(reader["ABUPMJ"], 0);
                    address.UpdateTimeId = CastDbNull.To<decimal>(reader["ABUPMT"], 0);
                    var company = CastDbNull.To(reader["ABAC29"], string.Empty).Trim();
                    address.Company = (company == "") ? 0 : Convert.ToDecimal(company);
                    address.UpdateDateTime = Utilities.Common.CastToDateTime(address.UpdateDateId, address.UpdateTimeId);
                    counter++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + counter);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return address;
        }
    }
}

