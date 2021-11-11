using System;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Types.E1;
using CGServices.Common.Types.Enterprise;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitSurfaceTable : DataTable
    {

        public SpecUnitSurfaceTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_SURFACE_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(long));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("EXT_PRESERVE_COLOR", typeof(string));
            Columns.Add("EXT_PRESERVE_CUTBACK", typeof(string));
            Columns.Add("EXT_SCORING_WIDTH", typeof(decimal));
            Columns.Add("INT_PRESERVE_COLOR", typeof(string));
            Columns.Add("INT_PRESERVE_CUTBACK", typeof(string));
            Columns.Add("INT_SCORING_WIDTH", typeof(decimal));
            Columns.Add("OPACIFIER", typeof(string));
            Columns.Add("OVERLAY_BAR", typeof(string));
            Columns.Add("PROTECTIVE_FILM_CD", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                DataRow dr = NewRow();
                dr["ITEM_NUM"] = item.ItemNum;
                dr["CONFIGURATION_ID"] = item.ConfigurationId;
                dr["EXT_PRESERVE_COLOR"] = item.GlassItem.ExteriorSurface.PreserveColor;
                dr["EXT_PRESERVE_CUTBACK"] = item.GlassItem.ExteriorSurface.PreserveCutback;
                dr["EXT_SCORING_WIDTH"] = item.GlassItem.ExteriorSurface.ScoringWidth;
                dr["INT_PRESERVE_COLOR"] = item.GlassItem.InteriorSurface.PreserveColor;
                dr["INT_PRESERVE_CUTBACK"] = item.GlassItem.InteriorSurface.PreserveCutback;
                dr["INT_SCORING_WIDTH"] = item.GlassItem.InteriorSurface.ScoringWidth;
                dr["OPACIFIER"] = item.GlassItem.ExteriorSurface.Opacifier;
                //dr["OVERLAY_BAR"] = item.GlassItem.CustomerOverlayBar;
                //dr["PROTECTIVE_FILM_CD"] = item.GlassItem.ExteriorSurface.PreserveProcessing;
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    if (line.F580021.S7Z58L1TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = (int)line.F580021.CMCFGID;
                        dr["EXT_PRESERVE_COLOR"] = line.F580024.S8Z587566;
                        dr["EXT_PRESERVE_CUTBACK"] = line.F580024.S8Z587563;
                        dr["EXT_SCORING_WIDTH"] = line.F580024.S8Z587569;
                        dr["INT_PRESERVE_COLOR"] = line.F580024.S8Z587578;
                        dr["INT_PRESERVE_CUTBACK"] = line.F580024.S8Z587575;
                        dr["INT_SCORING_WIDTH"] = line.F580024.S8Z587581;
                        dr["OPACIFIER"] = line.F580024.S8Z587584;
                        dr["OVERLAY_BAR"] = line.F580024.S8Z5805735;
                        //dr{"PRESERVE_PROCESSING_CD"] = line.F580024.S8Z5800520
                        //dr["PROTECTIVE_FILM_CD"] = item.GlassItem.ExteriorSurface.PreserveProcessing;
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitSurfaceEbsTable : DataTable
    {

        public SpecUnitSurfaceEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitSurface]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("ExtPreserveColor", typeof(string));
            Columns.Add("ExtPreserveCutback", typeof(string));
            Columns.Add("ExtScoringWidth", typeof(decimal));
            Columns.Add("IntPreserveColor", typeof(string));
            Columns.Add("IntPreserveCutback", typeof(string));
            Columns.Add("IntScoringWidth", typeof(decimal));
            Columns.Add("Opacifier", typeof(string));
            Columns.Add("OverlayBar", typeof(string));
            Columns.Add("ProtectiveFilm", typeof(string));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    if (line.F580021.S7Z58L1TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["ExtPreserveColor"] = line.F580024.S8Z587566;
                        dr["ExtPreserveCutback"] = line.F580024.S8Z587563;
                        dr["ExtScoringWidth"] = line.F580024.S8Z587569;
                        dr["IntPreserveColor"] = line.F580024.S8Z587578;
                        dr["IntPreserveCutback"] = line.F580024.S8Z587575;
                        dr["IntScoringWidth"] = line.F580024.S8Z587581;
                        dr["Opacifier"] = line.F580024.S8Z587584;
                        dr["OverlayBar"] = line.F580024.S8Z5805735;
                        //dr{"PRESERVE_PROCESSING_CD"] = line.F580024.S8Z5800520
                        //dr["PROTECTIVE_FILM_CD"] = item.GlassItem.ExteriorSurface.PreserveProcessing;
                        Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
