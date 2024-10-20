using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TileBehavior : MonoBehaviour
{
    private GameObject tiles;
    public GameObject[] tilesets;
    private int set_index;

    private static List<string> links = new List<string>(); // list of web links to signs
    private static List<string> words = new List<string>(); // list of vocabulary words

    public GameObject wordTextA, wordTextB; // textbox for words to display
    public GameObject videoA, videoB; // video player
    public TMP_Text scoreBox; // textbox for score display

    public Sprite redcard, bluecard, uisprite; // backs of cards, to change if it is a word vs an image
    public GameObject winText;
    public GameObject doneButton;
    public GameObject hintsButton;
    public GameObject errorbox;

    bool firstTileOpen;
    bool correct;
    GameObject firstTile, secondTile;
    public static int score;

    bool card_separation;
    bool hints, hintsActivated;
    bool hintsDisplayed;
    float timer;
    List<GameObject> htiles = new List<GameObject>();

    public List<TextAsset> questionsList = new List<TextAsset>(3);
    // public TextAsset questions;



    /*
     * Called before the first frame update.
     */
    void Start()
    {
        

        set_index = 0;

        card_separation = true;
        hints = true;

        errorbox.SetActive(false);

        // load words + video links
        ReadFromFileJSON();

        ResetTiles();
        //RandomLoadTilesJump();
    }

    /*
     * Called once per frame. Currently handles hints display.
     */
    void Update()
    {
        if (hintsActivated && !hintsDisplayed)
        {
            // by this point it is guaranteed that the first tile is open. thus it is okay to try hints
            // check for hints enabled is in the method already.
            //Debug.Log("displaying hints");
            hintsDisplayed = true;
            htiles = SetHints(); // set up the tiles for the hints
        }

        if (hintsDisplayed)
        {
            float fadeTime = 1.15f;
            // for each tile in the hints set, change the color
            foreach (GameObject tile in htiles)
            {
                tile.GetComponent<Image>().color = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, fadeTime));
            }

            // if we get a mouse input, stop lerping and reset the timer
            if (Input.GetMouseButtonDown(0))
            {
                foreach (GameObject x in htiles)
                {
                    x.GetComponent<Image>().color = Color.white;
                }
                hintsDisplayed = false;
                hintsActivated = false;
                htiles.Clear();
            }
        }
    }

    public void ChangeVocab(TextAsset q)
    {
        // switch vocab sets
        ReadFromFileJSON();

        ResetTiles();
    }

    /*
     * Toggles the menu, for the menu button.
     */
    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }

    /**
     * Sets hints to activated once hints button is pressed.
     */
    public void ActivateHints()
    {
        if (firstTileOpen)
            hintsActivated = true;
        else
        {
            errorbox.SetActive(true);
            errorbox.transform.GetChild(0).GetComponent<TMP_Text>().text = "You need to select a tile before you can view hints for a match.";
            StartCoroutine(ErrorWait());
        }
    }

    /*
     * Randomize the board.
     */
    public void ResetTiles()
    {
        if (tiles != null)
        {
            foreach (Transform tile in tiles.transform)
            {
                tile.gameObject.SetActive(true);
                tile.gameObject.GetComponent<TileProperties>().SetWordContent("null");
                tile.gameObject.GetComponent<TileProperties>().SetVideoContent("null");
                tile.gameObject.GetComponent<TileProperties>().occupied = false;
                //tile.gameObject.GetComponent<Image>().sprite = uisprite;
                tile.gameObject.GetComponent<Button>().interactable = true;
            }
            tiles.SetActive(false);
        }
        tiles = tilesets[set_index];
        tiles.SetActive(true);

        wordTextA.SetActive(false);
        videoA.SetActive(false);
        wordTextB.SetActive(false);
        videoB.SetActive(false);

        firstTileOpen = false;
        score = 0;
        winText.SetActive(false);

        RandomLoadTilesJump();
    }

    /*
     * Chooses the tiles to be included in the hints.
     */
    public List<GameObject> SetHints()
    {
        if (firstTile == null || (firstTile != null && secondTile != null)) return new List<GameObject>();

        //Debug.Log("hints enabled: " + hints);
        List<GameObject> htiles = new List<GameObject>();
        if (hints)
        {
            bool matchIsWord = false;

            // find correct answer and add to list
            int match = -1;
            if (firstTile.gameObject.GetComponent<TileProperties>().GetWordContent() == "null")
            {
                // get the matching word
                match = links.IndexOf(firstTile.gameObject.GetComponent<TileProperties>().GetVideoContent());
                matchIsWord = true;
            }
            else
            {
                // get the matching url
                match = words.IndexOf(firstTile.gameObject.GetComponent<TileProperties>().GetWordContent());
            }

            foreach (Transform tile in tiles.transform)
            {
                if (matchIsWord && tile.gameObject.GetComponent<TileProperties>().GetWordContent() == words[match]
                    || !matchIsWord && tile.gameObject.GetComponent<TileProperties>().GetVideoContent() == links[match])
                {
                    htiles.Add(tile.gameObject);
                }
            }
            //Debug.Log("correct answer found");

            // generate random numbers to select the rest
            int rand = -1;
            List<int> nums = new List<int>();
            nums.Add(match);
            for (int i = 0; i < 3 & tiles.GetComponentsInChildren<Transform>().GetLength(0) > 3; i++)
            {
                do
                {
                    rand = Random.Range(0, tiles.transform.childCount); // number of children -> randomly select a child
                } while (nums.IndexOf(rand) != -1); // while the random number has not already been selected
                nums.Add(rand);
                htiles.Add(tiles.transform.GetChild(rand).gameObject);
            }
        }
        return htiles;
    }

    /*
     * Enables and disables hints according to the menu settings.
     */
    public void ToggleHints(GameObject toggle)
    {
        GameObject handle = toggle.transform.GetChild(0).gameObject;
        handle.transform.localPosition = new Vector3(-handle.transform.localPosition.x, handle.transform.localPosition.y, handle.transform.localPosition.z);

        if (handle.transform.localPosition.x > 0)
        {
            toggle.GetComponent<Image>().color = new Color(212f / 255, 212f / 255, 212f / 255, 1);
            handle.GetComponent<Image>().color = new Color(227f / 255, 227f / 255, 227f / 255, 1);
        }
        else
        {
            toggle.GetComponent<Image>().color = new Color(44f / 255, 229f / 255, 158f / 255, 1);
            handle.GetComponent<Image>().color = new Color(20f / 255, 1, 173f / 255, 1);
        }

        hints = !hints;
        hintsButton.SetActive(!hintsButton.activeSelf);
        ResetTiles();
        //Debug.Log("hints changed to " + hints + ", tiles reset!");
    }

    /*
     * Changes the colors of the tiles according to the menu settings.
     */
    public void TogglePatterns(GameObject toggle)
    {
        GameObject handle = toggle.transform.GetChild(0).gameObject;
        handle.transform.localPosition = new Vector3(-handle.transform.localPosition.x, handle.transform.localPosition.y, handle.transform.localPosition.z);

        if (handle.transform.localPosition.x > 0)
        {
            toggle.GetComponent<Image>().color = new Color(212f / 255, 212f / 255, 212f / 255, 1);
            handle.GetComponent<Image>().color = new Color(227f / 255, 227f / 255, 227f / 255, 1);
        }
        else
        {
            toggle.GetComponent<Image>().color = new Color(44f / 255, 229f / 255, 158f / 255, 1);
            handle.GetComponent<Image>().color = new Color(20f / 255, 1, 173f / 255, 1);
        }

        card_separation = !card_separation;
        ResetTiles();
        //Debug.Log("cards changed to " + card_separation + ", tiles reset!");
    }

    /**
     * Play Again once the game is won.
     */
    public void PlayAgain()
    {
        ResetTiles();
        winText.SetActive(false);
    }

    /*
     * Changes the size of the board according to the menu settings.
     */
    public void SetBoardSize(int index)
    {
        if (index == 0 && words.Count < 6 || index == 1 && words.Count < 15 || index == 2 && words.Count < 28)
        {
            errorbox.SetActive(true);
            errorbox.transform.GetChild(0).GetComponent<TMP_Text>().text = "You don't have enough words to fill this board.";

            StartCoroutine(ErrorWait());
            return;
        }

        set_index = index;
        ResetTiles();
        //Debug.Log("board index changed to " + set_index + ", tiles reset!");
    }

    /*
     * Called when a tile is clicked. Dictates tile behavior, shows content, and determines if a match is correct.
     */
    public void RevealTile()
    {
        // play flip animation
        //EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = uisprite;

        // reveal word/video
        string word = EventSystem.current.currentSelectedGameObject.GetComponent<TileProperties>().GetWordContent();
        string url = EventSystem.current.currentSelectedGameObject.GetComponent<TileProperties>().GetVideoContent();
        //Debug.Log("word: " + word + "\nurl: " + url);
        if (word != "null")
        {
            if (!firstTileOpen)
            {
                wordTextA.SetActive(true);
                wordTextA.GetComponent<TMP_Text>().text = word;
                firstTile = EventSystem.current.currentSelectedGameObject;
                //firstTile.GetComponent<Image>().sprite = uisprite;

                var colors = firstTile.GetComponent<Button>().colors;
                //colors.disabledColor = new Color(245f / 255, 10f / 255, 10f / 255, 128f / 255);
                colors.disabledColor = new Color(0, 189f / 255, 104f / 255, 128f / 255);
                firstTile.GetComponent<Button>().colors = colors;
            }
            else
            {
                secondTile = EventSystem.current.currentSelectedGameObject;
                //secondTile.GetComponent<Image>().sprite = uisprite;

                var colors = secondTile.GetComponent<Button>().colors;
                //colors.disabledColor = new Color(245f / 255, 10f / 255, 10f / 255, 128f / 255);
                colors.disabledColor = new Color(0, 189f / 255, 104f / 255, 128f / 255);
                secondTile.GetComponent<Button>().colors = colors;

                //Debug.Log("secondTile is " + secondTile);
                // lock user input once selected
                foreach (Transform tile in tiles.transform)
                {
                    tile.gameObject.GetComponent<Button>().interactable = false;
                }

                wordTextB.SetActive(true);
                wordTextB.GetComponent<TMP_Text>().text = word;
            }
        }
        else
        {
            if (!firstTileOpen)
            {
                videoA.SetActive(true);
                videoA.GetComponent<VideoPlayer>().url = url; // Application.streamingAssetsPath + "/Chemistry/Beaker.mp4";
                videoA.GetComponent<VideoPlayer>().Play();
                firstTile = EventSystem.current.currentSelectedGameObject;
                //firstTile.GetComponent<Image>().sprite = uisprite;

                var colors = firstTile.GetComponent<Button>().colors;
                //colors.disabledColor = new Color(10f / 255, 10f / 255, 245f / 255, 128f / 255);
                colors.disabledColor = new Color(0, 189f / 255, 104f / 255, 128f / 255);
                firstTile.GetComponent<Button>().colors = colors;
            }
            else
            {
                secondTile = EventSystem.current.currentSelectedGameObject;
                //secondTile.GetComponent<Image>().sprite = uisprite;

                var colors = secondTile.GetComponent<Button>().colors;
                //colors.disabledColor = new Color(10f / 255, 10f / 255, 245f / 255, 128f / 255);
                colors.disabledColor = new Color(0, 189f / 255, 104f / 255, 128f / 255);
                secondTile.GetComponent<Button>().colors = colors;

                // lock user input once selected
                foreach (Transform tile in tiles.transform)
                {
                    tile.gameObject.GetComponent<Button>().interactable = false;
                }

                videoB.SetActive(true);
                videoB.GetComponent<VideoPlayer>().url = url;
                videoB.GetComponent<VideoPlayer>().Play();
            }
        }

        // check if other tile has been opened yet
        if (!firstTileOpen)
        {
            firstTileOpen = true;
            firstTile.GetComponent<Button>().interactable = false;
        }
        else
        {
            // check if correct - if other tile contains words vs video
            if (firstTile != null && firstTile.GetComponent<TileProperties>().GetWordContent() != "null")
            {
                correct = links.IndexOf(url) == words.IndexOf(firstTile.GetComponent<TileProperties>().GetWordContent());
            }
            else if (firstTile != null)
            {
                correct = words.IndexOf(word) == links.IndexOf(firstTile.GetComponent<TileProperties>().GetVideoContent());
            }
            firstTileOpen = false;

            // display the DONE button
            doneButton.SetActive(true);

            // NOTE: also display a +10 next to the score, which goes away once the score is incremented in EndTurn()
        }
    }

    /*
     * To be called when the DONE button is pressed.
     */
    public void EndTurn()
    {
        if (correct)
        {
            // if correct, play fade animation + destroy tiles + increment score
            //Debug.Log("match");

            firstTile.SetActive(false);
            secondTile.SetActive(false);

            score += 10;
            scoreBox.text = score.ToString();
            // PlayFabManager.saveScore(score);
        }
        else
        {
            // if incorrect, play flip animation backwards and reset turn
            //Debug.Log("not a match");
        }

        // allow tiles to be clicked again
        foreach (Transform tile in tiles.transform)
        {
            tile.gameObject.GetComponent<Button>().interactable = true;
            tile.gameObject.GetComponent<Image>().color = Color.white;
        }

        // clear word and video display
        wordTextA.GetComponent<TMP_Text>().text = "";
        wordTextA.SetActive(false);
        videoA.GetComponent<VideoPlayer>().Pause();
        videoA.SetActive(false);

        wordTextB.GetComponent<TMP_Text>().text = "";
        wordTextB.SetActive(false);
        videoB.GetComponent<VideoPlayer>().Pause();
        videoB.SetActive(false);

        doneButton.SetActive(false);

        // make them their respective cards again
        if (card_separation && firstTile.GetComponent<TileProperties>().GetWordContent() != "null")
            firstTile.GetComponent<Image>().sprite = redcard;
        else
            firstTile.GetComponent<Image>().sprite = bluecard;

        if (card_separation && secondTile.GetComponent<TileProperties>().GetWordContent() != "null")
            secondTile.GetComponent<Image>().sprite = redcard;
        else
            secondTile.GetComponent<Image>().sprite = bluecard;

        // reset the tiles' disabled colors
        var colors = firstTile.GetComponent<Button>().colors;
        colors.disabledColor = new Color(200f / 255, 200f / 255, 200f / 255, 128f / 255);
        firstTile.GetComponent<Button>().colors = colors;
        secondTile.GetComponent<Button>().colors = colors;

        firstTile = null;
        secondTile = null;
        CheckForWin();
    }

    /*
     * Checks if the player has won the game after each match.
     */
    public void CheckForWin()
    {
        foreach (Transform tile in tiles.transform)
        {
            if (tile.gameObject.activeSelf)
            {
                //Debug.Log("not won yet");
                return;
            }
        }
        //Debug.Log("you won the game!");
        winText.SetActive(true);
    }

    /* The two random load functions take different approaches.
     * The systematic approach iterates through each tile, randomly
     * chooses if it has a word or video, and randomly assigns a word or video.
     * However, this depends on the length of the list of words being
     * exactly half of the number of tiles for even distribution.
     * For the record: I don't like this one. I might delete it, since it's not used.
     */
    void RandomLoadTilesSystematic()
    {
        // copy lists over to temporary lists; makes it easier to take out used pairs when randomly assigning without having to repopulate original lists.
        List<string> clinks = new List<string>();
        List<string> cwords = new List<string>();

        for (int i = 0; i < words.Count; i++)
        {
            clinks.Add(links[i]);
            cwords.Add(words[i]);
        }

        int content = -1;
        int rand = -1;

        foreach (Transform child in tiles.transform)
        {
            content = (int)System.Math.Round((double)Random.Range(0, 1)); // note: rounding untested. before using, check to make sure 1 is ever actually generated.
            if (content == 0 && cwords.Count > 0)
            {
                rand = Random.Range(0, cwords.Count);
                child.gameObject.GetComponent<TileProperties>().SetWordContent(cwords[rand]);
                cwords.Remove(cwords[rand]);
            }
            else
            {
                rand = Random.Range(0, clinks.Count);
                child.gameObject.GetComponent<TileProperties>().SetVideoContent(clinks[rand]);
                clinks.Remove(clinks[rand]);
            }
        }
    }

    /* The two random load functions take different approaches.
     * The jump approach generates a random match of word + video
     * and assigns them to two randomly selected tiles. It continues to
     * do this until it either runs out of matches to place or
     * the board is full (i.e. iterating through the assignment loop
     * for the number of tiles / 2).
     */
    void RandomLoadTilesJump()
    {
        //Debug.Log("Loading tiles");
        // copy lists over to temporary lists; makes it easier to take out used pairs when randomly assigning without having to repopulate original lists.
        List<string> clinks = new List<string>();
        List<string> cwords = new List<string>();
        Debug.Log($"Tile Count: {tiles.transform.childCount }");
        int numberOfTiles = tiles.transform.childCount;
        for (int i = 0; i < numberOfTiles; i++)
        {
            if (i < words.Count){
                clinks.Add(links[i]);
                cwords.Add(words[i]);
            }
            else if(i >= words.Count){
                int randomIndex = Random.Range(0,words.Count); 
                cwords.Add(words[randomIndex]);
                clinks.Add(links[randomIndex]);
            }
          
        }

        // randomize words + videos on tiles
        int randMatch = -1;
        int randTile = -1;
        int numTilesAvailable = tiles.transform.childCount;


        for (int i = 0; i < tiles.transform.childCount / 2; i++)
        {
            // choose a random pair and a random tile, while tile is not occupied
            randMatch = Random.Range(0, clinks.Count);
            do
            {
                randTile = Random.Range(0, tiles.transform.childCount);
            } while (numTilesAvailable > 0 && tiles.transform.GetChild(randTile).gameObject.GetComponent<TileProperties>().occupied);

            //Debug.Log("match number: " + randMatch + "\ntile number: " + randTile);

            // assign the word to first random tile
            tiles.transform.GetChild(randTile).gameObject.GetComponent<TileProperties>().SetWordContent(cwords[randMatch]);
            if (card_separation)
                tiles.transform.GetChild(randTile).gameObject.GetComponent<Image>().sprite = redcard;
            else
                tiles.transform.GetChild(randTile).gameObject.GetComponent<Image>().sprite = bluecard;

            Debug.Log("word assigned to " + tiles.transform.GetChild(randTile).name);

            // decrement amount of available tiles and pick a new random tile for the video
            numTilesAvailable--;
            do
            {
                randTile = Random.Range(0, tiles.transform.childCount);
            } while (numTilesAvailable > 0 && tiles.transform.GetChild(randTile).gameObject.GetComponent<TileProperties>().occupied);
            tiles.transform.GetChild(randTile).gameObject.GetComponent<Image>().sprite = bluecard;

            //Debug.Log("match number: " + randMatch + "\ntile number: " + randTile);

            // assign matching video to second random tile
            tiles.transform.GetChild(randTile).gameObject.GetComponent<TileProperties>().SetVideoContent(clinks[randMatch]);

            Debug.Log("video url assigned to " + tiles.transform.GetChild(randTile).name);

            // remove pair from list
            clinks.Remove(clinks[randMatch]);
            cwords.Remove(cwords[randMatch]);

            // a just in case check, if something goes wrong (ex. less than 15 words in the list).
            if (clinks.Count <= 0 || cwords.Count <= 0 || numTilesAvailable <= 0) break;
        }
    }

    /*
     * Read the words and links from a CSV file.
     */
    public void ReadFromFileCSV()
    {
        // read questions from file + associated answers
        using (var reader = new StreamReader(@Application.streamingAssetsPath + "/questions.csv")) // placeholder; change to vocabulary
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                // Debug.Log(line);

                words.Add(values[0]);
                links.Add(values[1]);
            }
        }
    }

    /*
     * Read the words and links from a JSON file.
     */
    public void ReadFromFileJSON()
    {
        words.Clear();
        links.Clear();
        //Debug.Log("about to read file");
        // feed in textasset.text, add json file as text asset to a game object (forces load)
        

        //Questions questionsjson = JsonUtility.FromJson<Questions>(qs.text);
    
        // TODO: uncomment below lines
        // List<WordPack> wordPacks =  GlobalManager.currentLesson.wordPackList;


        // //Debug.Log("file read");
        // foreach (WordPack wordPack in wordPacks)
        // {
        //     //string fullLink = Application.streamingAssetsPath + q.Link;  //for streamingassets
        //     string fullLink = wordPack.videoLink;
        //     links.Add(fullLink);
        //     words.Add(wordPack.word);
        // }
    }

    /**
     * Wait time for in-game error messages.
     */
    IEnumerator ErrorWait()
    {
        yield return new WaitForSeconds(4);
        errorbox.SetActive(false);
    }
}

// ----------------------------------------------- JSON READING CLASSES ---------------------------------------------

// [System.Serializable]
// public class Question
// {
//     //these variables are case sensitive and must match the strings "Word" and "Link" in the JSON.
//     public string Word;
//     public string Link;
// }

// [System.Serializable]
// public class Questions
// {
//     //employees is case sensitive and must match the string "questions" in the JSON.
//     public Question[] questions;
// }