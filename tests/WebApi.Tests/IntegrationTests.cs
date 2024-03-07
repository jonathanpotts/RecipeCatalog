using System.Net;

namespace JonathanPotts.RecipeCatalog.WebApi.Tests;

public class IntegrationTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    [Theory]
    [InlineData("/api/v1/cuisines")]
    [InlineData("/api/v1/recipes")]
    public async void GetEndpointsReturnSuccessStatusCode(string endpoint)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(endpoint);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [InlineData("/api/v1/cuisines")]
    [InlineData("/api/v1/recipes")]
    public async void PostEndpointsReturnsUnauthorizedStatusCodeWhenUnauthorized(string endpoint)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsync(endpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
