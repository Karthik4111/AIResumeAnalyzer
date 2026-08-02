using AIResumeAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class ResumeVersionConfiguration : IEntityTypeConfiguration<ResumeVersion>
{
    public void Configure(EntityTypeBuilder<ResumeVersion> builder)
    {
        builder.ToTable("ResumeVersions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.VersionNumber)
               .IsRequired();

        builder.Property(r => r.FileName)
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(r => r.FilePath)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(r => r.ExtractedText)
               .HasColumnType("nvarchar(max)");

        builder.HasMany(r => r.ATSReports)
               .WithOne(a => a.ResumeVersion)
               .HasForeignKey(a => a.ResumeVersionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}