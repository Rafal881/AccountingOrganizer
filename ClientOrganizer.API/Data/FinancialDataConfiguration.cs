using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientOrganizer.API.Data
{
    public sealed class FinancialDataConfiguration : IEntityTypeConfiguration<FinancialData>
    {
        public void Configure(EntityTypeBuilder<FinancialData> record)
        {
            record.HasKey(f => f.Id);

            record.Property(f => f.ClientId).IsRequired();
            record.Property(f => f.Month).IsRequired();
            record.Property(f => f.Year).IsRequired();

            record.Property(f => f.IncomeTax).HasPrecision(18, 2);
            record.Property(f => f.Vat).HasPrecision(18, 2);
            record.Property(f => f.InsuranceAmount).HasPrecision(18, 2);

            record.HasIndex(f => new { f.ClientId, f.Month, f.Year }).IsUnique();

            record.HasOne(f => f.Client)
                  .WithMany(c => c.FinancialRecords)
                  .HasForeignKey(f => f.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);

            record.ToTable(tbl =>
            {
                tbl.HasCheckConstraint("CK_FinancialData_Month", "[Month] BETWEEN 1 AND 12");
                tbl.HasCheckConstraint("CK_FinancialData_Year", "[Year] >= 2000");
            });
        }
    }
}
