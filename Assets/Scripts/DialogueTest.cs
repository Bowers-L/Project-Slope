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

    DialogueRunner _dialogueRunner;
    InMemoryVariableStorage _inMemoryVariableStorage;
    CustomDialogueView _lineView;
    YarnProject _yarnProject;
    Animator _dialogueBoxAnimator;

    DialogueBoxType dialogueBoxType = DialogueBoxType.normal;
    string boxType = "normal";
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

        _inMemoryVariableStorage.SetValue("$jamming", true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNode();
            //ProgressDialogue();
            // dialogueProgressCooldown = false;
            // dialogueProgressCooldownTimer = Timer.Register(0.5f, () => dialogueProgressCooldown = true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _inMemoryVariableStorage.SetValue("$jamming", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _inMemoryVariableStorage.SetValue("$jamming", false);
        }
    }

    void ProgressDialogue()
    {
        if (_dialogueRunner.CurrentNodeName == null || _dialogueRunner.CurrentNodeName == "")
        {
            if (dialogueProgressTimer != null) dialogueProgressTimer.Cancel();
        } else {
            _lineView.UserRequestedViewAdvancement();

            currentLine = _lineView.GetCurrentLine();

            FlipDialogueBox();
            
            if (currentLine.Metadata != null) {
                // if (currentLine.Metadata.Length == 2)
                // {
                //     emoteType = currentLine.Metadata[1].Split(':')[1];
                // }
                // boxType = currentLine.Metadata[0];
                foreach (string s in currentLine.Metadata)
                {
                    if (s.Split(':')[0] == "boxtype") 
                    {
                        ChangeDialogueBox(s.Split(':')[1]);
                    }
                    if (s.Split(':')[0] == "emotetype")
                    {
                        //change emote
                    }
                }
            } else {
                Debug.Log("No metadata found for current line");
            }
        }
    }

    void FlipDialogueBox()
    {
        if (currentLine.CharacterName != null && currentLine.CharacterName == name1)
        {
            Debug.Log(currentLine.RawText + ", point left.");
            DialogueBox.transform.rotation = Quaternion.Euler(0, 0, 0);
            DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            NameTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (currentLine.CharacterName == name2)
        {
            Debug.Log(currentLine.RawText + ", point right.");
            DialogueBox.transform.rotation = Quaternion.Euler(0, 180, 0);
            DialogueTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            NameTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void ChangeDialogueBox(string boxType)
    {
        Debug.Log("ChangeDialogueBox(): " + boxType);
        switch(boxType){
            case "normal":
                _dialogueBoxAnimator.Play("DialogueBox-Normal");
                break;
            case "yell":
                _dialogueBoxAnimator.Play("DialogueBox-Yell");
                break;
            default:
                break;
        }
    }

    void ChangeEmoteType(string emoteType)
    {
        switch(emoteType) {
            case "default":
                //change current character's sprite to default
                break;
            default:
                break;
        }
    }

    public void StartNode(string node = "TestScript")
    {
        _dialogueRunner.StartDialogue(node);
        _lineView.UserRequestedViewAdvancement();

        dialogueProgressTimer = Timer.Register(
            duration: 2.5f,
            isLooped: true,
            onComplete: () => 
            {
                ProgressDialogue();
            }
        );
    }
}
