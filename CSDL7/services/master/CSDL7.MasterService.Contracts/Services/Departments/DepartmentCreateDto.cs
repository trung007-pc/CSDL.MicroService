using System.ComponentModel.DataAnnotations;

namespace CSDL7.MasterService.Services.Departments
{
    public class CreateDepartmentDto
    {
        //[Required]
        public string Name { get; set; } = null!;
    }
}