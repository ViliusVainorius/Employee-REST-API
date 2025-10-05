using VismaAPI.Models;
using VismaAPI.Validators;

namespace VismaAPIUnitTests.Validators;

[TestClass]
public class UpdateSalaryDtoValidatorTests
{
    private readonly UpdateSalaryDtoValidator _validator;

    public UpdateSalaryDtoValidatorTests()
    {
        _validator = new UpdateSalaryDtoValidator();
    }

    private UpdateSalaryDto CreateValidSalaryDto()
    {
        return new UpdateSalaryDto
        {
            NewSalary = 1000
        };
    }

    [TestMethod]
    public void Validate_ValidSalary_ShouldPassValidation()
    {
        var dto = CreateValidSalaryDto();

        var result = _validator.Validate(dto);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_NegativeSalary_ShouldHaveValidationError()
    {
        var dto = CreateValidSalaryDto();
        dto.NewSalary = -500;

        var result = _validator.Validate(dto);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "NewSalary"));
    }

    [TestMethod]
    public void Validate_ZeroSalary_ShouldPassValidation()
    {
        var dto = CreateValidSalaryDto();
        dto.NewSalary = 0;

        var result = _validator.Validate(dto);

        Assert.IsTrue(result.IsValid);
    }
}
