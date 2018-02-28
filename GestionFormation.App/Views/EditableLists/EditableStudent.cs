using System;
using System.ComponentModel;
using System.Threading.Tasks;
using GestionFormation.CoreDomain.Students.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class EditableStudent : EditableItem
    {
        public EditableStudent()
        {
            
        }
        public EditableStudent(IStudentResult result) : base(result.Id)
        {
            Lastname = result.Lastname;
            Firstname = result.Firstname;
        }


        [DisplayName("Nom")]
        public string Lastname { get; set; }
        [DisplayName("Prénom")]
        public string Firstname { get; set; }       
    }
}