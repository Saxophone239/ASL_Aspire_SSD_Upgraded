using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.IO;
using TMPro;

public class QuestionsHardcode : MonoBehaviour
{
    private static List<string> links = new List<string>(); // list of web links to signs
    private static List<string> words = new List<string>(); // list of vocabulary words

    // Start is called before the first frame update
    void Start()
    {
        // load words + video links
        ReadFromFile();
    }

    List<string> getLinks()
    {
        return links;
    }

    List<string> getWords()
    {
        return words;
    }

    public void ReadFromFile()
    {
        // read questions from file + associated answers
        using (var reader = new StreamReader(@Application.streamingAssetsPath + "/vocabulary.csv")) // placeholder; change to vocabulary
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

    public void ReadFromStaticList()
    {
        // the hardcoded stuff for now, until we figure out how streamingassets works
        words.Add("Food Chain");
        links.Add("https://dl.dropbox.com/s/1sv2d0htt4fqixv/Food%20Chain.mp4?dl=1");
        words.Add("Food Web");
        links.Add("https://dl.dropbox.com/s/azkboxl7li06yce/Food%20Web.mp4?dl=1");
        words.Add("Consume");
        links.Add("https://dl.dropbox.com/s/jrvudi96nmmh1tj/Consume.mp4?dl=1");
        words.Add("Carnivore");
        links.Add("https://dl.dropbox.com/s/856bg8028tqikfj/Carnivore.mp4?dl=1");
        words.Add("Symbiosis");
        links.Add("https://dl.dropbox.com/s/kvma9uu2zae26cu/Symbiosis.mp4?dl=1");
        words.Add("Decomposer");
        links.Add("https://dl.dropbox.com/s/dzuk9e949s2ael0/Decomposer.mp4?dl=1");
        words.Add("Ecosystem");
        links.Add("https://media.signbsl.com/videos/asl/thefamilyvocab/mp4/ecosystem.mp4");
        words.Add("Fungus");
        links.Add("https://dl.dropbox.com/s/8zgfiqyq2xo3qzf/Fungus.mp4?dl=1");
        words.Add("Bacteria");
        links.Add("https://dl.dropbox.com/s/udel6oj4y3izwro/Bacteria.mp4?dl=1");
        words.Add("Plant");
        links.Add("https://media.signbsl.com/videos/asl/signlanguagestudent/mp4/plant.mp4");
        words.Add("Prey");
        links.Add("https://dl.dropbox.com/s/i20xy2udokdf963/Prey.mp4?dl=1");
        words.Add("Predator");
        links.Add("https://dl.dropbox.com/s/j1fl7aswklzmbi4/Predator.mp4?dl=1");
        words.Add("Population");
        links.Add("https://dl.dropbox.com/s/u9ua053n050wt5f/Population.mp4?dl=1");
        words.Add("Organism");
        links.Add("https://dl.dropbox.com/s/sca3q9k0uh4txip/Organism.mp4?dl=1");
        words.Add("Microbe");
        links.Add("https://dl.dropbox.com/s/xw473pi6rgwcmpt/Microbes.mp4?dl=1");
    }
}
