using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GIFController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    //private string currentState;
    private RuntimeAnimatorController currentState;

    //private List<string> heredityVocab = new List<string> { "T-Cell", "Phenotype", "rRNA", "Wild-Type", "tRNA", "Nucleotide", "Genome", "Dominant Gene", "Chromosome", "Cell", "Base Pairs", "B-Cell", "Anticodon", "Amino-Acid", "Codon", "Deletion Mutation", "DNA Replication", "DNA", "Dominant Allele", "Gene", "Genotype", "Heredity", "mRNA", "Inversion Mutation", "Insertion Mutation", "RNA", "Recessive Gene", "Recessive Allele" };
    //private List<string> foodWebVocab = new List<string> { "Microbes", "Symbiosis", "Predator", "Material", "Bacteria", "Carnivore", "Consume", "Decomposer", "Food Chain", "Food Web", "Fungus", "Organism", "Population", "Prey", "Omnivore" };
    //private List<string> partsOfTheCellVocab = new List<string> { "Prokaryotic", "Mitochondria", "Endoplasmic Reticulum", "DNA", "Cytoplasm", "Cell", "Cell Wall", "Cell Membrane", "Eukaryotic", "Flagellum", "Golgi Apparatus", "Nucleus", "Organelle", "Ribosome", "Vacuole" };
    //private List<string> chemistryVocab = new List<string> { "Evaporation", "Equilibrium", "Spontaneous Reaction", "Solute", "Catalyst", "Beaker", "Activation Energy", "Boiling Point", "Reaction Mechanism", "Solvent", "Substrate", "Erlenmeyer Flask", "Condensation", "Gas" };
    //public List<string> currentVocabList;

    public List<RuntimeAnimatorController> heredityVocab = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> foodWebVocab = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> chemistryVocab = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> partsOfTheCellVocab = new List<RuntimeAnimatorController>();

    public List<RuntimeAnimatorController> currentVocabList;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateVocabSet(string newVocabSet)
    {
        switch (newVocabSet)
        {
            case "Heredity":
                currentVocabList = heredityVocab;
                break;
            case "Chemistry":
                currentVocabList = chemistryVocab;
                break;
            case "FoodWeb":
                currentVocabList = foodWebVocab;
                break;
            case "PartsOfTheCell":
                currentVocabList = partsOfTheCellVocab;
                break;
        }
    }

    public void ChangeAnimationState(RuntimeAnimatorController newState)
    {
        Debug.Log("made it here 1");
        Debug.Log($"newState = {newState.name}");
        if (currentState == newState) return;
        //string toPlay = vocabSetName + "." + newState;
        //Debug.Log($"newStateName = {toPlay}");
        //anim.Play(toPlay);
        //Debug.Log($"name of controller = {currentVocabList[newState].name}");
        //anim.runtimeAnimatorController = currentVocabList[newState] as RuntimeAnimatorController;
        anim.runtimeAnimatorController = newState as RuntimeAnimatorController;
        currentState = newState;
    }
}
