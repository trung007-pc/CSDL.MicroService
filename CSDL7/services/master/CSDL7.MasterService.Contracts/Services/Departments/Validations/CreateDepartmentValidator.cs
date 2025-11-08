using FluentValidation;

namespace CSDL7.MasterService.Services.Departments.Validations;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tên phòng ban không được để trống.");
    }
}