using FluentValidation;
using Libreria.Infrastructure.DTOs.Libro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Api.Validators
{
    public class LibroCreateValidator : AbstractValidator<LibroCreateDto>
    {
        public LibroCreateValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título del libro es obligatorio.")
                .MaximumLength(150);

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
