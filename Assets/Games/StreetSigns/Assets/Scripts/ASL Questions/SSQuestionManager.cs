using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class SSQuestionManager : MonoBehaviour
{
	[SerializeField] private StreetSignsUIManager uiManager;
	[SerializeField] private Sprite defaultIconToShow;
    // public List<string> VocabWords;
	public List<VocabularyEntry> AllPossibleVocabEntries;
	private List<VocabularyEntry> entriesNotYetAsked;
	public List<string> AllPossibleVocabWords;
    public string CorrectWord;
	public VocabularyEntry CorrectEntry;

	public SSQuestionType SelectedQuestionType;
	private bool isPlayerAnsweringQuestion;

	public enum SSQuestionType
	{
		ASLSignToEnglishWord,
		EnglishDefinitionToEnglishWord,
		IconToEnglishWord,
	}

	private int questionTypeCount;

    // Start is called before the first frame update
    private void Start()
    {
		questionTypeCount = System.Enum.GetNames(typeof(SSQuestionType)).Length;

		// Generate list of VocabularyEntries to use in game
		AllPossibleVocabEntries = VocabularyLoader.Instance.CreateVocabularyEntryListToUse(GlobalManager.Instance.CurrentPacket, GlobalManager.Instance.ReviewPreviousPackets);
		entriesNotYetAsked = new List<VocabularyEntry>(AllPossibleVocabEntries);

		SelectNewWord();

		// Generate list of words from AllPossibleVocabEntries, so we don't need to recompute this everytime we have a new question
		foreach (VocabularyEntry entry in AllPossibleVocabEntries)
		{
			AllPossibleVocabWords.Add(entry.English_Word);
		}
    }

	public void LoadQuestionToUI()
	{
		// Populate panel according to question type
		switch (SelectedQuestionType)
		{
			case SSQuestionType.ASLSignToEnglishWord:
				Debug.Log($"Loading question type: ASLSignToEnglishWord: {CorrectEntry.English_Word}");
				uiManager.UpdateQuestionVideoPanel("What is this sign?", CorrectEntry.ASL_Sign);

				if (GlobalManager.Instance.ReviewPreviousPackets)
					GlobalManager.Instance.currentReviewData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["ASL_Sign"] += 1;
				else
					GlobalManager.Instance.currentLessonData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["ASL_Sign"] += 1;
				break;

			case SSQuestionType.EnglishDefinitionToEnglishWord:
				Debug.Log($"Loading question type: EnglishDefinitionToEnglishWord: {CorrectEntry.English_Word}");
				uiManager.UpdateQuestionOnlyPanel($"\"{CorrectEntry.English_Definition}\" means...");

				if (GlobalManager.Instance.ReviewPreviousPackets)
					GlobalManager.Instance.currentReviewData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["English_Definition"] += 1;
				else
					GlobalManager.Instance.currentLessonData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["English_Definition"] += 1;
				break;

			case SSQuestionType.IconToEnglishWord:
				// uiManager.UpdateQuestionIconPanel("This image shows...", defaultIconToShow);
				Debug.Log($"Loading question type: IconToEnglishWord: {CorrectEntry.English_Word}");
				uiManager.UpdateQuestionIconPanel("This image shows...", GlobalManager.Instance.GetIcon(CorrectEntry.Vocabulary_ID));
				
				if (GlobalManager.Instance.ReviewPreviousPackets)
					GlobalManager.Instance.currentReviewData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["Icon"] += 1;
				else
					GlobalManager.Instance.currentLessonData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["Icon"] += 1;
				break;
		}
	}

    public void SelectNewWord()
    {
        // Make panel randomly select question type
		SelectedQuestionType = (SSQuestionType) Random.Range(0, questionTypeCount);

		// Randomly select correct answer
		if (entriesNotYetAsked.Count >= 1)
		{
			int i = Random.Range(0, entriesNotYetAsked.Count);
			CorrectEntry = entriesNotYetAsked[i];
			entriesNotYetAsked.RemoveAt(i);
		}
		else
		{
			CorrectEntry = AllPossibleVocabEntries[Random.Range(0, AllPossibleVocabEntries.Count)];
		}
		
		CorrectWord = CorrectEntry.English_Word;

		StartCoroutine(LoadQuestionToUIAfterDelay(5.0f));
    }

	public void HandleToggleShowQuestion()
	{
		if (isPlayerAnsweringQuestion)
		{
			isPlayerAnsweringQuestion = false;
			SelectNewWord();
			uiManager.ToggleShowQuestionUIPanel(true);
		}
		else
		{
			isPlayerAnsweringQuestion = true;
			uiManager.RestartVideo();
			uiManager.ToggleShowQuestionUIPanel(false);
		}
	}

	private IEnumerator LoadQuestionToUIAfterDelay(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);

		LoadQuestionToUI();
	}

    // public string GetWordURL()
    // {
    //     return VideoManager.VocabWordToPathDict[CorrectWord];
    // }
}
