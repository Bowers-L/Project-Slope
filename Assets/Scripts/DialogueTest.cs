using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityTimer;
using Yarn.Unity;

public class DialogueTest : MonoBehaviour
{
    public enum DialogueBoxType {normal, yell}

    public string name1;
    public string name2;

    [SerializeField] GameObject DialogueRunnerObject;
    [SerializeField] GameObject DialogueView;
    [SerializeField] GameObject DialogueBox;
    [SerializeField] GameObject DialogueTextObject;
    [SerializeField] GameObject NameTextObject;
    [SerializeField] GameObject char1;
    [SerializeField] GameObject char2;
    [SerializeField] Sprite[] emoteList;

    DialogueRunner _dialogueRunner;
    InMemoryVariableStorage _inMemoryVariableStorage;
    CustomDialogueView _lineView;
    YarnProject _yarnProject;
    Animator _dialogueBoxAnimator;

    DialogueBoxType dialogueBoxType = DialogueBoxType.normal;
    string boxType = "normal";

    SpriteRenderer char1sprite;
    SpriteRenderer char2sprite;
    Animator char1anim;
    Animator char2anim;
    string emoteType = "default";

    //Timer dialogueProgressCooldownTimer;
    Timer dialogueProgressTimer;
    bool dialogueProgressCooldown = true;

    LocalizedLine currentLine;

    void Start()
    {
        _dialogueRunner = DialogueRunnerObject.GetComponent<DialogueRunner>();
        _inMemoryVariableStorage = DialogueRunnerObject.GetComponent<InMemoryVariableStorage>();
        _lineView = DialogueView.GetComponent<CustomDialogueView>();
        _yarnProject = _dialogueRunner.yarnProject;

        _dialogueBoxAnimator = DialogueBox.GetComponent<Animator>();

        char1anim = char1.GetComponent<Animator>();
        char2anim = char2.GetComponent<Animator>();

        char1sprite = char1.GetComponent<SpriteRenderer>();
        char2sprite = char2.GetComponent<SpriteRenderer>();

        _inMemoryVariableStorage.SetValue("$jamming", true);
    }

    void Update()
    {
        //Debug Input
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _inMemoryVariableStorage.SetValue("$jamming", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _inMemoryVariableStorage.SetValue("$jamming", false);
        }
        */
    }

    void ProgressDialogue()
    {
        if (_dialogueRunner.CurrentNodeName == null || _dialogueRunner.CurrentNodeName == "")
        {
            if (dialogueProgressTimer != null) dialogueProgressTimer.Cancel();
            // Send back signal to start next segment?
        } else {
            _lineView.UserRequestedViewAdvancement();
            ProcessDialogue();
        }
    }

    void ProcessDialogue()
    {
        currentLine = _lineView.GetCurrentLine();

        FlipDialogueBox();
        
        if (currentLine.Metadata != null) {
            foreach (string s in currentLine.Metadata)
            {
                if (s.Split(':')[0] == "boxtype") 
                {
                    ChangeDialogueBox(s.Split(':')[1]);
                }
                if (s.Split(':')[0] == "emotetype")
                {
                    ChangeEmote(s.Split(':')[1]);
                }
            }
        } else {
            //Debug.Log("No metadata found for current line");
        }
    }

    void FlipDialogueBox()
    {
        if (currentLine.CharacterName != null && currentLine.CharacterName == name1)
        {
            DialogueBox.transform.rotation = Quaternion.Euler(0, 0, 0);
            DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            NameTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (currentLine.CharacterName == name2)
        {
            DialogueBox.transform.rotation = Quaternion.Euler(0, 180, 0);
            DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            NameTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void ChangeDialogueBox(string boxType)
    {
        //Debug.Log("ChangeDialogueBox(): " + boxType);
        switch(boxType){
            case "normal":
                _dialogueBoxAnimator.Play("DialogueBox-Normal");
                break;
            case "yell":
                _dialogueBoxAnimator.Play("DialogueBox-Yell");
                break;
            case "think":
                _dialogueBoxAnimator.Play("DialogueBox-Think");
                break;
            default:
                break;
        }
    }

    void ChangeEmote(string emoteType)
    {
        switch(emoteType) {
            case "evanhappy":
                char1sprite.sprite = emoteList[0];
                break;
            case "vivianhappy":
                char2sprite.sprite = emoteList[3];
                break;
            case "pouty":
                char2sprite.sprite = emoteList[5];
                break;
            case "angry":
                char2sprite.sprite = emoteList[4];
                break;
            case "idiot":
                char1sprite.sprite = emoteList[1];
                break;
            case "nervous":
                char1sprite.sprite = emoteList[2];
                break;
            default:
                break;
        }
    }

    public void StartNode(string node = "Intro")
    {
        _dialogueRunner.StartDialogue(node);
        ProcessDialogue();

        dialogueProgressTimer = Timer.Register(
            duration: 4f,
            isLooped: true,
            onComplete: () => 
            {
                ProgressDialogue();
            }
        );
    }
}
