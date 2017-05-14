using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CricketScoreSheet.Adapters
{
    public class BatsmanStatsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<List<string>> ItemClick;
        private List<PlayerStatistics> _mPlayers;

        public BatsmanStatsAdapter(List<PlayerStatistics> players)
        {
            _mPlayers = players.OrderByDescending(r => r.Runs)
                               .ThenByDescending(r=> r.BattingAvg).ToList();
        }

        public override int ItemCount => _mPlayers.Count;

        public void UpdatedList(List<PlayerStatistics> searchedPlayers)
        {
            _mPlayers = searchedPlayers.OrderByDescending(r => r.Runs)
                .ThenByDescending(r=> r.BattingAvg).ToList();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BatsmanStatsViewHolder vh = holder as BatsmanStatsViewHolder;

            vh?.ItemView.SetBackgroundColor(position % 2 == 1
                            ? new Android.Graphics.Color(ContextCompat.GetColor(holder.ItemView.Context, Resource.Color.rowtwo))
                            : new Android.Graphics.Color(ContextCompat.GetColor(holder.ItemView.Context, Resource.Color.rowone)));

            vh.BatsmanName.Text = _mPlayers[position].PlayerName;
            vh.Matches.Text = _mPlayers[position].Matches.ToString();
            vh.Inns.Text = _mPlayers[position].Innings.ToString();
            vh.HS.Text = _mPlayers[position].HS.ToString();
            vh.SR.Text = _mPlayers[position].SR.ToString();
            vh.Runs.Text = _mPlayers[position].Runs.ToString();
            vh.Avg.Text = _mPlayers[position].BattingAvg.ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.BatsmanStatsRow, parent, false);
            return new BatsmanStatsViewHolder(itemView, OnClick);
        }

        private void OnClick(int position)
        {
            var player = _mPlayers.Where(t => t.TeamName == _mPlayers[position].TeamName &&
                                t.PlayerName == _mPlayers[position].PlayerName).First();
            var playerdetails = new List<string>
            {
                player.PlayerName,
                $"Team Name : { player.TeamName}",
                $"Matches : {player.Matches}",
                $"Innings : {player. Innings}",
                $"NotOuts : {player. NotOuts}",
                $"Runs : {player.Runs}",
                $"Highest Score : {player. HS}",
                $"Balls : {player.Balls}",
                $"Hundreds : {player. Hundreds}",
                $"Fifties : {player. Fifties}",
                $"Batting Avg : {player. BattingAvg}",
                $"Batting SR : {player.SR}",
            };
            ItemClick?.Invoke(this, playerdetails);
        }
    }

    public class BatsmanStatsViewHolder : RecyclerView.ViewHolder
    {
        public TextView BatsmanName { get; private set; }
        public TextView Matches { get; private set; }
        public TextView Inns { get; private set; }
        public TextView Runs { get; private set; }
        public TextView HS { get; private set; }
        public TextView SR { get; private set; }
        public TextView Avg { get; private set; }

        public BatsmanStatsViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            BatsmanName = itemView.FindViewById<TextView>(Resource.Id.name_batsman);
            Matches = itemView.FindViewById<TextView>(Resource.Id.matches_batsman);
            Inns = itemView.FindViewById<TextView>(Resource.Id.inns_batsman);
            HS = itemView.FindViewById<TextView>(Resource.Id.hs_batsman);
            Runs = itemView.FindViewById<TextView>(Resource.Id.runs_batsman);
            SR = itemView.FindViewById<TextView>(Resource.Id.sr_batsman);
            Avg = itemView.FindViewById<TextView>(Resource.Id.avg_batsman);

            itemView.Click += (sender, e) => listener(AdapterPosition);
        }
    }
}