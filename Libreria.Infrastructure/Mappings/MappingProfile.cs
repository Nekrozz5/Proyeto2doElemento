using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Infrastructure.DTOs.Autor;
using Libreria.Infrastructure.DTOs.Cliente;
using Libreria.Infrastructure.DTOs.DetalleFactura;
using Libreria.Infrastructure.DTOs.Factura;
using Libreria.Infrastructure.DTOs.Libro;
using System;

namespace Libreria.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // =======================
            // LIBRO
            // =======================
            CreateMap<Libro, LibroDto>()
                .ForMember(dest => dest.AutorNombre,
                    opt => opt.MapFrom(src =>
                        src.Autor != null
                            ? $"{src.Autor.Nombre} {src.Autor.Apellido}"
                            : string.Empty))
                .ForMember(dest => dest.AnioPublicacion,
                    opt => opt.MapFrom(src => src.AnioPublicacion))
                .ForMember(dest => dest.Precio,
                    opt => opt.MapFrom(src => Math.Round(src.Precio, 2))) // <-- Redondeo
                .ForSourceMember(src => src.Autor, opt => opt.DoNotValidate());

            CreateMap<LibroCreateDto, Libro>()
                .ForMember(dest => dest.AnioPublicacion,
                    opt => opt.MapFrom(src => src.AnioPublicacion));

            CreateMap<LibroUpdateDto, Libro>()
                .ForMember(dest => dest.AnioPublicacion,
                    opt => opt.MapFrom(src => src.AnioPublicacion));

            // =======================
            // AUTOR
            // =======================
            CreateMap<Autor, AutorDTO>()
                .ForMember(d => d.Libros, o => o.MapFrom(s => s.Libros));
            CreateMap<AutorCreateDto, Autor>();
            CreateMap<AutorUpdateDto, Autor>();

            // =======================
            // CLIENTE
            // =======================
            CreateMap<Cliente, ClienteDto>();
            CreateMap<ClienteCreateDto, Cliente>();
            CreateMap<ClienteUpdateDto, Cliente>();

            // =======================
            // DETALLE FACTURA
            // =======================
            CreateMap<DetalleFactura, DetalleFacturaDTO>()
                .ForMember(d => d.LibroTitulo, o => o.MapFrom(s => s.Libro.Titulo))
                .ForMember(d => d.LibroPrecio, o => o.MapFrom(s => Math.Round(s.Libro.Precio, 2))) // <-- Redondeo
                .ForMember(d => d.PrecioUnitario, o => o.MapFrom(s => Math.Round(s.PrecioUnitario, 2))) // <-- Redondeo
                .ForMember(d => d.Subtotal, o => o.MapFrom(s => Math.Round(s.Subtotal, 2))); // <-- Redondeo

            CreateMap<DetalleFacturaCreateDTO, DetalleFactura>()
                .ForMember(dest => dest.PrecioUnitario, opt => opt.Ignore()) // calculado en servicio
                .ForMember(dest => dest.Subtotal, opt => opt.Ignore())       // calculado en servicio
                .ReverseMap();

            // =======================
            // FACTURA
            // =======================
            CreateMap<Factura, FacturaDTO>()
                .ForMember(d => d.ClienteNombre, o => o.MapFrom(s => s.Cliente.Nombre + " " + s.Cliente.Apellido))
                .ForMember(d => d.Detalles, o => o.MapFrom(s => s.DetalleFacturas))
                .ForMember(d => d.Total, o => o.MapFrom(s => Math.Round(s.Total, 2))); // <-- Redondeo

            CreateMap<FacturaCreateDTO, Factura>()
                .ForMember(dest => dest.Total, opt => opt.Ignore()) // calculado en servicio
                .ForMember(dest => dest.DetalleFacturas, opt => opt.MapFrom(src => src.Detalles))
                .ReverseMap();

            CreateMap<FacturaUpdateDTO, Factura>();
        }
    }
}
