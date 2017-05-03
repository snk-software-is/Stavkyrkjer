using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using ZXing;
using ZXing.Mobile;

using Result = Android.App.Result;

namespace AndyApp3
{
  [Activity(Label = "Velg post", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
  public class GroupSelector : Activity
  {
    IList<string> groups;
    IList<bool> answers;
    IList<string> colors;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      SetContentView(Resource.Layout.MainMenu);
      
      var menu = FindViewById<RelativeLayout>(Resource.Id.menu);

      groups = Intent.GetStringArrayExtra("Groups");
      answers = Intent.GetBooleanArrayExtra("Answers");
      colors = Intent.GetStringArrayExtra("Colors");

      var id = 21;
      for (var index = 0; index < groups.Count; index++)
      {
        var @group = groups[index];
        var background = Resources.GetDrawable(Resource.Drawable.ButtonBackground).Mutate();
        if (answers[index])
        {
          background.SetAlpha(50);
        }
        else
        {
          var color = new Color(int.Parse(colors[index], NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber));
          background.SetColorFilter(color, PorterDuff.Mode.Multiply);
        }
        var btn = new Button(this)
        {
          Id = ++id,
          Text = @group,
          Background = background,
          TextSize = 17,
          Enabled = !answers[index]
        };
        btn.SetTextSize(ComplexUnitType.Dip, 26);
        btn.SetTextColor(Color.Black);
        
        btn.Click += Btn_Click;

        var lp = new RelativeLayout.LayoutParams(305, 95);
        if (id <= 22)
          lp.AddRule(LayoutRules.AlignParentTop);
        else
          lp.AddRule(LayoutRules.Below, id - 1);

        menu.AddView(btn, lp);
        var divider = new RelativeLayout(this) { Id = ++id };
        var lpd = new RelativeLayout.LayoutParams(-2, 20);
        lpd.AddRule(LayoutRules.Below, id - 1);
        menu.AddView(divider, lpd);
      }

      var click = FindViewById<Button>(Resource.Id.scan);
      click.Click += Click_Click;

    }

    async void Click_Click(object sender, EventArgs e)
    {
      var scanner = new MobileBarcodeScanner();
      var options = new MobileBarcodeScanningOptions
      {
        PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }

      };
      var result = await scanner.Scan(options);

      if (result != null)
      {
        FindViewById<TextView>(Resource.Id.MainMenuInfo).Text = "Scanned Barcode: " + result.Text;
      }
    }

    private void Btn_Click(object sender, EventArgs e)
    {
      var btn = sender as Button;
      var groupId = (btn.Id - 22) / 2;

      var data = new Intent();
      data.PutExtra("GroupId", groups[groupId]);
      SetResult(Result.Ok, data);
      Finish();
    }
  }
}