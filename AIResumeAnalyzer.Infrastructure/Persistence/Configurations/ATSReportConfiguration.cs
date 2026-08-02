using AIResumeAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class ATSReportConfiguration : IEntityTypeConfiguration<ATSReport>
{
    public void Configure(EntityTypeBuilder<ATSReport> builder)
    {
        builder.ToTable("ATSReports");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AtsScore)
               .HasPrecision(5, 2);

        builder.Property(x => x.Summary)
               .HasColumnType("nvarchar(max)");

        builder.HasMany(x => x.Recommendations)
               .WithOne(r => r.ATSReport)
               .HasForeignKey(r => r.ATSReportId);

        builder.HasMany(x => x.InterviewQuestions)
               .WithOne(i => i.ATSReport)
               .HasForeignKey(i => i.ATSReportId);

        builder.HasOne(x => x.CoverLetter)
               .WithOne(c => c.ATSReport)
               .HasForeignKey<CoverLetter>(c => c.ATSReportId);
    }
}