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
    public class BatsmanFragment : Fragment
    {
        private readonly List<PlayerEntity> mPlayers;
        private Access Access { get; set; }

        public BatsmanFragment()
        {
            mPlayers = new List<PlayerEntity>();
            Access = new Access();
        }

        public BatsmanFragment(List<PlayerEntity> players)
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
            View view = inflater.Inflate(Resource.Layout.Match_Batsman, container, false);

            var batsmanAdapter = new BatsmanAdapter(mPlayers);
            batsmanAdapter.ItemClick += OnItemClick;

            var mRecyclerViewBatsman = view.FindViewById<RecyclerView>(Resource.Id.batsmanList);
            mRecyclerViewBatsman.SetLayoutManager(new LinearLayoutManager(this.Activity));
            mRecyclerViewBatsman.SetAdapter(batsmanAdapter);

            return view;
        }

        private void OnItemClick(object sender, int playerId)
        {
            var player = Access.PlayerService.GetPlayer(playerId);

            Intent intent = new Intent(this.Activity, typeof(ScoreActivity));
            intent.PutExtra("MatchId", player.MatchId);
            intent.PutExtra("TeamId", player.TeamId);
            intent.PutExtra("PlayerId", player.Id);
            intent.PutExtra("Play", "Batsman");
            StartActivity(intent);
        }
    }
}