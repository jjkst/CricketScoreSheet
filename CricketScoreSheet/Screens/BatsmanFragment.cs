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
    public class BatsmanFragment : Fragment
    {
        private readonly List<PlayerEntity> mPlayers;

        public BatsmanFragment()
        {
            mPlayers = new List<PlayerEntity>();
        }

        public BatsmanFragment(List<PlayerEntity> players)
        {
            mPlayers = players;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Match_Batsman, container, false);

            var mRecyclerViewBatsman = view.FindViewById<RecyclerView>(Resource.Id.batsmanList);
            mRecyclerViewBatsman.SetLayoutManager(new LinearLayoutManager(this.Activity));
            mRecyclerViewBatsman.SetAdapter(new BatsmanAdapter(mPlayers));

            return view;
        }

       
    }
}