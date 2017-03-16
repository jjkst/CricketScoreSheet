using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Views;
using CricketScoreSheet.Screens;
using CricketScoreSheet.Shared.Services;

namespace CricketScoreSheet
{
    [Activity(Label = "Circket Score Sheet", Theme = "@style/MyTheme", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class MainActivity : AppCompatActivity
    { 
        public ActionBarDrawerToggle DrawerToggle;
        private DrawerLayout mDrawerLayout;
        private Access Access;

        public MainActivity()
        {
            Access = new Access();
        }

        protected override void OnCreate(Bundle bundle)
        {
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
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_home):
                    ft.Replace(Resource.Id.FrameLayout, new HomeFragment());
                    break;
                case (Resource.Id.nav_matchresults):
                    ft.Replace(Resource.Id.FrameLayout, new MatchesFragment());
                    break;
                case (Resource.Id.nav_batsmanstats):
                    ft.Replace(Resource.Id.FrameLayout, new BatsmanStatsFragment());
                    break;
                case (Resource.Id.nav_bowlerstats):
                    ft.Replace(Resource.Id.FrameLayout, new BowlerStatsFragment());
                    break;
                case (Resource.Id.clearstorage):
                    Access.MatchService.DropMatchesTable();
                    Access.TeamService.DropTeamsTable();
                    Access.PlayerService.DropPlayersTable();
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
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
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
    }
}

