using AIResumeAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class CoverLetterConfiguration : IEntityTypeConfiguration<CoverLetter>
{
    public void Configure(EntityTypeBuilder<CoverLetter> builder)
    {
        builder.ToTable("CoverLetters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
               .HasColumnType("nvarchar(max)")
               .IsRequired();
    }
}