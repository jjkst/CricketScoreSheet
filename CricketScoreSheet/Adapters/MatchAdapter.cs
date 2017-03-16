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
using CricketScoreSheet.Shared.Models;

namespace CricketScoreSheet.Adapters
{
    public class MatchAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private readonly List<Match> _mMatches;

        public MatchAdapter(List<Match> matches)
        {
            _mMatches = matches;
        }

        public override int ItemCount => _mMatches.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as MatchViewHolder;

            vh?.ItemView.SetBackgroundColor(position % 2 == 1
                ? vh.ItemView.Resources.GetColor(Resource.Color.rowtwo)
                : vh.ItemView.Resources.GetColor(Resource.Color.rowone));

            var match = _mMatches[position];

            int hb; int ho = Math.DivRem(_mMatches[position].HomeTeam.Balls, 6, out hb);
            string hometeamovers = ho + "." + hb;

            int ab; int ao = Math.DivRem(_mMatches[position].AwayTeam.Balls, 6, out ab);
            string awayteamovers = ao + "." + ab;

            if (vh == null) return;
            vh.LineOne.Text = $"{match.Date}, at {match.Location}";
            vh.LineTwo.Text = $"{match.HomeTeam.Name} {match.HomeTeam.Runs}/{match.HomeTeam.Wickets} ({hometeamovers}/{match.TotalOvers}) " +
                              $"Extras (nb {match.HomeTeam.NoBalls}, w {match.HomeTeam.Wides}, b {match.HomeTeam.Byes},lb {match.HomeTeam.LegByes})";
            vh.LineThree.Text = $"{match.AwayTeam.Name} {match.AwayTeam.Runs}/{match.AwayTeam.Wickets} ({awayteamovers}/{match.TotalOvers}) " +
                                $"Extras(nb {match.AwayTeam.NoBalls}, w {match.AwayTeam.Wides}, b {match.AwayTeam.Byes},lb {match.AwayTeam.LegByes})";
            vh.LineFour.Text = match.Complete ? match.Comments : "In Progress";
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MatchRow, parent, false);
            return new MatchViewHolder(itemView, OnClick);
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, _mMatches[position].Id);
        }
    }

    public class MatchViewHolder : RecyclerView.ViewHolder
    {
        public TextView LineOne { get; }
        public TextView LineTwo { get; }
        public TextView LineThree { get; }
        public TextView LineFour { get; }

        public MatchViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            LineOne = itemView.FindViewById<TextView>(Resource.Id.lineone);
            LineTwo = itemView.FindViewById<TextView>(Resource.Id.linetwo);
            LineThree = itemView.FindViewById<TextView>(Resource.Id.linethree);
            LineFour = itemView.FindViewById<TextView>(Resource.Id.linefour);
            itemView.Click += (sender, e) => listener(AdapterPosition);
        }
    }
}