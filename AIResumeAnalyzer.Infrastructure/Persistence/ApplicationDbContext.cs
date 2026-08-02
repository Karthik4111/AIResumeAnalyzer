using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIResumeAnalyzer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzer.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Resume> Resumes => Set<Resume>();

    public DbSet<ResumeVersion> ResumeVersions => Set<ResumeVersion>();

    public DbSet<JobDescription> JobDescriptions => Set<JobDescription>();

    public DbSet<ATSReport> ATSReports => Set<ATSReport>();

    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();

    public DbSet<CoverLetter> CoverLetters => Set<CoverLetter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}