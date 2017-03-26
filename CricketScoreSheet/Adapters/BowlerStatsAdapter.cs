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
    public class BowlerStatsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<List<string>> ItemClick;
        private List<PlayerStatistics> _mPlayers;

        public BowlerStatsAdapter(List<PlayerStatistics> players)
        {
            _mPlayers = players.OrderByDescending(w => w.Wickets)
                               .ThenByDescending(w=>w.BowlingAvg).ToList(); 
        }

        public override int ItemCount => _mPlayers.Count;

        public void UpdatedList(List<PlayerStatistics> searchedPlayers)
        {
            _mPlayers = searchedPlayers.OrderByDescending(w => w.Wickets)
                                       .ThenByDescending(w => w.BowlingAvg).ToList();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BowlerStatsViewHolder vh = holder as BowlerStatsViewHolder;

            vh?.ItemView.SetBackgroundColor(position % 2 == 1
                            ? new Android.Graphics.Color(ContextCompat.GetColor(holder.ItemView.Context, Resource.Color.rowtwo))
                            : new Android.Graphics.Color(ContextCompat.GetColor(holder.ItemView.Context, Resource.Color.rowone)));

            vh.BowlerName.Text = _mPlayers[position].PlayerName;
            vh.Matches.Text = _mPlayers[position].Matches.ToString();
            vh.Balls.Text = _mPlayers[position].BallsBowled.ToString();
            vh.Runs.Text = _mPlayers[position].RunsGiven.ToString();
            vh.Wickets.Text = _mPlayers[position].Wickets.ToString();
            vh.Average.Text = _mPlayers[position].BowlingAvg.ToString();
            vh.Economy.Text = _mPlayers[position].Econ.ToString();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.BowlerStatsRow, parent, false);
            return new BowlerStatsViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            var player = _mPlayers.Where(t => t.TeamName == _mPlayers[position].TeamName &&
                                    t.PlayerName == _mPlayers[position].PlayerName).First();
            var playerdetails = new List<string>
            {
                player.PlayerName,
                $"Team Name : { player.TeamName}",
                $"Matches : { player.Matches}",
                $"Balls Bowled : {player. BallsBowled}",
                $"Maidens : { player.Maiden}",
                $"RunsGiven : {player. RunsGiven}",
                $"Wickets : {player.Wickets}",
                $"Five Wickets : {player.FWI}",
                $"Ten Wickets : {player.TWI}",
                $"Bowling Avg : {player.BowlingAvg}",
                $"Economy : {player.Econ}",
                $"Bowling SR : {player.BowlingSR }",
            };
            ItemClick?.Invoke(this, playerdetails);
        }
    }
    public class BowlerStatsViewHolder : RecyclerView.ViewHolder
    {
        public TextView BowlerName { get; private set; }
        public TextView Matches { get; private set; }
        public TextView Balls { get; private set; }
        public TextView Runs { get; private set; }
        public TextView Wickets { get; private set; }
        public TextView Average { get; private set; }
        public TextView Economy { get; private set; }

        public BowlerStatsViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            BowlerName = itemView.FindViewById<TextView>(Resource.Id.name_bowler);
            Matches = itemView.FindViewById<TextView>(Resource.Id.matches_bowler);
            Balls = itemView.FindViewById<TextView>(Resource.Id.balls_bowler);
            Runs = itemView.FindViewById<TextView>(Resource.Id.runs_bowler);
            Wickets = itemView.FindViewById<TextView>(Resource.Id.wickets_bowler);
            Average = itemView.FindViewById<TextView>(Resource.Id.average_bowler);
            Economy = itemView.FindViewById<TextView>(Resource.Id.economy_bowler);

            itemView.Click += (sender, e) => listener(AdapterPosition);
        }
    }
}