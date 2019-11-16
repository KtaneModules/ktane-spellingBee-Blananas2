using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class spellingBeeScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] keyboard;
    public KMSelectable play;
    public KMSelectable red;
    public KMSelectable green;
    public TextMesh word;
    public TextMesh timer;

    int chosenWord = 0;
    bool typing = false;
    public List<string> wordList = new List<string> { "abecedarian", "accretionary", "aggressor", "allocation", "ambiance", "auxiliary", "bacciferous", "bankruptcy", "bronchoscope", "camouflage", "chambray", "chihuahua", "chimera", "cilia", "cloying", "coffering", "conceptualize", "connoisseur", "controversial", "cortisol", "covenant", "crinoline", "deceit", "deleteriously", "demitasse", "disillusion", "dubonnet", "duologue", "effluent", "emollience", "entrepreneur", "expediently", "fatuously", "fluorescence", "garrulous", "glaucous", "griseous", "hemorrhage", "horripilation", "imposture", "incendiary", "inconsequential", "inexorable", "kalamata", "knickerbocker", "knoll", "malachite", "marginalia", "meerkat", "mellifluous", "metaphysicize", "metoposcopy", "monopsony", "nodule", "obsolescence", "occasionally", "opprobrious", "palliative", "panache", "parietal", "parturition", "parvanimity", "pelagic", "perambulate", "pishposh", "placidly", "placoderm", "predecessor", "protrusile", "pseudologist", "quadrennium", "quintessence", "revelry", "saccade", "salvageable", "scrofulous", "sedge", "sojourner", "solipsistic", "somesthetic", "sorghum", "stereognosis", "stymie", "subsultory", "supposition", "surveillance", "surficial", "symptomatology", "taffeta", "telenovela", "termagant", "thyroidectomy", "transilient", "trough", "unanimity", "upholstery", "vermilion", "vervain", "vestibule", "worcestershire" };
    string alphabet = "qwertyuiopasdfghjklzxcvbnm";
    string currentText = "";
    float endTime = 0;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        
        foreach (KMSelectable key in keyboard) {
            KMSelectable pressedKey = key;
            key.OnInteract += delegate () { keyPress(pressedKey); return false; };
        }
        
        play.OnInteract += delegate () { PlayButton(); return false; };
        green.OnInteract += delegate () { GreenButton(); return false; };
        red.OnInteract += delegate () { RedButton(); return false; };

    }

    // Use this for initialization
    void Start () {
		chosenWord = UnityEngine.Random.Range(0, 100);
    }
	
	// Update is called once per frame
	void Update () {
		if (typing == true)
        {
            timer.text = (Mathf.Floor(Bomb.GetTime() - endTime)).ToString();
            if (Mathf.Floor(Bomb.GetTime() - endTime) < 1)
            {
                Debug.LogFormat("[Spelling Bee #{0}] You took to long to spell your word, module striked and reset.", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
                chosenWord = UnityEngine.Random.Range(0, 100);
                typing = false;
                word.text = "spelling bee";
            }
        }
	}

    void keyPress(KMSelectable pressedKey)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            for (int i = 0; i < 26; i++)
            {
                if (pressedKey == keyboard[i])
                {
                    currentText += alphabet[i];
                    word.text = currentText;
                }
            }
        }
    }

    void PlayButton()
    {
        if (moduleSolved == false)
        {
            play.AddInteractionPunch();
            Audio.PlaySoundAtTransform(wordList[chosenWord], transform);
            if (typing == false)
            {
                typing = true;
                currentText = "";
                word.text = currentText;
                Debug.LogFormat("[Spelling Bee #{0}] Your word is: '{1}'", moduleId, wordList[chosenWord]);
                endTime = Mathf.Floor(Bomb.GetTime() - 60);
            }
        }
    }

    void GreenButton()
    {
        green.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            Debug.LogFormat("[Spelling Bee #{0}] You spelled it: '{1}'", moduleId, currentText);
            if (currentText == "vermilion" || currentText == "vermillion")
            {
                Debug.LogFormat("[Spelling Bee #{0}] Both 'vermilion' and 'vermillion' are acceptable spellings.", moduleId);
                currentText = "vermilion";
            }
            if (currentText == wordList[chosenWord])
            {
                Debug.LogFormat("[Spelling Bee #{0}] You spelled it correctly, module solved.", moduleId);
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
            }
            else
            {
                Debug.LogFormat("[Spelling Bee #{0}] You spelled it incorrectly, module striked and reset.", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
                chosenWord = UnityEngine.Random.Range(0, 100);
            }
            typing = false;
            word.text = "spelling bee";
        }
    }

    void RedButton()
    {
        red.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            currentText = "";
            word.text = currentText;
        }
    }
}
