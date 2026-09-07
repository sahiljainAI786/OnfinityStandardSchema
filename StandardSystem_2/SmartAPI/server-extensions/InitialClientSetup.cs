using VAdvantage.Model;          // MSetup, TenantInfoM
using VAdvantage.ProcessEngine;  // SvrProcess, ProcessInfoParameter
using VAdvantage.Utility;        // Util

namespace VAdvantage.Process
{
    /// <summary>
    /// Server-side wrapper that exposes the Initial Client Setup (MSetup.CreateClient) as a
    /// registered AD_Process, so it can be invoked headlessly via VAAPI RunProcessBySearchKey
    /// (and therefore through the Onfinity Smart API: POST /v1/Tenant/actions/initialSetup).
    ///
    /// Initial Client Setup is otherwise only reachable through the web setup wizard (VSetupModel),
    /// which calls MSetup.CreateClient(clientName, orgName, userClient, userOrg). This class reads the
    /// same four values from process parameters and calls it - provisioning the client, its first
    /// organisation, the admin + organisation users, roles, accounting schema, calendar and sequences
    /// in one transaction.
    ///
    /// REGISTER (see README.md): AD_Process Value/SearchKey = "InitialClientSetup",
    /// ClassName = "VAdvantage.Process.InitialClientSetup", with 4 String AD_Process_Para:
    /// ClientName, OrgName, AdminUserName, NormalUserName.
    /// </summary>
    public class InitialClientSetup : SvrProcess
    {
        private string _clientName;
        private string _orgName;
        private string _adminUser;   // userClient
        private string _orgUser;     // userOrg

        protected override void Prepare()
        {
            foreach (ProcessInfoParameter p in GetParameter())
            {
                if (p.GetParameter() == null) continue;
                switch (p.GetParameterName())
                {
                    case "ClientName":     _clientName = Util.GetValueOfString(p.GetParameter()); break;
                    case "OrgName":        _orgName    = Util.GetValueOfString(p.GetParameter()); break;
                    case "AdminUserName":  _adminUser  = Util.GetValueOfString(p.GetParameter()); break;
                    case "NormalUserName": _orgUser    = Util.GetValueOfString(p.GetParameter()); break;
                }
            }
        }

        protected override string DoIt()
        {
            if (string.IsNullOrEmpty(_clientName) || string.IsNullOrEmpty(_orgName)
                || string.IsNullOrEmpty(_adminUser) || string.IsNullOrEmpty(_orgUser))
                return "@Error@ ClientName, OrgName, AdminUserName and NormalUserName are required.";

            // Same call the setup wizard makes (VSetupModel.CreateClient).
            MSetup ms = new MSetup(GetCtx(), 0);
            TenantInfoM info = ms.CreateClient(_clientName, _orgName, _adminUser, _orgUser);

            if (info == null)
                return "@Error@ Initial Client Setup failed for '" + _clientName + "'.";

            return "Tenant '" + _clientName + "' created: organisation '" + _orgName +
                   "', users '" + _adminUser + "' / '" + _orgUser +
                   "', roles, accounting schema, calendar and sequences provisioned.";
        }
    }
}
