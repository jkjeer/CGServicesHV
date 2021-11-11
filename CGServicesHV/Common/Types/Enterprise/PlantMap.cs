using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable UnusedVariable

namespace CGServices.Common.Types.Enterprise
{
    [DataContract(Namespace = "")]
    public class PlantAttribute
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Code { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string NumericCode { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public int Numeric { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public int TimeOffset { get; set; }

        public PlantAttribute()
        {
        }

        public PlantAttribute(string plantCode)
        {
            NumericCode = CleanPlantNum(plantCode);
            var plantMap = new PlantMap();
            var plantAttribute = plantMap.GetAttributesByNum(NumericCode);
            Code = plantAttribute.Code;
            Numeric = plantAttribute.Numeric;
            TimeOffset = plantAttribute.TimeOffset;
        }

        public PlantAttribute(string code, string numericCode, int numeric, int timeOffset)
        {
            Code = code;
            NumericCode = numericCode;
            Numeric = numeric;
            TimeOffset = timeOffset;
        }

        public PlantAttribute GetPlantAttributeById(string plantId)
        {
            var plantMap = new PlantMap();
            return plantMap.Nums[CleanPlantNum(plantId)];
        }

        public static string CleanPlantNum(string plantCode)
        {
            var plantNum = "";
            if (string.IsNullOrEmpty(plantCode) || plantCode == "0")
            {
                plantNum = "0";
            }
            else if (plantCode.ToUpper().Contains("SUB"))
            {
                plantNum = "SUB";
            }
            else
            {
                try
                {
                    var plantId = plantCode.PadLeft(2, '0').ToUpper();
                    switch (plantId.Length)
                    {
                        case 2:
                            plantNum = plantId;
                            //GetPlantCode(plantId);
                            break;
                        case 4:
                            plantNum = GetPlantId(plantId);
                            break;
                    }
                }
                catch (Exception)
                {
                    throw new ApplicationException($"{plantCode} does not EXIST in Plant Map");
                }
            }
            return plantNum;
        }

        static string GetPlantCode(string plantId)
        {
            var plantMap = new PlantMap();
            var plantCode = plantMap.GetAttributesByNum(plantId).Code;
            return plantCode;
        }

        static string GetPlantId(string plantCode)
        {
            var plantMap = new PlantMap();
            var plantId = plantMap.GetAttributesByCode(plantCode).NumericCode;
            return plantId;
        }
    }

    [DataContract(Namespace = "")]
    public class PlantMap
    {
        public Dictionary<string, PlantAttribute> Codes { get; set; }
        public Dictionary<string, PlantAttribute> Nums { get; set; }

        public PlantMap()
        {
            var codes = new Dictionary<string, PlantAttribute>
            {
                {"", new PlantAttribute("", "0", 0, 0)},
                {"MPIG", new PlantAttribute("MPIG", "01", 1, 0)},
                {"GFIG", new PlantAttribute("GFIG", "02", 2, 0)},
                {"SGIG", new PlantAttribute("SGIG", "03", 3, 0)},
                {"NFCG", new PlantAttribute("NFCG", "04", 4, 0)},
                {"SGCG", new PlantAttribute("SGCG", "05", 5, 0)},
                {"TOIG", new PlantAttribute("TOIG", "06", 6, 0)},
                {"TOTG", new PlantAttribute("TOTG", "07", 7, 0)},
                {"AMLG", new PlantAttribute("AMLG", "08", 8, 0)},
                {"MNFG", new PlantAttribute("MNFG", "09", 9, 0)},
                {"BFCG", new PlantAttribute("BFCG", "10", 10, 1)},
                {"TWCG", new PlantAttribute("TWCG", "11", 11, -2)},
                {"PTFG", new PlantAttribute("PTFG", "12", 12, 0)},
                {"FMIG", new PlantAttribute("FMIG", "14", 14, 1)},
                {"MVCG", new PlantAttribute("MVCG", "15", 15, -2)},
                {"FRIG", new PlantAttribute("FRIG", "16", 16, 0)},
                {"OCIG", new PlantAttribute("OCIG", "17", 17, 1)},
                {"MRFG", new PlantAttribute("MRFG", "18", 18, 0)},
                {"WXIG", new PlantAttribute("WXIG", "19", 19, 0)},
                {"GTCG", new PlantAttribute("GTCG", "20", 20, -2)},
                {"WXCG", new PlantAttribute("WXCG", "21", 21, 0)},
                {"HRIG", new PlantAttribute("HRIG", "22", 22, -2)},
                {"DRFG", new PlantAttribute("DRFG", "23", 23, 0)},
                {"CGCG", new PlantAttribute("CGCG", "24", 24, -2)},
                {"ROIG", new PlantAttribute("ROIG", "25", 25, 1)},
                {"LLCG", new PlantAttribute("LLCG", "26", 26, -1)},
                {"WLFG", new PlantAttribute("WLFG", "27", 27, -2)},
                {"WBIG", new PlantAttribute("WBIG", "28", 28, 1)},
                {"OCLG", new PlantAttribute("OCLG", "29", 29, 1)},
                {"CHTG", new PlantAttribute("CHTG", "30", 30, -2)},
                {"DRML", new PlantAttribute("DRML", "31", 31, 0)},
                {"MZST", new PlantAttribute("MZST", "32", 32, 0)},
                {"SGSC", new PlantAttribute("SGSC", "33", 33, 0)},
                {"WKIG", new PlantAttribute("WKIG", "34", 34, 0)},
                {"SGAG", new PlantAttribute("SGAG", "35", 35, 0)},
                {"SGRT", new PlantAttribute("SGRT", "36", 36, 0)},
                {"NELG", new PlantAttribute("NELG", "37", 37, 1)},
                {"LACT", new PlantAttribute("LACT", "38", 38, -2)},
                {"ISCT", new PlantAttribute("ISCT", "39", 39, 0)},
                {"MACT", new PlantAttribute("MACT", "41", 41, 1)},
                {"UTCT", new PlantAttribute("UTCT", "42", 42, 1)},
                {"ETCT", new PlantAttribute("ETCT", "43", 43, 1)},
                {"ADCT", new PlantAttribute("ADCT", "44", 44, 1)},
                {"BFIG", new PlantAttribute("BFIG", "77", 77, 0)},
                {"HDDC", new PlantAttribute("HDDC", "92", 92, 0)}
            };
            Codes = codes;

            var nums = new Dictionary<string, PlantAttribute>
            {
                {"0", new PlantAttribute("", "0", 0, 0)},
                {"01", new PlantAttribute("MPIG", "01", 1, 0)},
                {"02", new PlantAttribute("GFIG", "02", 2, 0)},
                {"03", new PlantAttribute("SGIG", "03", 3, 0)},
                {"04", new PlantAttribute("NFCG", "04", 4, 0)},
                {"05", new PlantAttribute("SGCG", "05", 5, 0)},
                {"06", new PlantAttribute("TOIG", "06", 6, 0)},
                {"07", new PlantAttribute("TOTG", "07", 7, 0)},
                {"08", new PlantAttribute("AMLG", "08", 8, 0)},
                {"09", new PlantAttribute("MNFG", "09", 9, 0)},
                {"10", new PlantAttribute("BFCG", "10", 10, 1)},
                {"11", new PlantAttribute("TWCG", "11", 11, -2)},
                {"12", new PlantAttribute("PTFG", "12", 12, 0)},
                {"14", new PlantAttribute("FMIG", "14", 14, 1)},
                {"15", new PlantAttribute("MVCG", "15", 15, -2)},
                {"16", new PlantAttribute("FRIG", "16", 16, 0)},
                {"17", new PlantAttribute("OCIG", "17", 17, 1)},
                {"18", new PlantAttribute("MRFG", "18", 18, 0)},
                {"19", new PlantAttribute("WXIG", "19", 19, 0)},
                {"20", new PlantAttribute("GTCG", "20", 20, -2)},
                {"21", new PlantAttribute("WXCG", "21", 21, 0)},
                {"22", new PlantAttribute("HRIG", "22", 22, -2)},
                {"23", new PlantAttribute("DRFG", "23", 23, 0)},
                {"24", new PlantAttribute("CGCG", "24", 24, -2)},
                {"25", new PlantAttribute("ROIG", "25", 25, 1)},
                {"26", new PlantAttribute("LLCG", "26", 26, -1)},
                {"27", new PlantAttribute("WLFG", "27", 27, -2)},
                {"28", new PlantAttribute("WBIG", "28", 28, 1)},
                {"29", new PlantAttribute("OCLG", "29", 29, 1)},
                {"30", new PlantAttribute("CHTG", "30", 30, -2)},
                {"31", new PlantAttribute("DRML", "31", 31, 0)},
                {"32", new PlantAttribute("MZST", "32", 32, 0)},
                {"33", new PlantAttribute("SGSC", "33", 33, 0)},
                {"34", new PlantAttribute("WKIG", "34", 34, 0)},
                {"35", new PlantAttribute("SGAG", "35", 35, 0)},
                {"36", new PlantAttribute("SGRT", "36", 36, 0)},
                {"37", new PlantAttribute("NELG", "37", 37, 1)},
                {"38", new PlantAttribute("LACT", "38", 38, -2)},
                {"39", new PlantAttribute("ISCT", "39", 39, 0)},
                {"41", new PlantAttribute("MACT", "41", 41, 1)},
                {"42", new PlantAttribute("UTCT", "42", 42, 1)},
                {"43", new PlantAttribute("ETCT", "43", 43, 1)},
                {"44", new PlantAttribute("ADCT", "44", 44, 1)},
                {"77", new PlantAttribute("BFIG", "77", 77, 0)},
                {"92", new PlantAttribute("HDDC", "92", 92, 0)}
            };
            Nums = nums;
        }

        public PlantAttribute GetAttributebyId(string plantId)
        {
            var pt = new PlantAttribute();
            var _plantId = plantId.PadLeft(2, '0').ToUpper();
            try
            {
                switch (_plantId.Length)
                {
                    case 2:
                        pt = Nums[_plantId];
                        break;
                    case 4:
                        pt = Codes[_plantId];
                        break;
                }
            }
            catch (Exception e)
            {
                var eMsg = e.Message;
                throw new ApplicationException($"{plantId} does not EXIST in Plant Map");
            }
            return pt;
        }

        public PlantAttribute GetAttributesByCode(string plantCode)
        {
            try
            {
                return Codes[plantCode];
            }
            catch (Exception e)
            {
                var eMsg = e.Message;
                throw new ApplicationException($"{plantCode} does not EXIST in Plant Map");
            }
        }

        public PlantAttribute GetAttributesByNum(string plantNum)
        {
            try
            {
                return Nums[plantNum];
            }
            catch (Exception e)
            {
                var eMsg = e.Message;
                throw new ApplicationException($"{plantNum} does not EXIST in Plant Map");
            }
        }
    }
}
