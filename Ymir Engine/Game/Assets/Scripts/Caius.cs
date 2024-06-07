using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using YmirEngine;

public class Caius : YmirComponent
{
    public struct Dialogue
    {
        public int ID;
        public string Name;
        public string Text;
        public string Code;

        public Dialogue(int id, string name, string text, string code)
        {
            ID = id;
            Name = name;
            Text = text;
            Code = code;
        }
    }

    private Queue<string> dialogueQueue;

    Dictionary<int, Dialogue> dialogue = new Dictionary<int, Dialogue>();
    //string dialoguescsv;
    bool active_Dialogue;

    public Player player;
    public GameObject canvas_Caius = null;
    public GameObject name_Npc = null;
    public GameObject dialogue_Npc = null;
    public GameObject Ybutton = null;
    public GameObject Bbutton = null;
    public GameObject Abutton = null;
    public GameObject Xbutton = null;

    //Popup
    private GameObject popup;

    private bool retryDialogue;
    private float retryTimer;
    private const float retryDuration = 0.5f;

    //Save and Load
    string saveName;
    bool introDialogueDone;
    bool holoScreen;
    bool androidHead;
    bool corpse;
    bool lvl_1;
    bool lvl_2;
    bool hasDead;
    bool firstIncursion;
    bool boss;

    public enum Dialogue_id
    {
        ID_0,
        ID_1,
        ID_2,
        ID_3,
        ID_4,
        ID_5,
        ID_6,
        ID_7,
        ID_8,
        ID_9,
        ID_10,
        ID_11,
        ID_12,
        ID_13,
        ID_14,
        ID_15,
        ID_16,
        ID_17,
        ID_18,
        ID_19,
        ID_20,
        ID_21,
        ID_22,
        ID_23,
        ID_24,
        ID_25,
        ID_26
    }

