using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
	public VocabularyEntry vocabularyEntry;
    public int questionID;
    public string questionText;
    // See QuestionType.cs for explanation
    public QuestionType questionType;
    public string videoURL;
    public Sprite icon;
    // 0..3 correspond to A..D
    public string[] answers;
    public int correctAnswer;

	public QuizQuestion(VocabularyEntry entry, QuestionType questionType, VocabularyEntry[] allAnswers, int correctAnswerPos)
	{
		this.vocabularyEntry = entry;
		this.questionID = entry.Vocabulary_ID;
		this.questionType = questionType;
		this.answers = new string[allAnswers.Length];
		switch (questionType)
        {
            // case QuestionType.SignWordToWord:
            //     ActivateSupplementary(true, false);
            //     break;
            case QuestionType.DefToWord:
                this.questionText = $"\"{entry.English_Definition}\" means...";
				this.videoURL = entry.ASL_Sign;
				for (int i = 0; i < this.answers.Length; i++)
				{
					this.answers[i] = allAnswers[i].English_Word;
				}
                break;
            case QuestionType.WordToDef:
                this.questionText = $"\"{entry.English_Word}\" means...";
				this.videoURL = entry.ASL_Definition;
				for (int i = 0; i < this.answers.Length; i++)
				{
					this.answers[i] = allAnswers[i].English_Definition;
				}
                break;
            // case QuestionType.SignDefToDef:
            //     ActivateSupplementary(true, false);
            //     break;
            // case QuestionType.IconToWord:
            //     ActivateSupplementary(false, true);
            //     break;
            default:
                Debug.LogError("Unknown question type");
                break;
        }
		this.icon = GlobalManager.Instance.GetIcon(entry.Vocabulary_ID);
		this.correctAnswer = correctAnswerPos;
	}
}
