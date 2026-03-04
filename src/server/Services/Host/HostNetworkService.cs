using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Server.Services;

public partial class HostNetworkService
{
    [GeneratedRegex(@"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})", RegexOptions.IgnoreCase)]
    private static partial Regex IpMacRegex();

    public async Task<(string DnsName, string MacAddress)> GetAsync(string ipAddress)
    {
        return (string.Empty, string.Empty);
        // Get DNS name
        var dnsName = ipAddress;
        try
        {
            var entry = await Dns.GetHostEntryAsync(ipAddress);
            dnsName = entry.HostName;
        }
        catch
        {
            // fallback to IP if DNS lookup fails
        }

        using (Ping ping = new())
        {
            var pingReply = await ping.SendPingAsync(ipAddress, 1000);
            if (pingReply.Status != IPStatus.Success)
            {
                return (dnsName, string.Empty);
            }
        }

        using (System.Diagnostics.Process pProcess = new())
        {
            pProcess.StartInfo.FileName               = "arp";
            pProcess.StartInfo.Arguments              = "-a ";
            pProcess.StartInfo.UseShellExecute        = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow         = true;
            pProcess.Start();
            var cmdOutput = await pProcess.StandardOutput.ReadToEndAsync();
            foreach (Match m in IpMacRegex().Matches(cmdOutput))
            {
                if (m.Groups["ip"].Value != ipAddress) continue;
                return (dnsName, string.Join("", m.Groups["mac"].Value.Split('-', ':')));
            }
        }

        var ip = IPAddress.Parse(ipAddress);
        var targetNic = NetworkInterface
                       .GetAllNetworkInterfaces()
                       .Where(n => n.GetIPProperties().UnicastAddresses.Any(a => a.Address.Equals(ip)))
                       .OrderBy(n => n.Speed)
                       .FirstOrDefault();
        if (targetNic != null)
        {
            return (dnsName, targetNic.GetPhysicalAddress().ToString());
        }

        return (dnsName, string.Empty);
    }
}