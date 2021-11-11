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
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var stockTypes = Data.UnboxParm.To(parms, "StockTypes", new List<string>());
                var mgr = new ItemDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum, stockTypes));
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
