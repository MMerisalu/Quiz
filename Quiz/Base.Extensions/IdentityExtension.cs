using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Base.Extensions;

public static class IdentityExtension
{
    public static Guid GettingUserId(this ClaimsPrincipal user)
    {
        return GettingUserId<Guid>(user);
    }


    public static TKeyType GettingUserId<TKeyType>(this ClaimsPrincipal user)
    {
        if (typeof(TKeyType) != typeof(Guid) 
            && typeof(TKeyType) != typeof(string)
            && typeof(TKeyType) != typeof(int))
        {
            throw new ApplicationException($"This type of user id {typeof(TKeyType).Name} is not supported!");
        }

        var claimId = user.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.NameIdentifier));
        if (claimId == null) throw new NullReferenceException("Name identifier claim not found!");

        var res = (TKeyType) TypeDescriptor.GetConverter(typeof(TKeyType))
            .ConvertFromInvariantString(claimId.Value)!;
        return res;
    }

    /// <summary>
    ///     Check if this user has any role claims that match the requirement
    /// </summary>
    /// <param name="user">The user to extend and check the claims</param>
    /// <param name="role">The role that we want to match on</param>
    /// <returns>True if the user has a claim that matches the required <paramref name="role" /></returns>
    /// <exception cref="NullReferenceException">Expecting that the current user has a role claim</exception>
    public static bool UserIsInRole(this ClaimsPrincipal user, string role)
    {
        if (!user.Claims.Any(u => u.Type.Equals(ClaimTypes.Role)))
            throw new NullReferenceException("Role identifier claim not found!");
        var claimRoles = user.Claims.Where(u => u.Type.Equals(ClaimTypes.Role))
            .SelectMany(c => c.Value.Split(','))
            .Distinct()
            .ToList();
        return claimRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Check if this user has any role claims that match the requirement
    /// </summary>
    /// <param name="user">The user to extend and check the claims</param>
    /// <param name="role">The role that we want to match on</param>
    /// <returns>True if the user has a claim that matches the required <paramref name="role" /></returns>
    /// <exception cref="NullReferenceException">Expecting that the current user has a role claim</exception>
    public static IEnumerable<string> GettingUserRoleNames(this ClaimsPrincipal user)
    {
        if (!user.Claims.Any(u => u.Type.Equals(ClaimTypes.Role)))
            throw new NullReferenceException("Role identifier claim not found!");
        var claimRoles = user.Claims.Where(u => u.Type.Equals(ClaimTypes.Role))
            .SelectMany(c => c.Value.Split(','))
            .Distinct()
            .ToList();
        return claimRoles;
    }

    public static string GettingUserRoleName(this ClaimsPrincipal user)
    {
        // role: "Admin,User"
        // role: "Driver"
        var claimRole = user.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.Role));
        if (claimRole == null) throw new NullReferenceException("Role identifier claim not found!");

        /*var res = (TKeyType) TypeDescriptor.GetConverter(typeof(TKeyType))
            .ConvertFromInvariantString(claimRole.Value)!;
        return res;*/
        return claimRole.Value;
    }

    public static string GettingUserName(this ClaimsPrincipal user)
    {
        return $"{user.Claims.FirstOrDefault(c => c.Type.Equals("aspnet.lastname"))?.Value ?? "???"} " +
               $"{user.Claims.FirstOrDefault(c => c.Type.Equals("aspnet.firstname"))?.Value ?? "???"}";
    }

    public static string GenerateJwt(IEnumerable<Claim> claims, 
        string key, string issuer, string audience, DateTime expirationDateTime)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expirationDateTime,
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static string GettingUserEmail(this ClaimsPrincipal user)
    {
        // role: "Admin,User"
        // role: "Driver"
        var claimEmail = user.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.Email));
        if (claimEmail == null) throw new NullReferenceException("Email identifier claim not found!");

        /*var res = (TKeyType) TypeDescriptor.GetConverter(typeof(TKeyType))
            .ConvertFromInvariantString(claimRole.Value)!;
        return res;*/
        return claimEmail.Value;
    }
}