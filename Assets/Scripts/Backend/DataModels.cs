using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;




[System.Serializable]
    public class AllLoginSessions
    {

        public List<LoginSession> loginSessionList;

        public AllLoginSessions(){
            loginSessionList = new List<LoginSession>();
        }


    }

    [System.Serializable]
    public class LessonData
    {
		public int packetID; 
        public Dictionary<int,float> flashcardData;   //Will populate with wordID:TIMESPENT in flashcard area
		public Dictionary<int,Dictionary<string,int>> gameVocabCountDict; //Will populate with wordID:Dict<TypeOfRepresentationOfWord:Counts>
		
		public bool isUnlocked = false; //Manipulated manually by researchers
		public bool gameSessionComplete = false;
		public bool flashcardsComplete = false;
		public bool lessonComplete = false; 


		public LessonData()
        {
            flashcardData = new Dictionary<int, float>();
            gameVocabCountDict = new Dictionary<int, Dictionary<string, int>>();
        }
		
    }

	[System.Serializable]
	public class ReviewData
    {

		public int reviewID; 
		public Dictionary<int,Dictionary<string,int>> gameVocabCountDict; //Will populate with wordID:Dict<TypeOfRepresentationOfWord:Counts>		
		
		public List<QuizQuestionObject> quizQuestionObjectList; //Will populate with QuizQuestionObjectID: QuizQuestionObject data
		public bool isUnlocked = false; //Manipulated manually by researchers
		public bool gameSessionComplete;
		public bool quizComplete;
		public bool lessonComplete; 

		public ReviewData(){
			gameVocabCountDict = new Dictionary<int,Dictionary<string,int>>();
			quizQuestionObjectList = new List<QuizQuestionObject>();
		}
		
    }

	[System.Serializable]

	public class QuizQuestionObject{
		public bool successfulAnswer = false;
		public int numAttempts = 0; 
		public int vocabID;


	}


	[System.Serializable]

	public class LoginSession{
		public string date; 
		public List<int> packetsInteractedWith; //ID of the lessons/reviews interacted with, reviews will start with 1010 (ex. if I interact with review 2, it will show up as 101002)

		public List<GameSession> gameSessionList;


        public LoginSession(){
            DateTime currentDateTime = DateTime.Now;

            string dateTimeString = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.Log("Current Date and Time: " + dateTimeString);
            this.date = dateTimeString;

			packetsInteractedWith = new List<int>();
			gameSessionList = new List<GameSession>();
        }
	}


	[System.Serializable]

	public class GameSession{ //A class representing a single instance of a game being played
		public bool exitPressed; //if exit button pressed
		public bool tutorialPressed;
		public float timeSpent; 
		public int arcadeGameID; //Each game should have ID. Maze = 0, SignIt = 1, StreetSigns = 2
		public int sessionScore; 
		public int ticketsEarned;

	}






public class DataModels : MonoBehaviour
{
	public static DataModels Instance;

	// public VocabularyLoader vocabLoader; 
	//Initializing lessons and reviews

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public AllLoginSessions InitializeAllLoginSessions()
	{
		AllLoginSessions allLoginSessions = new AllLoginSessions();
		return allLoginSessions;
	}


    public ReviewData InitializeReviewFromVocabulary(int[] packetIDList){
        ReviewData review = new ReviewData
        {
            gameVocabCountDict = new Dictionary<int,Dictionary<string,int>>(),
            quizQuestionObjectList = new List<QuizQuestionObject>(),
            isUnlocked = false,
            gameSessionComplete = false,
            quizComplete = false,
            lessonComplete = false
            
        };

        foreach(int packetID in packetIDList){
            // var relevantEntries = vocabLoader.VocabularyData.Packets
			var relevantEntries = VocabularyLoader.Instance.VocabularyData.Packets
                .SelectMany(packet => packet.Entries)
                .Where(entry => entry.Packet == packetID);

            // Populate flashcardData and gameVocabCountDict based on filtered entries
            foreach (var entry in relevantEntries)
            {
                // Create a new quiz question object
                QuizQuestionObject tempQuestion = new QuizQuestionObject(); 
                tempQuestion.vocabID = entry.Vocabulary_ID;
                review.quizQuestionObjectList.Add(tempQuestion); // Initial time spent is set to 0

                // Populate the gameVocabCountDict dictionary
                review.gameVocabCountDict.Add(entry.Vocabulary_ID, new Dictionary<string, int>
                {
                    {"Icon", 0},
                    {"ASL_Definition", 0},
                    {"ASL_Sign", 0},
                    {"English_Definition", 0}
                });
            }

        }

        return review;



    }

	public LessonData InitializeLessonFromVocabulary(int lessonPacket)
    {
        // Create a new LessonData object
        LessonData lesson = new LessonData
        {
            flashcardData = new Dictionary<int, float>(),
            gameVocabCountDict = new Dictionary<int, Dictionary<string, int>>(),
            isUnlocked = lessonPacket == 0 ? true: false, // Or set this based on your logic
            gameSessionComplete = false,
            flashcardsComplete = false,
            lessonComplete = false
        };

        // Filter entries based on the lessonPacket
        // var relevantEntries = vocabLoader.VocabularyData.Packets
		var relevantEntries = VocabularyLoader.Instance.VocabularyData.Packets
            .SelectMany(packet => packet.Entries)
            .Where(entry => entry.Packet == lessonPacket);

        // Populate flashcardData and gameVocabCountDict based on filtered entries
        foreach (var entry in relevantEntries)
        {
            // Populate the flashcardData dictionary
            lesson.flashcardData.Add(entry.Vocabulary_ID, 0f); // Initial time spent is set to 0

            // Populate the gameVocabCountDict dictionary
            lesson.gameVocabCountDict.Add(entry.Vocabulary_ID, new Dictionary<string, int>
            {
                {"Icon", 0},
                {"ASL_Definition", 0},
                {"ASL_Sign", 0},
                {"English_Definition", 0}
            });
        }

        return lesson;
    }


}
