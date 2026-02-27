using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Fitliyo.Pages;

public class Index_Tests : FitliyoWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
