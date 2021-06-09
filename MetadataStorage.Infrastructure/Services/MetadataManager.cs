using System;
using System.Collections.Generic;
using System.Linq;
using MetadataStorage.Infrastructure.Contracts;
using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Services
{
    /// <summary>
    /// Handles metadata and schema CRUD and tranformation
    /// </summary>
    public class MetadataManager : IMetadataManager
    {
        private readonly IMetaDataStorageService _metaDataStorageService;
        private readonly IFieldTransformer _fieldTransformer;
        private readonly IMetadataValidator _metadataValidator;

        public MetadataManager(IMetaDataStorageService metaDataService, IFieldTransformer fieldTransformer, IMetadataValidator metadataValidator)
        {
            _metaDataStorageService = metaDataService;
            _fieldTransformer = fieldTransformer;
            _metadataValidator = metadataValidator;
        }

        public (bool, string) AddEntityMetadataValues(Guid entityId, int tenantId, IEnumerable<PublicFieldValue> publicFieldValues)
        {
            var transformationResult = _fieldTransformer.TransformPublicFieldToMetadataField(entityId, tenantId, publicFieldValues);

            var (isSuccess, errorMessage) = _metadataValidator.ValidateMetadataFieldValuesForSchema(transformationResult);

            if (!isSuccess)
                return (false, errorMessage);

            (isSuccess, errorMessage) = _metadataValidator.ValidatePublicFieldValuesForType(transformationResult);

            if (!isSuccess)
                return (false, errorMessage);

            var result = _metaDataStorageService.AddEntityMetadataFieldValues(transformationResult.MetadataFieldValues);

            return result ? (true, Constants.ErrorMessage.NewRecordCreated)
                          : (false, Constants.ErrorMessage.InvalidParameter);
        }

        public (bool, string) AddSchemaToFields(MetadataSchemaDetails schema, MetadataField field)
        {
            if (schema.TenantId != field.TenantId)
            {
                return (false, Constants.ErrorMessage.SchemaFieldTenantError);
            }

            var result = _metaDataStorageService.AddMetaDataSchemaFields(new MetadataSchemaFields { FieldId = field.Id, SchemaId = schema.Id }, true);
            return result ? (true, Constants.ErrorMessage.NewRecordCreated)
                          : (false, Constants.ErrorMessage.InvalidParameter);
        }

        public (bool, string) AddSchemaToFields(MetadataSchemaDetails schema, IEnumerable<MetadataField> fields)
        {
            if (fields.Any(x => x.TenantId != schema.TenantId))
            {
                return (false, Constants.ErrorMessage.SchemaFieldTenantError);
            }

            var result = _metaDataStorageService.AddMetaDataSchemaFields(fields.Select(x => new MetadataSchemaFields { SchemaId = schema.Id, FieldId = x.Id }));

            return result ? (true, Constants.ErrorMessage.NewRecordCreated)
                          : (false, Constants.ErrorMessage.InvalidParameter);
        }

        public (bool, string) AddSchemaToEntity(Guid entityId, Guid schemaId)
        {
            var result = _metaDataStorageService.AddEntityMetadataSchema(new EntityMetadataSchema { SubjectId = entityId, SchemaId = schemaId });

            return result ? (true, Constants.ErrorMessage.NewRecordCreated)
                          : (false, Constants.ErrorMessage.InvalidParameter);
        }

        public (bool, string) DeleteAllEntityMetadataValues(Guid entityId)
        {
            return _metaDataStorageService.DeleteAllEntityMetadataValues(entityId);
        }

        public (bool, string) DeleteAllSchemaFields(Guid schemaId)
        {
            return _metaDataStorageService.DeleteAllSchemaFields(schemaId);
        }

        public (bool, string) DeleteEntity(Guid Id)
        {
            return _metaDataStorageService.DeleteEntity(Id);
        }

        public (bool, string) DeleteEntityByTenant(int? tenantId)
        {
            return _metaDataStorageService.DeleteEntityByTenant(tenantId);
        }

        public (bool, string) DeleteMetadataField(Guid fieldId)
        {
            return _metaDataStorageService.DeleteMetadataField(fieldId);
        }

        public (bool, string) DeleteMetadataSchema(Guid schemaId)
        {
            return _metaDataStorageService.DeleteMetadataSchema(schemaId);
        }

        public (bool, string) DeleteEntitySchema(Guid entityId)
        {
            return _metaDataStorageService.DeleteEntitySchema(entityId);
        }

        public (bool, string) DeleteSchemaField(Guid schemaId, Guid fieldId)
        {
            return _metaDataStorageService.DeleteSchemaField(schemaId, fieldId);
        }

        public EntityMetadataDetails GetEntity(Guid Id)
        {
            return _metaDataStorageService.GetMetadataEntity(Id);
        }

        public IEnumerable<EntityMetadataDetails> GetEntityByTenant(int? tenantId)
        {
            return _metaDataStorageService.GetAllMetadataEntityByTenant(tenantId);
        }

        public MetadataField GetMetadataField(Guid fieldId)
        {
            return _metaDataStorageService.GetMetadataFields(new List<Guid> { fieldId }).FirstOrDefault();
        }

        public IEnumerable<MetadataField> GetMetadataFields(IEnumerable<Guid> fieldIds)
        {
            return _metaDataStorageService.GetMetadataFields(fieldIds);
        }

        public IEnumerable<MetadataField> GetMetadataFieldsByTenant(int? tenantId)
        {
            return _metaDataStorageService.GetMetadataFieldsByTenant(tenantId);
        }

        public MetadataSchemaDetails GetMetadataSchema(Guid schemaId)
        {
            return _metaDataStorageService.GetMetaDataSchemaDetails(schemaId);
        }

        public (bool, string, PublicFieldTransformationResult) GetEntityMetadataValues(Guid entityId)
        {
            var metadataValues = _metaDataStorageService.GetEntityMetadataFieldValues(entityId).ToList();

            PublicFieldTransformationResult transformationResult = _fieldTransformer.TransfromMetadataFieldToPublicField(entityId, metadataValues);

            var (isSuccess, errorMessage) = _metadataValidator.ValidateDatabaseFieldValuesForSchema(transformationResult);

            return (isSuccess, errorMessage, transformationResult);
        }

        public MetadataSchemaDetails GetEntitySchema(Guid entityId)
        {
            return _metaDataStorageService.GetMetaDataSchemaDetailsByEntityId(entityId);
        }

        public IEnumerable<MetadataField> GetEntitySchemaFields(Guid entityId)
        {
            return _metaDataStorageService.GetMetadataFieldsByEntityId(entityId);
        }

        public IEnumerable<MetadataField> GetSchemaFields(Guid schemaId)
        {
            return _metaDataStorageService.GetMetadataFieldsBySchemaId(schemaId);
        }

        public (bool, string) PostEntity(EntityMetadataDetails metadataEntity)
        {
            return _metaDataStorageService.AddMetadataEntity(metadataEntity);
        }

        public (bool, string) PostMetadataField(MetadataField metadataField)
        {
            return _metaDataStorageService.AddMetadataField(metadataField);
        }

        public (bool, string) PostMetadataSchema(MetadataSchemaDetails metaDataSchema)
        {
            return _metaDataStorageService.AddMetadataSchemaDetails(metaDataSchema);
        }

        public (bool, string) PutMetadataEntity(EntityMetadataDetails metadataEntity)
        {
            return _metaDataStorageService.PutMetadataEntity(metadataEntity);
        }

        public (bool, string) PutMetadataField(MetadataField metadataField)
        {
            return _metaDataStorageService.PutMetadataField(metadataField);
        }

        public (bool, string) PutMetadataSchema(MetadataSchemaDetails metaDataSchema)
        {
            return _metaDataStorageService.PutMetadataSchema(metaDataSchema);
        }

        public (bool, string) DeleteEntityMetadataValue(Guid entityId, string fieldPublicName)
        {
            var entityMetadataFields = _metaDataStorageService.GetMetadataFieldsByEntityId(entityId);
            var fieldDetails = entityMetadataFields.FirstOrDefault(x => x.PublicName.Equals(fieldPublicName, StringComparison.OrdinalIgnoreCase));

            if (fieldDetails == null)
            {
                return (false, Constants.ErrorMessage.MetadataFieldNotAvailable);
            }
            else if (fieldDetails.IsMandatory)
            {
                return (false, Constants.ErrorMessage.MetadataFieldMandatory);
            }

            return _metaDataStorageService.DeleteEntityMetadataValue(entityId, fieldDetails.Id);
        }

        public (bool, string) PutEntityMetadataValues(Guid entityId, int tenantId, PublicFieldValue publicFieldValue)
        {
            var transformationResult = _fieldTransformer.TransformPublicFieldToMetadataField(entityId, tenantId, publicFieldValue);

            var (isSuccess, errorMessage) = _metadataValidator.ValidateMetadataFieldValuesForSchema(transformationResult);

            if (!isSuccess)
                return (false, errorMessage);

            (isSuccess, errorMessage) = _metadataValidator.ValidatePublicFieldValuesForType(transformationResult);

            if (!isSuccess)
                return (false, errorMessage);

            return _metaDataStorageService.PutEntityMetadataFieldValue(transformationResult.MetadataFieldValues.FirstOrDefault());
        }

        public (bool, string) PutEntityMetadataValues(Guid entityId, int tenantId, IEnumerable<PublicFieldValue> publicFieldValues)
        {
            foreach (var item in publicFieldValues)
            {
                var (isSuccess, errorMessage) = PutEntityMetadataValues(entityId, tenantId, item);
                if (!isSuccess)
                    return (isSuccess, errorMessage);
            }

            return (true, Constants.ErrorMessage.RecordUpdated);
        }
    }
}