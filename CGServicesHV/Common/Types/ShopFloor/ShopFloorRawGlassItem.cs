using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class ShopFloorRawGlassItems : List<ShopFloorRawGlassItem>
    {
    }

    [DataContract(Namespace = "")]
    public class ShopFloorRawGlassItem
    {
        public ShopFloorRawGlassItem()
        {
            ItemNumber = "";
            BusinessUnit = "";
            PartType = "";
            WorderNum = 0;
            DocType = "";
            Desc1 = "";
            Desc2 = "";
            Location = "";
            OrderQty = 0;
            QtyAvail = 0;
            LiteThickness = 0;
            LiteHeat = "";
            LiteColor = "";
            FirstSrfCoating = "";
            SecondSrfCoating = "";
            LitePattern = "";
            BaseDim = 0;
            BaseN = 0;
            BaseD = 0;
            LeftDim = 0;
            LeftN = 0;
            LeftD = 0;
            RightDim = 0;
            RightN = 0;
            RightD = 0;
            TopDim = 0;
            TopN = 0;
            TopD = 0;
            ShapeNum = "";
            E1BatchID = "";
            S7ITM = 0;
            S7Z58SCN = 0;
            WMOPSQ = 0;
        }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string ItemNumber { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string BusinessUnit { get; set; }
        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string PartType { get; set; }
        [DataMember(Order = 40, EmitDefaultValue = false)]
        public decimal WorderNum { get; set; }
        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string DocType { get; set; }
        [DataMember(Order = 60, EmitDefaultValue = false)]
        public string Desc1 { get; set; }
        [DataMember(Order = 70, EmitDefaultValue = false)]
        public string Desc2 { get; set; }
        [DataMember(Order = 80, EmitDefaultValue = false)]
        public string Location { get; set; }
        [DataMember(Order = 90, EmitDefaultValue = false)]
        public decimal OrderQty { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public decimal QtyAvail { get; set; }
        [DataMember(Order = 110, EmitDefaultValue = false)]
        public decimal LiteThickness { get; set; }
        [DataMember(Order = 120, EmitDefaultValue = false)]
        public string LiteHeat { get; set; }
        [DataMember(Order = 130, EmitDefaultValue = false)]
        public string LiteColor { get; set; }
        [DataMember(Order = 140, EmitDefaultValue = false)]
        public string FirstSrfCoating { get; set; }
        [DataMember(Order = 150, EmitDefaultValue = false)]
        public string SecondSrfCoating { get; set; }
        [DataMember(Order = 160, EmitDefaultValue = false)]
        public string LitePattern { get; set; }
        [DataMember(Order = 170, EmitDefaultValue = false)]
        public decimal BaseDim { get; set; }
        [DataMember(Order = 180, EmitDefaultValue = false)]
        public decimal BaseN { get; set; }
        [DataMember(Order = 190, EmitDefaultValue = false)]
        public decimal BaseD { get; set; }
        [DataMember(Order = 200, EmitDefaultValue = false)]
        public decimal LeftDim { get; set; }
        [DataMember(Order = 210, EmitDefaultValue = false)]
        public decimal LeftN { get; set; }
        [DataMember(Order = 220, EmitDefaultValue = false)]
        public decimal LeftD { get; set; }
        [DataMember(Order = 230, EmitDefaultValue = false)]
        public decimal RightDim { get; set; }
        [DataMember(Order = 240, EmitDefaultValue = false)]
        public decimal RightN { get; set; }
        [DataMember(Order = 250, EmitDefaultValue = false)]
        public decimal RightD { get; set; }
        [DataMember(Order = 260, EmitDefaultValue = false)]
        public decimal TopDim { get; set; }
        [DataMember(Order = 270, EmitDefaultValue = false)]
        public decimal TopN { get; set; }
        [DataMember(Order = 280, EmitDefaultValue = false)]
        public decimal TopD { get; set; }

        [DataMember(Order = 290, EmitDefaultValue = false)]
        public string ShapeNum { get; set; }
        [DataMember(Order = 300, EmitDefaultValue = false)]
        public string E1BatchID { get; set; }
        [DataMember(Order = 310, EmitDefaultValue = false)]
        public decimal S7ITM { get; set; }
        [DataMember(Order = 320, EmitDefaultValue = false)]
        public decimal S7Z58SCN { get; set; }
        [DataMember(Order = 330, EmitDefaultValue = false)]
        public decimal WMOPSQ { get; set; }
    }
}
