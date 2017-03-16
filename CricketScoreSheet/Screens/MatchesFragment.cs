using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.Services;
using Android.Support.V7.Widget;
using CricketScoreSheet.Adapters;

namespace CricketScoreSheet.Screens
{
    public class MatchesFragment : Fragment
    {
        private readonly List<Match> Matches;
        private Access Access;

        public MatchesFragment()
        {
            Access = new Access();
            Matches = Access.MatchService.GetMatches();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Matches_NoAppBar, container, false);

            var mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.cardList);            
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));

            var adapter = new MatchAdapter(Matches.Where(c => c.Complete == true).ToList());
            adapter.ItemClick += OnItemClick;
            mRecyclerView.SetAdapter(adapter);

            return view;
        }

        void OnItemClick(object sender, int matchId)
        {
            var currentMatchActivity = new Intent(this.Activity, typeof(CurrentMatchActivity));
            currentMatchActivity.PutExtra("MatchId", matchId);
            StartActivity(currentMatchActivity);
        }
    }
}