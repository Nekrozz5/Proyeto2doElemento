
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Infrastructure.DTOs.Autor;
using Libreria.Infrastructure.DTOs.Autor.Libreria.Api.DTOs.Autor;
using Libreria.Infrastructure.DTOs.Cliente;
using Libreria.Infrastructure.DTOs.DetalleFactura;
using Libreria.Infrastructure.DTOs.Factura;
using Libreria.Infrastructure.DTOs.Libro;

namespace Libreria.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 📚 LIBRO
            CreateMap<Libro, LibroDto>()
                .ForMember(dest => dest.AutorNombre, opt => opt.MapFrom(src => src.Autor.Nombre));
            CreateMap<LibroCreateDto, Libro>();
            CreateMap<LibroUpdateDto, Libro>();

            // ✍️ AUTOR
            CreateMap<Autor, AutorDTO>();
            CreateMap<AutorCreateDto, Autor>();
            CreateMap<AutorUpdateDto, Autor>();

            // 👤 CLIENTE
            CreateMap<Cliente, ClienteDto>();
            CreateMap<ClienteCreateDto, Cliente>();
            CreateMap<ClienteUpdateDto, Cliente>();

            // 🧾 FACTURA
            CreateMap<Factura, FacturaDTO>()
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Nombre))
                .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.DetalleFacturas));
            CreateMap<FacturaCreateDTO, Factura>();
            CreateMap<FacturaUpdateDTO, Factura>();

            // 🧩 DETALLE FACTURA
            CreateMap<DetalleFactura, DetalleFacturaDTO>()
                .ForMember(dest => dest.LibroTitulo, opt => opt.MapFrom(src => src.Libro.Titulo));
        }
    }
}

