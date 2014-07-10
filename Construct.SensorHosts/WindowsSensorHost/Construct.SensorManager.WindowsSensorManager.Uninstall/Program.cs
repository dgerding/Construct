using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetFwTypeLib;

namespace Construct.SensorManager.WindowsSensorManager.Uninstall
{
    class Program
    {
        static void Main(string[] args)
        {
            //INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            //firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            //firewallRule.InterfaceTypes = "All";
            //firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            //firewallRule.LocalPorts = "8086-8286";
            //firewallRule.LocalAddresses = "192.168.82.0/24";
            //firewallRule.RemoteAddresses = "192.168.82.0/24";
            //firewallRule.Enabled = true;
            //firewallRule.Name = "Construct Sensor Manager / Sensors";
            //firewallRule.Description = "Allows incoming connections to the Construct Sensor Manager and Sensor suite";

            //INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            //firewallPolicy.Rules.Remove("Construct Sensor Manager / Sensors");


            INetFwRule httpFirewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            httpFirewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            httpFirewallRule.InterfaceTypes = "All";
            httpFirewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            httpFirewallRule.LocalPorts = "80";
            httpFirewallRule.LocalAddresses = "192.168.82.0/24";
            httpFirewallRule.RemoteAddresses = "192.168.82.0/24";
            httpFirewallRule.Enabled = true;
            httpFirewallRule.Name = "HTTP";
            httpFirewallRule.Description = "Allows incoming connections for http traffic";

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            firewallPolicy.Rules.Remove("HTTP");
        }
    }
}
