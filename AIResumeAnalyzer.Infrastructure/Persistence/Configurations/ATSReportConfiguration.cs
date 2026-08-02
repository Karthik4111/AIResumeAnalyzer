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

        // ResumeVersion -> ATSReport
        builder.HasOne(x => x.ResumeVersion)
               .WithMany(x => x.ATSReports)
               .HasForeignKey(x => x.ResumeVersionId)
               .OnDelete(DeleteBehavior.Restrict);

        // JobDescription -> ATSReport
        builder.HasOne(x => x.JobDescription)
               .WithMany(x => x.ATSReports)
               .HasForeignKey(x => x.JobDescriptionId)
               .OnDelete(DeleteBehavior.Restrict);

        // ATSReport -> Recommendation
        builder.HasMany(x => x.Recommendations)
               .WithOne(r => r.ATSReport)
               .HasForeignKey(r => r.ATSReportId)
               .OnDelete(DeleteBehavior.Cascade);

        // ATSReport -> InterviewQuestion
        builder.HasMany(x => x.InterviewQuestions)
               .WithOne(i => i.ATSReport)
               .HasForeignKey(i => i.ATSReportId)
               .OnDelete(DeleteBehavior.Cascade);

        // ATSReport -> CoverLetter
        builder.HasOne(x => x.CoverLetter)
               .WithOne(c => c.ATSReport)
               .HasForeignKey<CoverLetter>(c => c.ATSReportId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}