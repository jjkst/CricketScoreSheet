using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Shared.Services;
using System.Collections.Generic;

namespace CricketScoreSheet.Screens
{
    public class BowlerFragment : Fragment
    {
        private readonly List<PlayerEntity> mPlayers;
        private Access Access { get; set; }

        public BowlerFragment() 
        {
            mPlayers = new List<PlayerEntity>();
            Access = new Access();
        }

        public BowlerFragment(List<PlayerEntity> players)
        {
            mPlayers = players;
            Access = new Access();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Match_Bowler, container, false);

            var bowlerAdapter = new BowlerAdapter(mPlayers);
            bowlerAdapter.ItemClick += OnItemClick;

            var mRecyclerViewBowler = view.FindViewById<RecyclerView>(Resource.Id.bowlerList);
            mRecyclerViewBowler.SetLayoutManager(new LinearLayoutManager(Activity));
            mRecyclerViewBowler.SetAdapter(bowlerAdapter);

            return view;
        }

        private void OnItemClick(object sender, int playerId)
        {
            var player = Access.PlayerService.GetPlayer(playerId);

            Intent intent = new Intent(this.Activity, typeof(ScoreActivity));
            intent.PutExtra("MatchId", player.MatchId);
            intent.PutExtra("TeamId", player.TeamId);
            intent.PutExtra("PlayerId", player.Id);
            intent.PutExtra("Play", "Bowler");
            StartActivity(intent);
        }
    }
}