using System;
using System.Collections.Generic;
using CGServicesHV.Common.Managers.Process;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using SoaCore.Wcf;

namespace CGServicesHV.ScheduleSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ScheduleService : IScheduleService
    {
        public MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.GetUnprocessedMTSScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public MTSScheduleEvents GetUnprocessedOutsideMTSScheduleEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            try
            {
                return OutsideMTSScheduleProcessor.GetUnprocessedOutsideMTSScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse BackoutSchedule(string plantCode, string schedulePlantNum, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"SchedulePlantNum", schedulePlantNum},
                    {"ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.BackoutSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse CancelMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CancelMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.ResetMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse ResetOutsideMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.ResetOutsideMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse SuccessMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.SuccessMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CompleteMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CompleteMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadHVQueTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.DownloadHVQueTable(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideHVQueTable(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                return OutsideMTSScheduleProcessor.DownloadOutsideHVQueTable(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ScheduleRestService : IScheduleRestService
    {
        public MTSScheduleEvents GetUnprocessedMTSScheduleEvents(string plantCode = null)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.GetUnprocessedMTSScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public MTSScheduleEvents GetUnprocessedOutsideMTSScheduleEvents(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            try
            {
                return OutsideMTSScheduleProcessor.GetUnprocessedOutsideMTSScheduleEvents(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse BackoutSchedule(string plantCode, string schedulePlantNum, decimal scheduleNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException($"4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"ScheduleNum", scheduleNum}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plantCode, ScheduleProcessor.BackoutSchedule, false, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse CancelMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CancelMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse ResetMTSScheduleEvent(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.ResetMTSScheduleEvent(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse ResetOutsideMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.ResetOutsideMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse SuccessMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.SuccessMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse CompleteMTSScheduleEvent(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new ApplicationException("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.CompleteMTSScheduleEvent(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
        }

        public ProcessResponse DownloadHVQueTable(string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                return MTSScheduleProcessor.DownloadHVQueTable(plantCode, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadOutsideHVQueTable(string plantCode, string schedulePlantNum, decimal scheduleNum, decimal scheduleDateE1 = 0, string batchNum = null)
        {
            if (string.IsNullOrEmpty(plantCode) || plantCode.Length < 4)
            {
                throw new ApplicationException("4 letter Plant Code is REQUIRED.");
            }
            if (string.IsNullOrEmpty(schedulePlantNum) || schedulePlantNum.Length < 2)
            {
                throw new ApplicationException($"2 digit Schedule Plant Num is REQUIRED.");
            }
            if (scheduleNum == 0)
            {
                throw new Exception("Schedule Number is REQUIRED.");
            }
            try
            {
                return OutsideMTSScheduleProcessor.DownloadOutsideHVQueTable(plantCode, schedulePlantNum, scheduleNum, scheduleDateE1, batchNum);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
