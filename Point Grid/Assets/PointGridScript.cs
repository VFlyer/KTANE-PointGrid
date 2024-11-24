using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KModkit;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class PointGridScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    string[] directionLabels = { "↑", "↗", "→", "↘", "↓", "↙", "←", "↖", " " };
    private int[,] numberGrid = new int[6, 6]
    {
        { 0,0,0,0,0,0 },
        { 0,0,0,0,0,0 },
        { 0,0,0,0,0,0 },
        { 0,0,0,0,0,0 },
        { 0,0,0,0,0,0 },
        { 0,0,0,0,0,0 }
    };

    public KMSelectable[] TopArrowSelectables;
    public KMSelectable[] RightArrowSelectables;
    public KMSelectable CheckButton;

    public TextMesh[] TopTextMeshes;
    public TextMesh[] RightTextMeshes;
    public TextMesh[] NumberLabels;

    int[] TopDirectionPositions = new int[6];
    int[] RightDirectionPositions = new int[6];

    public Material green;

    string[] topSolution = new string[6];
    string[] rightSolution = new string[6];
   void Awake () {
      ModuleId = ModuleIdCounter++;
        /*
        foreach (KMSelectable object in keypad) {
            object.OnInteract += delegate () { keypadPress(object); return false; };
        }
        */

        //button.OnInteract += delegate () { buttonPress(); return false; };
        //TopArrowSelectables[0].OnInteract += delegate ()
        //{
        //    TopDirectionPositions[0]++;
        //    if (TopDirectionPositions[0] > 5)
        //        TopDirectionPositions[0] = 0;
        //    return false;
        //};
        foreach (KMSelectable button in TopArrowSelectables)
        {
            button.OnInteract += delegate ()
            {
                TopPress(button);
                return false;
            };
        }
        foreach (KMSelectable button in RightArrowSelectables)
        {
            button.OnInteract += delegate ()
            {
                RightPress(button);
                return false;
            };
        }
        CheckButton.OnInteract += delegate ()
        {
            CheckButton.AddInteractionPunch();
            CheckSolution();
            return false;
        };
    }
    void TopPress(KMSelectable arrow)
    {
        int index = Array.IndexOf(TopArrowSelectables, arrow);
        TopDirectionPositions[index]++;
        if (TopDirectionPositions[index] > 8)
            TopDirectionPositions[index] = 0;
        TopTextMeshes[index].text = directionLabels[TopDirectionPositions[index]];
    }
    void RightPress(KMSelectable arrow)
    {
        int index = Array.IndexOf(RightArrowSelectables, arrow);
        RightDirectionPositions[index]--;
        if (RightDirectionPositions[index] < 0)
            RightDirectionPositions[index] = 8;
        RightTextMeshes[index].text = directionLabels[RightDirectionPositions[index]];
    }
    void Start () {
        GenerateSolution();
        Debug.LogFormat("[Point Grid #{0}] Top arrow solution: {1} {2} {3} {4} {5} {6}", ModuleId, TopTextMeshes[0].text, TopTextMeshes[1].text, TopTextMeshes[2].text, TopTextMeshes[3].text, TopTextMeshes[4].text, TopTextMeshes[5].text);
        Debug.LogFormat("[Point Grid #{0}] Right arrow solution: {1} {2} {3} {4} {5} {6}", ModuleId, RightTextMeshes[0].text, RightTextMeshes[1].text, RightTextMeshes[2].text, RightTextMeshes[3].text, RightTextMeshes[4].text, RightTextMeshes[5].text);
        Debug.LogFormat("[Point Grid #{0}] Generated Grid:", ModuleId);
        for (var x = 0; x < 6; x++)
            Debug.LogFormat("[Point Grid #{0}] {1}", ModuleId, Enumerable.Range(0, 6).Select(a => numberGrid[x, a]).Join());

        for (int i = 0; i < 6; i++)
        {
            topSolution[i] = TopTextMeshes[i].text;
            rightSolution[i] = RightTextMeshes[i].text;
            TopTextMeshes[i].text = directionLabels[8];
            TopDirectionPositions[i] = 8;
            RightTextMeshes[i].text = directionLabels[8];
            RightDirectionPositions[i] = 8;
        }
        
   }
    void CheckSolution()
    {
        bool correct = true;
        for (int i = 0; i < 6; i++)
        {
            if (!topSolution[i].Equals(TopTextMeshes[i].text))
            {
                correct = false;
                i = 6;
            }
            else if (!rightSolution[i].Equals(RightTextMeshes[i].text))
            {
                correct = false;
                i = 6;
            }
        }
        if (!correct)
            GetComponent<KMBombModule>().HandleStrike();
        else
        {
            //Debug.LogFormat("Correct");
            StartCoroutine(SolveAnim());
        }
        //int[,] tempGrid = new int[6, 6]
        //{
        //    { 0,0,0,0,0,0 },
        //    { 0,0,0,0,0,0 },
        //    { 0,0,0,0,0,0 },
        //    { 0,0,0,0,0,0 },
        //    { 0,0,0,0,0,0 },
        //    { 0,0,0,0,0,0 }
        //};
        //for (int i = 0; i < 6; i++)
        //{
        //    TopAffectGrid(i, Array.IndexOf(directionLabels, TopTextMeshes[i].text), tempGrid);
        //    RightAffectGrid(i, Array.IndexOf(directionLabels, RightTextMeshes[i].text), tempGrid);
        //}
        //if (tempGrid == numberGrid)
        //    StartCoroutine(SolveAnim());
        //else
        //    GetComponent<KMBombModule>().HandleStrike();
    }
    void GenerateSolution()
    {
        for (int i = 0; i < 6; i++) //top arrows
        {
            int directionInt;
            if(i==0)
            {
                directionInt = Rnd.Range(3, 5);
                TopTextMeshes[i].text = directionLabels[directionInt];
            }
            else if(i == 5)
            {
                directionInt = Rnd.Range(4, 6);
                TopTextMeshes[i].text = directionLabels[directionInt];
            }
            else
            {
                directionInt = Rnd.Range(3, 6);
                TopTextMeshes[i].text = directionLabels[directionInt];
            }
            TopDirectionPositions[i] = directionInt;
            TopAffectGrid(i, directionInt, numberGrid);
        }
        for(int i = 0; i < 6; i++) //right arrows
        {
            int directionInt;
            if(i==0)
            {
                directionInt = Rnd.Range(5, 7);
                RightTextMeshes[i].text = directionLabels[directionInt];
            }
            else if(i == 5)
            {
                directionInt = Rnd.Range(6, 8);
                RightTextMeshes[i].text = directionLabels[directionInt];
            }
            else
            {
                directionInt = Rnd.Range(5, 8);
                RightTextMeshes[i].text = directionLabels[directionInt];
            }
            RightDirectionPositions[i] = directionInt;
            RightAffectGrid(i, directionInt, numberGrid);
        }
        for(int i = 0; i < 36; i++)
        {
            NumberLabels[i].text = numberGrid[i / 6, i % 6] + "";
        }
    }
    
    void TopAffectGrid(int index, int direction, int[,] grid) //BR = 3, B = 4, BL = 5
    {
        if(direction==4)
        {
            for(int i = 0; i < 6; i++)
            {
                grid[i, index]++;
            }
        }
        else if(direction==3)
        {
            int r = 0, c = index + 1;
            while(r<6 && c<6)
            {
                grid[r, c]++;
                r++;
                c++;
            }
        }
        else if(direction==5)
        {
            int r = 0, c = index - 1;
            while(r<6 && c>-1)
            {
                grid[r, c]++;
                r++;
                c--;
            }
        }
    }
    void RightAffectGrid(int index, int direction, int[,] grid) //BL = 5, L = 6, TL = 7
    {
        if(direction==6)
        {
            for (int i = 5; i > -1; i--)
            {
                grid[index, i]++;
            }
        }
        else if(direction==5)
        {
            int r = index + 1, c = 5;
            while(r<6 && c>-1)
            {
                grid[r, c]++;
                r++;
                c--;
            }
        }
        else if(direction==7)
        {
            int r = index - 1, c = 5;
            while(r>-1&&c>-1)
            {
                grid[r, c]++;
                r--;
                c--;
            }
        }
    }
    IEnumerator SolveAnim()
    {
        //Debug.LogFormat("Playing solve animation");
        Audio.PlaySoundAtTransform("point-grid-solve-actual", transform);
        for (int i = 0; i < 6; i++)
        {
            TopArrowSelectables[i].GetComponent<MeshRenderer>().material = green;
            TopTextMeshes[i].color = Color.black;
            yield return new WaitForSeconds(0.1f);
        }
        for(int i = 0; i < 6; i++)
        {
            RightArrowSelectables[i].GetComponent<MeshRenderer>().material = green;
            RightTextMeshes[i].color = Color.black;
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<KMBombModule>().HandlePass();
        ModuleSolved = true;
    }

    // "i aint doin allat :skull:"
    // isokey, kuro do it <3

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use '!{0} <arrow> <direction>' to set the state of an arrow | "
                                                + "'!{0} submit' to submit the current state | Arrows are 1-12 in reading order; "
                                                + "directions are u/ur/r/dr/d/dl/l/ul; chain commands with semicolons (;).";
#pragma warning restore 414

    private string[] _directions = new string[] { "U", "UR", "R", "DR", "D", "DL", "L", "UL" };

    private IEnumerator ProcessTwitchCommand (string bigCommand) {
        List<string> commands = bigCommand.Split(';').Select(com => com.Trim()).ToList();
        bool submit = false;

        if (commands.Count() > 13) {
            yield return "sendtochaterror Too many commands! There is no need to send more than 13 commands.";
        }

        for (int i = 0; i < commands.Count(); i++) {
            string commandAsUpper = commands[i].ToUpper();
            if (commandAsUpper == string.Empty) {
                yield return "sendtochaterror You cannot have any empty commands!";
            }

            if (commandAsUpper == "SUBMIT") {
                if (commands.Count() != i + 1) {
                    yield return "sendtochaterror There cannot be any commands after submitting";
                }
                else {
                    commands.RemoveAt(commands.Count() - 1);
                    submit = true;
                }
            }
            else {
                string[] splitCommand = commandAsUpper.Split(' ');
                int arrowPosition;

                if (splitCommand.Length != 2 || !int.TryParse(splitCommand[0], out arrowPosition) || arrowPosition > 12 || arrowPosition <= 0 || !_directions.Contains(splitCommand[1])) {
                    yield return "sendtochaterror '" + commands[i] + "' is not a valid command!";
                }
                else {
                    commands[i] = arrowPosition + " " + Array.IndexOf(_directions, splitCommand[1]);
                }
            }
        }
        yield return null;

        foreach (string command in commands) {
            string[] splitCommand = command.Split(' ');
            int position = int.Parse(splitCommand[0]) - 1;
            int direction = int.Parse(splitCommand[1]);

            KMSelectable arrowToPress;
            TextMesh arrowText;
            if (position >= 6) {
                position -= 6;
                arrowToPress = RightArrowSelectables[position];
                arrowText = RightTextMeshes[position];
            }
            else {
                arrowToPress = TopArrowSelectables[position];
                arrowText = TopTextMeshes[position];
            }

            while (arrowText.text != directionLabels[direction]) {
                arrowToPress.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (submit) {
            CheckButton.OnInteract();
            yield return "solve";
        }
    }

    private IEnumerator TwitchHandleForcedSolve () {
        yield return null;

        for (int i = 0; i < 6; i++) {
            while (TopTextMeshes[i].text != topSolution[i]) {
                TopArrowSelectables[i].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }

            while (RightTextMeshes[i].text != rightSolution[i]) {
                RightArrowSelectables[i].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }

        CheckButton.OnInteract();
        while (!ModuleSolved)
            yield return true;
    }

}
