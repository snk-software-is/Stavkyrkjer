using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AndyApp3
{
  public class ClueView : LinearLayout
  {
    public string Text;
    public string[] Clues;
    public bool[] Answers;
    public Color BgColor;

    public ClueView(Context context) : base(context)
    {
      Initialize();
    }

    public ClueView(Context context, IAttributeSet attrs) :
      base(context, attrs)
    {
      Initialize();
    }

    public ClueView(Context context, IAttributeSet attrs, int defStyle) :
      base(context, attrs, defStyle)
    {
      Initialize();
    }

    public ClueView(Context context, string[] clues, bool[] answers, Color bgColor) : base(context)
    {
      Clues = clues;
      Answers = answers;
      BgColor = bgColor;
      Initialize();
    }


    private void Initialize()
    {
      /*
< TextView
          android:
id = "@+id/Clues"
          android:
layout_width = "wrap_content"
          android:
layout_height = "wrap_content"
          android:
layout_centerHorizontal = "true"
          android:
textColor = "@android:color/black"
          android:
textSize = "30sp" />
*/
      SetBackgroundColor(BgColor);

      var clued = string.Join("__", Clues.Select((c, i) => Answers[i] ? c : string.Format($"{{0,{c.Length}}}", " ")));

      const int w = 34;
      const int h = 51;
      const int b = 1;

      for (var i = 0; i < Clues.Length; i++)
      {
        if (i > 0)
        {
          var space2 = new RelativeLayout(Context);
          space2.SetBackgroundColor(BgColor);
          AddView(space2, w + b * 2 * 2, h + b * 2 * 2);
        }
        var letters = Answers[i] ? Clues[i].ToCharArray() : Enumerable.Repeat(' ', Clues[i].Length).ToArray();
        for (var k = 0; k < letters.Length; k++)
        {
          var space = new RelativeLayout(Context);
          space.SetBackgroundColor(BgColor);
          AddView(space, w + (b+1)*2*2, h + (b + 1) * 2*2);

          var box = new RelativeLayout(Context);
          box.SetBackgroundColor(Color.Black);
          var bp = new RelativeLayout.LayoutParams(w + b*2, h + b*2);
          bp.AddRule(LayoutRules.CenterInParent);
          space.AddView(box, bp);

          var txt = new TextView(Context);
          txt.Text = $"{letters[k]}";
          txt.SetBackgroundColor(BgColor);
          txt.Typeface = Typeface.DefaultBold;
          //txt.TextSize = 21;
          txt.SetTextSize(ComplexUnitType.Dip, 40);
          txt.SetTextColor(Color.Black);
          txt.TextAlignment = TextAlignment.Center;

          txt.SetPadding(4,-5,0,5);
          var lp = new RelativeLayout.LayoutParams(w, h);
          lp.AddRule(LayoutRules.CenterInParent);
          box.AddView(txt, lp);
        }

      }


    }
  }
}