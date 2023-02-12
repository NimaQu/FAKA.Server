namespace FAKA.Server.Auth;

public abstract class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    
    public static IEnumerable<string> GetRoles()
    {
        return new string[] { Admin, User };
    }
}