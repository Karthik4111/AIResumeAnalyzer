using AIResumeAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class JobDescriptionConfiguration : IEntityTypeConfiguration<JobDescription>
{
    public void Configure(EntityTypeBuilder<JobDescription> builder)
    {
        builder.ToTable("JobDescriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Company)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasColumnType("nvarchar(max)")
               .IsRequired();

        builder.HasOne(x => x.User)
               .WithMany(u => u.JobDescriptions)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ATSReports)
               .WithOne(a => a.JobDescription)
               .HasForeignKey(a => a.JobDescriptionId);
    }
}