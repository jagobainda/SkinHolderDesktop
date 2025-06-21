using System.Net.NetworkInformation;

namespace SkinHolderDesktop.Utils;

public static class ConnectionPing
{
    public static long GetPingTime(string url)
    {
        try
        {
            using var ping = new Ping();

            var reply = ping.Send(url);

            if (reply.Status == IPStatus.Success) return reply.RoundtripTime;

            return -1;
        }
        catch
        {
            return -1;
        }
    }
}
