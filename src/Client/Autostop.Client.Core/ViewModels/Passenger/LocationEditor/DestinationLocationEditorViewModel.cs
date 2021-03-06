﻿using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Autostop.Client.Abstraction.Factories;
using Autostop.Client.Abstraction.Models;
using Autostop.Client.Abstraction.Providers;
using Autostop.Client.Abstraction.Services;
using Autostop.Client.Abstraction.Subscribers;
using Autostop.Client.Core.Extensions;
using Autostop.Client.Core.Models;
using Autostop.Client.Core.ViewModels.Passenger.ChooseOnMap;
using Autostop.Client.Core.ViewModels.Passenger.LocationEditor.Base;

namespace Autostop.Client.Core.ViewModels.Passenger.LocationEditor
{
	public class DestinationLocationEditorViewModel : BaseLocationEditorViewModel
	{
		private readonly INavigationService _navigationService;
		private readonly IChooseOnMapViewModelFactory _chooseOnMapViewModelFactory;
		private readonly IEmptyAutocompleteResultProvider _autocompleteResultProvider;

		public DestinationLocationEditorViewModel(
			ISchedulerProvider schedulerProvider,
			INavigationService navigationService,
			IPlacesProvider placesProvider,
			IGeocodingProvider geocodingProvider,
			IChooseOnMapViewModelFactory chooseOnMapViewModelFactory,
			IEmptyAutocompleteResultProvider autocompleteResultProvider,
			ISelectedDestinationByMapSubscriber destinationByMapSubscriber)
			: base(schedulerProvider, placesProvider, geocodingProvider, navigationService)
		{
			_navigationService = navigationService;
			_chooseOnMapViewModelFactory = chooseOnMapViewModelFactory;
			_autocompleteResultProvider = autocompleteResultProvider;

			this.Changed(() => SelectedSearchResult)
				.Where(r => r is SetLocationOnMapResultModel)
				.ObserveOn(schedulerProvider.SynchronizationContextScheduler)
				.Subscribe(NavigateToChooseDestinationOnMapViewModel);

			destinationByMapSubscriber.Publisher.Handler.Subscribe(address =>
			{
				SelectedAddress = address;
				navigationService.GoBack();
			});
		}

		private void NavigateToChooseDestinationOnMapViewModel(IAutoCompleteResultModel autoCompleteResultModel)
		{
			var chooseDestinationOnMapViewModel = _chooseOnMapViewModelFactory.GetChooseDestinationOnMapViewModel() as ChooseDestinationOnMapViewModel;
			_navigationService.NavigateTo(chooseDestinationOnMapViewModel);
		}

		protected override ObservableCollection<IAutoCompleteResultModel> GetEmptyAutocompleteResult()
		{
			return new ObservableCollection<IAutoCompleteResultModel>
			{
				_autocompleteResultProvider.GetHomeResultModel(),
				_autocompleteResultProvider.GetWorkResultModel(),
				_autocompleteResultProvider.GetSetLocationOnMapResultModel()
			};
		}

		public override string PlaceholderText => "Set destination location";

		/// <summary>
		/// We should call load always when view model appears, because search result should be updated after setting home or work address 
		/// </summary>
		public void LoadEmptyAutocompleteResult()
		{
			SearchResults = GetEmptyAutocompleteResult();
		}
	}
}