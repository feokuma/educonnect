using EduConnect.Application.Common;

namespace EduConnect.Infrastructure.Identifiers;

public sealed class UuidV7IdGenerator : IIdGenerator
{
    public Guid NewId()
    {
        return Guid.CreateVersion7();
    }
}