    public Dialogue_id dialogue_;
    public void Start()
    {
        player = InternalCalls.GetGameObjectByName("Player").GetComponent<Player>();

        active_Dialogue = false;
        canvas_Caius = InternalCalls.GetGameObjectByName("Npc_Dialogue");
        name_Npc = InternalCalls.GetGameObjectByName("Name_Npc");
        dialogue_Npc = InternalCalls.GetGameObjectByName("dialogue_Npc");
        //dialoguescsv = InternalCalls.CSVToString("Assets/Dialogue/Caius_Intro_Dialogue.csv");   //Dialogo de la intro, luego va cambiando su valor en relacion a las acciones del player
        Ybutton = InternalCalls.GetGameObjectByName("buttonY");
        Bbutton = InternalCalls.GetGameObjectByName("buttonB");
        Abutton = InternalCalls.GetGameObjectByName("buttonA");
        Xbutton = InternalCalls.GetGameObjectByName("buttonX");

        popup = InternalCalls.CS_GetChild(gameObject, 1);

        //Animation - WIP
        Animation.SetLoop(InternalCalls.CS_GetChild(gameObject, 0), "Caius_Idle", true);
        Animation.SetSpeed(InternalCalls.CS_GetChild(gameObject, 0), "Caius_Idle", 0.2f);
        Animation.PlayAnimation(InternalCalls.CS_GetChild(gameObject, 0), "Caius_Idle");

        saveName = SaveLoad.LoadString(Globals.saveGameDir, Globals.saveGamesInfoFile, Globals.saveCurrentGame);
        introDialogueDone = false;
        holoScreen = false;
        androidHead = false;
        corpse = false;
        lvl_1 = false;
        lvl_2 = false;
        hasDead = false;
        firstIncursion = false;
        boss = false;


        dialogueQueue = new Queue<string>(); // Inicializar la cola
        dialogueQueue.Enqueue("Assets/Dialogue/Caius_Intro_Dialogue.csv");
        LoadDialogues(InternalCalls.CSVToString(dialogueQueue.Peek()));
        dialogue_ = Dialogue_id.ID_1;
    }
    public void Update()
    {
        popup.SetAsBillboard();

        CheckHiddenDialogues();

        if (active_Dialogue)
        {
            if (popup.IsActive())
            {
                popup.SetActive(false);
            }

            HandleDialogue();

            DialogueManager();
        }
        else
        {
            popup.SetActive(false);
        }

        if (retryDialogue)
        {
            retryTimer -= Time.deltaTime;
            if (retryTimer <= 0)
            {
                retryDialogue = false;
            }
        }
    }
    public void DialogueManager()
    {
        if (dialogueQueue.Peek() == "Assets/Dialogue/Caius_Intro_Dialogue.csv")
        {
            //Intro Dialogue ID 001
            if (!introDialogueDone)
            {
                switch (dialogue_)
                {
                    case Dialogue_id.ID_0:

                        break;

                    case Dialogue_id.ID_1:
                        UI.TextEdit(name_Npc, dialogue[1].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[1].Text);
                        UI.TextEdit(Ybutton, dialogue[2].Text);
                        UI.TextEdit(Bbutton, dialogue[3].Text);
                        UI.TextEdit(Abutton, dialogue[4].Text);
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_5:
                        UI.TextEdit(name_Npc, dialogue[5].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[5].Text);
                        UI.TextEdit(Ybutton, dialogue[6].Text);
                        UI.TextEdit(Bbutton, dialogue[8].Text);
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_7:
                        UI.TextEdit(name_Npc, dialogue[7].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[7].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_9:
                        UI.TextEdit(name_Npc, dialogue[9].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[9].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_10:
                        UI.TextEdit(name_Npc, dialogue[10].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[10].Text);
                        UI.TextEdit(Ybutton, dialogue[11].Text);
                        UI.TextEdit(Bbutton, dialogue[17].Text);
                        UI.TextEdit(Abutton, dialogue[23].Text);
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_12:
                        UI.TextEdit(name_Npc, dialogue[12].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[12].Text);
                        UI.TextEdit(Ybutton, dialogue[13].Text);
                        UI.TextEdit(Bbutton, dialogue[15].Text);
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_14:
                        UI.TextEdit(name_Npc, dialogue[14].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[14].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_16:
                        UI.TextEdit(name_Npc, dialogue[16].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[16].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_18:
                        UI.TextEdit(name_Npc, dialogue[18].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[18].Text);
                        UI.TextEdit(Ybutton, dialogue[19].Text);
                        UI.TextEdit(Bbutton, dialogue[21].Text);
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_20:
                        UI.TextEdit(name_Npc, dialogue[20].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[20].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_22:
                        UI.TextEdit(name_Npc, dialogue[22].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[22].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_24:
                        UI.TextEdit(name_Npc, dialogue[24].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[24].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;
                    case Dialogue_id.ID_26:
                        UI.TextEdit(name_Npc, dialogue[26].Name);
                        UI.TextEdit(dialogue_Npc, dialogue[26].Text);
                        UI.TextEdit(Ybutton, " ");
                        UI.TextEdit(Bbutton, " ");
                        UI.TextEdit(Abutton, " ");
                        UI.TextEdit(Xbutton, " ");
                        break;

                }
            }
        }
        else if (dialogueQueue.Peek() == "Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv")
        {
            UI.TextEdit(name_Npc, dialogue[1].Name);
            UI.TextEdit(dialogue_Npc, dialogue[1].Text);
            UI.TextEdit(Ybutton, dialogue[2].Text);
            UI.TextEdit(Bbutton, dialogue[3].Text);
            UI.TextEdit(Abutton, " ");
            UI.TextEdit(Xbutton, " ");
        }
        else if (dialogueQueue.Peek() == "Assets/Dialogue/CAIUS_RAISEN_ID006.csv")
        {
            UI.TextEdit(name_Npc, dialogue[1].Name);
            UI.TextEdit(dialogue_Npc, dialogue[1].Text);
            UI.TextEdit(Ybutton, dialogue[2].Text);
            UI.TextEdit(Bbutton, dialogue[3].Text);
            UI.TextEdit(Abutton, " ");
            UI.TextEdit(Xbutton, " ");
        }
        
    }

    private void HandleDialogue()
    {
        Debug.Log("Cuerrent Dialogue Name: " + dialogueQueue.Peek());
        if (dialogueQueue.Peek() == "Assets/Dialogue/Caius_Intro_Dialogue.csv")
        {
            //player.PlayerStopState(true);
            //ID 1
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_1)
            {
                dialogue_ = Dialogue_id.ID_5;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_1)
            {
                dialogue_ = Dialogue_id.ID_10;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.A) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_1)
            {
                dialogue_ = Dialogue_id.ID_26;
                return;
            }
            //ID 5
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_5)
            {
                dialogue_ = Dialogue_id.ID_7;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_5)
            {
                dialogue_ = Dialogue_id.ID_9;
                return;
            }
            //ID 10
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_10)
            {
                dialogue_ = Dialogue_id.ID_12;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_10)
            {
                dialogue_ = Dialogue_id.ID_18;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.A) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_10)
            {
                dialogue_ = Dialogue_id.ID_24;
                return;
            }
            //ID 12
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_12)
            {
                dialogue_ = Dialogue_id.ID_14;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_12)
            {
                dialogue_ = Dialogue_id.ID_16;
                return;
            }
            //ID 18
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_18)
            {
                dialogue_ = Dialogue_id.ID_20;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_18)
            {
                dialogue_ = Dialogue_id.ID_22;
                return;
            }

            //EXITS on any input
            if ((Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN ||
                Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN ||
                Input.GetGamepadButton(GamePadButton.A) == KeyState.KEY_DOWN ||
                Input.GetGamepadButton(GamePadButton.X) == KeyState.KEY_DOWN) &&
                (dialogue_ == Dialogue_id.ID_26 || dialogue_ == Dialogue_id.ID_24 || dialogue_ == Dialogue_id.ID_22 ||
                dialogue_ == Dialogue_id.ID_20 || dialogue_ == Dialogue_id.ID_7 || dialogue_ == Dialogue_id.ID_9 ||
                dialogue_ == Dialogue_id.ID_14 || dialogue_ == Dialogue_id.ID_16))
            {
                ExitDialogue();

                return;
            }
        }
        else if (dialogueQueue.Peek() == "Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv")
        {
            //ID 1
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_1)
            {
                //Open crafting menu
                ExitDialogue();
                player.currentMenu = "Crafting Canvas";
                player.ToggleMenu(true);
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_1)
            {
                ExitDialogue();
                return;
            }
        }
        else if (dialogueQueue.Peek() == "Assets/Dialogue/CAIUS_RAISEN_ID006.csv")
        {
            //ID 1
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_1)
            {
                dialogue_ = Dialogue_id.ID_3;
                return;
            }
            //ID 3
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_3)
            {
                dialogue_ = Dialogue_id.ID_8;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_3)
            {
                dialogue_ = Dialogue_id.ID_11;
                return;
            }
            //ID 8
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_8)
            {
                dialogue_ = Dialogue_id.ID_3;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_8)
            {
                ExitDialogue();
                return;
            }
            //ID 11
            if (Input.GetGamepadButton(GamePadButton.Y) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_11)
            {
                dialogue_ = Dialogue_id.ID_3;
                return;
            }
            if (Input.GetGamepadButton(GamePadButton.B) == KeyState.KEY_DOWN && dialogue_ == Dialogue_id.ID_8)
            {
                ExitDialogue();
                return;
            }
        }
    }
    public void OnCollisionStay(GameObject other)
    {
        if (other.Tag == "Player" && !active_Dialogue)
        {
            popup.SetActive(true);
        }

        if (other.Tag == "Player" && (Input.IsGamepadButtonAPressedCS() || Input.GetKey(YmirKeyCode.SPACE) == KeyState.KEY_DOWN) && !active_Dialogue && !retryDialogue && player.currentMenu != "Crafting Canvas")
        {
            canvas_Caius.SetActive(true);
            active_Dialogue = true;
            player.PlayerStopState(true);
        }
    }
    public void OnCollisionExit(GameObject other)
    {
        if (other.Tag == "Player")
        {
            canvas_Caius.SetActive(false);
            active_Dialogue = false;
            dialogue_ = Dialogue_id.ID_1;
        }
    }
    public void LoadDialogues(string dialogueData)
    {
        //Vaciar el actual dialogue dictionary
        if (dialogue.Count > 0)
        {
            dialogue.Clear();
        }

        string[] lines = dialogueData.Split(new string[] { "<end>" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] dialogueParts = line.Split(';');

            if (dialogueParts.Length >= 4)
            {
                Dialogue _dialogue = new Dialogue();
                Debug.Log("[WARNING] 1");
                _dialogue.ID = int.Parse(dialogueParts[0]);
                Debug.Log("[WARNING] 1");
                _dialogue.Name = dialogueParts[1];
                Debug.Log("[WARNING] 4");
                _dialogue.Text = dialogueParts[2];
                Debug.Log("[WARNING] 5");
                _dialogue.Code = dialogueParts[3];
                Debug.Log("[WARNING] 6");

                dialogue.Add(_dialogue.ID, _dialogue);
                Debug.Log("[WARNING] Ended");
            }
        }

        //Debug.Log("[WARNING] GG Loading dialogue data" + lines[0]);
    }

    private void LoadNextDialogeInQueue()
    {
        Debug.Log("LoadNextDialogeInQueue");

        // Verifica si hay al menos dos di�logos en la cola
        if (dialogueQueue.Count >= 2)
        {
            Debug.Log("Hay al menos dos di�logos en la cola, por tanto no a�adimos ninguno: " + dialogueQueue.Count);
            // Muestra el di�logo actual
            LoadDialogues(InternalCalls.CSVToString(dialogueQueue.Peek()));

            // Quita el di�logo que se est� mostrando de la lista de pendientes
            dialogueQueue.Dequeue();
        }
        else
        {
            Debug.Log("Hay menos de dos di�logos en la cola, por tanto a�adimos el defalut: " + dialogueQueue.Count);
            // Agrega un di�logo base a la cola
            dialogueQueue.Enqueue("Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv");

            Debug.Log("1. Count: " +  dialogueQueue.Count);
            // Quita el dialogo que se esta mostrando de la lista de pendientes
            dialogueQueue.Dequeue();

            Debug.Log("2. Count: " + dialogueQueue.Count);
            // Muestra el nuevo di�logo
            LoadDialogues(InternalCalls.CSVToString(dialogueQueue.Peek()));

            Debug.Log("3. Count: " + dialogueQueue.Count);
        }
    }

    private void ExitDialogue()
    {
        dialogue_ = Dialogue_id.ID_1;
        //EXIT
        player.PlayerStopState(false);
        active_Dialogue = false;
        canvas_Caius.SetActive(false);

        retryDialogue = true;
        retryTimer = retryDuration;

        if (dialogueQueue.Peek() == "Assets/Dialogue/Caius_Intro_Dialogue.csv")
        {
            introDialogueDone = true;
            SaveLoad.SaveBool(Globals.saveGameDir, saveName, "Caius intro dialogue", introDialogueDone);
            Debug.Log("Caius intro dialogue: true");
        }

        LoadNextDialogeInQueue();
    }

    private void AddDialogueOverDefault(string toAdd)
    {
        bool foundDefault = false;
        int queueSize = dialogueQueue.Count;

        // Crear una lista temporal para almacenar los elementos de la cola.
        List<string> tempQueue = new List<string>();

        // Dequeue todos los elementos y comprobar si contienen el valor predeterminado.
        for (int i = 0; i < queueSize; i++)
        {
            string current = dialogueQueue.Dequeue();
            if (current == "Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv" && !foundDefault)
            {
                // Reemplazar la primera ocurrencia del valor predeterminado por toAdd.
                tempQueue.Add(toAdd);
                foundDefault = true;
            }
            else
            {
                tempQueue.Add(current);
            }
        }

        // A�adir "Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv" al final si se encontr� y reemplaz�
        if (foundDefault)
        {
            tempQueue.Add("Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv");
        }
        else
        {
            // Si no se encontr� el valor predeterminado, simplemente a�adir toAdd al final.
            tempQueue.Add(toAdd);
        }

        // Re-enqueue todos los elementos desde la lista temporal.
        foreach (var item in tempQueue)
        {
            dialogueQueue.Enqueue(item);
        }
    }

    private void AddDialogueOverAll(string toAdd)
    {
        // Vaciar la cola actual.
        dialogueQueue.Clear();

        // A�adir el string toAdd a la cola.
        dialogueQueue.Enqueue(toAdd);

        // A�adir "Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv" a la cola.
        dialogueQueue.Enqueue("Assets/Dialogue/CAIUS_RAISEN_DEFAULT.csv");
    }

    private void RemoveDialogue(string toRemove)
    {
        int queueSize = dialogueQueue.Count;

        // Crear una lista temporal para almacenar los elementos de la cola.
        List<string> tempQueue = new List<string>();

        // Dequeue todos los elementos y a�adir a la lista temporal los que no coincidan con toRemove.
        for (int i = 0; i < queueSize; i++)
        {
            string current = dialogueQueue.Dequeue();
            if (current != toRemove)
            {
                tempQueue.Add(current);
            }
        }

        // Re-enqueue todos los elementos desde la lista temporal.
        foreach (var item in tempQueue)
        {
            dialogueQueue.Enqueue(item);
        }
    }

    private void CheckHiddenDialogues()
    {
        if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Interacted Holo Screen") && !holoScreen)
        {
            holoScreen = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID006.csv");

        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Interacted Android Head") && !androidHead)
        {
            androidHead = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID008.csv");
        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Interacted Corpse") && !corpse)
        {
            corpse = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID012.csv");
        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Lvl 1 Completed") && !lvl_1)
        {
            lvl_1 = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID010.csv");
        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Lvl 2 Completed") && !lvl_2)
        {
            lvl_2 = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID011.csv");
        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Has dead") && !hasDead)
        {
            hasDead = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID007.csv");
        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "Boss Fight") && !boss)
        {
            boss = true;
            RemoveDialogue("Assets/Dialogue/CAIUS_RAISEN_ID012.csv");
            AddDialogueOverAll("Assets/Dialogue/CAIUS_RAISEN_FINAL.csv");
        }
        else if (SaveLoad.LoadBool(Globals.saveGameDir, saveName, "First Incursion") && !firstIncursion)
        {
            firstIncursion = true;
            AddDialogueOverDefault("Assets/Dialogue/CAIUS_RAISEN_ID009.csv");
        }
    }

}