using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using NLog;

namespace CGServicesHV.ItemSvc
{
    public class ItemProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse RunSpecUnitAudit(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Rack Detail Audit Processor", plant);
            try
            {
                var lookbackDays = Data.UnboxParm.To(parms, "LookbackDays", 30);
                var sendReport = Data.UnboxParm.To(parms, "SendReport", false);
                var mgr = new SpecUnitAuditManager();
                response.ProcessResponses.Add(mgr.Process(plant, sqlCns, lookbackDays, sendReport));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemAttributes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Attribute Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                response.ProcessResponses.Add(ItemAttributeDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemAttributeMaster(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Attribute Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(ItemAttributeMasterDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadBillOfMaterials(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"BOM Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(BillOfMaterialDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse AccountBalanceDownload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse("Account Balance Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var century = Data.UnboxParm.To(parms, "Century", 0);
                var year = Data.UnboxParm.To(parms, "Year", 0);
                response = AccountBalanceDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, century, year);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse AccountDownload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse("Account Balance Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response = AccountDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadGlassItemMetrics(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Glass Item Metric Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var metrics = Data.UnboxParm.To(parms, "Metrics", new List<string>());
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                response.ProcessResponses.Add(GlassItemMetricDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, metrics, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close(true);
            return response;
        }

        public static ProcessResponse DownloadItemMetrics(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Metric Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var documentTypes = Data.UnboxParm.ToStringList(parms, "DocumentType");
                response.ProcessResponses.Add(ItemMetricDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, documentTypes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemBalances(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Balance Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response.ProcessResponses.Add(ItemBalanceDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemCosts(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Cost Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new ItemCostDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemCostComponents(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Cost Component Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new ItemCostComponentDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemCostArchive(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Cost Archive Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                response.ProcessResponses.Add(ItemCostArchiveDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemUOMs(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item UOM Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new ItemUOMDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }


        /********************************************************************************
        * ITEM
        ********************************************************************************/
        public static ProcessResponse DownloadItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var stockingTypes = Data.UnboxParm.To(parms, "StockingTypes", new List<string>());
                var glCategories = Data.UnboxParm.To(parms, "GlCategories", new List<string>());
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new ItemDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, stockingTypes, glCategories, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadGlassItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Glass Item Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var stockTypes = Data.UnboxParm.To(parms, "StockTypes", new List<string>());
                var mgr = new GlassItemDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum, stockTypes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadItemXref(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Item Xref Download Processor", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var xrefTypes = Data.UnboxParm.To(parms, "XrefTypes", new List<string>());
                var mgr = new ItemXrefDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, xrefTypes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadMtsSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Spec Unit MTS Processor");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new SpecUnitMtsDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadMtoSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Spec Unit MTO Processor");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackStatusCodes = Data.UnboxParm.To(parms, "RackStatusCodes", new List<string>());
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var configId = Data.UnboxParm.To<decimal>(parms, "ConfigId", 0);
                var documentNum = Data.UnboxParm.To<decimal>(parms, "DocumentNum", 0);
                var documentType = Data.UnboxParm.To(parms, "DocumentType", String.Empty);
                var woStatusCodes = Data.UnboxParm.To(parms, "WOStatusCodes", new List<string>());
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new SpecUnitMtoDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackStatusCodes, rackId, configId, documentNum, documentType, woStatusCodes, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse DownloadSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Spec Unit Processor");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackStatusCodes = Data.UnboxParm.To(parms, "RackStatusCodes", new List<string>());
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var configId = Data.UnboxParm.To<decimal>(parms, "ConfigId", 0);
                var documentNum = Data.UnboxParm.To<decimal>(parms, "DocumentNum", 0);
                var documentType = Data.UnboxParm.To(parms, "DocumentType", String.Empty);
                var woStatusCodes = Data.UnboxParm.To(parms, "WOStatusCodes", new List<string>());
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mtsMgr = new SpecUnitMtsDownloadManager(session);
                response.ProcessResponses.Add(mtsMgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum));
                var mtoMgr = new SpecUnitMtoDownloadManager(session);
                response.ProcessResponses.Add(mtoMgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackStatusCodes, rackId, configId, documentNum, documentType, woStatusCodes, itemNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }


        public static ProcessResponse UploadMachineParts(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"Machine Part Upload Manager", plant);
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var mgr = new MachinePartUploadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, "N"));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }
    }
}
