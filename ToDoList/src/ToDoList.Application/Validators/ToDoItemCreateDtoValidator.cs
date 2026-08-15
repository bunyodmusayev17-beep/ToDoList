using FluentValidation;
using ToDoList.Application.Dtos;

namespace ToDoList.Application.Validators;

public class ToDoItemCreateDtoValidator : AbstractValidator<ToDoItemCreateDto>
{
    public ToDoItemCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority is not a valid value.");
    }
}
