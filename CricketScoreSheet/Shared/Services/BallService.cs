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

namespace CricketScoreSheet.Shared.Services
{
    public class BallService
    {
        public CalcBall CalcBall { get; set; }

        public BallService(Ball ball, int teamno)
        {
            CalcBall = new CalcBall(ball);
            if (teamno == 1)
            {
                Access.TeamOneBalls.Add(CalcBall);
            }
            else
            {
                Access.TeamTwoBalls.Add(CalcBall);
            }
        }
    }
}