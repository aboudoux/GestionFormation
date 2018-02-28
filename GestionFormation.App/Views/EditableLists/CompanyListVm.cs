using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Companies;
using GestionFormation.CoreDomain.Companies.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class CompanyListVm : EditableListVm<EditableCompany>
    {
        private readonly ICompanyQueries _companyQueries;

        public override string Title => "Liste des sociétés";

        public CompanyListVm(IApplicationService applicationService, ICompanyQueries companyQueries) : base(applicationService)
        {
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
        }

        protected override async Task<IReadOnlyList<EditableCompany>> LoadAsync()
        {
            return await Task.Run(() => _companyQueries.GetAll().Select(a => new EditableCompany(a)).ToList());
        }

        protected override async Task CreateAsync(EditableCompany item)
        {
            await Task.Run(()=> ApplicationService.Command<CreateCompany>().Execute(item.Name, item.Address, item.ZipCode, item.City));
        }

        protected override async Task UpdateAsync(EditableCompany item)
        {
            await Task.Run(()=> ApplicationService.Command<UpdateCompany>().Execute(item.GetId(), item.Name, item.Address, item.ZipCode, item.City));
        }

        protected override async Task DeleteAsync(EditableCompany item)
        {
            await Task.Run(()=>ApplicationService.Command<DeleteCompany>().Execute(item.GetId()));
        }
    }

    public class EditableCompany : EditableItem
    {
        public EditableCompany()
        {
            
        }

        public EditableCompany(ICompanyResult result) : base(result.CompanyId)
        {
            Name = result.Name;
            Address = result.Address;
            ZipCode = result.ZipCode;
            City = result.City;
        }                

        [DisplayName("Nom")]
        public string Name { get; set; }        
        [DataType(DataType.MultilineText)]
        [DisplayName("Adresse")]
        public string Address { get; set; }
        [DisplayName("Code postal")]
        public string ZipCode { get; set; }
        [DisplayName("Ville")]
        public string City { get; set; }        
    }
}