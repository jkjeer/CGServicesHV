using System;
using System.Data;
using System.ServiceModel;
using CGServices.Common.Types.Enterprise;

namespace CGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitAirspaceTable : DataTable
    {

        public SpecUnitAirspaceTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SPEC_UNIT_AIRSPACE_LEDGER]");
            Columns.Add("SPEC_UNIT_ID", typeof(long));
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("AIRSPACE_NO", typeof(int));
            Columns.Add("CHANNEL_TYPE_CD", typeof(string));
            Columns.Add("CHANNEL_SIZE", typeof(decimal));
            Columns.Add("NARROW_MARGIN_CD", typeof(string));
            Columns.Add("GAS_MIXTURE_CD", typeof(string));
            Columns.Add("BUTYL_COLOR", typeof(string));
            Columns.Add("SILICON_COLOR", typeof(string));
            Columns.Add("MCODE1", typeof(decimal));
            Columns.Add("MCODE1_ORIENTATION", typeof(string));
            Columns.Add("MCODE2", typeof(decimal));
            Columns.Add("MCODE2_ORIENTATION", typeof(string));
            Columns.Add("CCODE", typeof(string));
            Columns.Add("BUMP_ON_TYPE", typeof(string));
            Columns.Add("BUMP_ON_QTY", typeof(decimal));
            Columns.Add("BUMP_ON_CFG", typeof(string));
            Columns.Add("SHADES", typeof(string));
            Columns.Add("BREATHER_TUBE", typeof(string));
            Columns.Add("TAPE", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                if (item.GlassItem.UnitType != "1")
                {
                    foreach (Airspace a in item.GlassItem.Airspaces)
                    {
                        DataRow dr = NewRow();
                        dr["ITEM_NUM"] = item.ItemNum;
                        dr["CONFIGURATION_ID"] = item.ConfigurationId;
                        dr["AIRSPACE_NO"] = a.Position;
                        dr["CHANNEL_TYPE_CD"] = a.Spacer.SpacerType;
                        dr["CHANNEL_SIZE"] = a.Spacer.Width;
                        dr["NARROW_MARGIN_CD"] = item.GlassItem.NarrowMarginCode;
                        dr["GAS_MIXTURE_CD"] = a.GasMixture;
                        dr["BUTYL_COLOR"] = a.ButylColor;
                        dr["SILICON_COLOR"] = a.SiliconeColor;
                        dr["SHADES"] = a.Shades;
                        dr["BREATHER_TUBE"] = a.Tubes;
                        dr["TAPE"] = a.Tape;
                        // BARS
                        if (a.BarAssembly != null)
                        {
                            dr["CCODE"] = a.BarAssembly.CCode;
                            dr["BUMP_ON_TYPE"] = a.BarAssembly.BumperType;
                            dr["BUMP_ON_QTY"] = a.BarAssembly.BumperQty;
                            dr["BUMP_ON_CFG"] = a.BarAssembly.BumperConfig;
                            dr["MCODE1"] = (decimal)a.BarAssembly.MuntinBars[0].MCodeNum;
                            dr["MCODE1_ORIENTATION"] = a.BarAssembly.MuntinBars[0].Orientation;
                            if (a.BarAssembly.MuntinBars.Count == 2)
                            {
                                dr["MCODE2"] = (decimal)a.BarAssembly.MuntinBars[1].MCodeNum;
                                dr["MCODE2_ORIENTATION"] = a.BarAssembly.MuntinBars[1].Orientation;
                            }
                        }
                        Rows.Add(dr);
                    }
                }
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
                    if (line.F580021.S7Z58L2TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
                        dr["AIRSPACE_NO"] = 1;
                        dr["CHANNEL_TYPE_CD"] = line.F580024.S8Z58L1STP;
                        dr["CHANNEL_SIZE"] = line.F580024.S8Z58L1SSZ/10;
                        dr["NARROW_MARGIN_CD"] = line.F580024.S8Z58IGNMC;
                        dr["GAS_MIXTURE_CD"] = line.F580024.S8Z58S1GM;
                        dr["BUTYL_COLOR"] = line.F580024.S8Z5801990;
                        dr["SILICON_COLOR"] = line.F580024.S8Z5802000;
                        dr["SHADES"] = line.F580024.S8Z58SA1;
                        dr["BREATHER_TUBE"] = line.F580024.S8Z58TA1;
                        dr["TAPE"] = line.F580024.S8Z5840470;
                        dr["CCODE"] = line.F580024.S8Z58CCA1;
                        dr["BUMP_ON_TYPE"] = line.F580024.S8Z58BA1;
                        dr["BUMP_ON_QTY"] = line.F580024.S8Z58NA1;
                        //dr["BUMP_ON_CFG"] = line.F580024.S8Z58M1A1O;
                        dr["MCODE1"] = line.F580024.S8Z58M1A1;
                        dr["MCODE1_ORIENTATION"] = line.F580024.S8Z58M1A1O;
                        dr["MCODE2"] = line.F580024.S8Z58M2A1;
                        dr["MCODE2_ORIENTATION"] = line.F580024.S8Z58M2A1O;
                        Rows.Add(dr);
                    }

                    if (line.F580021.S7Z58L3TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SPEC_UNIT_ID"] = line.SpecUnitId;
                        dr["ITEM_NUM"] = line.F580021.S7ITM;
                        dr["CONFIGURATION_ID"] = line.F580021.CMCFGID;
                        dr["AIRSPACE_NO"] = 2;
                        dr["CHANNEL_TYPE_CD"] = line.F580024.S8Z58L2STP;
                        dr["CHANNEL_SIZE"] = line.F580024.S8Z58L2SSZ / 10;
                        dr["NARROW_MARGIN_CD"] = line.F580024.S8Z58IGNMC;
                        dr["GAS_MIXTURE_CD"] = line.F580024.S8Z58S1GM;
                        dr["BUTYL_COLOR"] = line.F580024.S8Z5801990;
                        dr["SILICON_COLOR"] = line.F580024.S8Z5802000;
                        dr["SHADES"] = line.F580024.S8Z58SA2;
                        dr["BREATHER_TUBE"] = line.F580024.S8Z58TA2;
                        dr["TAPE"] = line.F580024.S8Z5840470;
                        dr["CCODE"] = line.F580024.S8Z58CCA2;
                        dr["BUMP_ON_TYPE"] = line.F580024.S8Z58BA2;
                        dr["BUMP_ON_QTY"] = line.F580024.S8Z58NA2;
                        //dr["BUMP_ON_CFG"] = line.F580024.S8Z58M1A1O;
                        dr["MCODE1"] = line.F580024.S8Z58M1A2;
                        dr["MCODE1_ORIENTATION"] = line.F580024.S8Z58M1A2O;
                        dr["MCODE2"] = line.F580024.S8Z58M2A2;
                        dr["MCODE2_ORIENTATION"] = line.F580024.S8Z58M2A2O;
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
    public class SpecUnitAirspaceEbsTable : DataTable
    {

        public SpecUnitAirspaceEbsTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitAirspace]");
            Columns.Add("SpecUnitId", typeof(int));
            Columns.Add("AirspaceNum", typeof(int));
            Columns.Add("SpacerType", typeof(string));
            Columns.Add("SpacerSize", typeof(decimal));
            Columns.Add("NarrowMarginCode", typeof(string));
            Columns.Add("GasMixture", typeof(string));
            Columns.Add("ButylColor", typeof(string));
            Columns.Add("SiliconColor", typeof(string));
            Columns.Add("MCode1", typeof(decimal));
            Columns.Add("MCode1Orient", typeof(string));
            Columns.Add("MCode2", typeof(decimal));
            Columns.Add("MCode2Orient", typeof(string));
            Columns.Add("CCode", typeof(string));
            Columns.Add("BumpOnType", typeof(string));
            Columns.Add("BumpOnQty", typeof(decimal));
            Columns.Add("BumpOnCfg", typeof(string));
            Columns.Add("Shades", typeof(string));
            Columns.Add("Tubes", typeof(string));
            Columns.Add("Tape", typeof(string));
        }

        public void Add(Schedule schedule)
        {
            try
            {
                foreach (var line in schedule.ScheduleLines)
                {
                    if (line.F580021.S7Z58L2TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["AirspaceNum"] = 1;
                        dr["SpacerType"] = line.F580024.S8Z58L1STP;
                        dr["SpacerSize"] = line.F580024.S8Z58L1SSZ / 10;
                        dr["NarrowMarginCode"] = line.F580024.S8Z58IGNMC;
                        dr["GasMixture"] = line.F580024.S8Z58S1GM;
                        dr["ButylColor"] = line.F580024.S8Z5801990;
                        dr["SiliconColor"] = line.F580024.S8Z5802000;
                        dr["Shades"] = line.F580024.S8Z58SA1;
                        dr["Tubes"] = line.F580024.S8Z58TA1;
                        dr["Tape"] = line.F580024.S8Z5840470;
                        dr["CCode"] = line.F580024.S8Z58CCA1;
                        dr["BumpOnType"] = line.F580024.S8Z58BA1;
                        dr["BumpOnQty"] = line.F580024.S8Z58NA1;
                        //dr["BUMP_ON_CFG"] = line.F580024.S8Z58M1A1O;
                        dr["MCode1"] = line.F580024.S8Z58M1A1;
                        dr["MCode1Orient"] = line.F580024.S8Z58M1A1O;
                        dr["MCode2"] = line.F580024.S8Z58M2A1;
                        dr["MCode2Orient"] = line.F580024.S8Z58M2A1O;
                        Rows.Add(dr);
                    }

                    if (line.F580021.S7Z58L3TK > 0)
                    {
                        DataRow dr = NewRow();
                        dr["SpecUnitId"] = line.SpecUnitId;
                        dr["AirspaceNum"] = 2;
                        dr["SpacerType"] = line.F580024.S8Z58L2STP;
                        dr["SpacerSize"] = line.F580024.S8Z58L2SSZ / 10;
                        dr["NarrowMarginCode"] = line.F580024.S8Z58IGNMC;
                        dr["GasMixture"] = line.F580024.S8Z58S1GM;
                        dr["ButylColor"] = line.F580024.S8Z5801990;
                        dr["SiliconColor"] = line.F580024.S8Z5802000;
                        dr["Shades"] = line.F580024.S8Z58SA2;
                        dr["Tubes"] = line.F580024.S8Z58TA2;
                        dr["Tape"] = line.F580024.S8Z5840470;
                        dr["CCode"] = line.F580024.S8Z58CCA2;
                        dr["BumpOnType"] = line.F580024.S8Z58BA2;
                        dr["BumpOnQty"] = line.F580024.S8Z58NA2;
                        //dr["BUMP_ON_CFG"] = line.F580024.S8Z58M1A1O;
                        dr["MCode1"] = line.F580024.S8Z58M1A2;
                        dr["MCode1Orient"] = line.F580024.S8Z58M1A2O;
                        dr["MCode2"] = line.F580024.S8Z58M2A2;
                        dr["MCode2Orient"] = line.F580024.S8Z58M2A2O;
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
