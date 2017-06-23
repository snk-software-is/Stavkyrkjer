using System;
using System.Collections.Generic;
using System.Globalization;

using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace UrnesStavkyrkje
{
  public class QuestionView : RelativeLayout
  {
    public event EventHandler<int> PreviousQuestion;
    public event EventHandler<int> NextQuestion;
    public event EventHandler Back;
    public event EventHandler<int> Submit;

    public string QuestionGroupName;
    public int QuestionNumber;
    public Dictionary<string, QuestionGroup> QuestionGroups;
    public Color BgColor;
    RadioGroup rbGroup;

    public QuestionView(Context context, Dictionary<string, QuestionGroup> questionGroups, string questionGroupName, int questionNumber, int savedAnswer) :
      base(context)
    {
      Initialize(questionGroups, questionGroupName, questionNumber, savedAnswer);
    }

    public QuestionView(Context context, Dictionary<string, QuestionGroup> questionGroups, string questionGroupName, int questionNumber, IAttributeSet attrs, int savedAnswer) :
      base(context, attrs)
    {
      Initialize(questionGroups, questionGroupName, questionNumber, savedAnswer);
    }

    public QuestionView(Context context, Dictionary<string, QuestionGroup> questionGroups, string questionGroupName, int questionNumber, IAttributeSet attrs, int defStyle, int savedAnswer) :
      base(context, attrs, defStyle)
    {
      Initialize(questionGroups, questionGroupName, questionNumber, savedAnswer);
    }

    void Initialize(Dictionary<string, QuestionGroup> questionGroups, string questionGroupName, int questionNumber, int savedAnswer)
    {
      QuestionGroups = questionGroups;
      QuestionGroupName = questionGroupName;
      QuestionNumber = questionNumber;
      BgColor = new Color(int.Parse(QuestionGroups[QuestionGroupName].Background, NumberStyles.AllowHexSpecifier| NumberStyles.HexNumber));

      SetBackgroundColor(BgColor);

      rbGroup = (RadioGroup)LayoutInflater.From(Context).Inflate(Resource.Layout.questionGroup, this, false);

      var lpg = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpg.AddRule(LayoutRules.CenterInParent);
      AddView(rbGroup, lpg);
      
      var question = questionGroups[questionGroupName].Questions[questionNumber];

      var txtQuestionNumber = new TextView(Context)
      {
        Text = $"{QuestionNumber+1} / {QuestionGroups[QuestionGroupName].Questions.Count}",
        TextSize = 16,
        Id = 18        
      };
      txtQuestionNumber.SetPaddingRelative(0,20,0,20);
      txtQuestionNumber.SetTextColor(Color.Black);
      var lpqn = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpqn.AddRule(LayoutRules.AlignParentTop);
      lpqn.AddRule(LayoutRules.CenterHorizontal);
      AddView(txtQuestionNumber, lpqn);

      var txtQuestion = new TextView(Context) { Text = question.Text, TextSize = 20 };
      txtQuestion.SetTextSize(ComplexUnitType.Dip, 26);
      txtQuestion.SetTextColor(Color.Black);
      txtQuestion.SetTypeface(Typeface.DefaultBold, TypefaceStyle.Bold);
      var lpt = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpt.AddRule(LayoutRules.CenterHorizontal);
      lpt.AddRule(LayoutRules.Below, 18);
      AddView(txtQuestion, lpt);

      var answers = question.Answers;

      for (var i = 0; i < answers.Count; i++)
      {
        var rbEntry = (RadioButton)LayoutInflater.From(Context).Inflate(Resource.Layout.questionEntry, this, false);
        rbEntry.Checked = false;
        rbEntry.Id = i;
        rbEntry.Text = $"{answers[i].Text}";
        rbEntry.CheckedChange += RbEntry_CheckedChange;
        rbGroup.AddView(rbEntry);
        if (i == savedAnswer)
          rbEntry.Checked = true;
      }

      var btn = new Button(Context)
      {
        Text = Resources.GetString(Resource.String.SubmitQuestion) + "  ",
        Background = null,
        Visibility = QuestionNumber < QuestionGroups[QuestionGroupName].Questions.Count-1? ViewStates.Gone : ViewStates.Visible
      };
      btn.SetTextSize(ComplexUnitType.Dip, 20);
      btn.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.Submit, 0);
      btn.Touch += Btn_Touch;
      var lpb = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpb.AddRule(LayoutRules.AlignParentRight);
      lpb.AddRule(LayoutRules.CenterVertical);
      AddView(btn,lpb);


      var buttonBar = new RelativeLayout(Context);
      buttonBar.SetBackgroundColor(Color.White);
      var buttonBarLP = new LayoutParams(LayoutParams.MatchParent, 110);
      buttonBarLP.AddRule(LayoutRules.AlignParentBottom);

      AddView(buttonBar, buttonBarLP);

      var btnPrev = new Button(Context)
      {
        Text = "  " + Resources.GetString(Resource.String.PrevQuestion),
        TextSize = 14,
        Background = null,
        Visibility = QuestionNumber == 0 ? ViewStates.Gone : ViewStates.Visible
      };
      btnPrev.SetTextSize(ComplexUnitType.Dip, 20);
      btnPrev.Click += BtnPrev_Click;
      var prevArrow = Resources.GetDrawable(Resource.Drawable.Prev);
      prevArrow.Bounds = new Rect(0, 0, 51, 51);
      btnPrev.SetCompoundDrawables(prevArrow, null, null, null);
      var lpbp = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpbp.AddRule(LayoutRules.AlignParentLeft);
      lpbp.AddRule(LayoutRules.CenterVertical);
      buttonBar.AddView(btnPrev, lpbp);


      var btnNext = new Button(Context)
      {
        Text = Resources.GetString(Resource.String.NextQuestion) + "  ",
        TextSize = 14,
        Background = null,
        Visibility =
          QuestionNumber >= QuestionGroups[QuestionGroupName].Questions.Count - 1 ? ViewStates.Gone : ViewStates.Visible
      };
      btnNext.SetTextSize(ComplexUnitType.Dip, 20);
      btnNext.Click += BtnNext_Click;
      var nextArrow = Resources.GetDrawable(Resource.Drawable.Next);
      nextArrow.Bounds = new Rect(0,0,51,51);
      btnNext.SetCompoundDrawables(null,null, nextArrow, null);
      var lpbn = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpbn.AddRule(LayoutRules.AlignParentRight);
      lpbn.AddRule(LayoutRules.CenterVertical);
      buttonBar.AddView(btnNext, lpbn);

      var btnMenu = new Button(Context)
      {
        Text = Resources.GetString(Resource.String.BackToMainMenu) + "  ",
        TextSize = 14,
        Background = null
      };
      btnMenu.SetTextSize(ComplexUnitType.Dip, 20);
      btnMenu.Click += BtnMenu_Click;
      var lpbb = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
      lpbb.AddRule(LayoutRules.CenterInParent);
      //lpbb.AddRule(LayoutRules.CenterVertical);
      buttonBar.AddView(btnMenu, lpbb);
    }

    private void BtnMenu_Click(object sender, EventArgs e)
    {
      Back?.Invoke(this, EventArgs.Empty);
    }

    private void Btn_Touch(object sender, TouchEventArgs e)
    {
      var btn = sender as Button;
      switch (e.Event.Action)
      {
        case MotionEventActions.Down:
          //btn.CompoundDrawableTintMode = PorterDuff.Mode.Multiply;
          //btn.CompoundDrawableTintList = new ColorStateList(
          //  new[] { StateSet.WildCard.ToArray() },
          //  new[] { new Color(BgColor.R - 30, BgColor.G - 30, BgColor.B - 30, (byte)200).ToArgb() });
          break;
        case MotionEventActions.Up:
          //btn.CompoundDrawableTintMode = PorterDuff.Mode.Multiply;
          //btn.CompoundDrawableTintList = new ColorStateList(
          //  new[] { StateSet.WildCard.ToArray() },
          //  new[] { Color.White.ToArgb() });
          Submit?.Invoke(this, rbGroup.CheckedRadioButtonId);
          break;
      }
    }

    private void RbEntry_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
    {
      var rbe = sender as RadioButton;
      rbe.Background =
        Resources.GetDrawable(rbe.Checked ? Resource.Drawable.ButtonBackgroundCheckedGrey : Resource.Drawable.ButtonBackground);
    }

    private void BtnNext_Click(object sender, EventArgs e)
    {
      NextQuestion?.Invoke(this, rbGroup.CheckedRadioButtonId);
    }

    private void BtnPrev_Click(object sender, EventArgs e)
    {
      if(QuestionNumber>0) PreviousQuestion?.Invoke(this, rbGroup.CheckedRadioButtonId);
      else Back?.Invoke(this, EventArgs.Empty);
    }
  }
}