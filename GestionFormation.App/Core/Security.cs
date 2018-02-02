using System;
using DevExpress.Mvvm;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.App.Core
{
    public class Security
    {
        private readonly IApplicationService _applicationService;

        public Security(IApplicationService applicationService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        public bool this [UserRole autorisedRole] => autorisedRole >= _applicationService.LoggedUser.Role;
    }
}