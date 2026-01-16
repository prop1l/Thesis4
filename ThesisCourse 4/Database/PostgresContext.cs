using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ThesisCourse_4.Database.Models;

namespace ThesisCourse_4.Database;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Avatar> Avatars { get; set; }

    public virtual DbSet<Edge> Edges { get; set; }

    public virtual DbSet<Graph> Graphs { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("avatars_pkey");

            entity.ToTable("avatars", "thesis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageData).HasColumnName("image_data");
            entity.Property(e => e.MimeType)
                .HasMaxLength(50)
                .HasColumnName("mime_type");
        });

        modelBuilder.Entity<Edge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("edges_pkey");

            entity.ToTable("edges", "thesis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FromNodeId).HasColumnName("from_node_id");
            entity.Property(e => e.GraphId).HasColumnName("graph_id");
            entity.Property(e => e.ToNodeId).HasColumnName("to_node_id");

            entity.HasOne(d => d.FromNode).WithMany(p => p.EdgeFromNodes)
                .HasForeignKey(d => d.FromNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("edges_from_node_id_fkey");

            entity.HasOne(d => d.Graph).WithMany(p => p.Edges)
                .HasForeignKey(d => d.GraphId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("edges_graph_id_fkey");

            entity.HasOne(d => d.ToNode).WithMany(p => p.EdgeToNodes)
                .HasForeignKey(d => d.ToNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("edges_to_node_id_fkey");
        });

        modelBuilder.Entity<Graph>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("graphs_pkey");

            entity.ToTable("graphs", "thesis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Graphs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("graphs_user_id_fkey");
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("nodes_pkey");

            entity.ToTable("nodes", "thesis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GraphId).HasColumnName("graph_id");
            entity.Property(e => e.Label)
                .HasMaxLength(255)
                .HasColumnName("label");
            entity.Property(e => e.XPosition).HasColumnName("x_position");
            entity.Property(e => e.YPosition).HasColumnName("y_position");

            entity.HasOne(d => d.Graph).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.GraphId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nodes_graph_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "thesis");

            entity.HasIndex(e => e.UserName, "users_user_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarId).HasColumnName("avatar_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UserName)
                .HasMaxLength(63)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Avatar).WithMany(p => p.Users)
                .HasForeignKey(d => d.AvatarId)
                .HasConstraintName("users_avatar_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
