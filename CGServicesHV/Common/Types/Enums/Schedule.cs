namespace CGServicesHV.Common.Types.Enums
{
    public class Schedule
    {
        public enum MTOScheduleEnvironmentEnum
        {
            Fuse,
            Karl,
            Both
        }

        public enum StatusUpdateEvent
        {
            Default,
            Reset,
            Cancel,
            Success,
            Complete
        }

    }
}
