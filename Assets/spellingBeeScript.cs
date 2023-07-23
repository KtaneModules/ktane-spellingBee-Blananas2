using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class spellingBeeScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable ModuleSelectable;

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
    private KeyCode[] TheKeys =
	{
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M, KeyCode.Return
	};
    private bool focused = false;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        if (Application.isEditor)
        {
            focused = true;
        }
        ModuleSelectable.OnFocus += delegate () { focused = true; };
        ModuleSelectable.OnDefocus += delegate () { focused = false; };
        
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
		if (typing)
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
            if (!focused) { return; }
            for (int i = 0; i < TheKeys.Count(); i++) {
                if (Input.GetKeyDown(TheKeys[i])) {
                    if (i == 26) {
                        GreenButton();
                    } else {
                        keyPress(keyboard[i]);
                    }
                }
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

    //twitch plays
    private bool cmdIsValid(string s)
    {
        string[] valids = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };
        if(s.Length < 1 || s.Length > 19)
        {
            return false;
        }
        for(int i = 0; i < s.Length; i++)
        {
            if (!valids.Contains(s.ElementAt(i) + "")){
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} play [Presses the play button] | !{0} submit <word> [Submits the specified word] | Submitted words must be in all caps";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*press play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            play.OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if(parameters.Length == 2)
            {
                if (cmdIsValid(parameters[1]))
                {
                    yield return null;
                    if (!typing)
                    {
                        yield return "sendtochaterror The play button must be pressed before a submittion can be made!";
                        yield break;
                    }
                    red.OnInteract();
                    for(int i = 0; i < parameters[1].Length; i++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        string comparer = parameters[1].ElementAt(i) + "";
                        comparer = comparer.ToLower();
                        if (comparer.Equals("q"))
                        {
                            keyboard[0].OnInteract();
                        }
                        else if (comparer.Equals("w"))
                        {
                            keyboard[1].OnInteract();
                        }
                        else if (comparer.Equals("e"))
                        {
                            keyboard[2].OnInteract();
                        }
                        else if (comparer.Equals("r"))
                        {
                            keyboard[3].OnInteract();
                        }
                        else if (comparer.Equals("t"))
                        {
                            keyboard[4].OnInteract();
                        }
                        else if (comparer.Equals("y"))
                        {
                            keyboard[5].OnInteract();
                        }
                        else if (comparer.Equals("u"))
                        {
                            keyboard[6].OnInteract();
                        }
                        else if (comparer.Equals("i"))
                        {
                            keyboard[7].OnInteract();
                        }
                        else if (comparer.Equals("o"))
                        {
                            keyboard[8].OnInteract();
                        }
                        else if (comparer.Equals("p"))
                        {
                            keyboard[9].OnInteract();
                        }
                        else if (comparer.Equals("a"))
                        {
                            keyboard[10].OnInteract();
                        }
                        else if (comparer.Equals("s"))
                        {
                            keyboard[11].OnInteract();
                        }
                        else if (comparer.Equals("d"))
                        {
                            keyboard[12].OnInteract();
                        }
                        else if (comparer.Equals("f"))
                        {
                            keyboard[13].OnInteract();
                        }
                        else if (comparer.Equals("g"))
                        {
                            keyboard[14].OnInteract();
                        }
                        else if (comparer.Equals("h"))
                        {
                            keyboard[15].OnInteract();
                        }
                        else if (comparer.Equals("j"))
                        {
                            keyboard[16].OnInteract();
                        }
                        else if (comparer.Equals("k"))
                        {
                            keyboard[17].OnInteract();
                        }
                        else if (comparer.Equals("l"))
                        {
                            keyboard[18].OnInteract();
                        }
                        else if (comparer.Equals("z"))
                        {
                            keyboard[19].OnInteract();
                        }
                        else if (comparer.Equals("x"))
                        {
                            keyboard[20].OnInteract();
                        }
                        else if (comparer.Equals("c"))
                        {
                            keyboard[21].OnInteract();
                        }
                        else if (comparer.Equals("v"))
                        {
                            keyboard[22].OnInteract();
                        }
                        else if (comparer.Equals("b"))
                        {
                            keyboard[23].OnInteract();
                        }
                        else if (comparer.Equals("n"))
                        {
                            keyboard[24].OnInteract();
                        }
                        else if (comparer.Equals("m"))
                        {
                            keyboard[25].OnInteract();
                        }
                    }
                    yield return new WaitForSeconds(0.1f);
                    green.OnInteract();
                }
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!typing)
        {
            yield return ProcessTwitchCommand("play");
        }
        yield return ProcessTwitchCommand("submit " + wordList[chosenWord].ToUpper());
    }
}