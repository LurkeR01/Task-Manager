using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BoardInviteConfigurations : IEntityTypeConfiguration<BoardInvite>
{
    public void Configure(EntityTypeBuilder<BoardInvite> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Role)
            .IsRequired();

        builder.Property(i => i.Status)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();
        
        builder.HasOne(i => i.Board)
            .WithMany(b => b.BoardInvites)
            .HasForeignKey(i => i.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invited user
        builder.HasOne(i => i.InvitedUser)
            .WithMany(u => u.ReceivedInvites)
            .HasForeignKey(i => i.InvitedUserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Invited by
        builder.HasOne(i => i.InvitedByUser)
            .WithMany(u => u.SentInvites)
            .HasForeignKey(i => i.InvitedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(i => new { i.BoardId, i.InvitedUserId });

        builder.HasIndex(i => i.Status);
    }
}