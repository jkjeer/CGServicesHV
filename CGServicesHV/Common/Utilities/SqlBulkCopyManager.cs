using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using CGServicesHV.Common.Types.Response;
using NLog;

namespace CGServicesHV.Common.Utilities
{
    [DataContract(Namespace = "")]
    public class SqlBulkCopyManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public long RowsCopied = 0;
        public string DbConnectionString { get; set; }

        public SqlBulkCopyResponse BulkTable(string tableName, DataTable dt)
        {
            var response = new SqlBulkCopyResponse
            {
                ConnectionString = DbConnectionString,
                TableName = tableName,
                //HasErrors = false,
                StartTime = DateTime.Now
            };

            var cn = new SqlConnection();
            try
            {
                cn = ConnectionManager.GetSqlConnection(DbConnectionString);
                cn.Open();
                var bulk = new SqlBulkCopy(cn, SqlBulkCopyOptions.FireTriggers, null)
                {
                    DestinationTableName = tableName,
                    BulkCopyTimeout = 300,
                    BatchSize = 25000
                };
                try
                {
                    bulk.SqlRowsCopied += OnSqlRowsCopied;
                    bulk.NotifyAfter = 1;
                    response.StartTime = DateTime.Now;
                    foreach (DataColumn col in dt.Columns)
                    {
                        var map = new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName);
                        bulk.ColumnMappings.Add(map);
                    }
                    bulk.WriteToServer(dt);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error(e));
                    response.Message = e.ToString();
                    Logger.Error("ERROR: " + cn.ConnectionString + e.Message + ": " + e.StackTrace);
                    throw new Exception(e.ToString());
                }
                finally
                {
                    bulk.Close();
                }

            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
                response.Message = e.ToString();
                Logger.Error("ERROR: " + cn.ConnectionString + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                cn.Close();
            }
            response.RowsCopied = RowsCopied;
            response.EndTime = DateTime.Now;
            return response;
        }

        public SqlBulkCopyResponse BulkReader(string tableName, IDataReader dr)
        {
            var response = new SqlBulkCopyResponse
            {
                ConnectionString = DbConnectionString,
                TableName = tableName,
                //HasErrors = false,
                StartTime = DateTime.Now
            };

            var cn = new SqlConnection();
            try
            {
                cn = ConnectionManager.GetSqlConnection(DbConnectionString);
                cn.Open();
                var bulk = new SqlBulkCopy(cn, SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.TableLock, null)
                {
                    DestinationTableName = tableName,
                    BulkCopyTimeout = 300,
                    BatchSize = 25000
                };
                var columnName = "";
                try
                {
                    bulk.SqlRowsCopied += OnSqlRowsCopied;
                    bulk.NotifyAfter = 1;
                    response.StartTime = DateTime.Now;
                    var schemaTable = dr.GetSchemaTable();
                    if (schemaTable != null)
                        try
                        {
                            foreach (DataRow row in schemaTable.Rows)
                            {
                                columnName = row["ColumnName"].ToString();
                                var map = new SqlBulkCopyColumnMapping(columnName, columnName);
                                bulk.ColumnMappings.Add(map);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new ApplicationException("SourceColumn: " + tableName + "." + columnName + ", DestinationColumn: " + columnName + ": " + e);
                        }
                    bulk.WriteToServer(dr);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(tableName + "  " + e.Message + ": " + e.StackTrace);
                }
                finally
                {
                    bulk.Close();
                }

            }
            catch (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = e.ToString();
                Logger.Error("ERROR: " + cn.ConnectionString + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                cn.Close();
            }
            response.RowsCopied = RowsCopied;
            response.EndTime = DateTime.Now;
            return response;
        }

        public SqlBulkCopyResponse BulkInsert<T>(string tableName, IList<T> list)
        {
            var response = new SqlBulkCopyResponse
            {
                ConnectionString = DbConnectionString,
                TableName = tableName,
                //HasErrors = false
            };
            var cn = new SqlConnection();
            try
            {
                cn = ConnectionManager.GetSqlConnection(DbConnectionString);
                cn.Open();
                var bulk = new SqlBulkCopy(cn, SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.TableLock, null)
                {
                    DestinationTableName = tableName,
                    BulkCopyTimeout = 300,
                    BatchSize = 25000
                };
                try
                {
                    bulk.SqlRowsCopied += OnSqlRowsCopied;
                    bulk.NotifyAfter = 1;
                    response.StartTime = DateTime.Now;

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))
                        //Dirty hack to make sure we only have system data types                                      
                        //i.e. filter out the relationships/collections                                     
                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                        .ToArray();
                    foreach (var propertyInfo in props)
                    {
                        bulk.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name,
                                          Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }
                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }
                        table.Rows.Add(values);
                    }
                    response.StartTime = DateTime.Now;
                    bulk.WriteToServer(table);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.ToString());
                }
                finally
                {
                    bulk.Close();
                }
            }
            catch
                (Exception e)
            {
                //response.HasErrors = true;
                response.Errors.Add(new Error(e));
                response.Message = e.ToString();
                Logger.Error("ERROR: " + cn.ConnectionString + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                cn.Close();
            }
            response.RowsCopied = RowsCopied;
            response.EndTime = DateTime.Now;
            return response;
        }

        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            RowsCopied = e.RowsCopied;
        }
    }
}
