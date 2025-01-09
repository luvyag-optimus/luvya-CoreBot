using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyDummyBot.Model
{
    public partial class QuestionsContext : DbContext
    {
        public QuestionsContext()
        {
        }

        public QuestionsContext(DbContextOptions<QuestionsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=CPC-rishi-465P1;Database=Questions;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Option1)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("option1");

                entity.Property(e => e.Option2)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("option2");

                entity.Property(e => e.Option3)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("option3");

                entity.Property(e => e.Option4)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("option4");

                entity.Property(e => e.Qdescription)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("qdescription");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
