using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;

public class QuestionManager : MonoBehaviour
{
    private int questionIdx;
    private bool lastAnswerCorrect;
    [SerializeField] private List<QuizQuestion> quizQuestions;
    [SerializeField] private AnswerButton[] answerButtons;

    [SerializeField] RectTransform companionFolder;
    [SerializeField] RectTransform startSlide;
    [SerializeField] RectTransform questionSlide;
    [SerializeField] RectTransform endSlide;
    [SerializeField] RectTransform invalidSlide;

    // Companion elements
	[Header("Companion elements")]
    [SerializeField] private Image speechBubble;

	// First Slide elements
	[Header("First Slide elements")]
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private GameObject spinner;
	[SerializeField] private Button startQuizButton;

    // Question Slide elements
	[Header("Question Slide elements")]
	[SerializeField] private TextMeshProUGUI questionTitleText;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI questionNumberText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Image progressMask;
    // Supplementary elements
    [SerializeField] private TextMeshProUGUI supplementaryText;
    [SerializeField] private RawImage videoScreen;
    [SerializeField] private Image icon;

    // End Slide elements
	[Header("End Slide elements")]
    [SerializeField] private TextMeshProUGUI numLearnedText;

    [SerializeField] private VideoPlayer videoPlayer;

	private List<VocabularyEntry> allPossibleVocabEntries; // List of entries from packet 0 to now
	private List<VocabularyEntry> currentQuizVocabEntries; // List of only entries used in quiz (same number as number of questions)
	private List<QuizQuestionObject> topThreeWorstQuestions; // Top 3 worst questions historrically to add to quiz

    // Start is called before the first frame update
    void Start()
    {
		spinner.SetActive(true);
		startQuizButton.gameObject.SetActive(false);

		allPossibleVocabEntries = new List<VocabularyEntry>();
		currentQuizVocabEntries = new List<VocabularyEntry>();
		topThreeWorstQuestions = new List<QuizQuestionObject>();

        icon.preserveAspect = true;

        questionIdx = -1;
        VerifyReviewData();
    }

    // Check if GlobalManager's currentReviewData is valid. Handle if not.
    private void VerifyReviewData()
    {
        if (GlobalManager.Instance.CurrentReview < 0 || GlobalManager.Instance.CurrentReview > 3
            || GlobalManager.Instance.currentReviewData == null
            || GlobalManager.Instance.currentReviewData.quizQuestionObjectList.Count <= 0)
        {
            HandleInvalidReview();
        }

		// Review is valid, load begin to load questions
		titleText.text = $"Review Quiz {GlobalManager.Instance.CurrentReview}";
		questionTitleText.text = $"Review Quiz {GlobalManager.Instance.CurrentReview}";
		StartCoroutine(PopulateQuestionsFromReview());
    }

	private IEnumerator PopulateQuestionsFromReview()
	{
		allPossibleVocabEntries = VocabularyLoader.Instance.CreateVocabularyEntryListToUse(GlobalManager.Instance.CurrentPacket, true);
		quizQuestions = new List<QuizQuestion>();

		// If review number is > 0, check all packets before it for 3 worst questions
		if (GlobalManager.Instance.CurrentReview > 0)
		{
			// We are not on our first review, check previous reviews for 3 worst answers
			for (int i = 0; i < GlobalManager.Instance.CurrentReview; i++)
			{
				bool isCompleted = false;
				PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
					result =>
					{
						OnReviewDataReceived(result, i, () => isCompleted = true);
					},
					err =>
					{
						Debug.LogError(err);
						isCompleted = true;
					}
				);
				
				yield return new WaitUntil(() => isCompleted);
			}
		}

		// Convert QuizQuestionObjects to VocabularyEntries
		// Debug.Log($"Sanity check! topThreeWorstQuestions size = {topThreeWorstQuestions.Count}, currentQuizVocabEntries size = {currentQuizVocabEntries.Count}, quizQuestionObjectList size = {GlobalManager.Instance.currentReviewData.quizQuestionObjectList.Count}");
		// if (GlobalManager.Instance.CurrentReview > 0)
		// {
		// 	// Add top 3 worst questions as entries
		// 	foreach (QuizQuestionObject quizQuestionObject in topThreeWorstQuestions)
		// 	{
		// 		VocabularyEntry entry = FindVocabularyEntryFromQuizQuestionObject(quizQuestionObject);
		// 		currentQuizVocabEntries.Add(entry);
		// 	}
		// }
		Debug.Log($"Sanity check! topThreeWorstQuestions size = {topThreeWorstQuestions.Count}, currentQuizVocabEntries size = {currentQuizVocabEntries.Count}, quizQuestionObjectList size = {GlobalManager.Instance.currentReviewData.quizQuestionObjectList.Count}");
		foreach (QuizQuestionObject quizQuestionObject in GlobalManager.Instance.currentReviewData.quizQuestionObjectList)
		{
			// Load questions from current review packet
			VocabularyEntry entry = FindVocabularyEntryFromQuizQuestionObject(quizQuestionObject);
			if (entry != null) currentQuizVocabEntries.Add(entry);
			else Debug.LogError($"Error: couldn't find VocabularyEntry from quizQuestionObject: {quizQuestionObject.vocabID}");
		}
		Debug.Log($"Sanity check! topThreeWorstQuestions size = {topThreeWorstQuestions.Count}, currentQuizVocabEntries size = {currentQuizVocabEntries.Count}, quizQuestionObjectList size = {GlobalManager.Instance.currentReviewData.quizQuestionObjectList.Count}");
		
