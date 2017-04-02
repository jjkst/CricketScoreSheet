using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.Services;
using CricketScoreSheet.Shared.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CricketScoreSheet.Screens
{
    public class NewGameDialogFragment : DialogFragment
    {
        private List<string> mTeams = new List<string>();
        private List<string> Locations = new List<string>();
        private List<string> Umpires = new List<string>();
        private string[] mOversArray;

        private Spinner mHomeTeamName;
        private Spinner mAwayTeamName;
        private Spinner mLocation;
        private Spinner mUmpireOne;
        private Spinner mUmpireTwo;
        private Spinner mOvers;
        private Button mCreateMatchBtn;
        private Access Access;

        public NewGameDialogFragment()
        {
            Access = new Access();
            mTeams.AddRange(new List<string> { "Select Team", "Add New Team" });
            if (Access.TeamService.TeamsCount() > 0)
                mTeams.AddRange(Access.TeamService.GetTeams().OrderBy(t => t.Name).Select(n => n.Name));

            Locations.AddRange(new List<string> { "Select Ground/Location", "Add Ground/Location" });
            Locations.AddRange(Access.MatchService.GetMatches().Where(x=> !string.IsNullOrEmpty(x.Location))
                .GroupBy(l => l.Location).Select(lo => lo.First().Location).ToList());

            Umpires.AddRange(new List<string> { "Select Umpire", "Add Umpire" });
            Umpires.AddRange(Access.UmpireService.GetUmpires().Select(u=>u.Name));

            mOversArray = new string[]{ "Ten10", "Twenty20", "ThirtyFive35", "Forty40", "Fifty50", "Custom"};
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.NewGame, container, false);

            var adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, mTeams.ToArray());

            // Set Home Team
            mHomeTeamName = view.FindViewById<Spinner>(Resource.Id.homeTeam);
            mHomeTeamName.Adapter = adapter;
            mHomeTeamName.ItemSelected += setTeam;

            // Set Away Team
            mAwayTeamName = view.FindViewById<Spinner>(Resource.Id.awayTeam);
            mAwayTeamName.Adapter = adapter;
            mAwayTeamName.ItemSelected += setTeam;

            // Set Overs            
            mOvers = view.FindViewById<Spinner>(Resource.Id.overs);
            mOvers.Adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, mOversArray);
            mOvers.ItemSelected += setOvers;

            // Set Location            
            mLocation = view.FindViewById<Spinner>(Resource.Id.location);
            mLocation.Adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, Locations.ToArray());
            mLocation.ItemSelected += setLocation;

            // Set Umpire one            
            mUmpireOne = view.FindViewById<Spinner>(Resource.Id.umpire1);
            mUmpireOne.Adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, Umpires.ToArray());
            mUmpireOne.ItemSelected += setUmpires;

            // Set Umpire two            
            mUmpireTwo = view.FindViewById<Spinner>(Resource.Id.umpire2);
            mUmpireTwo.Adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, Umpires.ToArray());
            mUmpireTwo.ItemSelected += setUmpires;

            // Create Match
            mCreateMatchBtn = view.FindViewById<Button>(Resource.Id.createMatchButton);
            mCreateMatchBtn.Enabled = false;
            mCreateMatchBtn.Click += (object sender, EventArgs e) =>
            {
                var matchId = AddMatch();
                if (matchId == 0) return;
                var currentMatchActivity = new Intent(this.Activity, typeof(CurrentMatchActivity));
                currentMatchActivity.PutExtra("MatchId", matchId);
                StartActivity(currentMatchActivity);
                Fragment prev = (DialogFragment)FragmentManager.FindFragmentByTag("newgame dialog");
                if (prev != null)
                {
                    DialogFragment df = (DialogFragment)prev;
                    df.Dismiss();
                }
            };

            return view;
        }

        private void DisableCreateMatchButtonIf()
        {
            if (mHomeTeamName.SelectedItem.ToString() == "Select Team" ||
                mHomeTeamName.SelectedItem.ToString() == "Add New Team" ||
                mAwayTeamName.SelectedItem.ToString() == "Select Team" ||
                mHomeTeamName.SelectedItem.ToString() == "Add New Team" ||
                mOvers.SelectedItem.ToString() == "Custom")
            {
                mCreateMatchBtn.Enabled = false;
            }
            else if (mHomeTeamName.SelectedItem.ToString() == mAwayTeamName.SelectedItem.ToString())
            {
                mCreateMatchBtn.Enabled = false;
                Toast.MakeText(this.Activity, "Home team and Away team cannot be same.", ToastLength.Long).Show();
            }
            else
            {
                mCreateMatchBtn.Enabled = true;
            }
        }

        private void setTeam(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            DisableCreateMatchButtonIf();
            if (e.Position != 1) return;
            var inputDialog = new AlertDialog.Builder(this.Activity);
            EditText userInput = new EditText(Activity);
            userInput.InputType = InputTypes.ClassText;
            inputDialog.SetTitle("Add New Team:");
            inputDialog.SetView(userInput);
            inputDialog.SetPositiveButton(
                "Ok",
                (senderAlert, args) =>
                {
                    var teamValid = new TeamValidator(Access.TeamService.GetTeams()).Validate(userInput.Text);
                    if (teamValid.Any())
                    {
                        Toast.MakeText(this.Activity, string.Join(System.Environment.NewLine, teamValid.ToArray()), ToastLength.Long).Show();
                        return;
                    }
                    else
                    {
                        var teamId = Access.TeamService.AddTeam(userInput.Text);
                        mTeams.Add(Access.TeamService.GetTeam(teamId).Name);
                        var adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, mTeams.ToArray());
                        if (Resource.Id.homeTeam == e.Parent.Id)
                        {
                            mHomeTeamName.Adapter = adapter;
                            mHomeTeamName.SetSelection(mTeams.Count - 1);
                        }
                        else
                        {
                            mAwayTeamName.Adapter = adapter;
                            mAwayTeamName.SetSelection(mTeams.Count - 1);
                        }
                    }
                });
            inputDialog.SetNegativeButton("Dismiss", (senderAlert, args) => {
                if (Resource.Id.homeTeam == e.Parent.Id) mHomeTeamName.SetSelection(0);
                else mAwayTeamName.SetSelection(0);
            });
            inputDialog.Show();
        }

        private void setUmpires(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            DisableCreateMatchButtonIf();
            if (e.Position != 1) return;
            var inputDialog = new AlertDialog.Builder(this.Activity);
            EditText userInput = new EditText(Activity);
            userInput.InputType = InputTypes.ClassText;
            inputDialog.SetTitle("Add Umpire (Optional):");
            inputDialog.SetView(userInput);
            inputDialog.SetPositiveButton(
                "Ok",
                (senderAlert, args) =>
                {
                    var umpireValid = new UmpireValidator(Access.UmpireService.GetUmpires()).Validate(userInput.Text);
                    if (umpireValid.Any())
                    {                       
                        Toast.MakeText(this.Activity, string.Join(System.Environment.NewLine, umpireValid.ToArray()), ToastLength.Short)
                            .Show();
                        return;
                    }
                    else
                    {
                        Umpires.Add(userInput.Text);
                        var adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, Umpires.ToArray());
                        if (Resource.Id.umpire1 == e.Parent.Id)
                        {
                            mUmpireOne.Adapter = adapter;
                            mUmpireOne.SetSelection(Umpires.Count - 1);
                        }
                        else
                        {
                            mUmpireTwo.Adapter = adapter;
                            mUmpireTwo.SetSelection(Umpires.Count - 1);
                        }
                    }
                });
            inputDialog.SetNegativeButton("Dismiss", (senderAlert, args) =>
            {
                if (Resource.Id.umpire1 == e.Parent.Id) mUmpireOne.SetSelection(0);
                else mUmpireTwo.SetSelection(0);
            });
            inputDialog.Show();
        }

        private void setLocation(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            DisableCreateMatchButtonIf();
            if (e.Position != 1) return;
            var inputDialog = new AlertDialog.Builder(this.Activity);
            EditText userInput = new EditText(Activity);
            userInput.InputType = InputTypes.ClassText;
            inputDialog.SetTitle("Add New Location/Ground (Optional):");
            inputDialog.SetView(userInput);
            inputDialog.SetPositiveButton(
                "Ok",
                (senderAlert, args) =>
                {
                    if (string.IsNullOrEmpty(userInput.Text))
                    {
                        Toast.MakeText(this.Activity, "Location cannot be blank.", ToastLength.Short)
                            .Show();
                        return;
                    }
                    else
                    {
                        Locations.Add(userInput.Text);
                        var adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, Locations.ToArray());
                        mLocation.Adapter = adapter;
                        mLocation.SetSelection(Locations.Count - 1);
                    }
                });
            inputDialog.SetNegativeButton("Dismiss", (senderAlert, args) =>
            {
                mLocation.SetSelection(0);
            });
            inputDialog.Show();
        }

        private void setOvers(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            DisableCreateMatchButtonIf();
            if (e.Position != 5) return;
            var inputDialog = new AlertDialog.Builder(this.Activity);
            EditText userInput = new EditText(Activity);
            userInput.InputType = InputTypes.ClassNumber;
            inputDialog.SetTitle("Enter Number Of Overs:");
            inputDialog.SetView(userInput);
            inputDialog.SetPositiveButton(
                "Ok",
                (senderAlert, args) =>
                {
                    if (userInput.Text != string.Empty)
                    {
                        Array.Resize(ref mOversArray, mOversArray.Length + 1);
                        mOversArray[mOversArray.Length - 1] = userInput.Text;
                        var adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, mOversArray);
                        mOvers.Adapter = adapter;
                        mOvers.SetSelection(mOversArray.Length - 1);
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, "Total Overs cannot be blank.", ToastLength.Short)
                            .Show();
                        return;
                    }
                });
            inputDialog.SetNegativeButton("Dismiss", (senderAlert, args) =>
            {
                mOvers.SetSelection(1);
            });
            inputDialog.Show();
        }

        private int AddMatch()
        {
            var tOvers = 20;
            switch (mOvers.SelectedItem.ToString())
            {
                case "Ten10":
                    tOvers = 10;
                    break;
                case "Twenty20":
                    tOvers = 20;
                    break;
                case "ThirtyFive35":
                    tOvers = 35;
                    break;
                case "Forty40":
                    tOvers = 40;
                    break;
                case "Fifty50":
                    tOvers = 50;
                    break;
                case "Custom":
                    tOvers = 0;
                    break;
                default:
                    tOvers = Convert.ToInt32(mOvers.SelectedItem.ToString());
                    break;
            }

            var teams = Access.TeamService.GetTeams();
            var homeTeam = teams.Where(n => n.Name.ToLower() == mHomeTeamName.SelectedItem.ToString().ToLower()).FirstOrDefault();
            var awayTeam = teams.Where(n => n.Name.ToLower() == mAwayTeamName.SelectedItem.ToString().ToLower()).FirstOrDefault();

            var match = new Match
            {
                TotalOvers = tOvers,
                Location = mLocation.SelectedItem.ToString(),
                UmpireOne = mUmpireOne.SelectedItem.ToString(),
                UmpireTwo = mUmpireTwo.SelectedItem.ToString(),
                HomeTeam = homeTeam == null ? null : Access.TeamService.MapTeamEntityToTeam(homeTeam),
                AwayTeam = awayTeam == null ? null : Access.TeamService.MapTeamEntityToTeam(awayTeam)
            };

            var matchvalid = new MatchValidator().Validate(match);
            if (matchvalid.Any())
            {
                Toast.MakeText(this.Activity, string.Join(System.Environment.NewLine, matchvalid.ToArray()), ToastLength.Long).Show();
                return 0;
            }
            match.Location = (match.Location == @"Select Ground/Location") ? "" : match.Location;
            match.UmpireOne = (match.UmpireOne == @"Select Umpire") ? "" : match.UmpireOne;
            match.UmpireTwo = (match.UmpireTwo == @"Select Umpire") ? "" : match.UmpireTwo;
            var matchId = Access.MatchService.AddMatch(match);
            if (!string.IsNullOrEmpty(match.UmpireOne))
                Access.UmpireService.AddUmpire(matchId, match.UmpireOne, true);
            if (!string.IsNullOrEmpty(match.UmpireTwo))
                Access.UmpireService.AddUmpire(matchId, match.UmpireTwo, string.IsNullOrEmpty(match.UmpireOne) ? true : false);

            //Copy players into new match
            var homeTeamPlayers = Access.PlayerService.GetPlayersPerTeam(homeTeam.Id).GroupBy(hp => hp.Name).Select(hn => hn.First()).ToList();

            //Remove multiple captain and keepers
            if (homeTeamPlayers.Where(n=>n.Name.Contains("*")).Count() > 1)
            {
                for(int i = 1; i < homeTeamPlayers.Count; i++)
                    homeTeamPlayers[i].Name = homeTeamPlayers[i].Name.Trim('*');
            }
            if (homeTeamPlayers.Where(n => n.Name.Contains("†")).Count() > 1)
            {
                for (int i = 1; i < homeTeamPlayers.Count; i++)
                    homeTeamPlayers[i].Name = homeTeamPlayers[i].Name.Trim('†');
            }
            foreach (var hname in homeTeamPlayers)
                Access.PlayerService.AddPlayer(homeTeam.Id, matchId, hname.Name);

            var awayTeamPlayers = Access.PlayerService.GetPlayersPerTeam(awayTeam.Id).GroupBy(ap => ap.Name).Select(an => an.First()).ToList();

            //Remove multiple captain and keepers
            if (awayTeamPlayers.Where(n => n.Name.Contains("*")).Count() > 1)
            {
                for (int i = 1; i < awayTeamPlayers.Count; i++)
                    awayTeamPlayers[i].Name = awayTeamPlayers[i].Name.Trim('*');
            }
            if (awayTeamPlayers.Where(n => n.Name.Contains("†")).Count() > 1)
            {
                for (int i = 1; i < awayTeamPlayers.Count; i++)
                    awayTeamPlayers[i].Name = awayTeamPlayers[i].Name.Trim('†');
            }
            foreach (var aname in awayTeamPlayers)
                Access.PlayerService.AddPlayer(awayTeam.Id, matchId, aname.Name);

            return matchId;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.style_dialogAnimation;
        }
    }
}