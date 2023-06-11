using Base.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Base.Domain.Identity;

public class BaseUser : BaseUser<Guid>, IDomainEntityId
{
    public BaseUser()
    {
    }

    public BaseUser(string username) : base(username)
    {
    }


    public bool IsDeleted { get; set; }
}

public class BaseUser<TKey> : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    public BaseUser()
    {
    }

    public BaseUser(string username) : base(username)
    {
    }
}