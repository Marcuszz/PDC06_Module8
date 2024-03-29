﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;
using System.Net.Cache;
using System.Collections.ObjectModel;
using System.Net.Http;
using static PDC06_Module08.SearchPage;

namespace PDC06_Module08
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UpdatePage : ContentPage
	{
		private const string url_search = "http://172.16.24.150/pdc6/api-search.php";
        private const string url_update = "http://172.16.24.150/pdc6/api-search.php";
		private HttpClient _Client = new HttpClient();
		private ObservableCollection<Post2> _posts = new ObservableCollection<Post2>();

        public UpdatePage(Post2 post)
		{
			InitializeComponent ();
			xID.Text = post.ID.ToString ();
		}
		private async Task UpdatePostAsync()
		{
			try
			{
				Post2 post = new Post2
				{
					ID = int.Parse(xID.Text),
					username = xUsername.Text,
					password = xPassword.Text,
				};
				var content = JsonConvert.SerializeObject(post);
				var response = await _Client.PostAsync(url_update, new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.IsSuccessStatusCode)
				{
					await DisplayAlert("Success", "Records updated sucessfully", "OK");
				}
				else
				{
					await DisplayAlert("Error", $"Failed to update", "OK");
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", $"An error occured:{ex.Message}", "OK");
			}
		}

		private async void OnUpdate(object sender, EventArgs e)
		{
			bool result = await DisplayAlert("Update Confirmation", $"Are you sure you want to update ID No:{xID.Text}?", "OK", "Cance;");
			if (result)
			{
				await UpdatePostAsync();
			}
		}
		private async void OnRetrievedChanged(object sender, TextChangedEventArgs e)
		{
			string seacrhQuery = e.NewTextValue;

			if (string.IsNullOrWhiteSpace(seacrhQuery)) 
			{
				xUsername.Text = string.Empty;
				xPassword.Text = string.Empty;
			}
			else
			{
				try
				{
					var searchUrl = $"{url_search}?ID={seacrhQuery}";

					System.Diagnostics.Debug.WriteLine($"Search URL:{searchUrl}");

					var content = await _Client.GetStringAsync(searchUrl);
					var responseObject = JsonConvert.DeserializeObject<ResponseObject>(content);
					if (responseObject.status)
					{
						var searchResults = JsonConvert.DeserializeObject<List<Post>>(responseObject.data.ToString());

						if (searchResults.Count > 0)
						{
							var firstResult = searchResults[0];

							xUsername.Text = firstResult.username;
							xPassword.Text = firstResult.password;
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"An error occured:{ex.Message}");
				}
			}
		}
	}
}