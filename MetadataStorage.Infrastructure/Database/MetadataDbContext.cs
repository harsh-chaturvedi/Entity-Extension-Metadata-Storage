using MetadataStorage.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace MetadataStorage.Infrastructure.Database
{
    public class MetadataDbContext : DbContext
    {
        public MetadataDbContext(DbContextOptions<MetadataDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EntityMetadataDetails>(me =>
            {
                me.HasIndex(t => t.EntityName);
                me.HasIndex(t => t.Id);
                me.HasIndex(t => t.TenantId);
            });

            builder.Entity<MetadataField>(mf =>
            {
                mf.HasIndex(t => t.FieldName);
                mf.HasIndex(t => t.PublicName);
                mf.HasIndex(t => t.TenantId);
                mf.HasIndex(t => t.Id);
                mf.HasMany(t => t.MetadataSchemaFields).WithOne(k => k.MetadataField);
            });

            builder.Entity<MetadataSchemaDetails>(ms =>
            {
                ms.HasIndex(t => t.EntityId);
                ms.HasIndex(t => t.Id);
                ms.HasIndex(t => t.SchemaName);
                ms.HasIndex(t => t.TenantId);
            });

            builder.Entity<MetadataSchemaFields>(sf =>
            {
                sf.HasKey(t => new { t.FieldId, t.SchemaId });
                sf.HasIndex(t => t.FieldId);
                sf.HasIndex(t => t.SchemaId);
                sf.HasOne(t => t.MetadataField).WithMany(z => z.MetadataSchemaFields);
            });

            builder.Entity<EntityMetadataSchema>(eb =>
            {
                eb.HasKey(t => new { t.SubjectId, t.SchemaId });
                eb.HasIndex(t => t.SubjectId);
                eb.HasIndex(t => t.SchemaId);
            });

            builder.Entity<EntityMetadataFieldValues>(ef =>
            {
                ef.HasIndex(t => t.SubjectId);
                ef.HasIndex(t => t.EntityTypeId);
                ef.HasIndex(t => t.TenantId);
                ef.HasIndex(t => t.FieldId);
            });

            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Gets or sets the MetadataFields
        /// </summary>
        public DbSet<MetadataField> MetadataFields { get; set; }

        /// <summary>
        /// Gets or sets the MetadataEntity
        /// </summary>
        public DbSet<EntityMetadataDetails> EntityMetadataDetails { get; set; }

        /// <summary>
        /// Gets or sets the Metadata schema details
        /// </summary>
        public DbSet<MetadataSchemaDetails> MetadataSchemaDetails { get; set; }

        /// <summary>
        /// Gets or sets the Metadata schema field mapping
        /// </summary>
        public DbSet<MetadataSchemaFields> MetadataSchemaFields { get; set; }

        /// <summary>
        /// Gets or sets the Tenant Metadata schema 
        /// </summary>
        public DbSet<EntityMetadataSchema> EntityMetadataSchemas { get; set; }

        /// <summary>
        /// Gets or sets the Tenant metadata metadata values
        /// </summary>
        public DbSet<EntityMetadataFieldValues> EntityMetadataFieldValues { get; set; }

    }
}