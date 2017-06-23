using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using ZXing;
using ZXing.Mobile;

using Result = Android.App.Result;

namespace UrnesStavkyrkje
{
  [Activity(Label = "@string/CorrectAnswer", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
  public class CorrectAnswer : Activity
  {
    public bool AllCorrect;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      SetContentView(Resource.Layout.CorrectAnswer);

      var wa = FindViewById<RelativeLayout>(Resource.Id.CorrectAnswer);
      var BgColor = new Color(int.Parse(Intent.GetStringExtra("BgColor"), NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber));
      wa.SetBackgroundColor(BgColor);

      var thumbs = Intent.GetIntExtra("Thumbs", -1);
      if (thumbs != -1)
      {
        int thumbImg;
        switch (thumbs)
        {
          case 0:
            thumbImg = Resource.Drawable.ThumbsUpForBlueBGByggjekunst; break;
          case 1:
            thumbImg = Resource.Drawable.ThumbsUpForPurpleBGReligion; break;
          case 2:
            thumbImg = Resource.Drawable.ThumbsUpForGreenBGGeografi; break;
          case 3:
            thumbImg = Resource.Drawable.ThumbsUpForYellowBGUNESCO; break;
          case 4:
            thumbImg = Resource.Drawable.ThumbsUpForPinkBGKultur; break;
          default:
            thumbImg = 0;
            break;
        }
        var t = FindViewById<ImageView>(Resource.Id.Thumbs);
        var d = Resources.GetDrawable(thumbImg);
        t.Background = d;
      }

      var txtClues = FindViewById<LinearLayout>(Resource.Id.Clues);
      var answers = Intent.GetBooleanArrayExtra("Answers");

      var clueView = new ClueView(this, Intent.GetStringArrayExtra("Clues"), answers, BgColor);
      txtClues.AddView(clueView);

      var btn = FindViewById<Button>(Resource.Id.scan);
      AllCorrect = answers.All(a => a);
      if (AllCorrect)
      {
        var stop = Resources.GetDrawable(Resource.Drawable.Stop);
        stop.Bounds = new Rect(0, 0, 60, 60);
        btn.Background = stop;
        btn.Text = "";
        var congrats = FindViewById<TextView>(Resource.Id.AllCorrect);
        congrats.Visibility = ViewStates.Visible;
      }
      else
      {
        var congrats = FindViewById<TextView>(Resource.Id.AllCorrect);
        congrats.Visibility = ViewStates.Gone;

        btn.Text = Resources.GetString(Resource.String.GoToNextGroup) + "  ";
        var nextArrow = Resources.GetDrawable(Resource.Drawable.Next);
        nextArrow.Bounds = new Rect(0, 0, 51, 51);
        btn.SetCompoundDrawables(null, null, nextArrow, null);
      }
      btn.Click += CameraButton_Click; // Btn_Click;


      var edit = FindViewById<EditText>(Resource.Id.editText1);
      edit.EditorAction += Edit_EditorAction;

      var sendCode = FindViewById<Button>(Resource.Id.Send);
      sendCode.Click += SendCode_Click;

      var cancelCode = FindViewById<Button>(Resource.Id.Cancel);
      cancelCode.Click += CancelCode_Click;
    }

    private void CancelCode_Click(object sender, EventArgs e)
    {
      var code = FindViewById<RelativeLayout>(Resource.Id.CodeEntry);
      code.Visibility = ViewStates.Gone;
    }

    private void SendCode_Click(object sender, EventArgs e)
    {
      SendCode();
    }

    void SendCode()
    {
      var edit = FindViewById<EditText>(Resource.Id.editText1);

      var code = Resources.GetString(Resource.String.RestartCode);
      var correctCode = edit.Text.Equals(code);

      if (correctCode)
      {
        SetResult(Result.Ok);
        Finish();
      }
    }

    private void Edit_EditorAction(object sender, TextView.EditorActionEventArgs e)
    {
      if (e.ActionId == ImeAction.Done)
      {
        SendCode();
      }
    }

    async void CameraButton_Click(object sender, EventArgs e)
    {
      if (AllCorrect)
      {
        var code = FindViewById<RelativeLayout>(Resource.Id.CodeEntry);
        code.Visibility = ViewStates.Visible;
      }
      else
      {
        var scanner = new MobileBarcodeScanner();
        var options = new MobileBarcodeScanningOptions
        {
          PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },          
        };
        scanner.BottomText = Resources.GetString(Resource.String.ScanHelp);

        var result = await scanner.Scan(options);

        var nextGroup = result?.Text;
        var data = new Intent();
        data.PutExtra("GroupId", nextGroup);
        SetResult(Result.Canceled, data);
        Finish();
      }
    }

    private void Btn_Click(object sender, EventArgs e)
    {
      if (AllCorrect)
      {
        var code = FindViewById<RelativeLayout>(Resource.Id.CodeEntry);
        code.Visibility = ViewStates.Visible;
      }
      else
      {
        CameraButton_Click(sender, e);
        //SetResult(Result.Canceled);
        //Finish();
      }
    }
  }
}