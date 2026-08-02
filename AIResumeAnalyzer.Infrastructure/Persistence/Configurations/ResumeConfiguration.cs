using AIResumeAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(r => r.Status)
               .IsRequired();

        builder.HasOne(r => r.User)
               .WithMany(u => u.Resumes)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Versions)
               .WithOne(v => v.Resume)
               .HasForeignKey(v => v.ResumeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}