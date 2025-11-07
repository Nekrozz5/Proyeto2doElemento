using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Infrastructure.DTOs.Autor;

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
            // Libro
            CreateMap<Libro, LibroDto>()
                .ForMember(d => d.AutorNombre, o => o.MapFrom(s => s.Autor != null ? (s.Autor.Nombre + " " + s.Autor.Apellido) : string.Empty));
            CreateMap<LibroCreateDto, Libro>();
            CreateMap<LibroUpdateDto, Libro>();

            // Autor
            CreateMap<Autor, AutorDTO>()
                .ForMember(d => d.Libros, o => o.MapFrom(s => s.Libros));
            CreateMap<AutorCreateDto, Autor>();
            CreateMap<AutorUpdateDto, Autor>();

            // Cliente
            CreateMap<Cliente, ClienteDto>();
            CreateMap<ClienteCreateDto, Cliente>();
            CreateMap<ClienteUpdateDto, Cliente>();

            // DetalleFactura
            CreateMap<DetalleFactura, DetalleFacturaDTO>()
                .ForMember(d => d.LibroTitulo, o => o.MapFrom(s => s.Libro.Titulo))
                .ForMember(d => d.LibroPrecio, o => o.MapFrom(s => s.Libro.Precio));
            CreateMap<DetalleFacturaCreateDTO, DetalleFactura>()
    .ForMember(dest => dest.PrecioUnitario, opt => opt.Ignore()) // <- lo calculamos en el servicio
    .ForMember(dest => dest.Subtotal, opt => opt.Ignore())       // <- idem
    .ReverseMap();


            // Factura
            CreateMap<Factura, FacturaDTO>()
                .ForMember(d => d.ClienteNombre, o => o.MapFrom(s => s.Cliente.Nombre + " " + s.Cliente.Apellido))
                .ForMember(d => d.Detalles, o => o.MapFrom(s => s.DetalleFacturas));
            CreateMap<FacturaCreateDTO, Factura>()
    .ForMember(dest => dest.Total, opt => opt.Ignore()) // <- lo calculamos en el servicio
    .ForMember(dest => dest.DetalleFacturas, opt => opt.MapFrom(src => src.Detalles))
    .ReverseMap();
            CreateMap<FacturaUpdateDTO, Factura>();
        }
    }
}
