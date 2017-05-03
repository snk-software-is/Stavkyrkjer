using System.Collections.Generic;

namespace UrnesStavkyrkje
{
  public class QuestionGroups
  {
    public Dictionary<string, QuestionGroup> Groups;
  }

  public class QuestionGroup
  {
    public string Name;
    public string Text;
    public string Background;
    public int Thumb;
    public string Clue;
    
    public List<Question> Questions;

    public QuestionGroup(string name, string text, string background, int thumb, string clue)
    {
      Name = name;
      Text = text;
      Background = background;
      Thumb = thumb;
      Clue = clue;
      Questions = new List<Question>();
    }
  }

  public class Question
  {
    public string Id;
    public string Text;

    public List<Answer> Answers;

    public Question(string id, string text)
    {
      Id = id;
      Text = text;

      Answers = new List<Answer>();
    }
  }

  public class Answer
  {
    public bool Correct;
    public string Text;

    public Answer(bool correct, string text)
    {
      Correct = correct;
      Text = text;
    }
  }
}