using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Quizzz.Models;

public partial class QuizzContext : DbContext
{
    public QuizzContext() { }

    public QuizzContext(DbContextOptions<QuizzContext> options)
        : base(options) { }

   // public virtual DbSet<Candidat> Candidats { get; set; }
    public virtual DbSet<Personne> Personnes { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Reponse> Reponses { get; set; }
    public virtual DbSet<Section> Sections { get; set; }
    public virtual DbSet<Test> Tests { get; set; }
    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<QuestionTest> QuestionTest { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code.
        optionsBuilder.UseSqlServer("Data Source=BEST-TECHNOLOGY\\SQLEXPRESS01;Initial Catalog=Quizz;Integrated Security=True;Encrypt=False;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QuestionTest>()
                .HasKey(qt => new { qt.QuestionID, qt.TestID });

        // Configurer les relations si besoin :
        modelBuilder.Entity<QuestionTest>()
            .HasOne(qt => qt.Question)
            .WithMany(q => q.QuestionTests)
            .HasForeignKey(qt => qt.QuestionID);

        modelBuilder.Entity<QuestionTest>()
            .HasOne(qt => qt.Test)
            .WithMany(t => t.QuestionTests)
            .HasForeignKey(qt => qt.TestID);

        modelBuilder.Entity<QuestionTest>()
         .HasOne(qt => qt.ReponseChoisie)
         .WithMany()
         .HasForeignKey(qt => qt.ReponseChoisieId)
         .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Question>()
            .HasOne(q => q.Section)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.Section_ID);

        modelBuilder.Entity<Question>()
            .Property(q => q.Section_ID)
            .HasColumnName("Section_ID");
     /*   modelBuilder.Entity<Candidat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC27785E1EE2");
            entity.ToTable("Candidat");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.UtilisateurId).HasColumnName("utilisateur_ID");
        });
        */
        modelBuilder.Entity<Personne>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personne__3214EC07E8F78695");
            entity.ToTable("Personne");
            entity.Property(e => e.Nom).HasMaxLength(100);
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Test__3214EC27");
            entity.ToTable("Test");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Date_Passage).HasColumnName("Date_Passage");
            entity.Property(e => e.NoteObtenu).HasColumnName("NoteObtenu");
            entity.Property(e => e.Est_reussi).HasColumnName("Est_reussi");
            entity.Property(e => e.Candidat_ID).HasColumnName("Candidat_ID");
            entity.Property(e => e.SectionID).HasColumnName("SectionID");

          /*  entity.HasOne(e => e.Candidat)
                .WithMany(c => c.Tests)
                .HasForeignKey(e => e.Candidat_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Test_Candidat");
          */
            entity.HasOne(e => e.Section)
                .WithMany(s => s.Tests)
                .HasForeignKey(e => e.SectionID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Test_Section");
        });

        modelBuilder.Entity<Reponse>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Reponse__3214EC27A49FC804");
            entity.ToTable("Reponse");
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Est_correcte).HasColumnName("Est_correcte");
            entity.Property(e => e.Question_ID).HasColumnName("Question_ID");
            entity.Property(e => e.Test_reponse).HasColumnName("Test_reponse");
            entity.HasOne(d => d.Question)
                .WithMany(p => p.Reponses)
                .HasForeignKey(d => d.Question_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reponse_Question");
           // entity.HasOne(d => d.Test)
             //   .WithMany(p => p.Reponses)
               // .OnDelete(DeleteBehavior.ClientSetNull)
               // .HasConstraintName("FK_Reponse_Test");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Section__3214EC27A4AF2C58");
            entity.ToTable("Section");

            entity.Property(e => e.Id).HasColumnName("ID");
           
            entity.Property(e => e.Nom).HasMaxLength(100);
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Utilisat__3214EC27C79DFFC5");
            entity.ToTable("Utilisateur");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MotPasseHache)
                .HasMaxLength(255)
                .HasColumnName("mot_passe_hache");
            entity.Property(e => e.NomUtilisateur)
                .HasMaxLength(100)
                .HasColumnName("nom_utilisateur");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Enonce).HasColumnName("Enonce");
            entity.Property(e => e.Section_ID).HasColumnName("Section_ID");
           
            //entity.HasOne(d => d.Section)
            //    .WithMany(p => p.Questions)
            //    .HasForeignKey(d => d.Section_ID)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Question_Section");

           
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
