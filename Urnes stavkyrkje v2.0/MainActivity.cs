using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

using Javax.Xml.Parsers;

using Org.Xml.Sax;

using ZXing.Mobile;

using Result = Android.App.Result;

namespace UrnesStavkyrkje
{
  [Activity(Label = "Urnes stavkyrkje", MainLauncher = true, Icon = "@drawable/icon2", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen", ScreenOrientation = ScreenOrientation.Landscape)]
  public class MainActivity : Activity
  {
    int questionNumber;
    Dictionary<string, QuestionGroup> questionGroups;
    Dictionary<string, int[]> answers = new Dictionary<string, int[]>();

    string currentQuestionGroup;
    bool splashClicked;

    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      var splash = new RelativeLayout(this);
      var img = new Button(this) { Background = Resources.GetDrawable(Resource.Drawable.Urnes) };
      img.Click += DismissSplash_Click;
      
      splash.AddView(img, -1,-1);
      AddContentView(splash, new RelativeLayout.LayoutParams(-1,-1));

      splash.PostDelayed(
        () =>
        {
          if (!splashClicked) ShowMenu();
        },
        3000);

      questionGroups = GetQuestionGroups(this);

      MobileBarcodeScanner.Initialize(Application);
      
    }

    public override void OnBackPressed()
    {
      ShowMenu();
    }

    void DismissSplash_Click(object sender, EventArgs e)
    {
      splashClicked = true;
      ShowMenu();
    }

    void ShowMenu()
    {

      var i = new Intent(this, typeof(ScanGroup));
      i.PutExtra("BgColor", "FFC0D6E8");
      i.PutExtra("Thumbs", -1);

      var clues = GetCluesAndAnswers();
      i.PutExtra("Clues", clues.Values.Select(t => t.Item1).ToArray());
      i.PutExtra("Answers", clues.Values.Select(t => t.Item2).ToArray());

      StartActivityForResult(i, 3);

      /*


      var i = new Intent(this, typeof(GroupSelector));

      i.PutExtra("Groups", questionGroups.Keys.ToArray());
      var clues = GetCluesAndAnswers();
      i.PutExtra("Answers", clues.Values.Select(t => t.Item2).ToArray());
      i.PutExtra("Colors", questionGroups.Values.Select(t=>t.Background).ToArray());
      StartActivityForResult(i, 2);*/
    }

    void SetupQuestion()
    {
      if (!answers.ContainsKey(currentQuestionGroup))
      {
        answers.Add(currentQuestionGroup, Enumerable.Repeat(-1, questionGroups[currentQuestionGroup].Questions.Count).ToArray());
      }

      var v = new QuestionView(
        this,
        questionGroups,
        currentQuestionGroup,
        questionNumber,
        answers[currentQuestionGroup][questionNumber]);
      v.NextQuestion += V_NextQuestion;
      v.PreviousQuestion += V_PreviousQuestion;
      v.Submit += V_Submit;
      v.Back += V_Back;
      SetContentView(v);
    }

    void V_Back(object sender, EventArgs e)
    {
      ShowMenu();
    }

    void V_Submit(object sender, int answer)
    {
      answers[currentQuestionGroup][questionNumber] = answer;

      if (!CheckAnswers(answers[currentQuestionGroup], questionGroups[currentQuestionGroup]))
      {
        var i = new Intent(this, typeof(WrongAnswerActivity));
        i.PutExtra("BgColor", questionGroups[currentQuestionGroup].Background);
        StartActivityForResult(i, 1);
      }
      else
      {
        var i = new Intent(this, typeof(CorrectAnswer));
        i.PutExtra("BgColor", questionGroups[currentQuestionGroup].Background);
        i.PutExtra("Thumbs", questionGroups[currentQuestionGroup].Thumb);

        var clues = GetCluesAndAnswers();
        i.PutExtra("Clues", clues.Values.Select(t => t.Item1).ToArray());
        i.PutExtra("Answers", clues.Values.Select(t => t.Item2).ToArray());

        StartActivityForResult(i, 3);
      }
    }

    Dictionary<string, Tuple<string, bool>> GetCluesAndAnswers()
    {
      var clues = new Dictionary<string, Tuple<string, bool>>();
      foreach (var kv in questionGroups)
      {
        clues.Add(
          kv.Key,
          new Tuple<string, bool>(
            kv.Value.Clue,
            answers.ContainsKey(kv.Key) && CheckAnswers(answers[kv.Key], questionGroups[kv.Key])));
      }
      return clues;
    }

    static bool CheckAnswers(int[] currentAnswers, QuestionGroup currentQuestions)
    {
      return currentAnswers!=null && !currentAnswers.Any(a=>a ==-1) &&
        !currentQuestions.Questions.Where(
          (t, i) => !t.Answers[currentAnswers[i]].Correct).Any();
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
      base.OnActivityResult(requestCode, resultCode, data);

      switch (requestCode)
      {
        case 1:
          if (currentQuestionGroup == null || !questionGroups.ContainsKey(currentQuestionGroup))
          {
            ShowMenu();
            break;
          }
          questionNumber = 0;
          SetupQuestion();
          break;
        case 3:
          if (resultCode == Result.Ok)
          {
            answers = new Dictionary<string, int[]>();
            ShowMenu(); // Show new welcome page
          }
          else
          {
            currentQuestionGroup = data?.GetStringExtra("GroupId");
            if (currentQuestionGroup == null || !questionGroups.ContainsKey(currentQuestionGroup))
            {
              ShowMenu();
              break;
            }
            questionNumber = 0;
            SetupQuestion();
          }
          break;
        case 2:
          currentQuestionGroup = data.GetStringExtra("GroupId");
          questionNumber = 0;
          SetupQuestion();
          break;
        
      }
    }

    void V_PreviousQuestion(object sender, int answer)
    {
      answers[currentQuestionGroup][questionNumber] = answer;
      if (questionNumber > 0) questionNumber--;
      SetupQuestion();
    }

    void V_NextQuestion(object sender, int answer)
    {
      answers[currentQuestionGroup][questionNumber] = answer;
      if (questionNumber < questionGroups[currentQuestionGroup].Questions.Count - 1) questionNumber++;
      SetupQuestion();
    }

    static Dictionary<string, QuestionGroup> GetQuestionGroups(Context context)
    {
      var qStream = context.Assets.Open("Questions.xml");
      var spf = SAXParserFactory.NewInstance();
      var sp = spf.NewSAXParser();
      var xr = sp.XMLReader;
      var questionsXmlHandler = new QuestionsXmlHandler();
      xr.ContentHandler = questionsXmlHandler;
      var inStream = new InputSource(qStream);

      xr.Parse(inStream);

      return questionsXmlHandler.QuestionGroups;
    }

  }
}

