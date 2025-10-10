using FluentValidation;
using Libreria.Infrastructure.DTOs.Factura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Api.Validators
{
    public class FacturaCreateValidator : AbstractValidator<FacturaCreateDTO>
    {
        public FacturaCreateValidator()
        {
            RuleFor(x => x.ClienteId)
                .GreaterThan(0).WithMessage("Debe seleccionar un cliente válido.");

            RuleFor(x => x.Detalles)
                .NotEmpty().WithMessage("Debe incluir al menos un detalle de factura.");
        }
    }
}
