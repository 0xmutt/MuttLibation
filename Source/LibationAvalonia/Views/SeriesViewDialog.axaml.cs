using AudibleApi.Common;
using AudibleApi;
using Avalonia.Controls;
using DataLayer;
using Dinah.Core;
using FileLiberator;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using LibationAvalonia.Dialogs;
using LibationUiBase.SeriesView;
using System;
using Avalonia.Media;

namespace LibationAvalonia.Views
{
	public partial class SeriesViewDialog : DialogWindow
	{
		private readonly LibraryBook LibraryBook;
		public AvaloniaList<TabItem> TabItems { get; } = new();
		public SeriesViewDialog()
		{
			InitializeComponent();
			DataContext = this;

			if (Design.IsDesignMode)
			{
				TabItems.Add(new TabItem { Header = "This is a Header", FontSize = 14, Content = new TextBlock { Text = "Some Text" } });
			}
			else
			{
				Loaded += SeriesViewDialog_Loaded;
			}
		}

		public SeriesViewDialog(LibraryBook libraryBook) : this()
		{
			LibraryBook = ArgumentValidator.EnsureNotNull(libraryBook, "libraryBook");
		}

		private async void SeriesViewDialog_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			try
			{
				var seriesEntries = await SeriesItem.GetAllSeriesItemsAsync(LibraryBook);

				foreach (var series in seriesEntries.Keys)
				{
					TabItems.Add(new TabItem
					{
						Header = series.Title,
						FontSize = 14,
						Content = new SeriesViewGrid(LibraryBook, series, seriesEntries[series])
					});
				}

			}
			catch (Exception ex)
			{
				Serilog.Log.Logger.Error(ex, "Error loading searies info");

				TabItems.Add(new TabItem
				{
					Header = "ERROR",
					Content = new TextBlock { Text = "ERROR LOADING SERIES INFO\r\n\r\n" + ex.Message, Foreground = Brushes.Red }
				});
			}
		}
	}
}
