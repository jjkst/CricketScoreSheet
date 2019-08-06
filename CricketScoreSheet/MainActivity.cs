using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Screens;
using CricketScoreSheet.Shared.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using AlertDialog = Android.App.AlertDialog;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace CricketScoreSheet
{
    [Activity(Label = "Cricket Score Sheet", Theme = "@style/MyTheme", MainLauncher = true, Icon = "@drawable/ic_launcher"
        , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    { 
        public ActionBarDrawerToggle DrawerToggle;
        private DrawerLayout mDrawerLayout;
        private Shared.Services.Access Access;

        public MainActivity()
        {
            Access = new Shared.Services.Access();
        }

        protected async override void OnCreate(Bundle bundle)
        {
            IO.Fabric.Sdk.Android.Fabric.With(this, new Com.Crashlytics.Android.Crashlytics());
            await GetPermissionsAsync();
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var nav = Intent.GetStringExtra("Nav");

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            // Initialize toolbar
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            DrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, toolbar,
                Resource.String.ApplicationName, Resource.String.ApplicationName);
            mDrawerLayout.AddDrawerListener(DrawerToggle);
            DrawerToggle.SyncState();

            var ft = FragmentManager.BeginTransaction();
            if(nav == "matches")
            {
                ft.Add(Resource.Id.FrameLayout, new MatchesFragment());
                ft.AddToBackStack("matches");
            }
            else
            {
                ft.Add(Resource.Id.FrameLayout, new HomeFragment());
                ft.AddToBackStack("home");                
            }
            ft.Commit();

        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            var ft = FragmentManager.BeginTransaction();
            try
            {                
                var prevFragment = FragmentManager.FindFragmentById(Resource.Id.FrameLayout);
                if (prevFragment != null) ft.Remove(prevFragment);
            }
            catch(Exception ex)
            {
                Log.Debug("ClearingFragment", ex.Message);
            }
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_home):
                    SupportActionBar.SetTitle(Resource.String.ApplicationName);
                    ft.Replace(Resource.Id.FrameLayout, new HomeFragment());
                    break;
                case (Resource.Id.nav_matchresults):
                    SupportActionBar.SetTitle(Resource.String.CompletedMatches);
                    ft.Replace(Resource.Id.FrameLayout, new MatchesFragment());
                    break;
                case (Resource.Id.nav_batsmanstats):
                    SupportActionBar.SetTitle(Resource.String.BatsmanStats);
                    ft.Replace(Resource.Id.FrameLayout, new BatsmanStatsFragment());
                    break;
                case (Resource.Id.nav_bowlerstats):
                    SupportActionBar.SetTitle(Resource.String.BowlerStats);
                    ft.Replace(Resource.Id.FrameLayout, new BowlerStatsFragment());
                    break;
                case (Resource.Id.clearstorage):
                    Access.MatchService.DropMatchesTable();
                    Access.TeamService.DropTeamsTable();
                    Access.PlayerService.DropPlayersTable();
                    Access.UmpireService.DropUmpiresTable();
                    break;
            }
            ft.Commit();
            mDrawerLayout.CloseDrawers();            
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                case Resource.Id.action_help:
                    // Copy pdf file to external drive
                    var filename = "Help_CSS.pdf";
                    Java.IO.File pdfFile = new Java.IO.File(Helper.DownloadPath, filename);
                    pdfFile.SetReadable(true);
                    if (!pdfFile.Exists())
                    {
                        AssetManager assetManager = Assets;
                        var inputstream = assetManager.Open(filename);
                        using (var memoryStream = new MemoryStream())
                        {
                            inputstream.CopyTo(memoryStream);
                            Helper.SavePdfFile(filename, memoryStream.ToArray());
                        }
                    }
                    Android.Net.Uri result;
                    if (Build.VERSION.SdkInt < (BuildVersionCodes)24)
                    {
                        result = Android.Net.Uri.FromFile(pdfFile);
                    }
                    else
                    {
                        result = Android.Support.V4.Content.FileProvider.GetUriForFile(this, this.ApplicationContext.PackageName + ".provider", pdfFile);
                    }
                    Intent pdfIntent = new Intent(Intent.ActionView);
                    pdfIntent.SetDataAndType(result, "application/pdf");
                    pdfIntent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                    pdfIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    StartActivity(pdfIntent);                        
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount != 0)
            {
                FragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
            }
        }
        #region RuntimePermissions

        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }


        }

        const int RequestLocationId = 0;

        readonly string[] PermissionsGroupLocation =
            {
                            Manifest.Permission.WriteExternalStorage             
            };

        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.WriteExternalStorage;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                //TODO change the message to show the permissions name
                Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                //set alert for executing the task
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Permissions Needed");
                alert.SetMessage("The application need special permissions to continue");
                alert.SetPositiveButton("Request Permissions", (senderAlert, args) =>
                {
                    RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();


                return;
            }

            RequestPermissions(PermissionsGroupLocation, RequestLocationId);

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();

                        }
                        else
                        {
                            //Permission Denied :(
                            Toast.MakeText(this, "Special permissions denied", ToastLength.Short).Show();

                        }
                    }
                    break;
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion
    }
}

