using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Bunit;
using ChessTournamentManager.Components;
using BlazorBootstrap;
using ChessTournamentManager.LanguageAssets.TournamentManagementLabels;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;


namespace Tests
{

	public class Localizer : IStringLocalizer<TournamentManagementLabels>
	{

		public LocalizedString this[string name] => new LocalizedString(name, name) ;

		public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			throw new NotImplementedException();
		}
	}

	public class SaveCancelTests : TestContext
	{

		public SaveCancelTests()
		{
			Services.AddSingleton<IStringLocalizer<TournamentManagementLabels>>(new Localizer());
			Services.AddBlazorBootstrap();
		}

		[Fact]
		public void DefaultState_ShowsButtons()
		{	
			IRenderedComponent<SaveCancelButtons> component = RenderComponent<SaveCancelButtons>();

			int numberOfButtons = component.FindComponents<Button>().Count;
			Assert.Equal(2, numberOfButtons);
		}


		[Fact]
		public void VisibleFalse_DoesNotShowButtons()
		{
			IRenderedComponent<SaveCancelButtons> component = RenderComponent<SaveCancelButtons>(parameters => parameters.Add(p => p.Visible, false));

			int numberOfButtons = component.FindComponents<Button>().Count;
			Assert.Equal(0, numberOfButtons);
		}
	}
}
