using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Adapters;
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.Services;
using System.Collections.Generic;
using System.Linq;

namespace CricketScoreSheet.Screens
{
    public class BowlerStatsFragment : Fragment
    {
        private RecyclerView mRecyclerView;
        private List<PlayerStatistics> Players;
        private BowlerStatsAdapter bowlerAdapter;
        private EditText mSearchText;
        private Access Access;

        public BowlerStatsFragment()
        {
            Access = new Access();
            Players = Access.PlayerService.GetPlayers().GroupBy(x => new { x.TeamId, x.Name })
                        .Select(a => new PlayerStatistics(a.ToList())).ToList();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.BowlerStats, container, false);

            mSearchText = view.FindViewById<EditText>(Resource.Id.searchTextBowler);
            mSearchText.TextChanged += SearchText_TextChanged;
            
            bowlerAdapter = new BowlerStatsAdapter(Players);
            bowlerAdapter.ItemClick += OnItemClick;

            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.bowlerStatsList);
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
            mRecyclerView.SetAdapter(bowlerAdapter);

            var scrollview = view.FindViewById<ScrollView>(Resource.Id.bowlerStatsListScrollView);
            scrollview.SmoothScrollingEnabled = true;
            scrollview.SmoothScrollTo(0, 0);
            return view;
        }

        private void OnItemClick(object sender, List<string> playerdetails)
        {
            var playerdetail = new AlertDialog.Builder(this.Activity);
            playerdetail.SetTitle(playerdetails[0]);
            playerdetails.RemoveAt(0);
            ArrayAdapter adapter = new ArrayAdapter(this.Activity, Android.Resource.Layout.SimpleListItem1, playerdetails);
            playerdetail.SetAdapter(adapter, delegate { });
            playerdetail.SetPositiveButton("Ok",
                    (senderAlert, args) =>
                    {
                        playerdetail.Dispose();
                    });
            playerdetail.Show();
        }

        private void SearchText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            List<PlayerStatistics> searchedBatsman = (from b in Players
                                                      where b.PlayerName.ToLower().Contains(mSearchText.Text.ToLower())
                                                      select b).ToList();
            bowlerAdapter.UpdatedList(searchedBatsman);
            mRecyclerView.SetAdapter(bowlerAdapter);
        }
    }
}