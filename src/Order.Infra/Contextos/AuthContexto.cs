using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Order.Infra.Contextos;

public class AuthContexto : IdentityDbContext
{
    public AuthContexto(DbContextOptions<AuthContexto> options) : base(options) { }
}

