using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Models;

namespace NegotiationsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<NegotiationsApi.Models.ProductModel> ProductModel { get; set; } = default!;

        public DbSet<NegotiationsApi.Models.NegotiationModel> NegotiationModel { get; set; } = default!;
    }
}
