using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaCep.Models
{
    public class LocatorContext: DbContext
    {
        public DbSet<Locator> Locator { get; set; }

        public LocatorContext(DbContextOptions<LocatorContext> options) :
            base(options)
        {
        }
    }
}
