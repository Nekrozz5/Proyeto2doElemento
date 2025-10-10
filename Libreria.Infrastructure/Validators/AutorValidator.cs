using FluentValidation;
using Libreria.Infrastructure.DTOs.Autor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Api.Validators
{
    public class AutorCreateValidator : AbstractValidator<AutorCreateDto>
    {
        public AutorCreateValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50);
        }
    }
}
