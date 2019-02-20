using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class RepositoryContext:DbContext
    {
        public RepositoryContext(DbContextOptions dbContextOptions)
         : base(dbContextOptions)
        {

        }

        DbSet<Owner> Owners { get; set; }
        DbSet<Account> Accounts { get; set; }
    }
}
