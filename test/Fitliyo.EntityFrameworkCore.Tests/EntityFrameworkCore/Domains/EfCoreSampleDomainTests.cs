using Fitliyo.Samples;
using Xunit;

namespace Fitliyo.EntityFrameworkCore.Domains;

[Collection(FitliyoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FitliyoEntityFrameworkCoreTestModule>
{

}
