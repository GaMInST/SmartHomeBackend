namespace SmartHomeBackend.Models
{
    public record LanSwitchDto(string Ip, string LocalKey, string DeviceId, bool On);
}
