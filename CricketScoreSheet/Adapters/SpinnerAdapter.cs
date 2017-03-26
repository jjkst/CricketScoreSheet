using Android.Content;
using Android.Views;
using Android.Widget;

namespace CricketScoreSheet.Adapters
{
    public class SpinnerAdapter : ArrayAdapter<string>
    {
        string[] objects;

        public SpinnerAdapter(Context context, int textViewResourceId, string[] objects) :
            base(context, textViewResourceId, objects)
        {
            this.objects = objects;
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            return GetCustomView(position, convertView, parent);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return GetCustomView(position, convertView, parent);
        }

        public View GetCustomView(int position, View convertView, ViewGroup parent)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Row, parent, false);
            TextView label = (TextView)itemView.FindViewById(Resource.Id.label);
            label.Text = objects[position];
            label.SetTypeface(null, Android.Graphics.TypefaceStyle.BoldItalic);
            return itemView;
        }    
    }
}