using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Identity;

namespace Fitliyo.Account;

/// <summary>
/// Kayıt sırasında ExtraProperties içinden "Role" (Trainer veya Student) alıp kullanıcıya atar.
/// </summary>
public class FitliyoAccountAppService : AccountAppService
{
    public const string RoleExtraPropertyKey = "Role";
    public const string RoleTrainer = "Trainer";
    public const string RoleStudent = "Student";

    public FitliyoAccountAppService(
        IdentityUserManager userManager,
        Volo.Abp.Identity.IIdentityRoleRepository roleRepository,
        IAccountEmailer accountEmailer,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions)
        : base(userManager, roleRepository, accountEmailer, identitySecurityLogManager, identityOptions)
    {
    }

    public override async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        var result = await base.RegisterAsync(input);

        var roleName = input.ExtraProperties != null && input.ExtraProperties.TryGetValue(RoleExtraPropertyKey, out var roleVal) ? roleVal?.ToString() : null;
        if (string.IsNullOrWhiteSpace(roleName))
            return result;

        if (roleName != RoleTrainer && roleName != RoleStudent)
            return result;

        var user = await UserManager.GetByIdAsync(result.Id);
        if (user != null)
        {
            await UserManager.AddToRoleAsync(user, roleName);
        }

        return result;
    }
}
