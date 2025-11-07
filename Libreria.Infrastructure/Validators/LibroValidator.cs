using FluentValidation;
using Libreria.Infrastructure.DTOs.Libro;

namespace Libreria.Infrastructure.Validators
{
    public class LibroValidator : AbstractValidator<LibroCreateDto>
    {
        public LibroValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es obligatorio.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria.");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

            RuleFor(x => x.AutorId)
                .GreaterThan(0).WithMessage("Debe especificar un autor válido.");
        }
    }
}
