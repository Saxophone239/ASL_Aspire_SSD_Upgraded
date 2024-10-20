using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataUnitTests : MonoBehaviour     //Testing models and routes without requiring the full app loaded
{

    public Login login;
    public  PlayfabPostManager postManager;
    public PlayfabGetManager getManager;
    public DataModels dataModels;
 
    void Start(){
        login.StudentLoginActivate("Student2");
    }
 
    //General Tests

    public void CheckGlobalLesson(){
        int currentID = GlobalManager.Instance.currentLessonData.packetID;
        Debug.Log($"Current Global Lesson: {currentID}");
    }

    public void CheckGlobalReview(){
        int currentID = GlobalManager.Instance.currentReviewData.reviewID;
        Debug.Log($"Current Global Review: {currentID}");
    }

    public void CheckEmptyLessonCreation(){
        int lessonID = 1; 
        LessonData emptyLesson = dataModels.InitializeLessonFromVocabulary(lessonID);
        postManager.PostLesson(emptyLesson);
    }

    public void CheckReviewCreation(){
        int[] packetList = {1,2,3};
        ReviewData emptyReview = dataModels.InitializeReviewFromVocabulary(packetList);
        postManager.PostReview(emptyReview);
    }

    //Post Route Testing
    public void PostLessonDataTest(){
        LessonData lesson = new LessonData();
        lesson.packetID = 100;
        // Populating flashcardData with example word IDs and time spent
        lesson.flashcardData.Add(101, 3.5f); // wordID: 101, timeSpent: 3.5 seconds
        lesson.flashcardData.Add(102, 5.0f); // wordID: 102, timeSpent: 5.0 seconds

        // Populating gameVocabCountDict with example word IDs and their representations
        lesson.gameVocabCountDict.Add(101, new Dictionary<string, int>
        {
            {"SIGNEDWORD-ENGWORD", 2},  // TypeOfRepresentation: SIGNEDWORD-ENGWORD
            {"ICON-ENGWORD", 1}   // TypeOfRepresentation: ICON-ENGWORD, Counts: 1
        });
        lesson.gameVocabCountDict.Add(102, new Dictionary<string, int>
        {
            {"SIGNEDWORD-ENGWORD", 1},  
            {"ENGDEF-ENGWORD", 3},
            {"ICON-ENGWORD",1}   
        });

        lesson.isUnlocked = true;
        lesson.gameSessionComplete = false;
        lesson.flashcardsComplete = true;
        lesson.lessonComplete = false;

        Debug.Log("Flashcard Data:");
        foreach (var item in lesson.flashcardData)
        {
            Debug.Log($"WordID: {item.Key}, Time Spent: {item.Value} seconds");
        }

        Debug.Log("\nGame Vocab Count Data:");
        foreach (var item in lesson.gameVocabCountDict)
        {
            Debug.Log($"WordID: {item.Key}");
            foreach (var subItem in item.Value)
            {
                Debug.Log($"   Type: {subItem.Key}, Count: {subItem.Value}");
            }
        }

        Debug.Log($"\nIs Unlocked: {lesson.isUnlocked}");
        Debug.Log($"Game Session Complete: {lesson.gameSessionComplete}");
        Debug.Log($"Flashcards Complete: {lesson.flashcardsComplete}");
        Debug.Log($"Lesson Complete: {lesson.lessonComplete}");


        //Attempt Post
        postManager.PostLesson(lesson);         

    }


    public void QuizQuestionObjectTest()
    {
        QuizQuestionObject quizQuestion = new QuizQuestionObject();

        quizQuestion.successfulAnswer = true;
        quizQuestion.numAttempts = 2;
        quizQuestion.vocabID = 101;


        Debug.Log("Quiz Question Object Data:");
        Debug.Log($"Successful Answer: {quizQuestion.successfulAnswer}");
        Debug.Log($"Number of Attempts: {quizQuestion.numAttempts}");
        Debug.Log($"Vocabulary ID: {quizQuestion.vocabID}");


        // postManager.PostQuizQuestion(quizQuestion);
    }


 public void PostReviewTest()
    {
        ReviewData review = new ReviewData();
        review.reviewID = 12;
        // Populating gameVocabCountDict with example word IDs and their representations
        review.gameVocabCountDict.Add(101, new Dictionary<string, int>
        {
            {"ENGDEF-ENGWORD", 3},  
            {"ICON-ENGWORD", 2}   
        });

        review.gameVocabCountDict.Add(102, new Dictionary<string, int>
        {
            {"ASLSIGN-ENGWORD", 1},  
            {"ASLDEF-ENGWORD", 4}   
        });

        review.quizQuestionObjectList.Add(new QuizQuestionObject
        {
            successfulAnswer = true,
            numAttempts = 1,
            vocabID = 101,

        });

        review.quizQuestionObjectList.Add(new QuizQuestionObject
        {
            successfulAnswer = false,
            numAttempts = 3,
            vocabID = 102,
    
        });

        // Setting the other flags
        review.isUnlocked = true;
        review.gameSessionComplete = true;
        review.quizComplete = false;
        review.lessonComplete = false;

        Debug.Log("Review Data - Game Vocab Count:");
        foreach (var vocabEntry in review.gameVocabCountDict)
        {
            Debug.Log($"WordID: {vocabEntry.Key}");
            foreach (var representation in vocabEntry.Value)
            {
                Debug.Log($"   Type: {representation.Key}, Count: {representation.Value}");
            }
        }

      

        Debug.Log($"\nIs Unlocked: {review.isUnlocked}");
        Debug.Log($"Game Session Complete: {review.gameSessionComplete}");
        Debug.Log($"Lesson Complete: {review.lessonComplete}");

        postManager.PostReview(review);
    }


    public void InitializeLoginSessionsList(){
        AllLoginSessions allLoginSessions = new AllLoginSessions();
        LoginSession loginSession = new LoginSession();
        allLoginSessions.loginSessionList.Add(loginSession);
        postManager.PostAllLoginSessions(allLoginSessions);

    }
    //Get Route Testing

    public void GetLessonTest(int packetID){
        getManager.GetLessonData(packetID);
    }

    public void GetReviewTest(int reviewID){
        getManager.GetReviewData(reviewID);
    }

    public void GetSessionDataTest(){
        getManager.GetSessionDataCoroutine();

    }

}
