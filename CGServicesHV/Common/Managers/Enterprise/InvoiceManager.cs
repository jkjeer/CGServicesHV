using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class InvoiceManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CustomerManager _customerManager;
        private readonly PlantManager _plantManager;

        public InvoiceManager()
        {
        }

        public InvoiceManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"InvoiceManager.GetAaiAccount", GetAaiAccount()},
                {"InvoiceManager.IsExistingF03B11", IsExistingF03B11()},
                {"InvoiceManager.IsExistingF03B11Z1", IsExistingF03B11Z1()},
                {"InvoiceManager.IsExistingF0911Z1", IsExistingF0911Z1()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            _customerManager = new CustomerManager(e1Session);
            _plantManager = new PlantManager(e1Session);
        }

        /******************************************************************
        * Get Invoices
        ******************************************************************/
        //private string getInvoiceSelectClause = @"
        //    IV.PLANT_CD ,
        //    PT.PLANT_NO,
        //    IV.CUST_NO,
        //    ISNULL(CX.E1_SOLD_CUST_NO, 0) AS CUST_SOLD_NO,
        //    ISNULL(CX.E1_SHIP_CUST_NO, 0) AS CUST_SHIP_NO,
        //    --CX.E1_TERMS_CD,
        //    ISNULL(TX.TERMS_CD, 'ARD') AS E1_TERMS_CD,  
        //    IV.REF_NO,
        //    IV.INVOICE_ID ,
        //    IV.TRAN_DATE ,
        //    IV.DUE_DATE,
        //    IV.ORIG_TRAN_DATE ,
        //    IV.TRAN_TYPE ,
        //    IV.E1_ORDER_TYPE,
        //    IV.GROSS_AMT ,
        //    IV.PAID_AMT ,
        //    IV.OPEN_AMT ,
        //    IV.TRANSFER_BATCH_NO ,
        //    IV.TRANSFER_STATUS ,
        //    IV.TRANSFER_MSG ,
        //    IV.TRANSFER_DATE ,
        //    IV.WRITTEN_DATE";

        public SqlCommand GetInvoicesById()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        IV.PLANT_CD ,
                        PT.PLANT_NO,
                        IV.CUST_NO,
                        ISNULL(CX.E1_SOLD_CUST_NO, 0) AS CUST_SOLD_NO,
                        ISNULL(CX.E1_SHIP_CUST_NO, 0) AS CUST_SHIP_NO,
                        --CX.E1_TERMS_CD,
                        ISNULL(TX.TERMS_CD, 'ARD') AS E1_TERMS_CD,  
                        IV.REF_NO,
                        IV.INVOICE_ID ,
                        IV.TRAN_DATE ,
                        IV.DUE_DATE,
                        IV.ORIG_TRAN_DATE ,
                        IV.TRAN_TYPE ,
                        IV.E1_ORDER_TYPE,
                        IV.GROSS_AMT ,
                        IV.PAID_AMT ,
                        IV.OPEN_AMT ,
                        IV.TRANSFER_BATCH_NO ,
                        IV.TRANSFER_STATUS ,
                        IV.TRANSFER_MSG ,
                        IV.TRANSFER_DATE ,
                        IV.WRITTEN_DATE
                    FROM
                        CT.INVOICE IV
                        INNER JOIN CT.PLANT PT
                        ON PT.PLANT_CD = IV.PLANT_CD
                        LEFT OUTER JOIN CT.CUSTOMER_XREF CX
                        ON CX.PLANT_CD = IV.PLANT_CD AND
                        CX.CUST_NO = IV.CUST_NO
                        LEFT OUTER JOIN CT.TERMS_XREF TX
                        ON TX.TERMS_DESC = IV.TERMS
                    WHERE
                        IV.INVOICE_ID = @InvoiceId";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int));
            return cmd;
        }

        public SqlCommand GetInvoicesByBatch()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        IV.PLANT_CD ,
                        PT.PLANT_NO,
                        IV.CUST_NO,
                        ISNULL(CX.E1_SOLD_CUST_NO, 0) AS CUST_SOLD_NO,
                        ISNULL(CX.E1_SHIP_CUST_NO, 0) AS CUST_SHIP_NO,
                        --CX.E1_TERMS_CD,
                        ISNULL(TX.TERMS_CD, 'ARD') AS E1_TERMS_CD,  
                        IV.REF_NO,
                        IV.INVOICE_ID ,
                        IV.TRAN_DATE ,
                        IV.DUE_DATE,
                        IV.ORIG_TRAN_DATE ,
                        IV.TRAN_TYPE ,
                        IV.E1_ORDER_TYPE,
                        IV.GROSS_AMT ,
                        IV.PAID_AMT ,
                        IV.OPEN_AMT ,
                        IV.TRANSFER_BATCH_NO ,
                        IV.TRANSFER_STATUS ,
                        IV.TRANSFER_MSG ,
                        IV.TRANSFER_DATE ,
                        IV.WRITTEN_DATE
                    FROM
                        CT.INVOICE IV
                        INNER JOIN CT.PLANT PT
                        ON PT.PLANT_CD = IV.PLANT_CD
                        LEFT OUTER JOIN CT.CUSTOMER_XREF CX
                        ON CX.PLANT_CD = IV.PLANT_CD AND
                        CX.CUST_NO = IV.CUST_NO
                        LEFT OUTER JOIN CT.TERMS_XREF TX
                        ON TX.TERMS_DESC = IV.TERMS
                    WHERE
                        INVOICE_ID > 0 AND
                        ISNULL(IV.TRANSFER_BATCH_NO, '') = @BatchNum";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar, 15));
            return cmd;
        }

        public SqlCommand GetInvoicesByStatus()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        IV.PLANT_CD ,
                        PT.PLANT_NO,
                        IV.CUST_NO,
                        ISNULL(CX.E1_SOLD_CUST_NO, 0) AS CUST_SOLD_NO,
                        ISNULL(CX.E1_SHIP_CUST_NO, 0) AS CUST_SHIP_NO,
                        --CX.E1_TERMS_CD,
                        ISNULL(TX.TERMS_CD, 'ARD') AS E1_TERMS_CD,
                        IV.REF_NO,
                        IV.INVOICE_ID,
                        IV.TRAN_DATE,
                        IV.DUE_DATE,
                        IV.ORIG_TRAN_DATE ,
                        IV.TRAN_TYPE ,
                        IV.E1_ORDER_TYPE,
                        IV.GROSS_AMT ,
                        IV.PAID_AMT ,
                        IV.OPEN_AMT ,
                        IV.TRANSFER_BATCH_NO ,
                        IV.TRANSFER_STATUS ,
                        IV.TRANSFER_MSG ,
                        IV.TRANSFER_DATE ,
                        IV.WRITTEN_DATE
                    FROM
                        CT.INVOICE IV
                        INNER JOIN CT.PLANT PT
                        ON PT.PLANT_CD = IV.PLANT_CD
                        LEFT OUTER JOIN CT.CUSTOMER_XREF CX
                        ON CX.PLANT_CD = IV.PLANT_CD AND
                        CX.CUST_NO = IV.CUST_NO
                        LEFT OUTER JOIN CT.TERMS_XREF TX
                        ON TX.TERMS_DESC = IV.TERMS
                    WHERE
                        INVOICE_ID > 0 AND
                        OPEN_AMT != 0 AND
                        ISNULL(TRANSFER_STATUS, '') = @Status";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 2));
            return cmd;
        }

        public Invoices GetInvoices(SqlConnection cn, BusinessSystemSession e1Session, int invoiceId = 0, string batchNum = null, string transferStatus = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var invoices = new Invoices();
            try
            {
                var cmd = new SqlCommand();
                var dict = new Dictionary<int, InvoiceLines>();
                if (invoiceId != 0)
                {
                    cmd = GetInvoicesById();
                    cmd.Parameters[0].Value = invoiceId;
                    dict = GetInvoiceLines(e1Session, invoiceId);
                }

                if (batchNum != null)
                {
                    cmd = GetInvoicesByBatch();
                    cmd.Parameters[0].Value = batchNum;
                    dict = GetInvoiceLines(e1Session, 0, batchNum);
                }

                if (transferStatus != null)
                {
                    cmd = GetInvoicesByStatus();
                    cmd.Parameters[0].Value = transferStatus;
                    dict = GetInvoiceLines(e1Session, 0, null, transferStatus);
                }

                cmd.Connection = cn;
                Logger.Trace("CN: " + cn.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var invoice = new Invoice
                        {
                            InvoiceId = Data.CastDbNull.To(reader["INVOICE_ID"], 0),
                            InvoiceNum = Convert.ToDecimal(Data.CastDbNull.To(reader["REF_NO"], string.Empty).Trim()),
                            InvoiceDate = Data.CastDbNull.To(reader["TRAN_DATE"], new DateTime(1900, 1, 1)),
                            OriginalInvoiceDate = Data.CastDbNull.To(reader["ORIG_TRAN_DATE"], new DateTime(1900, 1, 1)),
                            InvoiceType = Data.CastDbNull.To(reader["E1_ORDER_TYPE"], string.Empty).Trim()
                        };
                        var plantNum = Data.CastDbNull.To(reader["PLANT_NO"], string.Empty).Trim();
                        invoice.Plant = PlantManager.GetPlant(plantNum);
                        var soldToCustomerNum = Data.CastDbNull.To<decimal>(reader["CUST_SOLD_NO"], 0);
                        invoice.SoldToCustomer = _customerManager.GetCustomer(e1Session, soldToCustomerNum);
                        var taxCode = _customerManager.GetTaxRate(e1Session, soldToCustomerNum);
                        invoice.SoldToCustomer.TaxRateCode = taxCode;
                        var shipToCustomerNum = Data.CastDbNull.To<decimal>(reader["CUST_SHIP_NO"], 0);
                        invoice.ShipToCustomer = _customerManager.GetCustomer(e1Session, shipToCustomerNum);
                        var taxCode1 = _customerManager.GetTaxRate(e1Session, shipToCustomerNum);
                        invoice.ShipToCustomer.TaxRateCode = taxCode1;
                        invoice.IsInternalCustomer = invoice.SoldToCustomer.IsPlant;
                        invoice.Terms = Data.CastDbNull.To(reader["E1_TERMS_CD"], string.Empty).Trim();
                        //invoice.Terms = _customerManager.GetTerms(e1Session, invoice.SoldToCustomer.CustomerNum);
                        invoice.GrossAmt = Convert.ToDecimal(Data.CastDbNull.To<decimal>(reader["GROSS_AMT"], 0));
                        invoice.ReceivedAmt = Convert.ToDecimal(Data.CastDbNull.To<decimal>(reader["PAID_AMT"], 0));
                        invoice.OpenAmt = Convert.ToDecimal(Data.CastDbNull.To<decimal>(reader["OPEN_AMT"], 0));
                        //if (invoice.IsCredit)
                        //{
                        //    invoice.GrossAmt = invoice.GrossAmt * -1;
                        //    invoice.ReceivedAmt = invoice.ReceivedAmt * -1;
                        //    invoice.OpenAmt = invoice.OpenAmt * -1;
                        //}
                        invoice.TransferBatchNum = Data.CastDbNull.To(reader["TRANSFER_BATCH_NO"], string.Empty).Trim();
                        invoice.TransferStatus = Data.CastDbNull.To(reader["TRANSFER_STATUS"], string.Empty).Trim();
                        invoice.TransferMessage = Data.CastDbNull.To(reader["TRANSFER_MSG"], string.Empty).Trim();
                        invoice.TransferDate = Data.CastDbNull.To(reader["TRANSFER_DATE"], DateTime.MinValue);
                        // Discount Date
                        var dt = invoice.InvoiceDate;
                        //var dt = invoice.OriginalInvoiceDate; 
                        invoice.DiscountDate = new DateTime(1900, 1, 1);
                        if (invoice.Terms == "ARD")
                        {
                            if (dt.Day < 15)
                            {
                                invoice.DiscountDate = new DateTime(dt.Year, dt.Month, 25);
                            }
                            else
                            {
                                var nextMonth = dt.AddMonths(1);
                                invoice.DiscountDate = new DateTime(nextMonth.Year, nextMonth.Month, 10);
                            }

                            if (invoice.DiscountDate <= invoice.InvoiceDate)
                            {
                                invoice.DiscountDate = new DateTime(1900, 1, 1);
                            }
                        }

                        // Due Date
                        invoice.DueDate = Data.CastDbNull.To(reader["DUE_DATE"], new DateTime(1900, 1, 1));
                        if (invoice.DueDate < invoice.InvoiceDate)
                        {
                            invoice.DueDate = invoice.InvoiceDate;
                        }

                        // Invoice Lines
                        invoice.InvoiceLines = dict[invoice.InvoiceId];
                        //if (invoice.IsCredit)
                        //{
                        //    foreach (CGServicesHV.Common.Types.InvoiceLine line in invoice.InvoiceLines)
                        //    {
                        //        line.GrossAmt = line.GrossAmt * -1;
                        //        line.OpenAmt = line.OpenAmt * -1;
                        //    }
                        //}
                        invoice.JournalEntrys = GetJournalEntrys(e1Session, invoice);
                        invoice.BatchInvoices = GetBatchInvoices(e1Session, invoice);
                        invoice.BatchJournalEntrys = GetBatchJournalEntrys(e1Session, invoice.JournalEntrys);
                        invoice.TransferStatus = "C";
                        invoices.Add(invoice);
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + invoices.Count);
                    //ValidateInvoices(e1Session, invoices);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return invoices;
        }

        public void ValidateInvoices(BusinessSystemSession e1Session, Invoices invoices)
        {
            foreach (var invoice in invoices)
            {
                // Validations
                if (invoice.SoldToCustomer.CustomerNum == 0)
                {
                    invoice.TransferBatchNum = "";
                    invoice.TransferStatus = "E";
                    invoice.TransferMessage = "Invalid Customer";
                }
                if (invoice.InvoiceType == "SO")
                {
                    if (IsExistingF03B11(e1Session, invoice.InvoiceId))
                    {
                        invoice.TransferBatchNum = "";
                        invoice.TransferStatus = "E";
                        invoice.TransferMessage = "Existing F03B11 Invoice";
                    }
                    if (IsExistingF03B11Z1(e1Session, invoice.InvoiceId))
                    {
                        invoice.TransferBatchNum = "";
                        invoice.TransferStatus = "E";
                        invoice.TransferMessage = "Existing F03B11Z1 Invoice";
                    }
                }
                if (IsExistingF0911Z1(e1Session, invoice.InvoiceId))
                {
                    invoice.TransferBatchNum = "";
                    invoice.TransferStatus = "E";
                    invoice.TransferMessage = "Existing F0911Z1 Invoice";
                }
                if (invoice.InvoiceLines.OpenAmount != invoice.OpenAmt)
                {
                    invoice.TransferBatchNum = "";
                    invoice.TransferStatus = "E";
                    invoice.TransferMessage = "Lines Open Amount " + invoice.InvoiceLines.OpenAmount.ToString(CultureInfo.InvariantCulture) +
                                              " not equal Invoice Open Amount " + invoice.OpenAmt.ToString(CultureInfo.InvariantCulture);
                }
                if (invoice.InvoiceLines.TaxAmount != 0 & string.IsNullOrEmpty(invoice.ShipToCustomer.TaxRateCode))
                {
                    invoice.TransferBatchNum = "";
                    invoice.TransferStatus = "E";
                    invoice.TransferMessage = "Taxable Amount for Customer with no Tax Setup";
                }
            }
        }

        /******************************************************************
        * Invoice Lines
        ******************************************************************/
        //private string getInvoiceLinesSelectClause = @"
        //            i.INVOICE_ID,
        //            i.E1_ORDER_TYPE,
        //            il.E1_GL_CODE,
        //            SUM(i.GROSS_AMT) AS GROSS_AMT";

        public SqlCommand GetInvoiceLinesById()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        IV.PLANT_CD ,
                        IV.CUST_NO ,
                        IV.REF_NO ,
                        IV.INVOICE_ID ,
                        IV.E1_ORDER_TYPE ,
                        IL.E1_GL_CODE ,
                        SUM(IL.GROSS_AMT) AS GROSS_AMT,
                        SUM(IL.OPEN_AMT) AS OPEN_AMT
                    FROM
                        CT.INVOICE IV WITH ( NOLOCK )
                        INNER JOIN CT.INVOICE_LINE IL WITH ( NOLOCK )
                        ON IL.PLANT_CD = IV.PLANT_CD AND
                            IL.CUST_NO = IV.CUST_NO AND
                            IL.REF_NO = IV.REF_NO
                    WHERE
                        IV.INVOICE_ID = @InvoiceId
                    GROUP BY
                        IV.PLANT_CD ,
                        IV.CUST_NO ,
                        IV.REF_NO ,
                        IV.INVOICE_ID ,
                        IV.E1_ORDER_TYPE ,
                        IL.E1_GL_CODE
                        ORDER BY
                        IV.INVOICE_ID ,
                        IV.E1_ORDER_TYPE ,
                        IL.E1_GL_CODE";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int));
            return cmd;
        }

        public SqlCommand GetInvoiceLinesByBatch()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                  SELECT
                    IV.PLANT_CD ,
                    IV.CUST_NO ,
                    IV.REF_NO ,
                    IV.INVOICE_ID ,
                    IV.E1_ORDER_TYPE ,
                    IL.E1_GL_CODE ,
                    SUM(IL.GROSS_AMT) AS GROSS_AMT,
                    SUM(IL.OPEN_AMT) AS OPEN_AMT
                  FROM
                    CT.INVOICE IV WITH ( NOLOCK )
                    INNER JOIN CT.INVOICE_LINE IL WITH ( NOLOCK )
                    ON IL.PLANT_CD = IV.PLANT_CD AND
                       IL.CUST_NO = IV.CUST_NO AND
                       IL.REF_NO = IV.REF_NO
                  WHERE
                    IV.TRANSFER_BATCH_NO = @BatchNum
                  GROUP BY
                    IV.PLANT_CD ,
                    IV.CUST_NO ,
                    IV.REF_NO ,
                    IV.INVOICE_ID ,
                    IV.E1_ORDER_TYPE ,
                    IL.E1_GL_CODE
                  ORDER BY
                    IV.INVOICE_ID ,
                    IV.E1_ORDER_TYPE ,
                    IL.E1_GL_CODE";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar, 15));
            return cmd;
        }

        public SqlCommand GetInvoiceLinesByStatus()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                  SELECT
                    IV.PLANT_CD ,
                    IV.CUST_NO ,
                    IV.REF_NO ,
                    IV.INVOICE_ID ,
                    IV.E1_ORDER_TYPE ,
                    IL.E1_GL_CODE ,
                    SUM(IL.GROSS_AMT) AS GROSS_AMT,
                    SUM(IL.OPEN_AMT) AS OPEN_AMT
                  FROM
                    CT.INVOICE IV WITH ( NOLOCK )
                    INNER JOIN CT.INVOICE_LINE IL WITH ( NOLOCK )
                    ON IL.PLANT_CD = IV.PLANT_CD AND
                       IL.CUST_NO = IV.CUST_NO AND
                       IL.REF_NO = IV.REF_NO
                  WHERE
                    IV.GROSS_AMT != 0 AND
                    IV.TRANSFER_STATUS = @Status
                  GROUP BY
                    IV.PLANT_CD ,
                    IV.CUST_NO ,
                    IV.REF_NO ,
                    IV.INVOICE_ID ,
                    IV.E1_ORDER_TYPE ,
                    IL.E1_GL_CODE
                  ORDER BY
                    IV.INVOICE_ID ,
                    IV.E1_ORDER_TYPE ,
                    IL.E1_GL_CODE";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 2));
            return cmd;
        }

        public Dictionary<int, InvoiceLines> GetInvoiceLines(BusinessSystemSession e1Session, int invoiceId = 0, string batchNum = null, string transferStatus = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var dict = new Dictionary<int, InvoiceLines>();
            try
            {
                var cmd = new SqlCommand();
                if (invoiceId != 0)
                {
                    cmd = GetInvoiceLinesById();
                    cmd.Parameters[0].Value = invoiceId;
                }

                if (batchNum != null)
                {
                    cmd = GetInvoiceLinesByBatch();
                    cmd.Parameters[0].Value = batchNum;
                }

                if (transferStatus != null)
                {
                    cmd = GetInvoiceLinesByStatus();
                    cmd.Parameters[0].Value = transferStatus;
                }

                var cn = ConnectionManager.GetSqlConnectionFromKey("CT");
                cn.Open();
                cmd.Connection = cn;
                Logger.Trace("CN: " + cn.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var prevId = 0;
                    var currLines = new InvoiceLines();
                    while (reader.Read())
                    {
                        var currId = Data.CastDbNull.To(reader["INVOICE_ID"], 0);
                        if (prevId != currId & prevId != 0)
                        {
                            var saveLines = new InvoiceLines();
                            saveLines.AddRange(currLines);
                            dict.Add(prevId, saveLines);
                            currLines.Clear();
                        }

                        var line = new InvoiceLine
                        {
                            InvoiceId = currId,
                            OrderType = Data.CastDbNull.To(reader["E1_ORDER_TYPE"], string.Empty).Trim(),
                            GrossAmt = Data.CastDbNull.To<decimal>(reader["GROSS_AMT"], 0),
                            OpenAmt = Data.CastDbNull.To<decimal>(reader["OPEN_AMT"], 0),
                            GlCategory = Data.CastDbNull.To(reader["E1_GL_CODE"], string.Empty).Trim()
                        };
                        line.GlAccount = GetAaiAccount(e1Session, 4230, line.OrderType, line.GlCategory).ObjectAcct;
                        currLines.Add(line);
                        prevId = line.InvoiceId;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + currLines.Count);
                    dict.Add(prevId, currLines);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return dict;
        }

        /******************************************************************
        * Get Invoice Batch
        ******************************************************************/
        public SqlCommand GetInvoiceBatch()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        TRANSFER_BATCH_NO,
                        COUNT(*) AS Lines,
                        MIN(TRANSFER_DATE) AS StartDate,
                        MAX(TRANSFER_DATE) AS EndDate
                    FROM
                        CT.INVOICE
                    WHERE
                        TRANSFER_BATCH_NO = @BatchNum
                    GROUP BY 
                        TRANSFER_BATCH_NO";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar, 20));
            return cmd;
        }

        public InvoiceBatch GetInvoiceBatch(SqlConnection cn, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var batch = new InvoiceBatch();
            try
            {
                var cmd = GetInvoiceBatch();
                cmd.Connection = cn;
                cmd.Parameters[0].Value = batchNum;
                Logger.Trace("CN: " + cn.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        batch.BatchNum = Convert.ToInt32(Data.CastDbNull.To(reader["TRANSFER_BATCH_NO"], string.Empty).Trim());
                        batch.BatchDate = Data.CastDbNull.To(reader["StartDate"], new DateTime(1900, 1, 1));
                        batch.Lines = Data.CastDbNull.To(reader["Lines"], 0);
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return batch;
        }

        /******************************************************************
        * Set Invoice Transfer
        ******************************************************************/
        public SqlCommand SaveInvoice()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    UPDATE
                        CT.INVOICE
                    SET
                        TRANSFER_BATCH_NO = @BatchNum,
                        TRANSFER_STATUS = @Status,
                        TRANSFER_MSG = @Message,
                        TRANSFER_DATE = GETDATE()
                    WHERE
                        INVOICE_ID = @InvoiceId";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar, 15));
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 2));
            cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar, 2048));
            return cmd;
        }

        public SqlDmlResponses SaveInvoices(SqlConnection cn, SqlTransaction tran, Invoices invoices)
        {
            var responses = new SqlDmlResponses();
            try
            {
                var cmd = SaveInvoice();
                cmd.Connection = cn;
                cmd.Transaction = tran;
                foreach (var invoice in invoices)
                {
                    var response = new SqlDmlResponse();
                    try
                    {
                        cmd.Parameters[0].Value = invoice.InvoiceId;
                        cmd.Parameters[1].Value = invoice.TransferBatchNum;
                        cmd.Parameters[2].Value = invoice.TransferStatus;
                        cmd.Parameters[3].Value = invoice.TransferMessage;
                        response.Init(cmd);
                        var rows = cmd.ExecuteNonQuery();
                        response.RowsAffected = rows;
                        response.EndTime = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        //response.HasErrors = true;
                        response.Errors.Add(new Error(e));
                        response.Message = e.Message;
                        response.EndTime = DateTime.Now;
                        throw new ApplicationException(e.ToString());
                    }
                    responses.Add(response);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }

        /******************************************************************
        * Set Invoice Transfer
        ******************************************************************/
        public SqlCommand UpdateInvoiceBatchById()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    UPDATE
                        CT.INVOICE
                    SET
                        TRANSFER_BATCH_NO = @BatchNum,
                        TRANSFER_DATE = GETDATE()
                    WHERE
                        INVOICE_ID = @InvoiceId";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar, 15));
            cmd.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int));
            return cmd;
        }

        public SqlCommand UpdateInvoiceBatchByStatus()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    UPDATE
                        CT.INVOICE
                    SET
                        TRANSFER_BATCH_NO = @BatchNum,
                        TRANSFER_DATE = GETDATE()
                    WHERE
                        TRANSFER_STATUS = @Status";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new SqlParameter("@BatchNum", SqlDbType.VarChar, 15));
            cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 2));
            return cmd;
        }

        public SqlDmlResponse UpdateInvoiceBatch(SqlConnection cn, string batchNum, int invoiceId = 0, string status = null)
        {
            var response = new SqlDmlResponse();
            var cmd = new SqlCommand();
            try
            {
                if (invoiceId != 0)
                {
                    cmd = UpdateInvoiceBatchById();
                    cmd.Parameters[0].Value = batchNum;
                    cmd.Parameters[1].Value = invoiceId;
                }
                if (status != null)
                {
                    cmd = UpdateInvoiceBatchByStatus();
                    cmd.Parameters[0].Value = batchNum;
                    cmd.Parameters[1].Value = status;
                }
                cmd.Connection = cn;
                response.Init(cmd);
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                response.EndTime = DateTime.Now;
            }
            catch (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = e.Message;
                response.EndTime = DateTime.Now;
            }
            return response;
        }

        /******************************************************************
        * Legacy Customer Num
        ******************************************************************/
        private string GetLegacyCustomerNum(decimal customerNum)
        {
            var sb = new StringBuilder();
            try
            {
                sb.Append("00000");
                sb.Append(Convert.ToString(customerNum, CultureInfo.InvariantCulture));
                var s = sb.ToString();
                return s.Substring(s.Length - 5) + "00";
            }
            catch (ApplicationException e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(Convert.ToString(customerNum, CultureInfo.InvariantCulture));
            }
        }

        /******************************************************************
        * GL Account
        ******************************************************************/
        public iDB2Command GetAaiAccount()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        MLANUM,
                        MLCO,
                        MLDCTO,
                        MLDCT,
                        MLGLPT,
                        MLCOST,
                        MLMCU,
                        MLOBJ,
                        MLSUB
                    FROM
                        F4095
                    WHERE
                        MLANUM = ? AND
                        MLDCT = ? AND
                        MLGLPT = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("TableNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TableNum"].Precision = 4;
            cmd.Parameters["TableNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DocumentType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("GLCategory", iDB2DbType.iDB2Graphic, 4));
            return cmd;
        }

        public AaiAccount GetAaiAccount(BusinessSystemSession e1Session, decimal tableNum, string documentType, string glCategory)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var account = new AaiAccount();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("InvoiceManager.GetAaiAccount")) ? e1Session.DbCommands["InvoiceManager.GetAaiAccount"] : GetAaiAccount();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = tableNum;
                cmd.Parameters[1].Value = documentType;
                cmd.Parameters[2].Value = glCategory;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        account.TableNum = Data.CastDbNull.To<decimal>(reader["MLANUM"], 0);
                        account.Company = Data.CastDbNull.To(reader["MLCO"], string.Empty).Trim();
                        account.OrderType = Data.CastDbNull.To(reader["MLDCTO"], string.Empty).Trim();
                        account.DocumentType = Data.CastDbNull.To(reader["MLDCT"], string.Empty).Trim();
                        account.GlCategory = Data.CastDbNull.To(reader["MLGLPT"], string.Empty).Trim();
                        account.CostType = Data.CastDbNull.To(reader["MLCOST"], string.Empty).Trim();
                        account.CostCenter = Data.CastDbNull.To(reader["MLMCU"], string.Empty).Trim();
                        account.ObjectAcct = Data.CastDbNull.To(reader["MLOBJ"], string.Empty).Trim();
                        account.Subsidiary = Data.CastDbNull.To(reader["MLSUB"], string.Empty).Trim();
                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return account;
        }

        /******************************************************************
        * Existing Invoice
        ******************************************************************/
        public iDB2Command IsExistingF03B11()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        COUNT(*) AS RECS
                    FROM
                        F03B11
                    WHERE
                        RPDOC = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("InvoiceId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["InvoiceId"].Precision = 8;
            cmd.Parameters["InvoiceId"].Scale = 0;
            return cmd;
        }

        public bool IsExistingF03B11(BusinessSystemSession e1Session, decimal invoiceId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var retValue = false;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("InvoiceManager.IsExistingF03B11")) ? e1Session.DbCommands["InvoiceManager.IsExistingF03B11"] : IsExistingF03B11();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = invoiceId;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        var recs = Data.CastDbNull.To(reader["RECS"], 0);
                        if (recs > 0)
                        {
                            retValue = true;
                        }

                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return retValue;
        }

        public iDB2Command IsExistingF03B11Z1()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        COUNT(*) AS RECS
                    FROM
                        F03B11Z1
                    WHERE
                        VJDOC = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("InvoiceId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["InvoiceId"].Precision = 8;
            cmd.Parameters["InvoiceId"].Scale = 0;
            return cmd;
        }

        public bool IsExistingF03B11Z1(BusinessSystemSession e1Session, decimal invoiceId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var retValue = false;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("InvoiceManager.IsExistingF03B11Z1")) ? e1Session.DbCommands["InvoiceManager.IsExistingF03B11Z1"] : IsExistingF03B11Z1();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = invoiceId;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        var recs = Data.CastDbNull.To(reader["RECS"], 0);
                        if (recs > 0)
                        {
                            retValue = true;
                        }

                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return retValue;
        }

        public iDB2Command IsExistingF0911Z1()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        COUNT(*) AS RECS
                    FROM
                        F0911Z1
                    WHERE
                        VNDOC = ?";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("InvoiceId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["InvoiceId"].Precision = 8;
            cmd.Parameters["InvoiceId"].Scale = 0;
            return cmd;
        }

        public bool IsExistingF0911Z1(BusinessSystemSession e1Session, decimal invoiceId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var retValue = false;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("InvoiceManager.IsExistingF0911Z1")) ? e1Session.DbCommands["InvoiceManager.IsExistingF0911Z1"] : IsExistingF0911Z1();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = invoiceId;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        var recs = Data.CastDbNull.To(reader["RECS"], 0);
                        if (recs > 0)
                        {
                            retValue = true;
                        }

                        counter++;
                    }

                    reader.Close();
                    Logger.Trace("ROWS: " + counter);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }

            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return retValue;
        }

        /******************************************************************
        * Get Journal Lines
        ******************************************************************/
        public JournalEntrys GetJournalEntrys(BusinessSystemSession e1Session, Invoice invoice)
        {
            var entrys = new JournalEntrys();
            var prevLine = new InvoiceLine();
            decimal amount = 0;
            try
            {
                foreach (var line in invoice.InvoiceLines)
                {
                    if (line.GlCategory != prevLine.GlCategory & prevLine.GlCategory != null)
                    {
                        var entry = BuildJournalEntrys(e1Session, invoice, prevLine, amount, entrys.Count);
                        entrys.AddRange(entry);
                        amount = 0;
                    }
                    amount += line.OpenAmt;
                    prevLine = (InvoiceLine)line.Clone();
                }
                var entry1 = BuildJournalEntrys(e1Session, invoice, prevLine, amount, entrys.Count);
                entrys.AddRange(entry1);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return entrys;
        }

        private JournalEntrys BuildJournalEntrys(BusinessSystemSession e1Session, Invoice invoice, InvoiceLine line, decimal amount, int lineNum)
        {
            var entrys = new JournalEntrys();
            var debitEntry = new JournalEntry();
            var creditEntry = new JournalEntry();
            try
            {
                switch (invoice.InvoiceType)
                {
                    case "SO":
                    case "CO":
                        creditEntry.EdiUserId = "CTAR";
                        creditEntry.TransactionType = "I";
                        creditEntry.BatchType = "I";
                        creditEntry.CompanyNum = invoice.Plant.CompanyNum;
                        creditEntry.ShortCompanyNum = invoice.Plant.ShortCompanyNum;
                        creditEntry.CustomerNum = invoice.SoldToCustomer.CustomerNum;
                        creditEntry.CostCenter = invoice.Plant.PlantNum;
                        creditEntry.BatchNum = invoice.TransferBatchNum;
                        creditEntry.DocumentNum = invoice.InvoiceId;
                        creditEntry.DocumentLineNum = (lineNum + 1) * 1000;
                        creditEntry.DocumentType = "RI";
                        creditEntry.OrderType = invoice.InvoiceType;
                        creditEntry.CategoryCode = line.GlCategory;
                        if (line.GlCategory == "TAX")
                        {
                            var taxCode = _customerManager.GetTaxRate(e1Session, invoice.ShipToCustomer.CustomerNum);
                            if (string.IsNullOrEmpty(taxCode))
                            {
                                //var msg = "Invalid Tax Code - Customer " + invoice.ShipToCustomer.CustomerNum.ToString(CultureInfo.InvariantCulture);
                                //throw new ApplicationException(msg);
                            }
                            var acct = GetAaiAccount(e1Session, 4250, invoice.InvoiceType, taxCode);
                            creditEntry.Account = acct.ObjectAcct;
                            creditEntry.Subsidiary = acct.Subsidiary;
                            creditEntry.TaxCode = taxCode;
                        }
                        else
                        {
                            var acct = GetAaiAccount(e1Session, 4230, invoice.InvoiceType, line.GlCategory);
                            creditEntry.Account = acct.ObjectAcct;
                            creditEntry.Subsidiary = "";
                            creditEntry.TaxCode = "";
                        }
                        creditEntry.JournalDate = invoice.InvoiceDate;
                        creditEntry.Amount = amount * -1;
                        creditEntry.SubledgerType = "";
                        creditEntry.Subledger = "";
                        entrys.Add(creditEntry);
                        break;
                    case "ST":
                    case "CT":
                        creditEntry.EdiUserId = "CTGL";
                        creditEntry.TransactionType = "J";
                        creditEntry.BatchType = "G";
                        creditEntry.CostCenter = invoice.Plant.PlantNum;
                        creditEntry.CompanyNum = invoice.Plant.CompanyNum;
                        creditEntry.ShortCompanyNum = invoice.Plant.ShortCompanyNum;
                        creditEntry.CustomerNum = invoice.SoldToCustomer.CustomerNum;
                        creditEntry.BatchNum = invoice.TransferBatchNum;
                        creditEntry.DocumentNum = invoice.InvoiceId;
                        creditEntry.DocumentLineNum = (lineNum + 1) * 1000;
                        creditEntry.DocumentType = "RI";
                        creditEntry.OrderType = invoice.InvoiceType;
                        creditEntry.CategoryCode = line.GlCategory;
                        creditEntry.Account = GetAaiAccount(e1Session, 4230, invoice.InvoiceType, line.GlCategory).ObjectAcct;
                        creditEntry.JournalDate = invoice.InvoiceDate;
                        creditEntry.Amount = amount * -1;
                        var crAccountList = new List<string> { "4015", "4025", "4030" };
                        if (crAccountList.Contains(creditEntry.Account))
                        {
                            creditEntry.Subledger = invoice.SoldToCustomer.ShortCompanyNum.PadLeft(8);
                            creditEntry.SubledgerType = "C";
                        }
                        entrys.Add(creditEntry);

                        debitEntry.EdiUserId = "CTGL";
                        debitEntry.TransactionType = "J";
                        debitEntry.BatchType = "G";
                        debitEntry.CostCenter = invoice.SoldToCustomer.PlantNum;
                        debitEntry.CompanyNum = invoice.Plant.CompanyNum;
                        debitEntry.ShortCompanyNum = invoice.Plant.ShortCompanyNum;
                        debitEntry.CustomerNum = invoice.SoldToCustomer.CustomerNum;
                        debitEntry.BatchNum = invoice.TransferBatchNum;
                        debitEntry.DocumentNum = invoice.InvoiceId;
                        debitEntry.DocumentLineNum = (lineNum + 2) * 1000;
                        debitEntry.DocumentType = "RI";
                        debitEntry.OrderType = invoice.InvoiceType;
                        debitEntry.CategoryCode = line.GlCategory;
                        debitEntry.Account = GetAaiAccount(e1Session, 4245, invoice.InvoiceType, line.GlCategory).ObjectAcct;
                        debitEntry.JournalDate = invoice.InvoiceDate;
                        debitEntry.Amount = amount;
                        var dbAccountList = new List<string> { "1247" };
                        if (dbAccountList.Contains(debitEntry.Account))
                        {
                            debitEntry.Subledger = invoice.Plant.PlantNum.PadLeft(8);
                            debitEntry.SubledgerType = "C";
                        }
                        entrys.Add(debitEntry);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return entrys;
        }

        /******************************************************************
        * Get E1 Batch Invoices
        ******************************************************************/
        public decimal GetNextBatch(BusinessSystemSession e1Session)
        {
            var batchNumManager = new BatchNumManager(e1Session);
            try
            {
                return batchNumManager.GetNextBatchAR(e1Session);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        /******************************************************************
        * Get E1 Batch Invoices
        ******************************************************************/
        public BatchInvoices GetBatchInvoices(BusinessSystemSession e1Session, Invoice invoice)
        {
            var recs = new BatchInvoices();
            var batchInvoiceMgr = new BatchInvoiceMgr(e1Session);
            try
            {
                if (invoice.IsExternal)
                {
                    var rec = new BatchInvoice
                    {
                        VJEDUS = "CTAR",
                        VJEDSP = "0",
                        VJEDTC = "A",
                        VJCRRM = "D",
                        VJAM = "2",
                        VJKCO = invoice.Plant.CompanyNum,
                        VJCO = invoice.Plant.CompanyNum,
                        VJEDTR = "I",
                        VJICUT = "I",
                        VJEDTN = Convert.ToString(invoice.InvoiceId).PadLeft(22),
                        VJEDLN = 1000,
                        VJEDDT = Utilities.Common.CastToE1Date(DateTime.Now),
                        VJEDBT = invoice.TransferBatchNum,
                        VJDOC = invoice.InvoiceId,
                        VJDCT = "RI",
                        VJAN8 = invoice.SoldToCustomer.CustomerNum,
                        VJAG = Math.Round(invoice.OpenAmt * 100, 0),
                        VJAAP = Math.Round(invoice.OpenAmt * 100, 0),
                        VJMCU = invoice.Plant.PlantNum.PadLeft(12),
                        VJPTC = invoice.Terms,
                        VJEDDH = "1",
                        VJDIVJ = Utilities.Common.CastToE1Date(invoice.InvoiceDate),
                        VJDGJ = Utilities.Common.CastToE1Date(invoice.InvoiceDate),
                        VJUPMJ = Utilities.Common.CastToE1Date(DateTime.Now),
                        VJUPMT = Utilities.Common.CastToE1Time(DateTime.Now),
                        VJRMK = "Num: " + invoice.InvoiceNum.ToString(CultureInfo.InvariantCulture) + " Date: " + invoice.OriginalInvoiceDate.ToShortDateString(),
                        VJUSER = "SVCSOA",
                        VJPID = "CORPSVC"
                    };
                    //rec.VJEDDH = invoice.Terms == "" ? "0" : "1";
                    //rec.VJDDNJ = invoice.DiscountDate == Convert.ToDateTime("01/01/1900")
                    //                 ? 0
                    //                 : CGICommon.Utilities.Common.CastToE1Date(invoice.DiscountDate);
                    //rec.VJDDJ = invoice.DueDate == Convert.ToDateTime("01/01/1900")
                    //                ? 0
                    //                : CGICommon.Utilities.Common.CastToE1Date(invoice.DueDate);
                    recs.Add(rec);
                }
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.ToString());
            }
            return recs;
        }

        /******************************************************************
        * E1 Batch Journal Entrys
        ******************************************************************/
        public BatchJournalEntrys GetBatchJournalEntrys(BusinessSystemSession e1Session, JournalEntrys entrys)
        {
            var batchEntrys = new BatchJournalEntrys();
            try
            {
                decimal lineNum = 0;
                foreach (var entry in entrys)
                {
                    lineNum += 1000;
                    var rec = new BatchJournalEntry
                    {
                        VNEDUS = entry.EdiUserId,
                        VNEDSP = "0",
                        VNEDTC = "A",
                        VNAM = "2",
                        VNKCO = entry.CompanyNum,
                        VNCO = entry.CompanyNum,
                        VNEDTR = entry.TransactionType,
                        VNICUT = entry.BatchType,
                        VNEDTN = Convert.ToString(entry.DocumentNum, CultureInfo.InvariantCulture).PadLeft(22),
                        VNEDLN = lineNum,
                        VNEDDT = Utilities.Common.CastToE1Date(DateTime.Now),
                        VNEDBT = entry.BatchNum,
                        VNDOC = entry.DocumentNum,
                        VNDCT = "RI",
                        VNAN8 = entry.CustomerNum,
                        VNAA = Math.Round(entry.Amount * 100, 0),
                        VNMCU = entry.CostCenter.PadLeft(12)
                    };
                    if (entry.CategoryCode == "TAX")
                    {
                        rec.VNANI = entry.ShortCompanyNum.PadLeft(12) + "." + entry.Account + "." + entry.Subsidiary;
                    }
                    else
                    {
                        rec.VNANI = rec.VNMCU + "." + entry.Account.PadRight(16);
                    }
                    rec.VNSUB = entry.Subsidiary;
                    rec.VNSBL = entry.Subledger;
                    rec.VNSBLT = entry.SubledgerType;
                    rec.VNDGJ = Utilities.Common.CastToE1Date(entry.JournalDate);
                    rec.VNIVD = Utilities.Common.CastToE1Date(entry.JournalDate);
                    rec.VNUPMJ = Utilities.Common.CastToE1Date(DateTime.Now);
                    rec.VNUPMT = Utilities.Common.CastToE1Time(DateTime.Now);
                    rec.VNEXA = "AAI Mapping: " + entry.OrderType + "-" + entry.CategoryCode;
                    rec.VNEXR = "AAI Mapping: " + entry.OrderType + "-" + entry.CategoryCode;
                    rec.VNUSER = "SVCSOA";
                    rec.VNPID = "CORPSVC";
                    batchEntrys.Add(rec);
                }
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.ToString());
            }
            return batchEntrys;
        }
    }
}
