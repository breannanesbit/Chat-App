using Microsoft.EntityFrameworkCore;

namespace Shared.Data;

public partial class MessageContext : DbContext
{
    public MessageContext()
    {
    }

    public MessageContext(DbContextOptions<MessageContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ContainerLocation> ContainerLocations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageContainerLocation> MessageContainerLocations { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContainerLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ContainerLocation_pkey");

            entity.ToTable("ContainerLocation");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Messages_pkey");

            entity.Property(e => e.ImagePath).HasMaxLength(512);
            entity.Property(e => e.Sender).HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<MessageContainerLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MessageContainerLocation_pkey");

            entity.ToTable("MessageContainerLocation");

            entity.HasOne(d => d.ContainerLocation).WithMany(p => p.MessageContainerLocations)
                .HasForeignKey(d => d.ContainerLocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MessageContainerLocation_ContainerLocationId_fkey");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageContainerLocations)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MessageContainerLocation_MessageId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
