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
    public class PlayerNameAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PlayerEntity> ItemClick;
        private readonly List<PlayerEntity> _mPlayers;

        public PlayerNameAdapter(List<PlayerEntity> matchplayers)
        {
            _mPlayers = matchplayers;
        }

        public override int ItemCount => _mPlayers.Count;

        public override int GetItemViewType(int position)
        {
            return base.GetItemViewType(position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PlayerNameViewHolder vh = holder as PlayerNameViewHolder;

            vh?.ItemView.SetBackgroundColor(position % 2 == 1
                ? vh.ItemView.Resources.GetColor(Resource.Color.rowtwo)
                : vh.ItemView.Resources.GetColor(Resource.Color.rowone));

            vh.PlayerName.Text = _mPlayers[position].Name;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.PlayerNameRow_ViewDelete, parent, false);
            return new PlayerNameViewHolder(itemView, OnClick);
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, _mPlayers[position]);
        }
    }

    public class PlayerNameViewHolder : RecyclerView.ViewHolder
    {
        public TextView PlayerName { get; private set; }
        private Button mDeletePlayer;

        public PlayerNameViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            PlayerName = itemView.FindViewById<TextView>(Resource.Id.textplayername);
            mDeletePlayer = itemView.FindViewById<Button>(Resource.Id.deleteplayer);
            mDeletePlayer.Click += (sender, e) => listener(AdapterPosition);
        }
    }
}