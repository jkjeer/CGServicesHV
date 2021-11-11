using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CGServices.Common.Managers.E1;
using CGServices.Common.Managers.Enterprise;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;
using CGServices.Common.Types.Response;
using CGServices.Common.Types.ShopFloor;
using CGServices.Common.Utilities;
using NLog;

namespace CGServices.Common.Managers.ShopFloor
{
    public class SpecUnitDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /******************************************************************
        * Build Shop Floor objects
        ******************************************************************/
        public ProcessResponse ProcessMto(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan span, bool useHighWaterMark, decimal configId, List<string> docTypes)
        {
            var response = new ProcessResponse("MTO Unit Download");

            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F3201") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));

            // Get Enterprise Objects
            var r1 = new ResponseTime("Get MTO Units");
            var specUnitMgr = new Common.Managers.Enterprise.SpecUnitManager(session);
            var ids = specUnitMgr.GetMtoUnits(session, plant, processRunSpan, configId, docTypes);
            Logger.Info("MTO Units: " + ids.Count);
            //response.ResponseTimes.Add(r1.Done());

            // Shop Floor Tables
            var dts = new List<DataTable>();
            var glassTypeTable = new SpecGlassTypeTable();
            var glassProcessingTable = new SpecGlassProcessingTable();
            var unitTable = new SpecUnitTable();
            var unitXrefTable = new SpecUnitXrefTable();
            var unitShapeTable = new SpecUnitShapeTable();
            var unitAirspaceTable = new SpecUnitAirspaceTable();
            var unitGridTable = new SpecUnitGridTable();
            var unitSurfaceTable = new SpecUnitSurfaceTable();
            var unitFlexTable = new SpecUnitFlexTable();

            foreach (SpecUnit item in ids)
            {
                unitXrefTable.Add(item);
                glassTypeTable.Add(item);
                glassProcessingTable.Add(item);
                unitTable.Add(item);
                unitShapeTable.Add(item);
                unitAirspaceTable.Add(item);
                unitGridTable.Add(item);
                unitSurfaceTable.Add(item);
                unitFlexTable.Add(item);
            }


            dts.Add(unitXrefTable);
            dts.Add(glassTypeTable);
            dts.Add(glassProcessingTable);
            dts.Add(unitTable);
            dts.Add(unitShapeTable);
            dts.Add(unitAirspaceTable);
            dts.Add(unitGridTable);
            dts.Add(unitSurfaceTable);
            dts.Add(unitFlexTable);

            Logger.Info("Xref Rows: " + unitXrefTable.Rows.Count);
            Logger.Info(XmlSerializationUtility.SerializeObject(Encoding.UTF8, unitXrefTable.Rows));

            foreach (DataTable dt in dts)
            {
                response.SqlBulkCopyResponses.Add(BulkCopyManager.BulkTable(dt.ExtendedProperties["Target"].ToString(), dt, cn));
            }

            if (mark != null)
            {
                mark.EnterpriseDateTime = unitTable.MaxDate;
                response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, mark));
            }

            return response;
        }

        public ProcessResponse ProcessMts(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan span, bool useHighWaterMark, decimal itemNum)
        {
            var response = new ProcessResponse("MTS Unit Download");

            // Process Run Span
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580002") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));
            Logger.Info(XmlSerializationUtility.SerializeObject(Encoding.UTF8, processRunSpan));

            // Get Enterprise Objects
            var r1 = new ResponseTime("Get MTS Units");
            var specUnitMgr = new SpecUnitManager(session);
            var ids = specUnitMgr.GetMtsUnits(session, plant, processRunSpan, itemNum);
            Logger.Info("MTS Units: " + ids.Count);
            r1.Done();
            //response.ResponseTimes.Add(r1);

            if (ids.Count > 0)
            {

                // Shop Floor Tables
                var dts = new List<DataTable>();

                var glassTypeTable = new SpecGlassTypeTable();
                var glassProcessingTable = new SpecGlassProcessingTable();
                var unitOverallTable = new SpecUnitTable();
                var unitShapeTable = new SpecUnitShapeTable();
                var unitAirspaceTable = new SpecUnitAirspaceTable();
                var unitGridTable = new SpecUnitGridTable();
                var unitSurfaceTable = new SpecUnitSurfaceTable();
                //var unitFlexTable = new SpecUnitFlexTable();

                foreach (SpecUnit item in ids)
                {
                    glassTypeTable.Add(item);
                    glassProcessingTable.Add(item);
                    unitOverallTable.Add(item);
                    unitShapeTable.Add(item);
                    unitAirspaceTable.Add(item);
                    unitGridTable.Add(item);
                    unitSurfaceTable.Add(item);
                    //unitFlexTable.Add(item);
                }

                dts.Add(glassTypeTable);
                dts.Add(glassProcessingTable);
                dts.Add(unitOverallTable);
                dts.Add(unitShapeTable);
                dts.Add(unitAirspaceTable);
                dts.Add(unitGridTable);
                dts.Add(unitSurfaceTable);
                //dts.Add(unitFlexTable);

                foreach (DataTable dt in dts)
                {
                    response.SqlBulkCopyResponses.Add(
                        BulkCopyManager.BulkTable(dt.ExtendedProperties["Target"].ToString(), dt, cn));
                }

                if (mark != null)
                {
                    mark.EnterpriseDateTime = unitOverallTable.MaxDate;
                    response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, mark));
                }
            }

            return response;
        }
    }
}
