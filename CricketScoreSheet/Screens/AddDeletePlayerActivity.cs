using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Shared.Services;
using CricketScoreSheet.Shared.Validation;
using System.Linq;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace CricketScoreSheet.Screens
{
    [Activity(Label = "Add / Delete Players", Theme = "@style/MyTheme")]
    public class AddDeletePlayerActivity : AppCompatActivity
    {
        private CheckBox mCaptain;
        private CheckBox mKeeper;
        private EditText mNewPlayerName;
        private Button mAddNewPlayerBtn;
        private RecyclerView mPlayerNameRecycler;

        private int MatchId { get; set; }
        private int BattingTeamId { get; set; }
        private int BowlingTeamId { get; set; }
        private string SelectedTeam { get; set; }
        private Access Access;

        public AddDeletePlayerActivity()
        {
            Access = new Access();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddDeletePlayer);
            MatchId = Intent.GetIntExtra("MatchId", 1);
            BattingTeamId = Intent.GetIntExtra("BattingTeamId", 1);
            BowlingTeamId = Intent.GetIntExtra("BowlingTeamId", 1);
            SelectedTeam = Intent.GetStringExtra("SelectedTeam");

            // Initialize toolbar
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var teamname = Access.TeamService.GetTeam(SelectedTeam == "Batting" ? BattingTeamId : BowlingTeamId).Name;
            SupportActionBar.Title = teamname + " team players";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            mCaptain = FindViewById<CheckBox>(Resource.Id.captain);
            mKeeper = FindViewById<CheckBox>(Resource.Id.keeper);
            mNewPlayerName = FindViewById<EditText>(Resource.Id.newplayer);
            mAddNewPlayerBtn = FindViewById<Button>(Resource.Id.addnewplayer);
            mAddNewPlayerBtn.Click += AddNewPlayerBtn_Click;

            mPlayerNameRecycler = FindViewById<RecyclerView>(Resource.Id.playernamelist);
            mPlayerNameRecycler.SetLayoutManager(new LinearLayoutManager(this));
        }

        protected override void OnStart()
        {
            base.OnStart();
            GetPlayers_SetAdapter();
        }

        private void AddNewPlayerBtn_Click(object sender, System.EventArgs e)
        {
            var newPlayerName = mNewPlayerName.Text + (mCaptain.Checked ? "*" : "") + (mKeeper.Checked ? "†" : "");
            var valid = new PlayerValidator(Access.PlayerService.GetPlayersPerTeamPerMatch(SelectedTeam == "Batting" ? 
                BattingTeamId : BowlingTeamId, MatchId)).Validate(newPlayerName);
            if (valid.Any())
            {
                Toast.MakeText(this, string.Join(System.Environment.NewLine, valid.ToArray()), ToastLength.Long).Show();
            }
            else
            {
                Access.PlayerService.AddPlayer(SelectedTeam == "Batting" ? BattingTeamId : BowlingTeamId, MatchId, newPlayerName);
                GetPlayers_SetAdapter();
            }
            mNewPlayerName.Text = string.Empty;
            mCaptain.Checked = false;
            mKeeper.Checked = false;
        }

        private void DeletePlayerBtn_ItemClick(object sender, PlayerEntity player)
        {
            Toast.MakeText(this, "Player will be deleted only for this match.", ToastLength.Short).Show();
            Access.PlayerService.DeletePlayer(player);            
            GetPlayers_SetAdapter();
        }

        private void GetPlayers_SetAdapter()
        {
            var players = Access.PlayerService.GetPlayersPerTeamPerMatch(SelectedTeam == "Batting" ? BattingTeamId : BowlingTeamId, MatchId);
            var adapter = new PlayerNameAdapter(players);
            adapter.ItemClick += DeletePlayerBtn_ItemClick;
            mPlayerNameRecycler.SetAdapter(adapter);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    var currentMatchActivity = new Intent(this, typeof(CurrentMatchActivity));
                    currentMatchActivity.PutExtra("MatchId", MatchId);
                    currentMatchActivity.PutExtra("BattingTeamId", BattingTeamId);
                    currentMatchActivity.PutExtra("BowlingTeamId", BowlingTeamId);
                    StartActivity(currentMatchActivity);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}