
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DMS_3
{

	[Activity(Label = "SocActivity", NoHistory = true)]
	public class SocActivity : Activity
	{
		string txtspinner;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.societe);

			Spinner spinner = FindViewById<Spinner>(Resource.Id.spinnerSoc);
			Button valideSoc = FindViewById<Button>(Resource.Id.valideSoc);

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource(
					this, Resource.Array.societelist, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			ISharedPreferences pref = Application.Context.GetSharedPreferences("AppInfo", FileCreationMode.Private);

			valideSoc.Click += delegate {
				ISharedPreferencesEditor edit = pref.Edit();
				edit.PutString("SOC", txtspinner);
				edit.Apply();
				StartActivity(new Intent(Application.Context, typeof(SplashActivity)));
			};
		}

		void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			txtspinner = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
		}
	}
}

