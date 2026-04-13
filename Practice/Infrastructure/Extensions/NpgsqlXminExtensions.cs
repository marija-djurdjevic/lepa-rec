using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Npgsql.EntityFrameworkCore.PostgreSQL
{
    public static class NpgsqlModelBuilderExtensions
    {
        public static EntityTypeBuilder UseXminAsConcurrencyToken(this EntityTypeBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property<uint>("xmin")
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

            return builder;
        }
    }
}
