using System;
using System.Collections.Generic;
using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Contracts
{
    /// <summary>
    /// Handles metadata and schema CRUD, tranformation and validation
    /// </summary>
    public interface IMetadataManager
    {
        (bool, string) PostEntity(EntityMetadataDetails metadataEntity);

        IEnumerable<EntityMetadataDetails> GetEntityByTenant(int? tenantId);

        EntityMetadataDetails GetEntity(Guid Id);

        (bool, string) DeleteEntityByTenant(int? tenantId);

        (bool, string) DeleteEntity(Guid Id);

        (bool, string) PutMetadataEntity(EntityMetadataDetails metadataEntity);


        (bool, string) PostMetadataField(MetadataField metadataField);

        MetadataField GetMetadataField(Guid fieldId);

        IEnumerable<MetadataField> GetMetadataFields(IEnumerable<Guid> fieldIds);

        (bool, string) PutMetadataField(MetadataField metadataField);

        (bool, string) DeleteMetadataField(Guid fieldId);

        IEnumerable<MetadataField> GetMetadataFieldsByTenant(int? tenantId);


        (bool, string) PostMetadataSchema(MetadataSchemaDetails metaDataSchema);

        MetadataSchemaDetails GetMetadataSchema(Guid schemaId);

        (bool, string) PutMetadataSchema(MetadataSchemaDetails metaDataSchema);

        (bool, string) DeleteMetadataSchema(Guid schemaId);


        (bool, string) AddSchemaToFields(MetadataSchemaDetails schema, MetadataField field);

        (bool, string) AddSchemaToFields(MetadataSchemaDetails schema, IEnumerable<MetadataField> fields);

        IEnumerable<MetadataField> GetSchemaFields(Guid schemaId);

        (bool, string) DeleteAllSchemaFields(Guid schemaId);

        (bool, string) DeleteSchemaField(Guid schemaId, Guid fieldId);


        (bool, string) AddSchemaToEntity(Guid entityId, Guid schemaId);

        MetadataSchemaDetails GetEntitySchema(Guid entityId);

        (bool, string) DeleteEntitySchema(Guid entityId);

        IEnumerable<MetadataField> GetEntitySchemaFields(Guid entityId);


        (bool, string) AddEntityMetadataValues(Guid entityId, int tenantId, IEnumerable<PublicFieldValue> publicFieldValues);

        (bool, string, PublicFieldTransformationResult) GetEntityMetadataValues(Guid entityId);

        (bool, string) DeleteAllEntityMetadataValues(Guid entityId);

        (bool, string) DeleteEntityMetadataValue(Guid entityId, string fieldPublicName);

        (bool, string) PutEntityMetadataValues(Guid entityId, int tenantId, PublicFieldValue publicFieldValue);

        (bool, string) PutEntityMetadataValues(Guid entityId, int tenantId, IEnumerable<PublicFieldValue> publicFieldValue);
    }
}