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
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.Services;

namespace CricketScoreSheet.Screens
{
    public class DeclareDialogFragment : DialogFragment
    {
        private Match Match;
        private string[] MatchResults;
        private List<string> TeamNames = new List<string>();

        private Spinner mMatchResult;
        private Spinner mWinner;
        private EditText mComment;
        private Button mDeclareMatch;
        private Access Access;

        public DeclareDialogFragment(Match match)
        {
            Access = new Access();
            this.Match = match;
            TeamNames.Add(Match.HomeTeam.Name);
            TeamNames.Add(Match.AwayTeam.Name);
            MatchResults = new string[] { "Tie", "Declared", "Cancelled" };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DialogDeclare, container, false);

            // Set match result
            mMatchResult = view.FindViewById<Spinner>(Resource.Id.matchresult);
            mMatchResult.Adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, MatchResults);
            mMatchResult.ItemSelected += setMatchResult;

            // Set winning team
            mWinner = view.FindViewById<Spinner>(Resource.Id.winner);
            mWinner.Adapter = new SpinnerAdapter(this.Activity, Resource.Layout.Row, TeamNames.ToArray());
            mWinner.Enabled = false;

            mComment = view.FindViewById<EditText>(Resource.Id.comments);

            // Declare match
            mDeclareMatch = view.FindViewById<Button>(Resource.Id.declarematch);
            mDeclareMatch.Click += (object sender, EventArgs e) =>
            {
                Match.Comments = mComment.Text;
                Match.Complete = true;
                Match.WinningTeamName = mWinner.SelectedItem.ToString();
                Access.MatchService.UpdateMatch(Match);

                var currentMatchActivity = new Intent(this.Activity, typeof(CurrentMatchActivity));
                currentMatchActivity.PutExtra("MatchId", Match.Id);
                StartActivity(currentMatchActivity);
                Fragment prev = (DialogFragment)FragmentManager.FindFragmentByTag("declare dialog");
                if (prev != null)
                {
                    DialogFragment df = (DialogFragment)prev;
                    df.Dismiss();
                }
            };
            return view;
        }

        private void setMatchResult(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position == 1)
            {
                mWinner.Enabled = true;
            }
            else
            {
                mWinner.Enabled = false;
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.style_dialogAnimation;
        }
    }
}