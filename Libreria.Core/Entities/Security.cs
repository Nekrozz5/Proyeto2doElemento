using Libreria.Core.Enums;

namespace Libreria.Core.Entities
{
    public class Security : BaseEntity
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        // Cambiar string → RoleType enum
        public RoleType Role { get; set; }
    }
}
