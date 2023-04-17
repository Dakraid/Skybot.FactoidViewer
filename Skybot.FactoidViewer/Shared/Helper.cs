namespace Skybot.FactoidViewer.Shared
{
    public static class Helper
    {
        public static DateTime? FromUnixTime(this long? unixTime)
        {
            return unixTime.HasValue ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime.Value) : null;
        }
    }
}
