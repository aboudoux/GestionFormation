using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Companies;
using GestionFormation.CoreDomain.Companies.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class SocieteListVm : EditableListVm<EditableSociete>
    {
        private readonly ICompanyQueries _companyQueries;

        public override string Title => "Liste des sociétés";

        public SocieteListVm(IApplicationService applicationService, ICompanyQueries companyQueries) : base(applicationService)
        {
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
        }

        protected override async Task<IReadOnlyList<EditableSociete>> LoadAsync()
        {
            return await Task.Run(() => _companyQueries.GetAll().Select(a => new EditableSociete(a)).ToList());
        }

        protected override async Task CreateAsync(EditableSociete item)
        {
            await Task.Run(()=> ApplicationService.Command<CreateCompany>().Execute(item.Nom, item.Adresse, item.CodePostal, item.Ville));
        }

        protected override async Task UpdateAsync(EditableSociete item)
        {
            await Task.Run(()=> ApplicationService.Command<UpdateCompany>().Execute(item.GetId(), item.Nom, item.Adresse, item.CodePostal, item.Ville));
        }

        protected override async Task DeleteAsync(EditableSociete item)
        {
            await Task.Run(()=>ApplicationService.Command<DeleteCompany>().Execute(item.GetId()));
        }
    }

    public class EditableSociete : EditableItem
    {
        public EditableSociete()
        {
            
        }

        public EditableSociete(ICompanyResult result) : base(result.CompanyId)
        {
            Nom = result.Name;
            Adresse = result.Address;
            CodePostal = result.ZipCode;
            Ville = result.City;
        }        
        

        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
    }
}