
using Base.Domain.Contracts;

namespace Base.Domain;

public abstract class DomainEntityId : DomainEntityId<Guid>, IDomainEntityId
{
    public DomainEntityId()
    {
        Id = Guid.NewGuid();
    }
}

public abstract class DomainEntityId<TKey> : IDomainEntityId<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    
    
    
    public override bool Equals(object? obj)
    {
        return obj is DomainEntityId<TKey> dom && dom.Id.Equals(this.Id);
    }
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }
}