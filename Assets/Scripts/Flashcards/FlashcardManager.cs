using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class FlashcardManager : MonoBehaviour
{
    // Describes what "slide" we're on given n vocab terms
    // 0 == intro slide
    // 1..2n == flashcard slides
    // 2n + 1 == ending slide
    private int currentSlide;
    private int furthestSlide;
    private int currentWord;
    private VocabularyPacket currentPacket;
    // Set to false if at any point we encounter a playfab/data error
    private bool canPost;

    // Intro screen variables
    [SerializeField] private Image[] introIcons;
    
    // Basic flashcard variables
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI definitionText;
    [SerializeField] private Image flashcardIcon;

    // Video variables
    [SerializeField] private RectTransform screenParent;
    [SerializeField] private RectTransform frontScreen;
    [SerializeField] private RectTransform backScreen;
    [SerializeField] private VideoPlayer wordVideoPlayer;
    [SerializeField] private VideoPlayer definitionVideoPlayer;

    // Progress variables
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private Image progressMask;
    [SerializeField] private RectTransform starsFolder;
    [SerializeField] private GameObject starPrefab;
    private float starOffset = 30f;
    private ProgressStar[] progressStars;
    [SerializeField] private ParticleSystem confettiParticles;

    [SerializeField] private RectTransform flashcardMask;
    [SerializeField] private GameObject inputDisabler;
    [SerializeField] private RectTransform winScreen;
    [SerializeField] private Button nextArrow;
    [SerializeField] private Button prevArrow;

    // Animation controls
    [SerializeField] private AnimationCurve flipAnimCurve;
    private float flipTime = 0.8f;
    [SerializeField] private DialogueAnimator definitionAnimator;

    // Timing and analytics
    private Stopwatch stopwatch;
    private float[] timeSpentOnWords;

    // Dev variables
    [SerializeField] private Sprite devIcon;

    // Start is called before the first frame update
    void Start()
    {
        canPost = true;
        // Prepare flashcard data
        LoadWords();
        currentSlide = 0;
        currentWord = 0;

        // Prepare timer
        stopwatch = GetComponent<Stopwatch>();
        timeSpentOnWords = new float[currentPacket.Entries.Count];

        // Setup display
        SetIntroIcons();
        PrepVideo(wordVideoPlayer, currentPacket.Entries[0].ASL_Sign_and_Spelled);
        headerText.text = $"Welcome to Packet {GlobalManager.Instance.CurrentPacket}!";
        flashcardIcon.preserveAspect = true;
        InstantiateStars();

        Debug.Log($"Full object:\n {JsonConvert.SerializeObject(GlobalManager.Instance.currentLessonData, Formatting.Indented)}");
    }

    private void LoadWords()
    {
        int wordsToLoad = GlobalManager.Instance.CurrentPacket;
        if (wordsToLoad < 0 || wordsToLoad >= 11)
        {
            // Load packet 1 by default
            wordsToLoad = 0;
            canPost = false;
        }
        currentPacket = VocabularyLoader.Instance.VocabularyData.Packets[wordsToLoad];
    }

    // Sets the images for the intro screen icons
    private void SetIntroIcons()
    {
        for (int i = 0; i < introIcons.Length; i++)
        {
            introIcons[i].preserveAspect = true;
            introIcons[i].sprite = GlobalManager.Instance.GetIcon(currentPacket.Entries[i].Vocabulary_ID);
        }
    }

    // Instantiates the stars for the progress bar
    private void InstantiateStars()
    {
        progressStars = new ProgressStar[currentPacket.Entries.Count];
        for (int i = 1; i < currentPacket.Entries.Count + 1; i++)
        {
            GameObject starObj = Instantiate(starPrefab);
            starObj.transform.SetParent(starsFolder, false);
            RectTransform starTransform = starObj.GetComponent<RectTransform>();
            // Anchor it to the middle left of its parent
            starTransform.anchorMin = new Vector2(0, 0.5f);
            starTransform.anchorMax = new Vector2(0, 0.5f);
            // Then set it to the proper spacing
            starTransform.anchoredPosition = new Vector2(((float)i / currentPacket.Entries.Count) * progressBar.sizeDelta.x - starOffset, 0);
            progressStars[i - 1] = starObj.GetComponent<ProgressStar>();
        }
    }

    // Progresses flashcards to next slide. For use with button
    public void NextSlide()
    {
        // Don't want button to work while we're animating. Skip the text first
        if (definitionAnimator.InProgress)
            return;

        int prevSlide = currentSlide;
        currentSlide++;
        if (currentSlide > furthestSlide) furthestSlide = currentSlide;

        if (prevSlide == 0)
        {
            // Handle leaving intro slide
            foreach (Image icon in introIcons) icon.gameObject.SetActive(false);
            ChangeWord();
            flashcardMask.gameObject.SetActive(true);
        } else if (prevSlide >= 2 * currentPacket.Entries.Count)
        {
            // Handle moving to ending slide
            MoveToEnd();
        } else if (prevSlide % 2 == 1)
        {
            // Handle flipping flashcard
            FlipCard();
        } else
        {
            // Handle changing to next flashcard
            ChangeWord();
        }
    }

    // Goes to the previous word
    public void PrevSlide()
    {
        // Don't want button to work while we're animating. Skip the text first
        if (definitionAnimator.InProgress)
            return;

        int prevSlide = currentSlide;
        currentSlide--;

        if (prevSlide % 2 == 0)
        {
            // Handle unflipping flashcard
            UnflipCard();
        }
        else
        {
            // Handle changing to prev flashcard
            currentSlide--;
            ChangeWord(false);
        }
    }

    // "Flips" the card to see definition
    private void FlipCard()
    {
        headerText.text += " means...";
        StartCoroutine(AnimateFlip());
        SetProgress();
    }

    private void UnflipCard()
    {
        headerText.text = currentPacket.Entries[currentWord].English_Word;
        StartCoroutine(AnimateUnflip());
    }

    // Sets progress bar based on furthestSlide, or on progressAmount if a valid amount is provided
    private void SetProgress(float progressAmount = -1f)
    {
        if (progressAmount < 0f || progressAmount > 1f)
        {
            progressAmount = (float)furthestSlide / (float)(2 * currentPacket.Entries.Count);
        }

        progressMask.fillAmount = progressAmount;
        for (int i = 0; i < progressStars.Length; i++)
        {
            if ((i + 1) / (float)progressStars.Length <= progressAmount)
            {
                progressStars[i].SetAchieved();
            }
        }
    }

    // Animates the flipping of the flashcard, preparing videos properly
    private IEnumerator AnimateFlip()
    {
        inputDisabler.SetActive(true);

        backScreen.gameObject.SetActive(true);
        definitionVideoPlayer.Pause();
        definitionVideoPlayer.frame = 0;
        Vector2 startPos = screenParent.anchoredPosition;
        Vector2 endPos = new Vector2(screenParent.anchoredPosition.x, -backScreen.anchoredPosition.y);
        
        float t = 0;
        while (t < 1)
        {
            screenParent.anchoredPosition = Vector2.Lerp(startPos, endPos, flipAnimCurve.Evaluate(t));
            t += Time.deltaTime / flipTime;
            yield return null;
        }
        // Set final position since we overshoot t == 1
        screenParent.anchoredPosition = endPos;

        definitionVideoPlayer.Play();
        frontScreen.gameObject.SetActive(false);
        definitionText.gameObject.SetActive(true);
        definitionAnimator.PlayAnimation();
        prevArrow.gameObject.SetActive(true);
        inputDisabler.SetActive(false);

        if (currentWord < currentPacket.Entries.Count - 1)
        {
            PrepVideo(wordVideoPlayer, currentPacket.Entries[currentWord + 1].ASL_Sign_and_Spelled);
        }
        yield break;
    }

    // Animates the "unfilpping" of the flashcard (goes back to word)
    private IEnumerator AnimateUnflip()
    {
        inputDisabler.SetActive(true);

        PrepVideo(wordVideoPlayer, currentPacket.Entries[currentWord].ASL_Sign_and_Spelled);
        definitionText.gameObject.SetActive(false);
        frontScreen.gameObject.SetActive(true);
        Vector2 startPos = screenParent.anchoredPosition;
        Vector2 endPos = new Vector2(screenParent.anchoredPosition.x, 0);

        float t = 0;
        while (t < 1)
        {
            screenParent.anchoredPosition = Vector2.Lerp(startPos, endPos, flipAnimCurve.Evaluate(t));
            t += Time.deltaTime / flipTime;
            yield return null;
        }
        // Set final position since we overshoot t == 1
        screenParent.anchoredPosition = endPos;

        wordVideoPlayer.Play();
        backScreen.gameObject.SetActive(false);
        if (currentWord == 0) prevArrow.gameObject.SetActive(false);
        inputDisabler.SetActive(false);

        // Can't prep video since going back from here will go back to the previous word slide, not definition,
        // and we need the word video to show the current word
        yield break;
    }

    private void PrepVideo(VideoPlayer videoPlayer, string url)
    {
        videoPlayer.url = url;
        videoPlayer.Pause();
        videoPlayer.frame = 0;
    }

    // Move to the flashcard for the next word
    private void ChangeWord(bool isForward = true)
    {
        // "Push" timer entry
        if (stopwatch.TimerActive) PushTimer();
        stopwatch.StartWatch();

        // subtract 1 because slide 0 is the start slide
        currentWord = (currentSlide - 1) / 2;
        VocabularyEntry currentFlashcard = currentPacket.Entries[currentWord];

        SetProgress();

        if (currentWord > 0) prevArrow.gameObject.SetActive(true);
        else prevArrow.gameObject.SetActive(false);

        // Prep flashcard display
        Debug.Log($"current packet: {JsonConvert.SerializeObject(currentPacket, Formatting.Indented)}");
        Debug.Log($"current flashcard: {JsonConvert.SerializeObject(currentFlashcard, Formatting.Indented)}");
        definitionText.gameObject.SetActive(false);
        headerText.text = currentFlashcard.English_Word;
        definitionText.text = currentFlashcard.English_Definition;
        flashcardIcon.sprite = GlobalManager.Instance.GetIcon(currentFlashcard.Vocabulary_ID);
        definitionVideoPlayer.url = currentFlashcard.ASL_Definition;

        // Display flashcard
        if (!isForward) PrepVideo(wordVideoPlayer, currentFlashcard.ASL_Sign_and_Spelled);
        frontScreen.gameObject.SetActive(true);
        wordVideoPlayer.Play();
        screenParent.anchoredPosition = new Vector2(screenParent.anchoredPosition.x, 0f);
        backScreen.gameObject.SetActive(false);
    }

    private void PushTimer()
    {
        timeSpentOnWords[currentWord] += stopwatch.StopWatch();
    }

    // Move to the congratulations screen
    private void MoveToEnd()
    {
        PushTimer();
        definitionText.gameObject.SetActive(false);
        flashcardMask.gameObject.SetActive(false);
        nextArrow.gameObject.SetActive(false);
        prevArrow.gameObject.SetActive(false);
        headerText.text = "You did it!";
        SetProgress(1f);
        progressStars[progressStars.Length - 1].SetAchieved();
        winScreen.gameObject.SetActive(true);

        if (canPost) PostFlashcardData();
        confettiParticles.Play();
    }

    private void PostFlashcardData()
    {
        // Important to remember this copies by reference. We are changing the GlobalManager's LessonData object here.
        LessonData newLessonData = GlobalManager.Instance.currentLessonData;
        UpdateFlashcardTimes(newLessonData);
        newLessonData.flashcardsComplete = true;
        Debug.Log($"Full object:\n {JsonConvert.SerializeObject(newLessonData, Formatting.Indented)}");
        PostLesson(newLessonData);
    }

    private void UpdateFlashcardTimes(LessonData lessonData)
    {
        for (int i = 0; i < currentPacket.Entries.Count; i++)
        {
            int idx = currentPacket.Entries[i].Vocabulary_ID;
            float prevTime = 0f;
            if (lessonData.flashcardData.ContainsKey(idx))
            {
                prevTime = lessonData.flashcardData[idx];
            }
            lessonData.flashcardData[idx] = timeSpentOnWords[i] + prevTime;
        }
    }

    public void PostLesson(LessonData lessonData)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>{
                {$"Lesson {lessonData.packetID}",JsonConvert.SerializeObject(lessonData)}
            }
        };
        PlayFabClientAPI.UpdateUserData(request,
            res => Debug.Log("Successful lesson user data sent!"),
            err => Debug.LogError(err));
    }


    public void ExitToMap()
    {
        // TODO: Scene logic
        SceneManager.LoadScene("MapLayoutScene");
    }
}
