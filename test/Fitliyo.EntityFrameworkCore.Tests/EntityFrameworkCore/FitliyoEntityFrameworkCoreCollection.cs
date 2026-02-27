using Xunit;

namespace Fitliyo.EntityFrameworkCore;

[CollectionDefinition(FitliyoTestConsts.CollectionDefinitionName)]
public class FitliyoEntityFrameworkCoreCollection : ICollectionFixture<FitliyoEntityFrameworkCoreFixture>
{

}
