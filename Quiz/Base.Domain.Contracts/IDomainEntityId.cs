namespace Base.Domain.Contracts;

/// <summary>
///     Guid based domain entity id
/// </summary>
public interface IDomainEntityId : IDomainEntityId<Guid>
{
}

/// <summary>
///     Universal Domain Entity interface based on generic PK type
/// </summary>
/// <typeparam name="TKey">Type for primary key</typeparam>
public interface IDomainEntityId<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    
}