using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject word; // Word prefab already contains TextMeshPro, or at least it should
    [SerializeField] private GameObject[] powerUps; // Power-up prefabs in array

    [Header("World Bounds and Parameters")]
    // Set bounds for where words spawn
	[SerializeField] private Vector2 xBound;
	[SerializeField] private Vector2 yBound;
    [SerializeField] public float fallingSpeed = 0.2f;
    [SerializeField] private float spawnRate = 1.0f;

	[Header("References")]
	[SerializeField] private SignItGameManager gameManager;
    [SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private Image imageHolder;
	[SerializeField] private Sprite defaultIconToShow;
    private RawImage rawImage;

    private List<string> currentWordsToSpawn = new List<string>();
	private List<string> wordsNotYetSpawned;
    private int currentWordsToSpawnSize = 6;

    // Specific correct word/link chosen at period
	private List<VocabularyEntry> allPossibleVocabEntries;
    public string CorrectWord = "";
	private VocabularyEntry CorrectEntry;
    private bool isSpawnerActive;

	public enum SICIQuestionType
	{
		ASLSignToEnglishWord,
		IconToEnglishWord,
	}

    //private List<string> vidVocabList;

    // Start is called before the first frame update
    private void Start()
    {
        // Make videoplayer transparent
        rawImage = videoPlayer.gameObject.GetComponent<RawImage>();
        rawImage.color = new Color32(255, 255, 255, 0);

		// Generate list of VocabularyEntries to use in game
		allPossibleVocabEntries = VocabularyLoader.Instance.CreateVocabularyEntryListToUse(GlobalManager.Instance.CurrentPacket, GlobalManager.Instance.ReviewPreviousPackets);

		gameManager.OnGameActivated += StartSpawningWords;
    }

    public void StartSpawningWords()
    {
		// Handle difficulty setting
        switch (SignItGlobals.difficulty)
		{
			case SignItGlobals.Difficulty.Easy:
                fallingSpeed = 0.15f;
                spawnRate = 1.2f;
				break;
			case SignItGlobals.Difficulty.Medium:
				fallingSpeed = 0.25f;
                spawnRate = 1.0f;
				break;
			case SignItGlobals.Difficulty.Hard:
				fallingSpeed = 0.35f;
                spawnRate = 0.8f;
				break;
		}
		
        isSpawnerActive = true;
        ChangeCorrectWord();
        rawImage.color = new Color32(255, 255, 255, 255);
        StartCoroutine(SpawnRandomGameObject());
    }

    public void StopSpawningWords()
    {
        isSpawnerActive = false;
        StopCoroutine(SpawnRandomGameObject());
    }

    public IEnumerator SpawnRandomGameObject()
    {
        if (!isSpawnerActive) yield break;

        yield return new WaitForSeconds(spawnRate);

		int randomVocabWordIndex;
		string wordText;
		if (wordsNotYetSpawned.Count >= 1)
		{
			randomVocabWordIndex = Random.Range(0, wordsNotYetSpawned.Count);
			wordText = wordsNotYetSpawned[randomVocabWordIndex];
			wordsNotYetSpawned.RemoveAt(randomVocabWordIndex);
		}
		else
		{
			randomVocabWordIndex = Random.Range(0, currentWordsToSpawn.Count);
			wordText = currentWordsToSpawn[randomVocabWordIndex];
		}
        if (word.GetComponent<TextMeshPro>() != null)
        {

            word.GetComponent<TextMeshPro>().text = wordText;
            Rigidbody2D wordRigidBody = word.GetComponent<Rigidbody2D>();
            wordRigidBody.gravityScale = fallingSpeed;
            GameObject tmp = Instantiate(word, new Vector2(Random.Range(xBound.x, xBound.y), Random.Range(yBound.x, yBound.y)), Quaternion.identity);
            tmp.transform.SetParent(transform, false);
        }
        else
        {
            Debug.Log("Problem with word! TextMeshPro doesn't exist!");
        }

        if (Random.Range(0, 10) == 0)
        {
            StartCoroutine(SpawnRandomPowerUp());
        }
        StartCoroutine(SpawnRandomGameObject());

        yield return new WaitForSeconds(SignItGlobals.spawnRate);
    }

    IEnumerator SpawnRandomPowerUp()
    {
        yield return new WaitForSeconds(Random.Range(1, 2));

        int randomPowerUp = Random.Range(0, powerUps.Length);
        GameObject tmp = Instantiate(powerUps[randomPowerUp], new Vector2(Random.Range(xBound.x, xBound.y), Random.Range(yBound.x, yBound.y)), Quaternion.identity);
        tmp.transform.SetParent(transform, false);

    }

    // public void ReadFromFileJSON()
    // {
    //     //Debug.Log("about to read file");
    //     // feed in textasset.text, add json file as text asset to a game object (forces load)
    //     Questions questionsjson = JsonUtility.FromJson<Questions>(jsonFile.text);
    //     //Debug.Log("file read");
    //     foreach (Question q in questionsjson.questions)
    //     {
    //         links.Add(q.Link);
    //         words.Add(q.Word);
    //     }
    // }

    public void ChangeCorrectWord()
    {
		List<VocabularyEntry> levelVocabList = allPossibleVocabEntries;
        
        int randomWordIndex = Random.Range(0, levelVocabList.Count);
        // string randomWord = levelVocabList[randomWordIndex];
		VocabularyEntry randomWord = levelVocabList[randomWordIndex];
        // CorrectWord = randomWord;
		CorrectEntry = randomWord;
		CorrectWord = randomWord.English_Word;
        currentWordsToSpawn.Clear();
    
        if (levelVocabList.Count <= currentWordsToSpawnSize)
        {
			foreach (VocabularyEntry entry in levelVocabList)
			{
				currentWordsToSpawn.Add(entry.English_Word);
			}
            // currentWordsToSpawn.AddRange(levelVocabList);
        }
        else
        {
            currentWordsToSpawn.Add(CorrectWord);
            
            for (int i = 0; i < currentWordsToSpawnSize; i++)
            {
                int randomVocabWordIndex = Random.Range(0, levelVocabList.Count);
                currentWordsToSpawn.Add(levelVocabList[randomVocabWordIndex].English_Word);
            }
        }

		wordsNotYetSpawned = new List<string>(currentWordsToSpawn);

		// Choose whether to show video or icon of word
		int choice = Random.Range(0, 2);
		if (choice == 1)
		{
			// Let's show a video
			imageHolder.gameObject.SetActive(false);
			videoPlayer.gameObject.SetActive(true);
			Debug.Log($"About to play video: {CorrectWord}");
			videoPlayer.url = CorrectEntry.ASL_Sign;
			videoPlayer.Play();

			if (GlobalManager.Instance.ReviewPreviousPackets)
			{
				// We are a review
				GlobalManager.Instance.currentReviewData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["ASL_Sign"] += 1;
			}
			else
			{
				// We are a lesson
				GlobalManager.Instance.currentLessonData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["ASL_Sign"] += 1;
			}
		}
		else
		{
			// Let's show an icon
			videoPlayer.gameObject.SetActive(false);
			imageHolder.gameObject.SetActive(true);
			Debug.Log($"About to show icon: {CorrectWord}");
			imageHolder.sprite = GlobalManager.Instance.GetIcon(CorrectEntry.Vocabulary_ID);

			if (GlobalManager.Instance.ReviewPreviousPackets)
			{
				// We are a review
				GlobalManager.Instance.currentReviewData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["Icon"] += 1;
			}
			else
			{
				// We are a lesson
				GlobalManager.Instance.currentLessonData.gameVocabCountDict[CorrectEntry.Vocabulary_ID]["Icon"] += 1;
			}
		}
    }

	private void OnDestroy()
	{
		gameManager.OnGameActivated -= StartSpawningWords;
	}
}
