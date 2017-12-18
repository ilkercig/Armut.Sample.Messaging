using Armut.Sample.Messaging.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Data
{
    public class MessagingContext : DbContext
    {
        public MessagingContext(DbContextOptions<MessagingContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Blocking> Blockings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Blocking>().ToTable("Blocking");

            modelBuilder.Entity<Message>()
                .Property(b => b.Read)
                .HasDefaultValue(false);

            modelBuilder.Entity<Blocking>()
                .HasKey(b => new { b.BlockerId, b.UnWantedId });

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();


            modelBuilder.Entity<Blocking>()
                .HasIndex(b => b.UnWantedId);

            modelBuilder.Entity<Blocking>()
                .HasIndex(b => b.BlockerId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedMessages)
                .WithOne(um => um.Receiver)
                .HasForeignKey(um => um.ReceiverId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
                .HasMany(u => u.SentMessages)
                .WithOne(um => um.Sender)
                .HasForeignKey(um => um.SenderId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
                .HasMany(u => u.BlockedByMe)
                .WithOne(b => b.Blocker)
                .IsRequired(true)
                .HasForeignKey(b => b.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UnWantsMe)
                .WithOne(b => b.UnWanted)
                .IsRequired(true)
                .HasForeignKey(b => b.UnWantedId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
