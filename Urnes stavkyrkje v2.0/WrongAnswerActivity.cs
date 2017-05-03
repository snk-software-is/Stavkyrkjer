using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

using Java.Lang;

namespace AndyApp3
{
  [Activity(Label = "Feil svar", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
  public class WrongAnswerActivity : Activity
  {
    private class Counter : CountDownTimer
    {
      TextView txt;
      public event EventHandler Finish;

      public Counter(TextView txt, IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
      {
        this.txt = txt;
      }

      public Counter(TextView txt, long millisInFuture, long countDownInterval)
        : base(millisInFuture, countDownInterval)
      {
        this.txt = txt;
      }

      public override void OnFinish()
      {
        Finish?.Invoke(this, EventArgs.Empty);
      }

      public override void OnTick(long millisUntilFinished)
      {
        txt.Text = $"{millisUntilFinished / 1000}";
      }
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      SetContentView(Resource.Layout.WrongAnswer);

      var wa = FindViewById<RelativeLayout>(Resource.Id.WrongAnswer);
      if (Intent.HasExtra("BgColor"))
      {
        var BgColor = new Color(int.Parse(Intent.GetStringExtra("BgColor"), NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber));
        wa.SetBackgroundColor(BgColor);
      }


      var txt = FindViewById<TextView>(Resource.Id.countdown);
      txt.TextAlignment = TextAlignment.Center;

      var gifName = "hourglass.gif";
      var yourData = "<html style=\"margin: 0;\">\n" +
              "    <body style=\"margin: 0;\">\n" +
              $"    <img src={gifName} style=\"width: 100%; height: 100%\" />\n" +
              "    </body>\n" +
              "    </html>";

      var web = (WebView)FindViewById(Resource.Id.Hourglass);
      web.SetBackgroundColor(Color.Transparent); //for gif without background
      web.LoadUrl("file:///android_asset/htmls/Hourglass.html");
      web.Post(
        () =>
        {
          var cdt = new Counter(txt, 10000, 1000);
          cdt.Finish += Cdt_Finish;
          cdt.Start();
        });
    }

    private void Cdt_Finish(object sender, EventArgs e)
    {
      Finish();
      //var txt = FindViewById<TextView>(Resource.Id.countdown);
      //txt.Visibility = ViewStates.Gone;
      //var b = FindViewById<Button>(Resource.Id.back);
      //b.Visibility = ViewStates.Visible;
      //b.Click += B_Click;
    }

    private void B_Click(object sender, EventArgs e)
    {
      Finish();
    }
  }
}