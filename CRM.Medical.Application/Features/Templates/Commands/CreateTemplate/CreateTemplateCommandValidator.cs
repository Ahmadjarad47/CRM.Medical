using FluentValidation;

namespace CRM.Medical.Application.Features.Templates.Commands.CreateTemplate;

public sealed class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Role)
            .NotEmpty();
    }
}

