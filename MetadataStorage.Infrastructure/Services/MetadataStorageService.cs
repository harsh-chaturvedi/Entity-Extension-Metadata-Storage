using System;
using System.Collections.Generic;
using System.Linq;
using MetadataStorage.Infrastructure.Contracts;
using MetadataStorage.Infrastructure.Database;
using MetadataStorage.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace MetadataStorage.Infrastructure.Services
{
    public class MetadataStorageService : IMetaDataStorageService
    {
        private readonly MetadataDbContext _dbContext;

        public MetadataStorageService(MetadataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (bool, string) AddMetadataField(MetadataField metadataField)
        {
            if (metadataField == null || string.IsNullOrEmpty(metadataField.FieldName) || string.IsNullOrEmpty(metadataField.PublicName))
            {
                return (false, Constants.ErrorMessage.InvalidParameter);
            }

            var isDuplicate = _dbContext.MetadataFields.Any(t => (t.PublicName.Equals(metadataField.PublicName, StringComparison.OrdinalIgnoreCase)
                                                              || t.FieldName.Equals(metadataField.FieldName, StringComparison.OrdinalIgnoreCase))
                                                              && t.TenantId.Equals(metadataField.TenantId));

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateMetadataFieldTenant);
            }

            _dbContext.MetadataFields.Add(metadataField);
            _dbContext.SaveChanges();

            return (true, metadataField.Id.ToString());
        }

        public IEnumerable<MetadataField> GetAllMetadataFieldsByTenant(int tenantId)
        {
            var result = _dbContext.MetadataFields.Where(x => x.TenantId == tenantId);
            return result;
        }

        public (bool, string) PutMetadataField(MetadataField metadataField)
        {
            var isDuplicate = _dbContext.MetadataFields.Any(t => (t.PublicName.Equals(metadataField.PublicName, StringComparison.OrdinalIgnoreCase)
                                                              || t.FieldName.Equals(metadataField.FieldName, StringComparison.OrdinalIgnoreCase))
                                                              && t.TenantId.Equals(metadataField.TenantId)
                                                              && t.Id != metadataField.Id);

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateMetadataFieldTenant);
            }
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordUpdated);
        }

        public (bool, string) DeleteMetadataField(Guid fieldId)
        {
            var deleteMetadataField = _dbContext.MetadataFields.FirstOrDefault(x => x.Id == fieldId);

            if (deleteMetadataField == null)
            {
                return (false, Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            _dbContext.MetadataFields.Remove(deleteMetadataField);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public IEnumerable<MetadataField> GetMetadataFields(IEnumerable<Guid> fieldIds)
        {
            var result = _dbContext.MetadataFields.Where(x => fieldIds.Contains(x.Id));
            return result;
        }

        public IEnumerable<MetadataField> GetMetadataFieldsBySchemaId(Guid schemaId)
        {
            var result = _dbContext.MetadataFields.Include(x => x.MetadataSchemaFields).Where(x => x.MetadataSchemaFields.Any(t => t.SchemaId == schemaId));
            return result;
        }

        public IEnumerable<MetadataField> GetMetadataFieldsByTenant(int? tenantId)
        {
            var result = tenantId.HasValue ? _dbContext.MetadataFields.Where(x => x.TenantId == tenantId.Value)
                                           : _dbContext.MetadataFields;

            return result;
        }

        public IEnumerable<MetadataField> GetMetadataFieldsByEntityId(Guid entityId)
        {
            var query = _dbContext.EntityMetadataSchemas.Include(x => x.MetadataSchemaDetails).
                        ThenInclude(p => p.MetadataSchemaFields).ThenInclude(t => t.MetadataField).
                        Where(x => x.SubjectId == entityId).
                        SelectMany(x => x.MetadataSchemaDetails.MetadataSchemaFields.Select(z => z.MetadataField));

            return query;
        }


        public (bool, string) AddMetadataSchemaDetails(MetadataSchemaDetails metadataSchemaDetails)
        {
            if (metadataSchemaDetails == null || string.IsNullOrEmpty(metadataSchemaDetails.SchemaName) || string.IsNullOrEmpty(metadataSchemaDetails.Description))
            {
                return (false, Constants.ErrorMessage.InvalidParameter);
            }

            var isDuplicate = _dbContext.MetadataSchemaDetails.Any(t => t.SchemaName.Equals(metadataSchemaDetails.SchemaName, StringComparison.OrdinalIgnoreCase)
                                                                     && t.TenantId.Equals(metadataSchemaDetails.TenantId));

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateMetadataSchemaTenant);
            }

            _dbContext.MetadataSchemaDetails.Add(metadataSchemaDetails);
            _dbContext.SaveChanges();

            return (true, metadataSchemaDetails.Id.ToString());
        }

        public MetadataSchemaDetails GetMetaDataSchemaDetails(Guid schemaId)
        {
            var result = _dbContext.MetadataSchemaDetails.FirstOrDefault(x => x.Id == schemaId);
            return result;
        }

        public IEnumerable<MetadataSchemaDetails> GetAllMetaDataSchemaDetails(int tenantId)
        {
            var result = _dbContext.MetadataSchemaDetails.Where(x => x.TenantId == tenantId);
            return result;
        }

        public MetadataSchemaDetails GetMetaDataSchemaDetailsByEntityId(Guid entityId)
        {
            var query = _dbContext.EntityMetadataSchemas.Include(x => x.MetadataSchemaDetails).
                        Where(x => x.SubjectId == entityId).Select(x => x.MetadataSchemaDetails).FirstOrDefault();

            return query;
        }

        public (bool, string) PutMetadataSchema(MetadataSchemaDetails metaDataSchema)
        {
            var isDuplicate = _dbContext.MetadataSchemaDetails.Any(t => t.SchemaName.Equals(metaDataSchema.SchemaName, StringComparison.OrdinalIgnoreCase)
                                                                     && t.TenantId.Equals(metaDataSchema.TenantId)
                                                                     && t.Id != metaDataSchema.Id);

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateMetadataSchemaTenant);
            }

            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordUpdated);
        }

        public (bool, string) DeleteMetadataSchema(Guid schemaId)
        {
            var deleteSchema = _dbContext.MetadataSchemaDetails.FirstOrDefault(x => x.Id == schemaId);

            if (deleteSchema == null)
            {
                return (false, Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            _dbContext.MetadataSchemaDetails.Remove(deleteSchema);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }


        public bool AddMetaDataSchemaFields(MetadataSchemaFields metaDataSchemaFields, bool saveChange)
        {
            var isDuplicate = _dbContext.MetadataSchemaFields.Any(x => x.FieldId == metaDataSchemaFields.FieldId && x.SchemaId == metaDataSchemaFields.SchemaId);

            if (isDuplicate)
                return false;

            _dbContext.MetadataSchemaFields.Add(metaDataSchemaFields);

            if (saveChange)
                _dbContext.SaveChanges();

            return true;
        }

        public bool AddMetaDataSchemaFields(IEnumerable<MetadataSchemaFields> metaDataSchemaFields)
        {
            foreach (var item in metaDataSchemaFields)
            {
                if (AddMetaDataSchemaFields(item, false))
                    continue;
                else
                    return false;
            }

            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<MetadataSchemaFields> GetMetaDataSchemaFieldsBySchemaId(Guid schemaId)
        {
            var result = _dbContext.MetadataSchemaFields.Where(x => x.SchemaId == schemaId);
            return result;
        }

        public (bool, string) DeleteAllSchemaFields(Guid schemaId)
        {
            var deleteEntity = _dbContext.MetadataSchemaFields.Where(x => x.SchemaId == schemaId);

            if (deleteEntity == null || !deleteEntity.Any())
            {
                return (false, Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            _dbContext.MetadataSchemaFields.RemoveRange(deleteEntity);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public (bool, string) DeleteSchemaField(Guid schemaId, Guid fieldId)
        {
            var deleteEntity = _dbContext.MetadataSchemaFields.FirstOrDefault(x => x.SchemaId == schemaId && x.FieldId == fieldId);

            if (deleteEntity == null)
            {
                return (false, Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            _dbContext.MetadataSchemaFields.Remove(deleteEntity);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }


        public bool AddEntityMetadataSchema(EntityMetadataSchema entityMetadataSchema)
        {
            var isDuplicate = _dbContext.EntityMetadataSchemas.Any(x => x.SubjectId == entityMetadataSchema.SubjectId && x.SchemaId == entityMetadataSchema.SchemaId);

            if (isDuplicate)
                return false;

            _dbContext.EntityMetadataSchemas.Add(entityMetadataSchema);
            _dbContext.SaveChanges();

            return true;
        }

        public EntityMetadataSchema GetEntityMetadataSchema(Guid entityId)
        {
            var result = _dbContext.EntityMetadataSchemas.FirstOrDefault(x => x.SubjectId == entityId);
            return result;
        }

        public (bool, string) DeleteEntitySchema(Guid entityId)
        {
            var entityMetadatSchema = _dbContext.EntityMetadataSchemas.FirstOrDefault(x => x.SubjectId == entityId);

            if (entityMetadatSchema == null)
            {
                return (false, Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            _dbContext.EntityMetadataSchemas.Remove(entityMetadatSchema);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }


        public bool AddEntityMetadataFieldValues(IEnumerable<EntityMetadataFieldValues> entityMetadataValues)
        {
            foreach (var item in entityMetadataValues)
            {
                if (AddEntityMetadataFieldValues(item, false))
                    continue;
                else
                    return false;
            }

            _dbContext.SaveChanges();
            return true;
        }

        public bool AddEntityMetadataFieldValues(EntityMetadataFieldValues entityMetadataValues, bool saveChange)
        {
            var isDuplicate = _dbContext.EntityMetadataFieldValues.
                                Any(x => x.SubjectId == entityMetadataValues.SubjectId
                                        && x.FieldId == entityMetadataValues.FieldId);

            if (isDuplicate)
                return false;

            _dbContext.EntityMetadataFieldValues.Add(entityMetadataValues);

            if (saveChange)
                _dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<EntityMetadataFieldValues> GetEntityMetadataFieldValues(Guid entityId)
        {
            var result = _dbContext.EntityMetadataFieldValues.Where(x => x.SubjectId == entityId);
            return result;
        }

        public (bool, string) DeleteAllEntityMetadataValues(Guid entityId)
        {
            var entityMetadatValues = _dbContext.EntityMetadataFieldValues.Where(x => x.SubjectId == entityId);

            if (entityMetadatValues == null || !entityMetadatValues.Any())
            {
                return (false, Constants.ErrorMessage.MetadataFieldValueNotAvailable);
            }

            _dbContext.EntityMetadataFieldValues.RemoveRange(entityMetadatValues);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public (bool, string) DeleteEntityMetadataValue(Guid entityId, Guid fieldId)
        {
            var entityMetadatValues = _dbContext.EntityMetadataFieldValues.FirstOrDefault(x => x.SubjectId == entityId && x.FieldId == fieldId);

            if (entityMetadatValues == null)
            {
                return (false, Constants.ErrorMessage.MetadataFieldValueNotAvailable);
            }

            _dbContext.EntityMetadataFieldValues.Remove(entityMetadatValues);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public (bool, string) PutEntityMetadataFieldValue(EntityMetadataFieldValues entityMetadataValue)
        {
            var dbEntity = _dbContext.EntityMetadataFieldValues.FirstOrDefault(x => x.SubjectId == entityMetadataValue.SubjectId && x.FieldId == entityMetadataValue.FieldId);

            if (dbEntity == null)
            {
                var result = AddEntityMetadataFieldValues(entityMetadataValue, false);
                if (!result)
                    return (false, Constants.ErrorMessage.DuplicateMetadataFieldValue);
            }
            else
            {
                dbEntity.FieldValue = entityMetadataValue.FieldValue;
            }

            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordUpdated);
        }


        public (bool, string) AddMetadataEntity(EntityMetadataDetails metadataEntity)
        {
            if (metadataEntity == null || string.IsNullOrEmpty(metadataEntity.EntityName) || metadataEntity.TenantId < 0)
            {
                return (false, Constants.ErrorMessage.InvalidParameter);
            }

            var isDuplicate = _dbContext.EntityMetadataDetails.Any(t => t.EntityName.Equals(metadataEntity.EntityName, StringComparison.OrdinalIgnoreCase)
                                                              && t.TenantId.Equals(metadataEntity.TenantId));

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateMetadataEntityTenant);
            }

            _dbContext.EntityMetadataDetails.Add(metadataEntity);
            _dbContext.SaveChanges();

            return (true, metadataEntity.Id.ToString());
        }

        public IEnumerable<EntityMetadataDetails> GetAllMetadataEntityByTenant(int? tenantId)
        {
            var result = !tenantId.HasValue ?
                        _dbContext.EntityMetadataDetails
                        : _dbContext.EntityMetadataDetails.Where(x => x.TenantId == tenantId.Value);

            return result;
        }

        public EntityMetadataDetails GetMetadataEntity(Guid entityId)
        {
            var result = _dbContext.EntityMetadataDetails.FirstOrDefault(x => x.Id == entityId);
            return result;
        }

        public EntityMetadataDetails GetMetadataEntityByStorageEntityId(Guid storageEntityId)
        {
            var query = _dbContext.EntityMetadataSchemas.Include(x => x.MetadataSchemaDetails).ThenInclude(t => t.MetadataEntity).Where(x => x.SubjectId == storageEntityId).Select(z => z.MetadataSchemaDetails.MetadataEntity).FirstOrDefault();
            return query;
        }

        public (bool, string) DeleteEntityByTenant(int? tenantId)
        {
            var deleteEnities = tenantId.HasValue ?
                                _dbContext.EntityMetadataDetails.Where(x => x.TenantId == tenantId.Value)
                                : _dbContext.EntityMetadataDetails;

            if (deleteEnities == null || !deleteEnities.Any())
            {
                return (false, Constants.ErrorMessage.MetadataEntityNotFound);
            }

            _dbContext.EntityMetadataDetails.RemoveRange(deleteEnities);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public (bool, string) DeleteEntity(Guid Id)
        {
            var deleteEntity = _dbContext.EntityMetadataDetails.FirstOrDefault(x => x.Id == Id);

            if (deleteEntity == null)
            {
                return (false, Constants.ErrorMessage.MetadataEntityNotFound);
            }

            _dbContext.EntityMetadataDetails.Remove(deleteEntity);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public (bool, string) PutMetadataEntity(EntityMetadataDetails metadataEntity)
        {
            var isDuplicate = _dbContext.EntityMetadataDetails.Any(t => t.EntityName.Equals(metadataEntity.EntityName, StringComparison.OrdinalIgnoreCase)
                                                              && t.TenantId.Equals(metadataEntity.TenantId)
                                                              && t.Id != metadataEntity.Id);

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateMetadataEntityTenant);
            }
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordUpdated);
        }
    }
}