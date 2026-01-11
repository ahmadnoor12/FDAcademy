using Domain.Entities;
using Domain.Entities.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class CategorySeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData
             (
               new Category { Id = 1, Name = "InformationTechnology", Code = CategoryEnum.InformationTechnology },
               new Category { Id = 2, Name = "FullstackDev", Code = CategoryEnum.FullstackDev },
               new Category { Id = 3, Name = "Sales", Code = CategoryEnum.Sales },
               new Category { Id = 4, Name = "HumanResources", Code = CategoryEnum.HumanResources },
               new Category { Id = 5, Name = "Marketing", Code = CategoryEnum.Marketing }

             );
        }
    }
}
