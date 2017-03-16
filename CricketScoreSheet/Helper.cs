using System;
using Android.Content;
using Android.Views;
using Android.Graphics;
using System.IO;
using File = Java.IO.File;

namespace CricketScoreSheet
{
    public class Helper
    {
        public static string DbPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        public static string DownloadPath
        {
            get
            {
                string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Android", "data",
                     "CricketScoreSheet.CricketScoreSheet", "files");
                return filePath;
            }
        }

        public static bool TakeScreenShot(string filepath, View view)
        {
            bool val = true;
            try
            {
                var bitmap = Bitmap.CreateBitmap(view.DrawingCache);
                var stream = new FileStream(filepath, FileMode.Create);
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                stream.Flush();
                stream.Close();
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public static Intent OpenScreenShot(File file)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);
            var uri = Android.Net.Uri.FromFile(file);
            intent.SetDataAndType(uri, "image/*");
            return intent;
        }
    }
}