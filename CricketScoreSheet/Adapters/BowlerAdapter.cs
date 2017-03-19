
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
using Android.Support.V7.Widget;
using CricketScoreSheet.Shared.DataAccess.Entities;
using Android.Support.V4.Content;

namespace CricketScoreSheet.Adapters
{
    public class BowlerAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private readonly List<PlayerEntity> _mPlayers;

        public BowlerAdapter(List<PlayerEntity> matchplayers)
        {
            _mPlayers = matchplayers;
        }

        public override int ItemCount => _mPlayers.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BowlerViewHolder vh = holder as BowlerViewHolder;

            vh?.ItemView.SetBackgroundColor(position % 2 == 1
                            ? new Android.Graphics.Color(ContextCompat.GetColor(holder.ItemView.Context, Resource.Color.rowtwo))
                            : new Android.Graphics.Color(ContextCompat.GetColor(holder.ItemView.Context, Resource.Color.rowone)));


            vh.BowlerName.Text = _mPlayers[position].Name;
            string overs = Helper.ConvertBallstoOvers(_mPlayers[position].BallsBowled);
            vh.BowlerOvers.Text = overs;
            vh.BowlerRuns.Text = _mPlayers[position].RunsGiven.ToString();
            vh.BowlerWickets.Text = _mPlayers[position].Wickets.ToString();
            vh.BowlerEcon.Text = (_mPlayers[position].BallsBowled == 0) ? "0" :
                (decimal.Round((decimal)_mPlayers[position].RunsGiven / Convert.ToDecimal(overs), 2, MidpointRounding.AwayFromZero)).ToString();
            vh.BowlerDots.Text = _mPlayers[position].Dots.ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Match_BowlerRow, parent, false);
            return new BowlerViewHolder(itemView, OnClick);
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, _mPlayers[position].Id);
        }
    }

    public class BowlerViewHolder : RecyclerView.ViewHolder
    {
        public TextView BowlerName { get; private set; }
        public TextView BowlerOvers { get; private set; }
        public TextView BowlerRuns { get; private set; }
        public TextView BowlerWickets { get; private set; }
        public TextView BowlerEcon { get; private set; }
        public TextView BowlerDots { get; private set; }

        public BowlerViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            BowlerName = itemView.FindViewById<TextView>(Resource.Id.bowlerName);
            BowlerOvers = itemView.FindViewById<TextView>(Resource.Id.bowlerOvers);
            BowlerRuns = itemView.FindViewById<TextView>(Resource.Id.bowlerRuns);
            BowlerWickets = itemView.FindViewById<TextView>(Resource.Id.bowlerWickets);
            BowlerEcon = itemView.FindViewById<TextView>(Resource.Id.bowlerEcon);
            BowlerDots = itemView.FindViewById<TextView>(Resource.Id.bowlerDots);

            itemView.Click += (sender, e) => listener(AdapterPosition);
        }
    }
}