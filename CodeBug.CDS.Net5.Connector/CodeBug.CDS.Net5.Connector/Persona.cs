using System;
using System.Text.Json.Serialization;

namespace CodeBug.CDS.Net5.Connector
{
    public record Persona
    {
        public Guid BusinessUnitId { get; }
        public Guid UserId { get; }
        public Guid OrganizationId { get; }

        [JsonConstructor]
        public Persona(Guid businessUnitId, Guid userId, Guid organizationId) =>
            (BusinessUnitId, UserId, OrganizationId) = (businessUnitId, userId, organizationId);
    }
}
