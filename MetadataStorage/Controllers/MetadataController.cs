using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MetadataStorage.Ancilliary.Contracts;
using MetadataStorage.Extensions;
using MetadataStorage.Infrastructure.Contracts;
using MetadataStorage.Infrastructure.Extensions;
using MetadataStorage.Infrastructure.Model;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace MetadataStorage.Controllers
{
    public class MetadataController : ApiController
    {
        private readonly IMetadataManager _metadataManager;
        public MetadataController(ITenantOrganizationService organizationService, IMetadataManager metadataManager) : base(organizationService)
        {
            _metadataManager = metadataManager;
        }

        #region Metadata Entity

        /// <summary>
        /// Adds a metadata entity type for the tenant
        /// </summary>
        /// <param name="metadataEntity">Entity and tenant information for storage</param>
        /// <returns></returns>
        [Route("Entity")]
        [HttpPost]
        public IActionResult PostEntity([FromBody] EntityMetadataDetails metadataEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = metadataEntity.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(metadataEntity.TenantId))
            {
                return BadRequest(Constants.ErrorMessage.MetadataEntityNotFound);
            }

            var (isInserted, message) = _metadataManager.PostEntity(metadataEntity);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets all the meta data entity type for the current tenant
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Entity")]
        public IActionResult GetEntityByTenant()
        {
            if (User.IsSuperAdmin())
            {
                return Ok(_metadataManager.GetEntityByTenant(null));
            }
            if (IsUserExternalApplicationTenantAdmin)
            {
                return Ok(_metadataManager.GetEntityByTenant(LoggedInTenantId));
            }
            return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
        }

        /// <summary>
        /// Gets the metadata entity
        /// </summary>
        /// <param name="Id">Entity identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Entity/{Id}")]
        public IActionResult GetEntity([FromRoute] Guid Id)
        {
            var entity = _metadataManager.GetEntity(Id);

            if (entity == null || !CheckEntityAccessToLoggedInTenant(entity.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataEntityNotFound);
            }

            return Ok(entity);
        }

        /// <summary>
        /// Updates the metadata entity for the identifier
        /// </summary>
        /// <param name="Id">Entity identifier</param>
        /// <param name="metadataEntity">Metadata entity data</param>
        /// <returns></returns>
        [Route("Entity/{Id}")]
        [HttpPut]
        public IActionResult PutMetadataEntity([FromRoute] Guid Id, [FromBody] EntityMetadataDetails metadataEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = metadataEntity.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(metadataEntity.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            Delta<EntityMetadataDetails> delta = new Delta<EntityMetadataDetails>(typeof(EntityMetadataDetails));
            Type type = metadataEntity.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                delta.TrySetPropertyValue(prop.Name, prop.GetValue(metadataEntity));
            }

            var dbMetadataEntity = _metadataManager.GetEntity(Id);
            if (dbMetadataEntity != null)
            {
                delta.Patch(dbMetadataEntity);

                var (isUpdated, message) = _metadataManager.PutMetadataEntity(dbMetadataEntity);
                if (isUpdated)
                {
                    return Ok(message);
                }
                return BadRequest(message);
            }
            else
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataEntityNotFound);
            }
        }

        /// <summary>
        /// Deletes all the metadata entity for the logged in tenant
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("Entity")]
        public IActionResult DeleteEntityByTenant()
        {
            (bool, string) result = (false, "");
            if (User.IsSuperAdmin())
            {
                result = _metadataManager.DeleteEntityByTenant(null);
            }
            else if (IsUserExternalApplicationTenantAdmin)
            {
                result = _metadataManager.DeleteEntityByTenant(LoggedInTenantId);
            }
            else
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            if (result.Item1)
                return Ok(result.Item2);
            else
                return BadRequest(result.Item2);
        }

        /// <summary>
        /// Delete the metadata entity with the identifier
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Entity/{Id}")]
        public IActionResult DeleteEntity([FromRoute] Guid Id)
        {
            var entity = _metadataManager.GetEntity(Id);

            if (entity == null || !CheckEntityAccessToLoggedInTenant(entity.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataEntityNotFound);
            }

            var (isDeleted, message) = _metadataManager.DeleteEntity(Id);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        #endregion

        #region Metadata Field

        /// <summary>
        /// Adds a metadata field details 
        /// </summary>
        /// <param name="metadataField">Metadata field details</param>
        /// <returns></returns>
        [Route("Fields")]
        [HttpPost]
        public IActionResult PostMetadataField([FromBody] MetadataField metadataField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = metadataField.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(metadataField.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            var (isInserted, message) = _metadataManager.PostMetadataField(metadataField);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets the metadata field details by the identifier
        /// </summary>
        /// <param name="fieldId">Field identifier</param>
        /// <returns></returns>
        [Route("Fields/{fieldId}")]
        [HttpGet]
        public IActionResult GetMetadataField([FromRoute] Guid fieldId)
        {
            var entity = _metadataManager.GetMetadataField(fieldId);

            if (entity == null || !CheckEntityAccessToLoggedInTenant(entity.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            return Ok(entity);
        }

        /// <summary>
        /// Updates the field details of the specified identifier
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="metadataField"></param>
        /// <returns></returns>
        [Route("Fields/{fieldId}")]
        [HttpPut]
        public IActionResult PutMetadataField([FromRoute] Guid fieldId, [FromBody] MetadataField metadataField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = metadataField.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(metadataField.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            Delta<MetadataField> delta = new Delta<MetadataField>(typeof(MetadataField));
            Type type = metadataField.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                delta.TrySetPropertyValue(prop.Name, prop.GetValue(metadataField));
            }

            var dbMetadataField = _metadataManager.GetMetadataField(fieldId);
            if (dbMetadataField != null)
            {
                delta.Patch(dbMetadataField);

                var (isUpdated, message) = _metadataManager.PutMetadataField(dbMetadataField);
                if (isUpdated)
                {
                    return Ok(message);
                }
                return BadRequest(message);
            }
            else
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.UserNotFound);
            }
        }

        /// <summary>
        /// Deletes the metadata field with the specified identifier
        /// </summary>
        /// <param name="fieldId">Field identifier</param>
        /// <returns></returns>
        [Route("Fields/{fieldId}")]
        [HttpDelete]
        public IActionResult DeleteMetadataField([FromRoute] Guid fieldId)
        {
            var field = _metadataManager.GetMetadataField(fieldId);

            if (field == null || !CheckEntityAccessToLoggedInTenant(field.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            var (isDeleted, message) = _metadataManager.DeleteMetadataField(fieldId);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets all fields for the logged in tenant
        /// </summary>
        /// <returns></returns>
        [Route("Fields")]
        [HttpGet]
        public IActionResult GetMetadataFieldsByTenant()
        {
            if (User.IsSuperAdmin())
            {
                return Ok(_metadataManager.GetMetadataFieldsByTenant(null));
            }
            if (IsUserExternalApplicationTenantAdmin)
            {
                return Ok(_metadataManager.GetMetadataFieldsByTenant(LoggedInTenantId));
            }
            return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
        }

        #endregion

        #region Schema

        /// <summary>
        /// Adds the metadata schema details
        /// </summary>
        /// <param name="metaDataSchema">Metadata schema details</param>
        /// <returns></returns>
        [Route("Schema")]
        [HttpPost]
        public IActionResult PostMetadataSchema([FromBody] MetadataSchemaDetails metaDataSchema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = metaDataSchema.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(metaDataSchema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var (isInserted, message) = _metadataManager.PostMetadataSchema(metaDataSchema);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets the schema details for the specified schema identifier
        /// </summary>
        /// <param name="schemaId">Schema identifier</param>
        /// <returns></returns>
        [Route("Schema/{schemaId}")]
        [HttpGet]
        public IActionResult GetMetadataSchema([FromRoute] Guid schemaId)
        {
            var entity = _metadataManager.GetMetadataSchema(schemaId);

            if (entity == null || !CheckEntityAccessToLoggedInTenant(entity.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            return Ok(entity);
        }

        /// <summary>
        /// Updates the metadata schema for the specified schema identifier
        /// </summary>
        /// <param name="schemaId">Schema identifier</param>
        /// <param name="metaDataSchema">Metadata schema details</param>
        /// <returns></returns>
        [HttpPut]
        [Route("Schema/{schemaId}")]
        public IActionResult PutMetadataSchema([FromRoute] Guid schemaId, [FromBody] MetadataSchemaDetails metaDataSchema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = metaDataSchema.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(metaDataSchema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            Delta<MetadataSchemaDetails> delta = new Delta<MetadataSchemaDetails>(typeof(MetadataSchemaDetails));
            Type type = metaDataSchema.GetType();
            PropertyInfo[] props = type.GetProperties();

            foreach (var prop in props)
            {
                delta.TrySetPropertyValue(prop.Name, prop.GetValue(metaDataSchema));
            }

            var dbMetadataSchema = _metadataManager.GetMetadataSchema(schemaId);
            if (dbMetadataSchema != null)
            {
                delta.Patch(dbMetadataSchema);

                var (isUpdated, message) = _metadataManager.PutMetadataSchema(dbMetadataSchema);
                if (isUpdated)
                {
                    return Ok(message);
                }
                return BadRequest(message);
            }
            else
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.UserNotFound);
            }
        }

        /// <summary>
        /// Deletes the metadata schema details for the specified identifier
        /// </summary>
        /// <param name="schemaId">Schema Identifier</param>
        /// <returns></returns>
        [Route("Schema/{schemaId}")]
        [HttpDelete]
        public IActionResult DeleteMetadataSchema([FromRoute] Guid schemaId)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var (isDeleted, message) = _metadataManager.DeleteMetadataSchema(schemaId);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        #endregion

        #region Schema Field map

        /// <summary>
        /// Adds the specified field to the schema
        /// </summary>
        /// <param name="schemaId">Schema Identifier</param>
        /// <param name="fieldId">Field identifier</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Schema/{schemaId}/Fields/{fieldId}")]
        public IActionResult AddSchemaToFields([FromRoute] Guid schemaId, [FromRoute] Guid fieldId)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var metadataField = _metadataManager.GetMetadataField(fieldId);

            if (metadataField == null || !CheckEntityAccessToLoggedInTenant(metadataField.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            var (isInserted, message) = _metadataManager.AddSchemaToFields(schema, metadataField);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Adds the specified field to the schema
        /// </summary>
        /// <param name="schemaId">Schema Identifier</param>
        /// <param name="fieldIds">Field identifier</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Schema/{schemaId}/Fields")]
        public IActionResult AddSchemaToFields([FromRoute] Guid schemaId, [FromBody] IEnumerable<Guid> fieldIds)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var metadataFields = _metadataManager.GetMetadataFields(fieldIds);

            if (metadataFields == null || !metadataFields.Any(z => CheckEntityAccessToLoggedInTenant(z.TenantId)))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            var (isInserted, message) = _metadataManager.AddSchemaToFields(schema, metadataFields);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets the schema mapped fields for the specified schema identifier
        /// </summary>
        /// <param name="schemaId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Schema/{schemaId}/Fields")]
        public IActionResult GetSchemaFields([FromRoute] Guid schemaId)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var result = _metadataManager.GetSchemaFields(schemaId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes all the fields mapped to the specified schema identifier
        /// </summary>
        /// <param name="schemaId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Schema/{schemaId}/Fields")]
        public IActionResult DeleteAllSchemaFields([FromRoute] Guid schemaId)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var (isDeleted, message) = _metadataManager.DeleteAllSchemaFields(schemaId);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Deletes the specified schema and field mapping
        /// </summary>
        /// <param name="schemaId"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Schema/{schemaId}/Fields/{fieldId}")]
        public IActionResult DeleteSchemaFields([FromRoute] Guid schemaId, [FromRoute] Guid fieldId)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var entity = _metadataManager.GetMetadataField(fieldId);

            if (entity == null || !CheckEntityAccessToLoggedInTenant(entity.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            var (isDeleted, message) = _metadataManager.DeleteSchemaField(schemaId, fieldId);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        #endregion

        #region Entity Schema

        /// <summary>
        /// Adds schema details for the specified entity
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="schemaId">Schema identifier</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Entity/{entityId}/Schema/{schemaId}")]
        public IActionResult AddSchemaToEntity([FromRoute] Guid entityId, [FromRoute] Guid schemaId)
        {
            var schema = _metadataManager.GetMetadataSchema(schemaId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var (isInserted, message) = _metadataManager.AddSchemaToEntity(entityId, schemaId);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets the schema details for the specified entity
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Entity/{entityId}/Schema")]
        public IActionResult GetEntitySchema([FromRoute] Guid entityId)
        {
            var schema = _metadataManager.GetEntitySchema(entityId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            return Ok(schema);
        }

        /// <summary>
        /// Deletes the schema details for specified entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Entity/{entityId}/Schema")]
        public IActionResult DeleteEntitySchema([FromRoute] Guid entityId)
        {
            var schema = _metadataManager.GetEntitySchema(entityId);

            if (schema == null || !CheckEntityAccessToLoggedInTenant(schema.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataSchemaNotFound);
            }

            var (isDeleted, message) = _metadataManager.DeleteEntitySchema(entityId);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets the metadata fields in the schema for the specified entity
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Entity/{entityId}/Fields")]
        public IActionResult GetEntitySchemaFields([FromRoute] Guid entityId)
        {
            var fields = _metadataManager.GetEntitySchemaFields(entityId);

            if (fields == null || !CheckEntityAccessToLoggedInTenant(fields.First().TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.MetadataFieldsNotFound);
            }

            return Ok(fields);
        }

        #endregion

        #region Entity Metadata Value

        [HttpPost]
        [Route("Entity/{entityId}/Values")]
        public IActionResult AddEntityMetadataValues([FromRoute] Guid entityId, [FromBody] IEnumerable<PublicFieldValue> publicFieldValues)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = publicFieldValues.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            var schema = _metadataManager.GetEntitySchema(entityId);
            if (schema == null /*|| !CheckEntityAccessToLoggedInTenant(schema.TenantId)*/)
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            var (isInserted, message) = _metadataManager.AddEntityMetadataValues(entityId, LoggedInTenantId, publicFieldValues);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Gets the metadata values for the entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Entity/{entityId}/Values")]
        public IActionResult GetEntityMetadataValues([FromRoute] Guid entityId)
        {
            var schema = _metadataManager.GetEntitySchema(entityId);

            if (schema == null /*|| !CheckEntityAccessToLoggedInTenant(schema.TenantId)*/)
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            var (isSuccess, errorMessage, transformationResult) = _metadataManager.GetEntityMetadataValues(entityId);

            return Ok(transformationResult);
        }

        /// <summary>
        /// Deletes all stored metadata values for the specified entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Entity/{entityId}/Values")]
        public IActionResult DeleteAllEntityMetadataValues([FromRoute] Guid entityId)
        {
            var schema = _metadataManager.GetEntitySchema(entityId);

            if (schema == null/* || !CheckEntityAccessToLoggedInTenant(schema.TenantId)*/)
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            var (isDeleted, message) = _metadataManager.DeleteAllEntityMetadataValues(entityId);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Deletes all stored metadata values for the specified entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Entity/{entityId}/Values")]
        public IActionResult PutEntityMetadataValues([FromRoute] Guid entityId, [FromBody] IEnumerable<PublicFieldValue> publicFieldValue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = publicFieldValue.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            var schema = _metadataManager.GetEntitySchema(entityId);
            if (schema == null  /* || !CheckEntityAccessToLoggedInTenant(schema.TenantId)*/)
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }

            var (isInserted, message) = _metadataManager.PutEntityMetadataValues(entityId, LoggedInTenantId, publicFieldValue);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }
        #endregion
    }
}