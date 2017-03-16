using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Adapters;

namespace CricketScoreSheet.Screens
{
    public class BowlerFragment : Fragment
    {
        private readonly List<PlayerEntity> mPlayers;

        public BowlerFragment() 
        {
            mPlayers = new List<PlayerEntity>();
        }

        public BowlerFragment(List<PlayerEntity> players)
        {
            mPlayers = players;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Match_Bowler, container, false);

            var mRecyclerViewBowler = view.FindViewById<RecyclerView>(Resource.Id.bowlerList);
            mRecyclerViewBowler.SetLayoutManager(new LinearLayoutManager(Activity));
            mRecyclerViewBowler.SetAdapter(new BowlerAdapter(mPlayers));

            return view;
        }
    }
}