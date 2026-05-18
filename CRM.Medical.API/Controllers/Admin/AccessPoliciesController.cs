using System.Text.Json;
using CRM.Medical.API.Contracts.Admin.AccessPolicies;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/access-policies")]
public sealed class AccessPoliciesController(
    MedicalDbContext db,
    IAccessPolicyConditionParser parser,
    IAccessPolicyConditionValidator validator) : AdminBaseController
{
    private const string Wildcard = "*";

    [HttpGet]
    public async Task<IReadOnlyList<AccessPolicy>> List(CancellationToken cancellationToken) =>
        await db.AccessPolicies.AsNoTracking()
            .OrderBy(x => x.Resource)
            .ThenBy(x => x.Action)
            .ThenBy(x => x.Priority)
            .ToListAsync(cancellationToken);

    [HttpGet("tables")]
    public ActionResult<IReadOnlyList<AccessPolicyTableMetadataResponse>> GetTables()
    {
        var tables = db.Model.GetEntityTypes()
            .Select(CreateTableMetadata)
            .OfType<AccessPolicyTableMetadataResponse>()
            .OrderBy(table => table.TableName)
            .ToList();

        return Ok(tables);
    }

    [HttpGet("tables/{tableName}/fields")]
    public ActionResult<IReadOnlyList<AccessPolicyColumnMetadataResponse>> GetTableFields(string tableName)
    {
        var table = db.Model.GetEntityTypes()
            .Select(CreateTableMetadata)
            .OfType<AccessPolicyTableMetadataResponse>()
            .FirstOrDefault(x => string.Equals(x.TableName, tableName, StringComparison.OrdinalIgnoreCase));

        return table is null
            ? NotFound(new { Message = $"Table '{tableName}' was not found." })
            : Ok(table.Columns);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccessPolicy>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var policy = await db.AccessPolicies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return policy is null ? NotFound() : Ok(policy);
    }

    [HttpPost]
    public async Task<ActionResult<AccessPolicy>> Create(AccessPolicyUpsertRequest request, CancellationToken cancellationToken)
    {
        var policy = Map(request);
        policy.Id = Guid.NewGuid();
        policy.CreatedAt = DateTime.UtcNow;

        var validation = ValidateForResource(policy.Resource, policy.Condition);
        if (!validation.IsValid)
            return ValidationProblem(detail: string.Join("; ", validation.Errors));

        db.AccessPolicies.Add(policy);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = policy.Id }, policy);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccessPolicy>> Update(Guid id, AccessPolicyUpsertRequest request, CancellationToken cancellationToken)
    {
        var entity = await db.AccessPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        var mapped = Map(request);
        entity.Resource = mapped.Resource;
        entity.Action = mapped.Action;
        entity.Effect = mapped.Effect;
        entity.SubjectType = mapped.SubjectType;
        entity.SubjectKey = mapped.SubjectKey;
        entity.Condition = mapped.Condition;
        entity.Priority = mapped.Priority;
        entity.IsEnabled = mapped.IsEnabled;
        entity.Description = mapped.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        var validation = ValidateForResource(entity.Resource, entity.Condition);
        if (!validation.IsValid)
            return ValidationProblem(detail: string.Join("; ", validation.Errors));

        await db.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [HttpPatch("{id:guid}/enable")]
    public async Task<IActionResult> Enable(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.AccessPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();
        entity.IsEnabled = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.AccessPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();
        entity.IsEnabled = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.AccessPolicies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();
        db.AccessPolicies.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("validate")]
    public ActionResult<object> Validate(AccessPolicyUpsertRequest request)
    {
        var policy = Map(request);
        var result = ValidateForResource(policy.Resource, policy.Condition);
        return Ok(new { result.IsValid, result.Errors });
    }

    private static AccessPolicy Map(AccessPolicyUpsertRequest request)
    {
        JsonDocument? condition = null;
        if (request.Condition.HasValue)
            condition = JsonDocument.Parse(request.Condition.Value.GetRawText());

        return new AccessPolicy
        {
            Resource = request.Resource.Trim(),
            Action = request.Action.Trim(),
            Effect = request.Effect,
            SubjectType = request.SubjectType,
            SubjectKey = request.SubjectKey.Trim(),
            Condition = condition,
            Priority = request.Priority,
            IsEnabled = request.IsEnabled,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
        };
    }

    private AccessPolicyValidationResult ValidateForResource(string resource, JsonDocument? condition)
    {
        var parsed = parser.Parse(condition);
        if (resource == Wildcard)
            return ValidateWildcardResourceCondition(condition);

        return resource switch
        {
            "test_requests" => validator.Validate<TestRequest>(parsed),
            "test_results" => validator.Validate<TestResult>(parsed),
            "medical_tests" => validator.Validate<MedicalTest>(parsed),
            "external_patients" => validator.Validate<ExternalPatient>(parsed),
            "subscription_packages" => validator.Validate<SubscriptionPackage>(parsed),
            "conversations" => validator.Validate<Conversation>(parsed),
            "conversation_participants" => validator.Validate<ConversationParticipant>(parsed),
            "messages" => validator.Validate<Message>(parsed),
            "users" => validator.Validate<CRM.Medical.Domain.Entities.User>(parsed),
            "complaints" => validator.Validate<Complaint>(parsed),
            "banners" => validator.Validate<Banner>(parsed),
            "templates" => validator.Validate<Template>(parsed),
            _ => new AccessPolicyValidationResult
            {
                Errors = { $"Unknown resource '{resource}'." }
            }
        };
    }

    private static AccessPolicyValidationResult ValidateWildcardResourceCondition(JsonDocument? condition)
    {
        var result = new AccessPolicyValidationResult();
        if (condition is not null)
        {
            result.Errors.Add("Wildcard resource '*' only supports null condition.");
        }

        return result;
    }

    private static AccessPolicyTableMetadataResponse? CreateTableMetadata(IEntityType entityType)
    {
        var tableName = entityType.GetTableName();
        if (string.IsNullOrWhiteSpace(tableName))
            return null;

        var schema = entityType.GetSchema();
        var storeObject = StoreObjectIdentifier.Table(tableName, schema);
        var columns = entityType.GetProperties()
            .Select(property => new AccessPolicyColumnMetadataResponse(
                property.Name,
                property.GetColumnName(storeObject) ?? property.Name,
                property.ClrType.Name,
                property.IsNullable))
            .OrderBy(column => column.ColumnName)
            .ToList();

        return new AccessPolicyTableMetadataResponse(
            tableName,
            schema,
            entityType.ClrType.Name,
            columns);
    }

    public sealed record AccessPolicyTableMetadataResponse(
        string TableName,
        string? Schema,
        string EntityName,
        IReadOnlyList<AccessPolicyColumnMetadataResponse> Columns);

    public sealed record AccessPolicyColumnMetadataResponse(
        string FieldName,
        string ColumnName,
        string Type,
        bool IsNullable);
}
