using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VocabularyEntry
{
    public int Packet;
    public int Vocabulary_ID;
    public string English_Word;
    public string ASL_Sign_and_Spelled;
    public string ASL_Definition;
    public string ASL_Sign;
    public string ASL_Spelled;
    public string English_Definition;
}

[System.Serializable]
public class VocabularyPacket
{
    public string PacketName;
    public List<VocabularyEntry> Entries;
}

[System.Serializable]
public class VocabularyData
{
    public List<VocabularyPacket> Packets;
}


public class VocabularyLoader : MonoBehaviour
{
	// private static VocabularyLoader instance;
	// public static VocabularyLoader Instance
	// {
	// 	get
	// 	{
	// 		if (instance != null) return instance;

	// 		instance = FindObjectOfType<VocabularyLoader>();
	// 		if (instance == null)
	// 		{
	// 			Debug.LogWarning("No VocabularyLoader in the scene!");
	// 			return null;
	// 		}

	// 		return instance;
	// 	}
	// }

	public static VocabularyLoader Instance;

    public TextAsset vocabularyJson; // Assign the .txt file here via the Inspector
    public VocabularyData VocabularyData;
	
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

    void Start()
    {
        LoadVocabularyData();
    }

    void LoadVocabularyData(bool Verbose=false)
    {
        if (vocabularyJson != null)
        {
            string jsonContent = vocabularyJson.text;
            VocabularyData = JsonUtility.FromJson<VocabularyData>(jsonContent);
			if (Verbose){
				if (VocabularyData != null && VocabularyData.Packets != null)
				{
					Debug.Log("Vocabulary data loaded successfully!");

					// Example: Accessing data
					foreach (var packet in VocabularyData.Packets)
					{
						Debug.Log($"Packet: {packet.PacketName}");
						foreach (var entry in packet.Entries)
						{
							Debug.Log($"Word: {entry.English_Word}, Definition: {entry.English_Definition}");
						}
					}
				}
				else
				{
					Debug.LogError("Failed to parse JSON. Check the JSON structure.");
				}
			}
		}
		else
		{
			Debug.LogError("Vocabulary JSON not assigned!");
		}
	
    }

	public List<VocabularyEntry> CreateVocabularyEntryListToUse(int currentPacket, bool shouldReviewPreviousPackets)
	{
		List<VocabularyEntry> toReturn = new List<VocabularyEntry>();

		List<int> packetsIndicesToLookThrough = new List<int>();
		if (shouldReviewPreviousPackets)
		{
			for (int i = 0; i < currentPacket; i++)
			{
				packetsIndicesToLookThrough.Add(i);
			}
		}
		packetsIndicesToLookThrough.Add(currentPacket);

		foreach (int packetIndex in packetsIndicesToLookThrough)
		{
			foreach (VocabularyEntry entry in VocabularyData.Packets[packetIndex].Entries)
			{
				toReturn.Add(entry);
			}
		}

		return toReturn;
	}
}
