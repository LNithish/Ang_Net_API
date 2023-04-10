using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    //adding configuration details of Order entity for Entity framework
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            //address is owned by order entity
            builder.OwnsOne(o => o.ShipToAddress
            //Specify related entity
            , a =>
            {
                a.WithOwner();
            });
            //Setting up address as required entity,Our ShipTo address is a navigation property
            builder.Navigation(a=>a.ShipToAddress).IsRequired();
            //status from enum
            builder.Property(s => s.Status)
                //Getting EnumString rather than getting number 0,1,2,.... 
                .HasConversion(
                o => o.ToString(),
                o => (OrderStatus)Enum.Parse(typeof(OrderStatus),o)
                );
            //One to MAny relationship with OrderItem
            //OnDelete is for when deleting an order it will delete a related orderItem as well
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            //Setup for decimal value
            builder.Property(i => i.SubTotal).HasColumnType("decimal(18,2)");
        }

    }
}
