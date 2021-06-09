using System;
using System.Collections.Generic;
using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Contracts
{
    public interface IMetaDataStorageService
    {
        (bool, string) AddMetadataField(MetadataField metadataField);

        IEnumerable<MetadataField> GetAllMetadataFieldsByTenant(int tenantId);

        IEnumerable<MetadataField> GetMetadataFields(IEnumerable<Guid> fieldIds);

        IEnumerable<MetadataField> GetMetadataFieldsBySchemaId(Guid schemaId);

        IEnumerable<MetadataField> GetMetadataFieldsByEntityId(Guid entityId);

        IEnumerable<MetadataField> GetMetadataFieldsByTenant(int? tenantId);

        (bool, string) PutMetadataField(MetadataField metadataField);

        (bool, string) DeleteMetadataField(Guid fieldId);


        (bool, string) AddMetadataEntity(EntityMetadataDetails metadataEntity);

        IEnumerable<EntityMetadataDetails> GetAllMetadataEntityByTenant(int? tenantId);

        EntityMetadataDetails GetMetadataEntity(Guid Id);

        (bool, string) DeleteEntityByTenant(int? tenantId);

        (bool, string) DeleteEntity(Guid Id);

        (bool, string) PutMetadataEntity(EntityMetadataDetails metadataEntity);

        EntityMetadataDetails GetMetadataEntityByStorageEntityId(Guid storageEntityId);


        (bool, string) AddMetadataSchemaDetails(MetadataSchemaDetails metaDataSchemaDetails);

        MetadataSchemaDetails GetMetaDataSchemaDetails(Guid schemaId);

        IEnumerable<MetadataSchemaDetails> GetAllMetaDataSchemaDetails(int tenantId);

        MetadataSchemaDetails GetMetaDataSchemaDetailsByEntityId(Guid entityId);

        (bool, string) PutMetadataSchema(MetadataSchemaDetails metaDataSchema);

        (bool, string) DeleteMetadataSchema(Guid schemaId);


        bool AddMetaDataSchemaFields(MetadataSchemaFields metaDataSchemaFields, bool saveChange);

        bool AddMetaDataSchemaFields(IEnumerable<MetadataSchemaFields> metaDataSchemaFields);

        IEnumerable<MetadataSchemaFields> GetMetaDataSchemaFieldsBySchemaId(Guid schemaId);

        (bool, string) DeleteAllSchemaFields(Guid schemaId);

        (bool, string) DeleteSchemaField(Guid schemaId, Guid fieldId);


        bool AddEntityMetadataSchema(EntityMetadataSchema entityMetadataSchema);

        EntityMetadataSchema GetEntityMetadataSchema(Guid entityId);

        (bool, string) DeleteEntitySchema(Guid entityId);


        bool AddEntityMetadataFieldValues(IEnumerable<EntityMetadataFieldValues> entityMetadataValues);

        bool AddEntityMetadataFieldValues(EntityMetadataFieldValues entityMetadataValues, bool saveChange);

        IEnumerable<EntityMetadataFieldValues> GetEntityMetadataFieldValues(Guid entityId);

        (bool, string) DeleteAllEntityMetadataValues(Guid entityId);

        (bool, string) DeleteEntityMetadataValue(Guid entityId, Guid fieldId);

        (bool, string) PutEntityMetadataFieldValue(EntityMetadataFieldValues entityMetadataValue);
    }
}