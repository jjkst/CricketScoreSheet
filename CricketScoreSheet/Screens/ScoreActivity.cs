using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.Services;
using CricketScoreSheet.Shared.Validation;
using System;
using System.Linq;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace CricketScoreSheet.Screens
{
    [Activity(Label = "Score", Theme = "@style/MyTheme"
        , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ScoreActivity : AppCompatActivity, RadioGroup.IOnCheckedChangeListener
    {
        private Spinner mActiveBatsman;
        private Spinner mActiveBowler;
        private Spinner mFielder_Keeper;

        private RadioGroup mRunsWickets1;
        private RadioGroup mRunsWickets2;
        private RadioGroup mRunsWickets3;
        private RadioGroup mRunsWickets4;

        private RadioGroup mExtras1;
        private Spinner mExtra1Runs;
        private RadioGroup mExtras2;
        private Spinner mExtra2Runs;

        private Spinner RunoutRuns;
        private RadioGroup RunnerOut;
        private Spinner RunnerBatsman;

        private int MatchId;
        private Match Match;
        private int TeamId;
        private int BattingteamId;
        private int BowlingteamId;
        private int PlayerId;
        private string Play;
        private Ball ThisBall;
        private Access Access;

        public ScoreActivity()
        {
            Access = new Access();
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Score);

            // Initialize toolbar
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            MatchId = Intent.GetIntExtra("MatchId", 1);
            TeamId = Intent.GetIntExtra("TeamId", 1);
            PlayerId = Intent.GetIntExtra("PlayerId", 1);
            Play = Intent.GetStringExtra("Play");                  
            ThisBall = new Ball();

            // Active Players
            mActiveBatsman = FindViewById<Spinner>(Resource.Id.activeBatsman);
            mActiveBatsman.ItemSelected += ActiveBatsman_ItemSelected;
            mActiveBowler = FindViewById<Spinner>(Resource.Id.activeBowler);
            mActiveBowler.ItemSelected += ActiveBowler_ItemSelected;
            mFielder_Keeper = FindViewById<Spinner>(Resource.Id.fielder);
            mFielder_Keeper.ItemSelected += Fielder_Keeper_ItemSelected;

            

            // Score and wickets
            mRunsWickets1 = FindViewById<RadioGroup>(Resource.Id.runswickets1);
            mRunsWickets2 = FindViewById<RadioGroup>(Resource.Id.runswickets2);
            mRunsWickets3 = FindViewById<RadioGroup>(Resource.Id.runswickets3);
            mRunsWickets4 = FindViewById<RadioGroup>(Resource.Id.runswickets4);
            mRunsWickets1.Check(Resource.Id.dot);
            mRunsWickets1.SetOnCheckedChangeListener(this);
            mRunsWickets2.SetOnCheckedChangeListener(this);
            mRunsWickets3.SetOnCheckedChangeListener(this);
            mRunsWickets4.SetOnCheckedChangeListener(this);

            // Extras
            ArrayAdapter extrasAdapter = new SpinnerAdapter(this, Resource.Layout.Row, new[] { "0", "1", "2", "3", "4" });
            mExtras1 = FindViewById<RadioGroup>(Resource.Id.extras1);
            mExtras1.SetOnCheckedChangeListener(this);
            mExtra1Runs = FindViewById<Spinner>(Resource.Id.extras_one_runs);
            mExtra1Runs.Adapter = extrasAdapter;
            mExtra1Runs.Enabled = false;

            mExtras2 = FindViewById<RadioGroup>(Resource.Id.extras2);
            mExtras2.SetOnCheckedChangeListener(this);
            mExtra2Runs = FindViewById<Spinner>(Resource.Id.extras_two_runs);
            mExtra2Runs.Adapter = extrasAdapter;
            mExtra2Runs.Enabled = false;

            //Buttons
            var mClear = FindViewById<Button>(Resource.Id.clear);
            mClear.Click += ClearScore;
            var mUpdate = FindViewById<Button>(Resource.Id.updateScore);
            mUpdate.Click += UpdateScore;
            var mDeclare = FindViewById<Button>(Resource.Id.declare);
            mDeclare.Click += DeclareGame;
            var mUndo = FindViewById<Button>(Resource.Id.undo);
            mUndo.Click += UndoScore;
        }

        protected override void OnStart()
        {
            base.OnStart();
            Match = Access.MatchService.GetMatch(MatchId);
            Team battingTeam;
            Team bowlingTeam;

            if ((Play == "Batsman" && Match.HomeTeam.Id == TeamId) 
                || (Play == "Bowler" && Match.AwayTeam.Id == TeamId))
            {
                BattingteamId = Match.HomeTeam.Id;
                BowlingteamId = Match.AwayTeam.Id;
                battingTeam = Match.HomeTeam;
                bowlingTeam = Match.AwayTeam;
            }
            else 
            {
                BattingteamId = Match.AwayTeam.Id;
                BowlingteamId = Match.HomeTeam.Id;
                battingTeam = Match.AwayTeam;
                bowlingTeam = Match.HomeTeam;
            }

            SupportActionBar.Title = battingTeam.Name + " Innings";

            var batsman = battingTeam.Players.Where(o => o.HowOut == "not out").Select(p => new {p.Id, p.Name});
            var batsmanAdapter = new SpinnerAdapter(this, Resource.Layout.Row, batsman.Select(x=>x.Name).ToArray());
            mActiveBatsman.Adapter = batsmanAdapter;
            if (Play == "Batsman") 
                mActiveBatsman.SetSelection(Array.IndexOf(batsman.Select(i=>i.Id).ToArray(), PlayerId));

            var bowler = bowlingTeam.Players.Select(b => new { b.Id, b.Name });
            var bowlerAdapter = new SpinnerAdapter(this, Resource.Layout.Row, bowler.Select(x => x.Name).ToArray());
            mActiveBowler.Adapter = bowlerAdapter;
            if (Play == "Bowler")
                mActiveBowler.SetSelection(Array.IndexOf(bowler.Select(i => i.Id).ToArray(), PlayerId));

            mFielder_Keeper.Adapter = bowlerAdapter;
        }

        private void ActiveBatsman_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ThisBall.ActiveBatsman = mActiveBatsman.SelectedItem.ToString();
        }

        private void ActiveBowler_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ThisBall.ActiveBowler = mActiveBowler.SelectedItem.ToString();
            RadioButton bowled = FindViewById<RadioButton>(Resource.Id.bowled);
            RadioButton caught = FindViewById<RadioButton>(Resource.Id.caught);
            RadioButton lbw = FindViewById<RadioButton>(Resource.Id.lbw);
            RadioButton stumped = FindViewById<RadioButton>(Resource.Id.stumped);
            RadioButton hitout = FindViewById<RadioButton>(Resource.Id.hitout);
            if (bowled.Checked) ThisBall.HowOut = ThisBall.HowOut = $"b {ThisBall.ActiveBowler}";
            if (caught.Checked) ThisBall.HowOut = $"c {ThisBall.Fielder} b {ThisBall.ActiveBowler}";
            if (lbw.Checked) ThisBall.HowOut = ThisBall.HowOut = $"lbw {ThisBall.ActiveBowler}";
            if (stumped.Checked) ThisBall.HowOut = $"st †{ThisBall.Fielder} b {ThisBall.ActiveBowler}";
            if (hitout.Checked) ThisBall.HowOut = $"hit wicket {ThisBall.ActiveBowler}";
        }

        private void Fielder_Keeper_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ThisBall.Fielder = mFielder_Keeper.SelectedItem.ToString();
            RadioButton caught = FindViewById<RadioButton>(Resource.Id.caught);
            RadioButton runout = FindViewById<RadioButton>(Resource.Id.runout);
            RadioButton stumped = FindViewById<RadioButton>(Resource.Id.stumped);
            if (caught.Checked) ThisBall.HowOut = $"c {ThisBall.Fielder} b {ThisBall.ActiveBowler}";
            if (runout.Checked)
            {
                ThisBall.HowOut = ThisBall.HowOut.Contains("runout") ? $"runout({ThisBall.Fielder})" : "not out";
                if(!string.IsNullOrEmpty(ThisBall.RunnerHowOut))
                    ThisBall.RunnerHowOut = ThisBall.RunnerHowOut.Contains("runout") ? 
                        $"runout({ThisBall.Fielder})" : "not out";
            }
            if (stumped.Checked) ThisBall.HowOut = $"st †{ThisBall.Fielder} b {ThisBall.ActiveBowler}";
        }

        private void ResetRadioGroup(RadioGroup radioGroup)
        {
            radioGroup.SetOnCheckedChangeListener(null);
            radioGroup.ClearCheck();
            radioGroup.SetOnCheckedChangeListener(this);
        }

        private void ClearOtherRadioGroup(int id)
        {
            switch (id)
            {
                case Resource.Id.runswickets1:
                    ResetRadioGroup(mRunsWickets2);
                    ResetRadioGroup(mRunsWickets3);
                    ResetRadioGroup(mRunsWickets4);
                    break;
                case Resource.Id.runswickets2:
                    ResetRadioGroup(mRunsWickets1);
                    ResetRadioGroup(mRunsWickets3);
                    ResetRadioGroup(mRunsWickets4);
                    break;
                case Resource.Id.runswickets3:
                    ResetRadioGroup(mRunsWickets2);
                    ResetRadioGroup(mRunsWickets1);
                    ResetRadioGroup(mRunsWickets4);
                    break;
                case Resource.Id.runswickets4:
                    ResetRadioGroup(mRunsWickets2);
                    ResetRadioGroup(mRunsWickets3);
                    ResetRadioGroup(mRunsWickets1);
                    break;
            }
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {            
            switch (@group.Id)
            {
                case Resource.Id.extras1:
                    @group.SetOnCheckedChangeListener(null);
                    SetExtra1(checkedId);
                    @group.SetOnCheckedChangeListener(this);
                    break;
                case Resource.Id.extras2:
                    @group.SetOnCheckedChangeListener(null);
                    SetExtra2(checkedId);
                    @group.SetOnCheckedChangeListener(this);
                    break;
                default:
                    ClearOtherRadioGroup(group.Id);
                    mFielder_Keeper.Enabled = false;
                    SetScoreWicket(checkedId);
                    break;
            }
            
        }

        private void SetExtra1(int checkedId)
        {
            var extra1 = new BallValidator(ThisBall);
            switch (checkedId)
            {
                case Resource.Id.wide:
                    ThisBall.NoBall = 0;
                    var widevalid = extra1.Validate("wide");
                    if (widevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.wide).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, widevalid.ToArray()), ToastLength.Long).Show();
                    }                        
                    else ThisBall.Wide = 1;             
                    break;
                case Resource.Id.noball:
                    ThisBall.Wide = 0;
                    var noballvalid = extra1.Validate("noball");
                    if (noballvalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.noball).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, noballvalid.ToArray()), ToastLength.Long).Show();
                    }                        
                    else ThisBall.NoBall = 1;
                    break;
            }
            if (ThisBall.Wide == 1 || ThisBall.NoBall == 1)
            {
                mExtra1Runs.SetSelection(1);
                mExtra1Runs.Enabled = true;                
            }
            else
            {
                mExtra1Runs.SetSelection(0);
                mExtra1Runs.Enabled = false;
            }
        }

        private void SetExtra2(int checkedId)
        {
            var extra2 = new BallValidator(ThisBall);
            switch (checkedId)
            {
                case Resource.Id.bye:
                    ThisBall.LegByes = 0;
                    var byesvalid = extra2.Validate("byes");
                    if (byesvalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.bye).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, byesvalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.Byes = 1; 
                    
                    break;
                case Resource.Id.legbye:
                    ThisBall.Byes = 0;
                    var legbyesvalid = extra2.Validate("legbyes");
                    if (legbyesvalid.Any())
                    {                        
                        FindViewById<RadioButton>(Resource.Id.legbye).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, legbyesvalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.LegByes = 1;

                    break;
            }
            if (ThisBall.Byes == 1 || ThisBall.LegByes == 1)
            {
                mExtra2Runs.SetSelection(1);
                mExtra2Runs.Enabled = true;
            }
            else
            {
                mExtra2Runs.SetSelection(0);
                mExtra2Runs.Enabled = false;
            }
        }

        private void SetScoreWicket(int checkedId)
        {
            var scorewicket = new BallValidator(ThisBall);

            var scorevalid = scorewicket.Validate("runs");
            var wicket1valid = scorewicket.Validate("out");
            var wicket2valid = scorewicket.Validate("xout");
            switch (checkedId)
            {
                case Resource.Id.dot:
                    ThisBall.HowOut = "not out";
                    ThisBall.RunsScored = 0;                    
                    break;
                case Resource.Id.one:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.one).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 1;
                    break;
                case Resource.Id.two:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.two).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 2;
                    break;
                case Resource.Id.three:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.three).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 3;
                    break;
                case Resource.Id.four:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.four).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 4;
                    break;
                case Resource.Id.five:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.five).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 5;
                    break;
                case Resource.Id.six:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.six).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 6;
                    break;
                case Resource.Id.seven:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.seven).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 7;
                    break;
                case Resource.Id.eight:
                    ThisBall.HowOut = "not out";
                    if (scorevalid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.eight).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, scorevalid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.RunsScored = 8;
                    break;
                case Resource.Id.bowled:
                    ThisBall.RunsScored = 0;
                    if (wicket1valid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.bowled).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, wicket1valid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.HowOut = $"b {ThisBall.ActiveBowler}";                    
                    break;
                case Resource.Id.caught:
                    ThisBall.RunsScored = 0;
                    if (wicket1valid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.caught).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, wicket1valid.ToArray()), ToastLength.Long).Show();
                    }
                    else
                    {
                        ThisBall.HowOut = $"c {ThisBall.Fielder} b {ThisBall.ActiveBowler}";
                        mFielder_Keeper.Enabled = true;
                    }
                    break;
                case Resource.Id.lbw:
                    ThisBall.RunsScored = 0;
                    if (wicket1valid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.lbw).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, wicket1valid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.HowOut = $"lbw {ThisBall.ActiveBowler}";
                    break;
                case Resource.Id.runout:
                    mFielder_Keeper.Enabled = true;
                    ThisBall.RunsScored = 0;
                    ThisBall.HowOut = $"runout({ThisBall.Fielder})";
                    ShowRunoutDialog();
                    break;
                case Resource.Id.stumped:
                    ThisBall.RunsScored = 0;
                    if (wicket2valid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.stumped).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, wicket2valid.ToArray()), ToastLength.Long).Show();
                    }
                    else
                    {
                        ThisBall.HowOut = $"st †{ThisBall.Fielder} b {ThisBall.ActiveBowler}";
                        mFielder_Keeper.Enabled = true;
                    }
                    break;
                case Resource.Id.hitout:
                    ThisBall.RunsScored = 0;
                    if (wicket2valid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.hitout).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, wicket2valid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.HowOut = $"hit wicket {ThisBall.ActiveBowler}";
                    break;
                case Resource.Id.retried:
                    ThisBall.RunsScored = 0;
                    if (wicket1valid.Any())
                    {
                        FindViewById<RadioButton>(Resource.Id.hitout).Checked = false;
                        Toast.MakeText(this, string.Join(System.Environment.NewLine, wicket1valid.ToArray()), ToastLength.Long).Show();
                    }
                    else ThisBall.HowOut = "retired";
                    break;
            }
        }

        private void ShowRunoutDialog()
        {
            ArrayAdapter extrasAdapter = new SpinnerAdapter(this, Resource.Layout.Row, new[] { "0", "1", "2", "3", "4" });
            var batsman = Access.PlayerService.GetPlayersPerTeamPerMatch(BattingteamId, MatchId);
            var batsmanAdapter = new SpinnerAdapter(this, Resource.Layout.Row,
            batsman.Where(o => o.HowOut == "not out").Select(p => p.Name).ToArray());

            var runoutDialog = new Android.App.AlertDialog.Builder(this);
            View runoutdialoglayout = LayoutInflater.Inflate(Resource.Layout.DialogRunout, null);
            RunoutRuns = runoutdialoglayout.FindViewById<Spinner>(Resource.Id.runoutruns);
            RunoutRuns.Adapter = extrasAdapter;
            RunnerOut = runoutdialoglayout.FindViewById<RadioGroup>(Resource.Id.runnerout);
            RunnerOut.CheckedChange += Runnerout_CheckedChange;
            RunnerBatsman = runoutdialoglayout.FindViewById<Spinner>(Resource.Id.runnerbatsman);
            RunnerBatsman.Adapter = batsmanAdapter;
            RunnerOut.Check(Resource.Id.norunnerout);

            runoutDialog.SetView(runoutdialoglayout);
            runoutDialog.SetPositiveButton("Ok",
                    (senderAlert, args) =>
                    {
                        runoutDialog.Dispose();
                        ThisBall.RunsScored = Convert.ToInt16(RunoutRuns.SelectedItem.ToString());                        
                        if (RunnerBatsman.Enabled)
                        {
                            ThisBall.RunnerBatsman = RunnerBatsman.SelectedItem.ToString();
                            ThisBall.RunnerHowOut = $"runout({ThisBall.Fielder})";
                            ThisBall.HowOut = "not out";
                        }
                        else
                        {
                            ThisBall.HowOut = $"runout({ThisBall.Fielder})";
                        }

                    });
            runoutDialog.Show();
        }

        private void Runnerout_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            switch (e.CheckedId)
            {
                case Resource.Id.yesrunnerout:
                    RunnerBatsman.Enabled = true;
                    break;
                case Resource.Id.norunnerout:
                    RunnerBatsman.Enabled = false;
                    break;
            }
        }

        private void MatchComplete(string comments)
        {
            var matchComplete = new Android.App.AlertDialog.Builder(this);
            matchComplete.SetTitle("Match is Complete.");
            matchComplete.SetMessage(comments);
            matchComplete.SetPositiveButton("Ok",
                    (senderAlert, args) =>
                    {
                        matchComplete.Dispose();
                        GotoCurrentMatch();
                    });
            matchComplete.Show();
        }

        private void GotoCurrentMatch()
        {
            var currentMatchActivity = new Intent(this, typeof(CurrentMatchActivity));
            currentMatchActivity.PutExtra("MatchId", MatchId);
            currentMatchActivity.PutExtra("BattingTeamId", BattingteamId);
            currentMatchActivity.PutExtra("BowlingTeamId", BowlingteamId);
            StartActivity(currentMatchActivity);
        }

        private void ClearScore(object sender, EventArgs e)
        {
            ThisBall = new Ball();
            mRunsWickets1.Check(Resource.Id.dot);
            ResetRadioGroup(mExtras1);
            ResetRadioGroup(mExtras2);
            mExtra1Runs.SetSelection(0);
            mExtra1Runs.Enabled = false;
            mExtra2Runs.SetSelection(0);
            mExtra2Runs.Enabled = false;
        }

        private void UpdateScore(object sender, EventArgs e)
        {
            if (!mFielder_Keeper.Enabled) ThisBall.Fielder = null;

            //TODO : Handle Runout

            // Extras one
            if (ThisBall.Wide == 1)
            {
                ThisBall.Wide = Convert.ToInt32(mExtra1Runs.SelectedItem.ToString());
            }
            else if (ThisBall.NoBall == 1)
            {
                ThisBall.NoBall = Convert.ToInt32(mExtra1Runs.SelectedItem.ToString());
            }

            // Extras two
            if (ThisBall.Byes == 1)
            {
                ThisBall.Byes = Convert.ToInt32(mExtra2Runs.SelectedItem.ToString());
            }
            else if (ThisBall.LegByes == 1)
            {
                ThisBall.LegByes = Convert.ToInt32(mExtra2Runs.SelectedItem.ToString());
            }

            var calcball = new CalcBall(ThisBall);
            var team = (Match.HomeTeam.Id == BattingteamId) ? 1 : 2;                    
            if (team == 1)
            {
                Access.TeamOneBalls.Add(calcball);
            }
            else
            {
                Access.TeamTwoBalls.Add(calcball);
            }

            //Update score      
            if(Access.PlayerService.UpdateBatsmanThisBall(MatchId, BattingteamId, calcball)
                && Access.PlayerService.UpdateBowlerThisBall(MatchId, BowlingteamId, calcball))
                Access.MatchService.UpdateTeamScore(MatchId, BattingteamId, calcball);

            Match = Access.MatchService.GetMatch(MatchId);
            // Current innings is complete
            if ((Match.HomeTeam.Id == BattingteamId && Match.HomeTeam.InningsComplete)
                || (Match.AwayTeam.Id == BattingteamId && Match.AwayTeam.InningsComplete))
            {
                var currentInningsComplete = new Android.App.AlertDialog.Builder(this);
                currentInningsComplete.SetTitle("Current Innings is Complete.");
                var teamname = Match.HomeTeam.Id == BattingteamId ? Match.HomeTeam.Name : Match.AwayTeam.Name;
                var runs = Match.HomeTeam.Id == BattingteamId ? Match.HomeTeam.Runs : Match.AwayTeam.Runs;
                var wicktes = Match.HomeTeam.Id == BattingteamId ? Match.HomeTeam.Wickets : Match.AwayTeam.Wickets;
                currentInningsComplete.SetMessage($"{teamname} is scored {runs} for {wicktes} wickets.");
                currentInningsComplete.SetPositiveButton("Ok",
                        (senderAlert, args) =>
                        {
                            currentInningsComplete.Dispose();
                            if (Match.Complete) MatchComplete($"Congratulations, {Match.Comments}");
                            else GotoCurrentMatch();
                        });
                currentInningsComplete.Show();
            }
            // Match is complete
            else if (Match.Complete)
            {
                MatchComplete($"Congratulations, { Match.Comments}");
            }
            // One or less wickets left. Press 'Complete' to match complete or 'No, Continue' to add new batsman.
            else if (Access.PlayerService.GetPlayersPerTeamPerMatch(BattingteamId, Match.Id).Where(o => o.HowOut == "not out").Count() <= 1)
            {
                var wicketsgone = new Android.App.AlertDialog.Builder(this);
                wicketsgone.SetTitle("All Out");
                wicketsgone.SetMessage("One or less wickets left. Press 'Innings Over' if current innings is complete, 'No, Continue' to add new batsman.");
                wicketsgone.SetPositiveButton("Innings Over", (senderAlert, args) => 
                {
                    wicketsgone.Dispose();
                    if (Match.HomeTeam.Id == BattingteamId)
                        Match.HomeTeam.InningsComplete = true;
                    else Match.AwayTeam.InningsComplete = true;
                    if (Match.HomeTeam.InningsComplete && Match.AwayTeam.InningsComplete)
                        Match.Complete = true;
                    Access.MatchService.UpdateMatch(Match);                    
                    GotoCurrentMatch();
                });
                wicketsgone.SetNegativeButton("No, Continue", (senderAlert, args) => {
                    wicketsgone.Dispose();
                    Intent intent = new Intent(this.BaseContext, typeof(AddDeletePlayerActivity));
                    intent.PutExtra("MatchId", MatchId);
                    intent.PutExtra("BattingTeamId", BattingteamId);
                    intent.PutExtra("BowlingTeamId", BowlingteamId);
                    intent.PutExtra("SelectedTeam", "Batting");
                    StartActivity(intent);
                });
                wicketsgone.Show();
            }
            else
            {
                GotoCurrentMatch();
            }
        }

        private void UndoScore(object sender, EventArgs e)
        {
            int team = 1;
            if ( Access.MatchService.GetMatch(MatchId).AwayTeam.Id == BattingteamId) team = 2;
            CalcBall undoBall = null;
            if (team == 1 && Access.TeamOneBalls.Count > 0)
            {
                undoBall = Access.TeamOneBalls[Access.TeamOneBalls.Count - 1];
                Access.TeamOneBalls.RemoveAt(Access.TeamOneBalls.Count - 1);
            }
            else if (team == 2 && Access.TeamTwoBalls.Count > 0)
            {
                undoBall = Access.TeamTwoBalls[Access.TeamTwoBalls.Count - 1];
                Access.TeamTwoBalls.RemoveAt(Access.TeamTwoBalls.Count - 1);
            }
            if(Access.PlayerService.UndoBatsmanLastBall(MatchId, BattingteamId, undoBall)
                && Access.PlayerService.UndoBowlerLastBall(MatchId, BowlingteamId, undoBall))
                Access.MatchService.UndoTeamScore(MatchId, BattingteamId, undoBall);
            GotoCurrentMatch();
        }

        private void DeclareGame(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            DeclareDialogFragment declareDialog = new DeclareDialogFragment(Match);
            declareDialog.SetStyle(DialogFragmentStyle.NoTitle, 0);
            declareDialog.Show(transaction, "declare dialog");
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}