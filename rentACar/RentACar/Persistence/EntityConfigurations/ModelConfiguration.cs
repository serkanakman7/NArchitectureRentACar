using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations
{
    public class ModelConfiguration : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> builder)
        {
            builder.ToTable("Models").HasKey(m => m.Id);

            builder.Property(m => m.Id).HasColumnName("Id").IsRequired();
            builder.Property(m => m.BrandId).HasColumnName("BrandId").IsRequired();
            builder.Property(m => m.FuelId).HasColumnName("FuelId").IsRequired();
            builder.Property(m => m.TransmissId).HasColumnName("TransmissId").IsRequired();
            builder.Property(m => m.Name).HasColumnName("Name").IsRequired();
            builder.Property(m => m.DailyPrice).HasColumnName("DailyPrice").IsRequired();
            builder.Property(m => m.ImageUrl).HasColumnName("ImageUrl").IsRequired();
            builder.Property(m => m.CreatedDate).HasColumnName("CreatedDate").IsRequired();
            builder.Property(m => m.UpdatedDate).HasColumnName("UpdatedDate");
            builder.Property(m => m.DeletedDate).HasColumnName("DeletedDate");


            builder.HasIndex(m => m.Name, "UK_Models_Name").IsUnique();

            builder.HasOne(m => m.Brand);
            builder.HasOne(m => m.Fuel);
            builder.HasOne(m => m.Transmiss);
            builder.HasMany(m => m.Cars);

            builder.HasQueryFilter(m => !m.DeletedDate.HasValue);
        }
    }
}
