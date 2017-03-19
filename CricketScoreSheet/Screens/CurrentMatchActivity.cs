using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.Services;
using Newtonsoft.Json;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Java.IO;
using Java.Lang;

namespace CricketScoreSheet.Screens
{
    [Activity(Label = "Current Match", Theme = "@style/MyTheme"
        , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CurrentMatchActivity : AppCompatActivity, RadioGroup.IOnCheckedChangeListener
    {
        private RadioGroup mCurrentInnings;
        private RadioButton mHometeaminnings;
        private RadioButton mAwayteaminnings;

        private Match Match { get; set; }
        private int MatchId { get; set; }
        private int BattingteamId { get; set; }
        private int BowlingteamId { get; set; }
        private Access Access;

        public CurrentMatchActivity()
        {
            Access = new Access();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Match);
            MatchId = Intent.GetIntExtra("MatchId", 1);
            Match = Access.MatchService.GetMatch(MatchId);

            BattingteamId = Intent.GetIntExtra("BattingTeamId", Match.HomeTeam.Id);
            BowlingteamId = Intent.GetIntExtra("BowlingTeamId", Match.AwayTeam.Id);

            // Initialize toolbar
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = (Match.HomeTeam.Id == BattingteamId)
                ? Match.HomeTeam.Name : Match.AwayTeam.Name;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            mCurrentInnings = FindViewById<RadioGroup>(Resource.Id.currentinnings);
            mCurrentInnings.SetOnCheckedChangeListener(this);
            mHometeaminnings = FindViewById<RadioButton>(Resource.Id.hometeaminnings);
            int hb; int ho = System.Math.DivRem(Match.HomeTeam.Balls, 6, out hb);
            mHometeaminnings.Text =
                $"{Match.HomeTeam.Name}\n{Match.HomeTeam.Runs}/{Match.HomeTeam.Wickets}({ho}.{hb}/{Match.TotalOvers})\n" +
                $"Extras(n {Match.HomeTeam.NoBalls}, w {Match.HomeTeam.Wides}, b{Match.HomeTeam.Byes},lb {Match.HomeTeam.LegByes})";
            mAwayteaminnings = FindViewById<RadioButton>(Resource.Id.awayteaminnings);
            int ab; int ao = System.Math.DivRem(Match.AwayTeam.Balls, 6, out ab);
            mAwayteaminnings.Text =
                $"{Match.AwayTeam.Name}\n{Match.AwayTeam.Runs}/{Match.AwayTeam.Wickets}({ao}.{ab}/{Match.TotalOvers})\n" +
                $"Extras(n {Match.AwayTeam.NoBalls}, w {Match.AwayTeam.Wides}, b{Match.AwayTeam.Byes},lb {Match.AwayTeam.LegByes})";

            FragmentManager.BeginTransaction()
                .Add(Resource.Id.BatsmanFrameLayout, new BatsmanFragment())
                .Add(Resource.Id.BowlerFrameLayout, new BowlerFragment())
                .Commit();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Match = Access.MatchService.GetMatch(MatchId);     
        }

        protected override void OnResume()
        {
            base.OnResume();
            ReplaceFragments();
            var scrollview = FindViewById<ScrollView>(Resource.Id.playerslist);
            scrollview.SmoothScrollingEnabled = true;
            scrollview.SmoothScrollTo(0, 0);
        }

        private void ReplaceFragments()
        {
            mCurrentInnings.SetOnCheckedChangeListener(null);
            List<PlayerEntity> batsman;
            List<PlayerEntity> bowlers;
            if (BattingteamId == Match.HomeTeam.Id)
            {
                mHometeaminnings.Checked = true;                
                batsman = Match.HomeTeam.Players;
                bowlers = Match.AwayTeam.Players;                
            }
            else
            {
                mAwayteaminnings.Checked = true;
                batsman = Match.AwayTeam.Players;
                bowlers = Match.HomeTeam.Players;                
            }

            FragmentManager.BeginTransaction()
              .Replace(Resource.Id.BatsmanFrameLayout, new BatsmanFragment(batsman))
              .Replace(Resource.Id.BowlerFrameLayout, new BowlerFragment(bowlers))
              .Commit();            
            mCurrentInnings.SetOnCheckedChangeListener(this);
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            if (checkedId == -1) return;
            switch (checkedId)
            {
                case Resource.Id.hometeaminnings:
                    BattingteamId = Match.HomeTeam.Id;
                    BowlingteamId = Match.AwayTeam.Id;
                    break;
                case Resource.Id.awayteaminnings:
                    BattingteamId = Match.AwayTeam.Id;
                    BowlingteamId = Match.HomeTeam.Id;
                    break;
            }
            ReplaceFragments();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (Match.Complete)
            {
                MenuInflater.Inflate(Resource.Menu.menu_share, menu);
                var shareItem = menu.FindItem(Resource.Id.action_share);
                var mShareActionProvider = (Android.Support.V7.Widget.ShareActionProvider)MenuItemCompat.GetActionProvider(shareItem);
                mShareActionProvider.SetShareIntent(CreateShareIntent());
            }                
            else
            {
                MenuInflater.Inflate(Resource.Menu.menu_score, menu);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        private Intent CreateShareIntent()
        {
            List<IParcelable> sharingFiles = new List<IParcelable>();

            var matchResultHtmlFilePath = System.IO.Path.Combine(Helper.DownloadPath,
                $"{Match.HomeTeam.Name}_{Match.AwayTeam.Name}_{Match.Id}_{Match.Date}".Split(',')[0] + ".html");
            string html = Access.MatchService.CreateHtml(Match);
            if (System.IO.File.Exists(matchResultHtmlFilePath))
                System.IO.File.Delete(matchResultHtmlFilePath);
            System.IO.File.WriteAllText(matchResultHtmlFilePath, html);
            sharingFiles.Add(Android.Net.Uri.FromFile(new File(matchResultHtmlFilePath)));

            var matchResultJsonFilePath = System.IO.Path.Combine(Helper.DownloadPath, 
                $"{Match.HomeTeam.Name}_{Match.AwayTeam.Name}_{Match.Id}_{Match.Date}".Split(',')[0] + ".txt");
            string json = JsonConvert.SerializeObject(Match);
            if (System.IO.File.Exists(matchResultJsonFilePath))
                System.IO.File.Delete(matchResultJsonFilePath);
            System.IO.File.WriteAllText(matchResultJsonFilePath, json);
            sharingFiles.Add(Android.Net.Uri.FromFile(new File(matchResultJsonFilePath)));

            var myShareIntent = new Intent(Intent.ActionSendMultiple);
            myShareIntent.SetType("*/*");
            myShareIntent.PutParcelableArrayListExtra(Intent.ExtraStream, sharingFiles);

            return myShareIntent;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if (Match.Complete)
                    {
                        var mainActivity = new Intent(this, typeof(MainActivity));
                        mainActivity.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        mainActivity.PutExtra("Nav", "matches");
                        StartActivity(mainActivity);                        
                    }
                    else
                    {
                        var matchActivity = new Intent(this, typeof(MatchesActivity));
                        matchActivity.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        StartActivity(matchActivity);
                    }
                    return true;
                case Resource.Id.action_score:
                    Intent intent = new Intent(this.BaseContext, typeof(ScoreActivity));
                    intent.PutExtra("MatchId", Match.Id);
                    intent.PutExtra("BattingTeamId", BattingteamId);
                    intent.PutExtra("BowlingTeamId", BowlingteamId);
                    StartActivity(intent);
                    return true;
                case Resource.Id.action_addplayer:
                    PopupMenu popmenu = new PopupMenu(this, FindViewById(Resource.Id.action_addplayer));
                    popmenu.Inflate(Resource.Menu.menu_addplayer);
                    popmenu.MenuItemClick += Popmenu_MenuItemClick;
                    popmenu.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void Popmenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            Intent intent = new Intent(this.BaseContext, typeof(AddDeletePlayerActivity));
            intent.PutExtra("MatchId", Match.Id);
            intent.PutExtra("BattingTeamId", BattingteamId);
            intent.PutExtra("BowlingTeamId", BowlingteamId);
            switch (e.Item.ItemId)
            {
                case Resource.Id.action_addbatsman:
                    intent.PutExtra("SelectedTeam", "Batting");
                    break;
                case Resource.Id.action_addbowler:
                    intent.PutExtra("SelectedTeam", "Bowling");
                    break;
            }
            StartActivity(intent);
        }
    }
}