using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientOrganizer.API.Data
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(message => message.Id);

            builder.Property(message => message.EventType)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(message => message.Payload)
                .IsRequired();

            builder.Property(message => message.OccurredOnUtc)
                .IsRequired();

            builder.Property(message => message.RetryCount)
                .IsRequired();

            builder.Property(message => message.Error)
                .HasMaxLength(2000);
        }
    }
}
