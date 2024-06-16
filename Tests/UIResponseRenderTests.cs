using Bunit;
using ChessTournamentManager.Components;
using BlazorBootstrap;
using Microsoft.Extensions.DependencyInjection;
using ChessTournamentMate.Shared.QuickResponseMessages;
using System.Data.SqlTypes;
using Xunit;


namespace Tests
{

	public class UIResponseRenderTests : TestContext
	{

		public UIResponseRenderTests()
		{
			Services.AddBlazorBootstrap();
			JSInterop.SetupVoid("window.blazorBootstrap.alert.initialize", _ => true);
		}

		[Fact]
		public void DefaultState_DontShowMessage()
		{
			IRenderedComponent<UIResponseRender> component = RenderComponent<UIResponseRender>();
			int numberOfAlerts = component.FindComponents<Alert>().Count;
			Assert.Equal(0, numberOfAlerts);
		}

		[Fact]
		public void ShowIfMessage()
		{
			IRenderedComponent <UIResponseRender> component = RenderComponent<UIResponseRender>(parameters => parameters.Add(p => p.Message, new SuccessfulMessage("test")));
			int numberOfAlerts = component.FindComponents<Alert>().Count;
			Assert.Equal(1, numberOfAlerts);
		}

		[Fact]
		public void ShowCorrectColor()
		{
			IRenderedComponent<UIResponseRender> componentSuccessful = RenderComponent<UIResponseRender>(parameters => parameters.Add(p => p.Message, new SuccessfulMessage("test")));
			IRenderedComponent<UIResponseRender> componentUnSuccessful = RenderComponent<UIResponseRender>(parameters => parameters.Add(p => p.Message, new UnsuccessfulMessage("test")));
			IRenderedComponent<UIResponseRender> componentNotExists = RenderComponent<UIResponseRender>(parameters => parameters.Add(p => p.Message, new NotExistsInDatabase("test")));

			Alert alertSuccessful = componentSuccessful.FindComponent<Alert>().Instance;
			Alert alertUnSuccessful = componentUnSuccessful.FindComponent<Alert>().Instance;
			Alert alertNotExists = componentNotExists.FindComponent<Alert>().Instance;

			Assert.Equal(AlertColor.Success, alertSuccessful.Color);
			Assert.Equal(AlertColor.Danger, alertUnSuccessful.Color);
			Assert.Equal(AlertColor.Warning, alertNotExists.Color);
		}
	}
}
