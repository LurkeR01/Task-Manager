using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BoardUserConfigurations : IEntityTypeConfiguration<BoardUser>
{
    public void Configure(EntityTypeBuilder<BoardUser> builder)
    {
        builder.HasIndex(bu => new { bu.BoardId, bu.UserId })
            .IsUnique();
        
        builder.Property(bu => bu.Role).HasConversion<int>();
        
        builder.HasOne(bu => bu.Board)
            .WithMany(b => b.Members)
            .HasForeignKey(bu => bu.BoardId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(bu => bu.User)
            .WithMany(u => u.BoardUsers)
            .HasForeignKey(bu => bu.UserId)
            .OnDelete(DeleteBehavior.Restrict);;
    }
}