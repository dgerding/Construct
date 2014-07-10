using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;

namespace Firewall
{
	public static class AppTrust
	{
		private static INetFwMgr FirewallManager
		{
			get { return (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("304CE942-6E39-40D8-943A-B913C40C9CD4"))); }
		}


		private static INetFwAuthorizedApplications AuthorizedApplications
		{
			get { return FirewallManager.LocalPolicy.CurrentProfile.AuthorizedApplications; }
		}

		/* "Trust" refers to the Windows Firewalls' list of permitted applications. Can allow access on public
		 *	networks, haven't found any information for allowing access on private networks through this method. Can be enabled
		 *	through firewall rules.
		 */

		public static void TrustApp(String applicationPath, String applicationName)
		{
			applicationPath = Path.GetFullPath(applicationPath);

			Type type = Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication");
			INetFwAuthorizedApplication authapp = Activator.CreateInstance(type)
				as INetFwAuthorizedApplication;
			authapp.Name = applicationName;
			authapp.ProcessImageFileName = applicationPath;
			authapp.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;

			authapp.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY;
			authapp.Enabled = true;

			AuthorizedApplications.Add(authapp);
		}

		/* Note: RemoveTrust/HasTrust only work on applications that have been programmatically authorized. Don't know
		 * why, but applications that are approved through the Windows Firewall UI (that pops up when a program tries
		 * to listen) don't show up on the queried list of applications.
		 */

		public static void RemoveAppTrust(String applicationPath)
		{
			applicationPath = Path.GetFullPath(applicationPath);
			AuthorizedApplications.Remove(applicationPath);
		}

		public static bool AppHasTrust(String applicationPath)
		{
			applicationPath = Path.GetFullPath(applicationPath);
			try
			{
				var authorizedItem = AuthorizedApplications.Item(applicationPath);
				return authorizedItem != null && authorizedItem.Enabled;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
