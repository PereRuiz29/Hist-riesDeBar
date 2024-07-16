using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    DialogueManager m_dilogueManager;
    CoffeMinigameManager m_coffeMinigameManager;

    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private PlayerInput m_dialogueInput;
    [SerializeField] private PlayerInput m_pauseInput; //not inplemented

    public enum state
    {
        player,
        dialogue,
        dragAndDrop
    }

    private state m_CurrentState;

    Dictionary<drinkType, float> m_drinkOrder;
    Dictionary<drinkType, float> m_trayDrinks;

    public Dictionary<drinkType, float> drinkOrder => m_drinkOrder;
    public Dictionary<drinkType, float> trayDrinks => m_trayDrinks;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Manger in the scene!");
        }
        instance = this;

        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();

        m_dilogueManager = DialogueManager.GetInstance();
        m_coffeMinigameManager = CoffeMinigameManager.GetInstance();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }


    #region Dialogue
    public void EnterDialogue(TextAsset inkJSON)
    {
        m_playerInput.actions.FindActionMap("Player").Disable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Enable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

        m_dilogueManager.EnterDialogueMode(inkJSON); //Call the Dialogue Manager
    }

    public void ExitDialogue()
    {
        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

    }

    #endregion

    #region Coffe Minigame
    public void EnterCoffeMinigame()
    {
        m_playerInput.actions.FindActionMap("Player").Disable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Enable();

        m_coffeMinigameManager.OpenCoffeMinigame();
    }

    public void ExitCoffeMinigame()
    {
        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

        m_trayDrinks = m_coffeMinigameManager.GetTraydrinks();
        m_coffeMinigameManager.CloseCoffeMinigame();
    }

    public void SetOrder(Dictionary<drinkType, float> order)
    {
        m_drinkOrder = order;
    }

    public bool CheckOrder()
    {
        foreach (KeyValuePair<drinkType, float> drink in m_drinkOrder)
        {
            //if the tray doesn't have the type of drink or the number of this drink is lower than the order
            if (!m_trayDrinks.ContainsKey(drink.Key) || drink.Value > m_trayDrinks[drink.Key])
                return false;
        }

        return true;
    }

    #endregion

    public void PauseGame()
    {
        if(m_CurrentState == state.player)
            m_playerInput.actions.FindActionMap("Player").Disable();
        else if (m_CurrentState == state.dialogue)
            m_dialogueInput.actions.FindActionMap("Dialogue").Disable();

        m_pauseInput.actions.FindActionMap("Player").Enable();
    }

    public void ResumeGame()
    {
        m_pauseInput.actions.FindActionMap("Player").Disable();

        if (m_CurrentState == state.player)
            m_playerInput.actions.FindActionMap("Player").Enable();
        else if (m_CurrentState == state.dialogue)
            m_dialogueInput.actions.FindActionMap("Dialogue").Enable();

    }
}
