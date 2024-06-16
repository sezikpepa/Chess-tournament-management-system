using Bunit;
using ChessTournamentManager.Components;
using BlazorBootstrap;
using Microsoft.Extensions.DependencyInjection;
using ChessTournamentMate.Shared.QuickResponseMessages;
using System.Data.SqlTypes;
using Xunit;


namespace Tests
{

	public class BasicSpinnerTests : TestContext
	{

		public BasicSpinnerTests()
		{
			Services.AddBlazorBootstrap();
		}

		[Fact]
		public void DefaultState_ShowSpinner()
		{
			IRenderedComponent<BasicSpinner> component = RenderComponent<BasicSpinner>();
			int numberOfSpinner = component.FindComponents<Spinner>().Count;
			Assert.Equal(1, numberOfSpinner);
		}

		[Fact]
		public void Visible()
		{
			IRenderedComponent<BasicSpinner> component = RenderComponent<BasicSpinner>(parameters => parameters.Add(p => p.Visible, true));
			Spinner spinner = component.FindComponent<Spinner>().Instance;
			Assert.True(spinner.Visible);
		}

		[Fact]
		public void NotVisible()
		{
			IRenderedComponent<BasicSpinner> component = RenderComponent<BasicSpinner>(parameters => parameters.Add(p => p.Visible, false));
			Spinner spinner = component.FindComponent<Spinner>().Instance;
			Assert.False(spinner.Visible);
		}
	}
}
