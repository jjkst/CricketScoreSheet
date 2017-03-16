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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.Services;

namespace CricketScoreSheet.Screens
{
    [Activity(Label = "Saved Games", Theme = "@style/MyTheme")]
    public class MatchesActivity : AppCompatActivity
    {
        private RecyclerView _mRecyclerView;
        private Access Access;

        public MatchesActivity()
        {
            Access = new Access();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Matches);

            // Initialize toolbar
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            _mRecyclerView = FindViewById<RecyclerView>(Resource.Id.cardList);
            _mRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
        }

        protected override void OnStart()
        {
            base.OnStart();
            var adapter = new MatchAdapter(Access.MatchService.GetMatches().Where(c => c.Complete == false).ToList());
            adapter.ItemClick += OnItemClick;
            _mRecyclerView.SetAdapter(adapter);
        }

        private void OnItemClick(object sender, int matchId)
        {
            var currentMatchActivity = new Intent(this, typeof(CurrentMatchActivity));
            currentMatchActivity.PutExtra("MatchId", matchId);
            StartActivity(currentMatchActivity);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    var mainActivity = new Intent(this, typeof(MainActivity));
                    mainActivity.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(mainActivity);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}