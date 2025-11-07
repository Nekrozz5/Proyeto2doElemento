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
    .ForMember(dest => dest.AutorNombre,
        opt => opt.MapFrom(src =>
            src.Autor != null
                ? $"{src.Autor.Nombre} {src.Autor.Apellido}"
                : string.Empty))
    .ForMember(dest => dest.AnioPublicacion,
        opt => opt.MapFrom(src => src.AnioPublicacion))
    .ForSourceMember(src => src.Autor, opt => opt.DoNotValidate());

            CreateMap<LibroCreateDto, Libro>()
                .ForMember(dest => dest.AnioPublicacion,
                    opt => opt.MapFrom(src => src.AnioPublicacion));

            CreateMap<LibroUpdateDto, Libro>()
                .ForMember(dest => dest.AnioPublicacion,
                    opt => opt.MapFrom(src => src.AnioPublicacion));

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
