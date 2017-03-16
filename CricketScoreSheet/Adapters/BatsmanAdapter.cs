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

namespace CricketScoreSheet.Adapters
{
    public class BatsmanAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private readonly List<PlayerEntity> _mPlayers;

        public BatsmanAdapter(List<PlayerEntity> matchplayers)
        {
            _mPlayers = matchplayers;
        }

        public override int ItemCount => _mPlayers.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BatsmanViewHolder vh = holder as BatsmanViewHolder;

            vh?.ItemView.SetBackgroundColor(position % 2 == 1
                ? vh.ItemView.Resources.GetColor(Resource.Color.rowtwo)
                : vh.ItemView.Resources.GetColor(Resource.Color.rowone));

            vh.BatsmanName.Text = _mPlayers[position].Name;
            vh.HowOut.Text = _mPlayers[position].HowOut;
            vh.BatsmanRuns.Text = _mPlayers[position].RunsTaken.ToString();
            vh.BatsmanBalls.Text = _mPlayers[position].BallsPlayed.ToString();
            vh.BatsmanSR.Text = (_mPlayers[position].BallsPlayed == 0) ? "0" :
                (decimal.Round((decimal)_mPlayers[position].RunsTaken * 100 / _mPlayers[position].BallsPlayed, 2, MidpointRounding.AwayFromZero)).ToString();
            vh.BatsmanFours.Text = _mPlayers[position].Fours.ToString();
            vh.BatsmanSixes.Text = _mPlayers[position].Sixes.ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Match_BatsmanRow, parent, false);
            return new BatsmanViewHolder(itemView, OnClick);
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, _mPlayers[position].Id);
        }
    }

    public class BatsmanViewHolder : RecyclerView.ViewHolder
    {
        public TextView BatsmanName { get; private set; }
        public TextView HowOut { get; private set; }
        public TextView BatsmanRuns { get; private set; }
        public TextView BatsmanBalls { get; private set; }
        public TextView BatsmanSR { get; private set; }
        public TextView BatsmanFours { get; private set; }
        public TextView BatsmanSixes { get; private set; }

        public BatsmanViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            BatsmanName = itemView.FindViewById<TextView>(Resource.Id.batsmanName);
            HowOut = itemView.FindViewById<TextView>(Resource.Id.howout);
            BatsmanRuns = itemView.FindViewById<TextView>(Resource.Id.batsmanRuns);
            BatsmanBalls = itemView.FindViewById<TextView>(Resource.Id.batsmanBalls);
            BatsmanSR = itemView.FindViewById<TextView>(Resource.Id.batsmanSR);
            BatsmanFours = itemView.FindViewById<TextView>(Resource.Id.batsmanFours);
            BatsmanSixes = itemView.FindViewById<TextView>(Resource.Id.batsmanSixes);

            itemView.Click += (sender, e) => listener(AdapterPosition);
        }
    }
}