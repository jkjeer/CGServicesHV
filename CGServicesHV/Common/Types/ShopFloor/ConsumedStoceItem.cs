using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CGServicesHV.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class ConsumedStoceItems : List<ConsumedStoceItem>
    {
    }

    public class ConsumedStoceItem
    {
    [DataMember(Order = 1)]
    public int GLStockID { get; set; }
    [DataMember(Order = 2)]
    public string PlantID { get; set; }
    [DataMember(Order = 3)]
    public string ProductionLineCode { get; set; }
    [DataMember(Order = 4)]
    public string LineOperationCode { get; set; }
    [DataMember(Order = 5)]
    public string LineTypeCode { get; set; }
    [DataMember(Order = 6)]
    public string E1WorkCenter { get; set; }
    [DataMember(Order = 7)]
    public string E1Operation { get; set; }
    [DataMember(Order = 8)]
    public decimal RawThick { get; set; }
    [DataMember(Order = 9)]
    public string RawCTCFCode { get; set; }
    [DataMember(Order = 10)]
    public string Color { get; set; }
    [DataMember(Order = 11)]
    public string HeatTreatment { get; set; }
    [DataMember(Order = 12)]
    public string FirstSrfCoating { get; set; }
    [DataMember(Order = 13)]
    public string SecondSrfCoating { get; set; }
    [DataMember(Order = 14)]
    public string Pattern { get; set; }
    [DataMember(Order = 15)]
    public int RawGlassBaseL { get; set; }
    [DataMember(Order = 16)]
    public int RawGlassBaseN { get; set; }
    [DataMember(Order = 17)]
    public int RawGlassBaseD { get; set; }
    [DataMember(Order = 18)]
    public int RawGlassLeftL { get; set; }
    [DataMember(Order = 19)]
    public int RawGlassLeftN { get; set; }
    [DataMember(Order = 20)]
    public int RawGlassLeftD { get; set; }
    [DataMember(Order = 21)]
    public int SheetsConsumed { get; set; }
    [DataMember(Order = 22)]
    public int SheetsRejected { get; set; }
    [DataMember(Order = 23)]
    public string Shift { get; set; }
    [DataMember(Order = 24)]
    public string CuttingTable { get; set; }
    [DataMember(Order = 25)]
    public string RawCrateNumber { get; set; }
    [DataMember(Order = 26)]
    public DateTime? WrittenDate { get; set; }
    [DataMember(Order = 27)]
    public string Complete { get; set; }
    [DataMember(Order = 28)]
    public bool Transferred { get; set; }
    [DataMember(Order = 29)]
    public DateTime? TransferDate { get; set; }
    [DataMember(Order = 30)]
    public bool Upload { get; set; }
    [DataMember(Order = 31)]
    public string Crew { get; set; }
    [DataMember(Order = 32)]
    public string RejectCode { get; set; }
    [DataMember(Order = 33)]
    public DateTime? ProductionDate { get; set; }
    [DataMember(Order = 34)]
    public string Workstation { get; set; }
    [DataMember(Order = 35)]
    public int RawGlassRightL { get; set; }
    [DataMember(Order = 36)]
    public int RawGlassRightN { get; set; }
    [DataMember(Order = 37)]
    public int RawGlassRightD { get; set; }
    [DataMember(Order = 38)]
    public int RawGlassTopL { get; set; }
    [DataMember(Order = 39)]
    public int RawGlassTopN { get; set; }
    [DataMember(Order = 40)]
    public int RawGlassTopD { get; set; }
    [DataMember(Order = 41)]
    public int RawGlassM1L { get; set; }
    [DataMember(Order = 42)]
    public int RawGlassM1N { get; set; }
    [DataMember(Order = 43)]
    public int RawGlassM1D { get; set; }
    [DataMember(Order = 44)]
    public int RawGlassM2L { get; set; }
    [DataMember(Order = 45)]
    public int RawGlassM2N { get; set; }
    [DataMember(Order = 46)]
    public int RawGlassM2D { get; set; }
    [DataMember(Order = 47)]
    public bool DoNotUpload { get; set; }
    [DataMember(Order = 48)]
    public bool ShiftOverride { get; set; }
    [DataMember(Order = 49)]
    public bool DefaultSN { get; set; }
    [DataMember(Order = 50)]
    public bool DefaultSNNotification { get; set; }
    [DataMember(Order = 51)]
    public DateTime? DefaultSNNotificationTS { get; set; }
    [DataMember(Order = 52)]
    public int Archive { get; set; }
    [DataMember(Order = 53)]
    public string SNUpdateUID { get; set; }
    [DataMember(Order = 54)]
    public int ConsumedStoceID { get; set; }
    [DataMember(Order = 55)]
    public int RejectedStoceID { get; set; }
    [DataMember(Order = 56)]
    public bool ReportingOnly { get; set; }

    [DataMember(Order = 100)]
    public long TransferId { get; set; }
    [DataMember(Order = 100)]
    public long TransferBatchId { get; set; }
    [DataMember(Order = 102)]
    public string TransferStatus { get; set; }
    [DataMember(Order = 104)]
    public string TransferMessage { get; set; }


    }
}