		// Create QuizQuestions out of VocabularyEntries
		QuestionType questionType = QuestionType.DefToWord;
		foreach (VocabularyEntry entry in currentQuizVocabEntries)
		{
			VocabularyEntry[] answers = GetRandomAnswers(currentQuizVocabEntries, entry, 4).ToArray();
			int correctAnswerPos = 0;
			for (int i = 0; i < answers.Length; i++)
			{
				if (answers[i].Vocabulary_ID == entry.Vocabulary_ID)
				{
					correctAnswerPos = i;
					break;
				}
			}

			QuizQuestion question = new QuizQuestion
			(
				entry,
				questionType,
				answers,
				correctAnswerPos
			);
			quizQuestions.Add(question);

			// Alternate asking Word -> Definition and Definition -> Word
			if (questionType == QuestionType.DefToWord)
				questionType = QuestionType.WordToDef;
			else
				questionType = QuestionType.DefToWord;
		}

		// Loading is done, allow player to play quiz
		Debug.Log("Review Questions done loading!");
		spinner.SetActive(false);
		startQuizButton.gameObject.SetActive(true);
	}

	void OnReviewDataReceived(GetUserDataResult result, int reviewID, Action onComplete)
	{
		Debug.Log($"Fetching review {reviewID}");
		if (result.Data != null && result.Data.ContainsKey($"Review {reviewID}"))
		{
			Debug.Log($"Received student review data for review {reviewID}!");
			ReviewData reviewData = JsonConvert.DeserializeObject<ReviewData>(result.Data[$"Review {reviewID}"].Value);

			// Initialize topThreeWorstQuestions list if it doesn't have 3 items
			if (topThreeWorstQuestions.Count < 3)
			{
				int idx = 0;
				while (topThreeWorstQuestions.Count != 3)
				{
					topThreeWorstQuestions.Add(reviewData.quizQuestionObjectList[idx]);
					VocabularyEntry entry = FindVocabularyEntryFromQuizQuestionObject(reviewData.quizQuestionObjectList[idx]);
					if (entry != null) currentQuizVocabEntries.Add(entry);
					else Debug.LogError($"Error: couldn't find VocabularyEntry from quizQuestionObjectList: {reviewData.quizQuestionObjectList[idx].vocabID}");
					idx++;
				}
			}

			Debug.Log($"The size of topThreeWorstQuestions is {topThreeWorstQuestions.Count}");

			// Go through each review's question and figure out what are the top 3 worst questions
			foreach (QuizQuestionObject question in reviewData.quizQuestionObjectList)
			{
				for (int i = 0; i < topThreeWorstQuestions.Count; i++)
				{
					if (question.numAttempts > topThreeWorstQuestions[i].numAttempts)
					{
						topThreeWorstQuestions[i] = question;
						VocabularyEntry entry = FindVocabularyEntryFromQuizQuestionObject(question);
						if (entry != null) currentQuizVocabEntries[i] = entry;
						else Debug.LogError($"Error: couldn't find VocabularyEntry from question: {question.vocabID}");
						break;
					}
				}
			}
		}
		else
        {
			Debug.Log("No review found");
        }

		onComplete?.Invoke();
	}

	private VocabularyEntry FindVocabularyEntryFromQuizQuestionObject(QuizQuestionObject question)
	{
		int id = question.vocabID;
		foreach (VocabularyEntry entry in allPossibleVocabEntries)
		{
			if (entry.Vocabulary_ID == id)
			{
				return entry;
			}
		}
		Debug.LogWarning($"VocabularyEntry for QuizQuestionObject {question.vocabID} doesn't exist!");
		return null;
	}

	private QuizQuestionObject FindQuizQuestionObjectFromVocabularyEntry(VocabularyEntry entry)
	{
		int id = entry.Vocabulary_ID;
		foreach (QuizQuestionObject question in GlobalManager.Instance.currentReviewData.quizQuestionObjectList)
		{
			if (question.vocabID == id)
			{
				return question;
			}
		}
		Debug.LogWarning($"QuizQuestionObject for VocabularyEntry {entry.Vocabulary_ID} doesn't exist!");
		return null;
	}

	/// <summary>
    /// Takes a list of VocabularyEntries and randomly selects 4 words, one of them being the correct answer
    /// </summary>
    /// <param name="vocabList">List of vocab words</param>
    /// <param name="correctAns">The correct vocab word</param>
    /// <returns>Returns a size 4 array of vocab words, one of them being correct</returns>
    public List<VocabularyEntry> GetRandomAnswers(List<VocabularyEntry> vocabList, VocabularyEntry correctAns, int numberOfAnswers)
    {
        List<VocabularyEntry> toReturn = new List<VocabularyEntry>();

        // Make list of 4 random words, including current one
        List<VocabularyEntry> inputList = new List<VocabularyEntry>();
        for (int i = 0; i < vocabList.Count; i++)
        {
            inputList.Add(vocabList[i]);
        }

        toReturn.Add(correctAns);
        inputList.Remove(correctAns);

        for (int i = 0; i < numberOfAnswers-1; i++)
        {
            int rndNum = UnityEngine.Random.Range(0, inputList.Count);
            toReturn.Add(inputList[rndNum]);
            inputList.Remove(inputList[rndNum]);
        }

        // Randomize list
        for (int i = 0; i < toReturn.Count; i++)
        {
            VocabularyEntry temp = toReturn[i];
            int randomIndex = UnityEngine.Random.Range(i, toReturn.Count);
            toReturn[i] = toReturn[randomIndex];
            toReturn[randomIndex] = temp;
        }

        return toReturn;
    }

    // Display screen to clarify that review data is invalid
    private void HandleInvalidReview()
    {
        companionFolder.gameObject.SetActive(false);
        startSlide.gameObject.SetActive(false);
        questionSlide.gameObject.SetActive(false);
        endSlide.gameObject.SetActive(false);
        invalidSlide.gameObject.SetActive(true);
    }

    public void StartQuiz()
    {
        startSlide.gameObject.SetActive(false);
        questionSlide.gameObject.SetActive(true);
        lastAnswerCorrect = true;
        NextQuestion();
        foreach(QuizQuestion question in quizQuestions)
        {
            Debug.Log(JsonUtility.ToJson(question, true));
        }
    }

    public void NextQuestion()
    {

        // If last question was correct, move on with it in place
        // If it was incorrect shuffle it to the end and try again later
        if (lastAnswerCorrect)
        {
            questionIdx++;
        } else
        {
            QuizQuestion toShuffle = quizQuestions[questionIdx];
            quizQuestions.RemoveAt(questionIdx);
            quizQuestions.Add(toShuffle);
        }

        // If we've gotten them all correct, move to the end
        if (questionIdx >= quizQuestions.Count)
        {
            FinishQuiz();
            return;
        }

        // Hide appropriate elements
        videoScreen.GetComponent<FadeGraphic>().SetOut();
        icon.GetComponent<FadeGraphic>().SetOut();
        supplementaryText.gameObject.SetActive(false);

        continueButton.gameObject.SetActive(false);
        QuizQuestion currentQuestion = quizQuestions[questionIdx];

        // Do the following every time
        questionText.text = currentQuestion.questionText;
        questionNumberText.text = "Question " + (questionIdx + 1) + "/" + quizQuestions.Count;
        supplementaryText.text = currentQuestion.vocabularyEntry.English_Word;
        videoPlayer.url = currentQuestion.videoURL;
		videoScreen.gameObject.SetActive(false);
        icon.sprite = currentQuestion.icon;
        // Set the answer text
        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];
            answerButtons[i].ResetColor();
            answerButtons[i].GetComponent<Button>().enabled = true;
        }
        // Update progress bar
        progressMask.fillAmount = (float)questionIdx / quizQuestions.Count;
        // Update companion
        speechBubble.gameObject.SetActive(false);

        // Handle specific question type (hide or show supplementary)
        /*
        switch (currentQuestion.questionType)
        {
            case QuestionType.DefToWord:
                ActivateSupplementary(false, false);
                break;
            case QuestionType.WordToDef:
                ActivateSupplementary(false, false);
                break;
            default:
                Debug.LogError("Unknown question type");
                break;
        }
        */


        // Show appropriate elements
        answerButtons[0].transform.parent.GetComponent<FadeCanvasGroup>().SetIn();
        questionText.transform.parent.GetComponent<FadeCanvasGroup>().SetIn();
    }

    public void FinishQuiz()
    {
        questionSlide.gameObject.SetActive(false);
        numLearnedText.text = "You just learned " + quizQuestions.Count + " words!";
        endSlide.gameObject.SetActive(true);
		PostReviewData();
    }

    private void ActivateSupplementary(bool hasVideo, bool hasIcon)
    {
        videoScreen.gameObject.SetActive(hasVideo);
        icon.gameObject.SetActive(hasIcon);
    }

    public void CheckAnswer(int selectedAnswer)
    {
        QuizQuestion currentQuestion = quizQuestions[questionIdx];
        if (selectedAnswer != currentQuestion.correctAnswer)
        {
            // Highlight incorrect choice
            answerButtons[selectedAnswer].HighlightIncorrect();
            speechBubble.gameObject.SetActive(true);
            lastAnswerCorrect = false;
			QuizQuestionObject quizQuestionObject = FindQuizQuestionObjectFromVocabularyEntry(currentQuestion.vocabularyEntry);
			if (quizQuestionObject != null) quizQuestionObject.numAttempts += 1;
			else Debug.LogWarning($"Warning: couldn't find QuizQuestionObject for entry {currentQuestion.vocabularyEntry.Vocabulary_ID}, if word is from other review packet this is normal");
        } else
        {
            lastAnswerCorrect = true;
			QuizQuestionObject quizQuestionObject = FindQuizQuestionObjectFromVocabularyEntry(currentQuestion.vocabularyEntry);
			if (quizQuestionObject != null) quizQuestionObject.successfulAnswer = true;
			else Debug.LogWarning($"Warning: couldn't find QuizQuestionObject for entry {currentQuestion.vocabularyEntry.Vocabulary_ID}, if word is from other review packet this is normal");
        }
        answerButtons[currentQuestion.correctAnswer].HighlightCorrect();
        // Disable all the buttons, move on to next question
        foreach (AnswerButton answerButton in answerButtons)
        {
            answerButton.GetComponent<Button>().enabled = false;
        }
        StartCoroutine(AnimateSupplementalSlide());
    }

    private IEnumerator AnimateSupplementalSlide()
    {
        // Wait for buttons to finish
        while (AnyButtonAnimating())
        {
            yield return null;
        }

        // Fade the question and answers out
        FadeCanvasGroup questionCanvasFader = questionText.transform.parent.GetComponent<FadeCanvasGroup>();
        questionCanvasFader.FadeOut();
        answerButtons[0].transform.parent.GetComponent<FadeCanvasGroup>().FadeOut();
        while (questionCanvasFader.IsAnimating)
        {
            yield return null;
        }

        // Fade the vide and icon in
		videoScreen.gameObject.SetActive(true);
        icon.gameObject.SetActive(true);
        videoPlayer.Pause();
        videoPlayer.frame = 0;
        yield return null; // Allow extra frame so FadeGraphic can grab Graphics

        videoScreen.GetComponent<FadeGraphic>().FadeIn();
        icon.GetComponent<FadeGraphic>().FadeIn();
        while (videoScreen.GetComponent<FadeGraphic>().IsAnimating)
        {
            yield return null;
        }
        videoPlayer.Play();

        supplementaryText.gameObject.SetActive(true);
        supplementaryText.GetComponent<DialogueAnimator>().PlayAnimation();

        continueButton.gameObject.SetActive(true);
        yield break;
    }

    // returns true if any of the answer buttons are currently animating, false otherwise
    private bool AnyButtonAnimating()
    {
        foreach (AnswerButton answerButton in answerButtons)
        {
            if (answerButton.IsAnimating) return true;
        }
        return false;
    }

	private void PostReviewData()
    {
        // Important to remember this copies by reference. We are changing the GlobalManager's ReviewData object here.
        ReviewData newReviewData = GlobalManager.Instance.currentReviewData;
        // UpdateFlashcardTimes(newLessonData);
        newReviewData.quizComplete = true;
        Debug.Log($"Full object:\n {JsonConvert.SerializeObject(newReviewData, Formatting.Indented)}");
        PostReview(newReviewData);
    }

    // private void UpdateFlashcardTimes(LessonData lessonData)
    // {
    //     for (int i = 0; i < currentPacket.Entries.Count; i++)
    //     {
    //         lessonData.flashcardData[currentPacket.Entries[i].Vocabulary_ID] = timeSpentOnWords[i];
    //     }
    // }

    public void PostReview(ReviewData reviewData)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>{
                {$"Review {reviewData.reviewID}",JsonConvert.SerializeObject(reviewData)}
            }
        };
        PlayFabClientAPI.UpdateUserData(request,
            res => Debug.Log("Successful lesson user data sent!"),
            err => Debug.LogError(err));
    }

    public void ExitToMap()
    {
        // Scene logic
        SceneManager.LoadScene("MapLayoutScene");
    }
}
