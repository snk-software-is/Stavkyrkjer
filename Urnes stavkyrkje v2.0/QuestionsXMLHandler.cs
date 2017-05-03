using System;
using System.Collections.Generic;

using Org.Xml.Sax;
using Org.Xml.Sax.Helpers;

namespace AndyApp3
{
  public class QuestionsXmlHandler : DefaultHandler
  {
    public Dictionary<string, QuestionGroup> QuestionGroups;

    QuestionGroup questionGroup;
    Question question;
    Answer answer;

    public override void StartElement(string uri, string localName, string qName, IAttributes attributes)
    {
      switch (localName.ToLower())
      {
        case "questiongroups":
          QuestionGroups = new Dictionary<string, QuestionGroup>();
          break;
        case "questiongroup":
          questionGroup = new QuestionGroup(attributes.GetValue("name"), attributes.GetValue("text"), attributes.GetValue("background"), int.Parse(attributes.GetValue("thumb")), attributes.GetValue("clue"));
          break;
        case "question":
          question = new Question(attributes.GetValue("id"), attributes.GetValue("text"));
          break;
        case "answer":
          answer = new Answer(bool.Parse(attributes.GetValue("correct")), attributes.GetValue("text"));
          break;
      }
    }

    public override void EndElement(string uri, string localName, string qName)
    {
      switch (localName.ToLower())
      {
        case "questiongroups":
          
          break;
        case "questiongroup":
          QuestionGroups[questionGroup.Name] = questionGroup;
          break;
        case "question":
          questionGroup.Questions.Add(question);
          break;
        case "answer":
          question.Answers.Add(answer);
          break;
      }
    }
  }

}
