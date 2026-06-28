namespace Spotflix.Api.Authorization;

/// <summary>Nomes de papéis usados na aplicação.</summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly string[] All = { Admin, User };
}

/// <summary>Nomes de policies de autorização.</summary>
public static class Policies
{
    public const string AdminOnly = "AdminOnly";
}
