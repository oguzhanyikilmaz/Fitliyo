using Fitliyo.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Fitliyo.Permissions;

public class FitliyoPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FitliyoPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(FitliyoPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FitliyoResource>(name);
    }
}
