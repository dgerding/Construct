using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewall
{
    public class Manager
    {
        //  Code reference
        /* http://stackoverflow.com/questions/15269887/using-netfwtypelib-to-block-or-unblock-ports-using-firewall-settings-didnt-work */


        private static INetFwRule GenerateNewFirewallRule()
        {
            INetFwRule newRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("2C5BC43E-3369-4C33-AB0C-BE9469677AF4")));

            newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            newRule.Enabled = true;
			newRule.InterfaceTypes = "All";

            return newRule;
        }

        private static INetFwPolicy2 FirewallPolicy
        {
            get { return (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("E2B3C97F-6AE1-41AC-817A-F6F92166D7DD"))); }
        }

		//	Adds the application to the list of allowed network applications, removes all
		//	blocking rules for the application, and adds Allow rules for the program on the
		//	specified ports.
		public static void EnableApplicationNetworking(String applicationPath, ushort singlePort)
		{
			EnableApplicationNetworking(applicationPath, singlePort, singlePort);
		}

		public static void EnableApplicationNetworking(String applicationPath, ushort minPort, ushort maxPort)
		{
			//	Must be absolute path
			applicationPath = Path.GetFullPath(applicationPath);
			String humanName = Path.GetFileNameWithoutExtension(applicationPath);

			ResetApplicationAttributes(applicationPath);

			AppTrust.TrustApp(applicationPath, humanName);

			String portString;
			if (minPort == maxPort)
				portString = minPort.ToString();
			else
				portString = minPort + "-" + maxPort;

			//	PROBLEMS WITH THE CODE:
			
			/* 
			 * 
			 */

			//	Create Allow rule for listening
			var allowListenRule = GenerateNewFirewallRule();
			allowListenRule.ApplicationName = applicationPath;
			allowListenRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
			allowListenRule.Name = "Allow Receive " + humanName;
			//allowListenRule.LocalPorts = portString;
			FirewallPolicy.Rules.Add(allowListenRule);

			// Create rule for sending
			var allowSendRule = GenerateNewFirewallRule();
			allowSendRule.ApplicationName = applicationPath;
			allowSendRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
			allowSendRule.Name = "Allow Send " + humanName;
			//allowSendRule.RemotePorts = portString;
			FirewallPolicy.Rules.Add(allowSendRule);
		}

		//	Removes all firewall attributes for the given application
		public static void ResetApplicationAttributes(String applicationPath)
		{
			//	Must be absolute path
			applicationPath = Path.GetFullPath(applicationPath);

			var firewallRules = FirewallPolicy.Rules;

			List<String> existingRuleList = new List<String>();
			foreach (INetFwRule rule in firewallRules)
			{
				if (rule.ApplicationName == null)
					continue;

				if (rule.ApplicationName.ToLower() == applicationPath.ToLower())
					existingRuleList.Add(rule.Name);
			}

			existingRuleList.ForEach((name) => firewallRules.Remove(name));

			if (AppTrust.AppHasTrust(applicationPath))
				AppTrust.RemoveAppTrust(applicationPath);
		}
    }
}
