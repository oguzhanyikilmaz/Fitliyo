using Fitliyo.Samples;
using Xunit;

namespace Fitliyo.EntityFrameworkCore.Applications;

[Collection(FitliyoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FitliyoEntityFrameworkCoreTestModule>
{

}
