using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SpilGames.Unity.Base.Implementations.Tracking;

public class DebugPanelController : MonoBehaviour 
{
    public GameObject eventBtn1;
    public GameObject eventBtn2;
    public GameObject eventBtn3;
    public GameObject eventBtn4;
    public GameObject eventBtn5;
    public GameObject eventBtn6;
    public GameObject eventBtn7;

    public GameObject eventBtnUp;
    public GameObject eventBtnDown;

    public Text debugConsoleText;

    int currentPage = 1;
    int pageSize = 7;
    int amountOfEvents = System.Enum.GetNames(typeof(enumEvents)).Length;
    string[] enumEventsOrdered = System.Enum.GetNames(typeof(enumEvents)).OrderBy(a => a).ToArray();

    public void eventBtn1Click() { eventBtnClick(0); }
    public void eventBtn2Click() { eventBtnClick(1); }
    public void eventBtn3Click() { eventBtnClick(2); }
    public void eventBtn4Click() { eventBtnClick(3); }
    public void eventBtn5Click() { eventBtnClick(4); }
    public void eventBtn6Click() { eventBtnClick(5); }
    public void eventBtn7Click() { eventBtnClick(6); }

    public void eventBtnUpClick()
    {
        if(currentPage > 1)
        {
            currentPage -= 1;
            configureButtons();
        }
    }

    public void eventBtnDownClick()
    {
        if(currentPage * pageSize < amountOfEvents)
        {
            currentPage += 1;
            configureButtons();
        }
    }

    void OnEnable()
    {
        configureButtons();
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (message.StartsWith("SpilSDK: SendCustomEvent"))
        {
            debugConsoleText.text = message;
        }
        else if(message.StartsWith("SpilSDK: SpilSDK-Unity OnResponseReceived"))
        {
            debugConsoleText.text += "\r\n\r\n" + message;
        }
    }

    void configureButtons()
    {
        eventBtn1.SetActive(isButtonVisible(1));
        eventBtn1.GetComponentInChildren<Text>().text = getEnumNameForButton(1);

        eventBtn2.SetActive(isButtonVisible(2));
        eventBtn2.GetComponentInChildren<Text>().text = getEnumNameForButton(2);

        eventBtn3.SetActive(isButtonVisible(3));
        eventBtn3.GetComponentInChildren<Text>().text = getEnumNameForButton(3);

        eventBtn4.SetActive(isButtonVisible(4));
        eventBtn4.GetComponentInChildren<Text>().text = getEnumNameForButton(4);

        eventBtn5.SetActive(isButtonVisible(5));
        eventBtn5.GetComponentInChildren<Text>().text = getEnumNameForButton(5);

        eventBtn6.SetActive(isButtonVisible(6));
        eventBtn6.GetComponentInChildren<Text>().text = getEnumNameForButton(6);

        eventBtn7.SetActive(isButtonVisible(7));
        eventBtn7.GetComponentInChildren<Text>().text = getEnumNameForButton(7);

        eventBtnUp.GetComponentInChildren<Text>().text = currentPage > 1 ? "^" : "";
        eventBtnDown.GetComponentInChildren<Text>().text = amountOfEvents > pageSize && (pageSize * currentPage < amountOfEvents) ? "^" : "";
    }

    string getEnumNameForButton(int btnIndex)
    {
        int realBtnIndex = (currentPage - 1) * pageSize + (btnIndex - 1);
        return enumEventsOrdered.Length > realBtnIndex ? enumEventsOrdered[realBtnIndex] : "";
    }

    bool isButtonVisible(int btnIndex)
    {
        int pageStartIndex = pageSize * (currentPage - 1);
        return amountOfEvents > pageStartIndex + btnIndex;
    }

    void eventBtnClick(int btnIndex)
    {
        enumEvents enumEvent = (enumEvents)System.Enum.Parse(typeof(enumEvents), enumEventsOrdered[(currentPage - 1) * pageSize + btnIndex]);
        switch (enumEvent)
        {
            case enumEvents.notificationSent:
                new SpilTracking.BaseNotificationSent("uniqueNotificationId")
                .Track();
                break;
            case enumEvents.notificationReceived:
                new SpilTracking.BaseNotificationReceived("uniqueNotificationId")
                .Track();
                break;
            case enumEvents.notificationOpened:
                new SpilTracking.BaseNotificationOpened("uniqueNotificationId", false)
                .Track();
                break;
            case enumEvents.notificationDismissed:
                new SpilTracking.BaseNotificationDismissed("uniqueNotificationId")
                .Track();
                break;
            case enumEvents.milestoneAchieved:
                new SpilTracking.BaseMilestoneAchieved("name")
                .Track();
                break;
            case enumEvents.levelStart:
                new SpilTracking.BaseLevelStart("level")
                .Track();
                break;
            case enumEvents.levelComplete:
                new SpilTracking.BaseLevelComplete("level")
                .Track();
                break;
            case enumEvents.levelFailed:
                new SpilTracking.BaseLevelFailed("level")
                .Track();
                break;
            case enumEvents.levelUp:
                new SpilTracking.BaseLevelUp("level", "objectId")
                .Track();
                break;
            case enumEvents.equip:
                new SpilTracking.BaseEquip("equippedItem")
                .Track();
                break;
            case enumEvents.upgrade:
                new SpilTracking.BaseUpgrade("upgradeId", "level")
                .Track();
                break;
            case enumEvents.levelCreate:
                new SpilTracking.BaseLevelCreate("levelId", "level", "creatorId")
                .Track();
                break;
            case enumEvents.levelDownload:
                new SpilTracking.BaseLevelDownload("levelId", "creatorId")
                .Track();
                break;
            case enumEvents.levelRate:
                new SpilTracking.BaseLevelRate("levelId", "creatorId")
                .Track();
                break;
            case enumEvents.endlessModeStart:
                new SpilTracking.BaseEndlessModeStart()
                .Track();
                break;
            case enumEvents.endlessModeEnd:
                new SpilTracking.BaseEndlessModeEnd(0)
                .Track();
                break;
            case enumEvents.playerDies:
                new SpilTracking.BasePlayerDies("level")
                .Track();
                break;
            case enumEvents.iapPurchased:
                new SpilTracking.BaseIAPPurchased("skuId", "transactionId")
                .Track();
                break;
            case enumEvents.iapRestored:
                new SpilTracking.BaseIAPRestored("skuId", "originalTransactionId", "originalPurchaseDate")
                .Track();
                break;
            case enumEvents.iapFailed:
                new SpilTracking.BaseIAPFailed("skuId", "errorDescription")
                .Track();
                break;
            case enumEvents.tutorialComplete:
                new SpilTracking.BaseTutorialComplete()
                .Track();
                break;
            case enumEvents.tutorialSkipped:
                new SpilTracking.BaseTutorialSkipped()
                .Track();
                break;
            case enumEvents.register:
                new SpilTracking.BaseRegister("platform")
                .Track();
                break;
            case enumEvents.share:
                new SpilTracking.BaseShare("platform")
                .Track();
                break;
            case enumEvents.invite:
                new SpilTracking.BaseInvite("platform")
                .Track();
                break;
            case enumEvents.levelAppeared:
                new SpilTracking.BaseLevelAppeared("level")
                .Track();
                break;
            case enumEvents.levelDiscarded:
                new SpilTracking.BaseLevelDiscarded("level")
                .Track();
                break;
            case enumEvents.errorShown:
                new SpilTracking.BaseErrorShown("reason")
                .Track();
                break;
            case enumEvents.timeElapLoad:
                new SpilTracking.BaseTimeElapLoad(0, "pointInGame")
                .Track();
                break;
            case enumEvents.timeoutDetected:
                new SpilTracking.BaseTimeoutDetected(0, "pointInGame")
                .Track();
                break;
            case enumEvents.objectStateChanged:
                new SpilTracking.BaseObjectStateChanged("changedObject", "status", "reason")
                .Track();
                break;
            case enumEvents.uiElementClicked:
                new SpilTracking.BaseUIElementClicked("element")
                .Track();
                break;
            case enumEvents.sendGift:
                new SpilTracking.BaseSendGift("platform")
                .Track();
                break;
            case enumEvents.levelTimeOut:
                new SpilTracking.BaseLevelTimeOut()
                .Track();
                break;
            case enumEvents.dialogueChosen:
                new SpilTracking.BaseDialogueChosen("name", "choice", "choiceType", false, false, false, false, false)
                .Track();
                break;
            case enumEvents.friendAdded:
                new SpilTracking.BaseFriendAdded("friend")
                .Track();
                break;
            case enumEvents.gameObjectInteraction:
                new SpilTracking.BaseGameObjectInteraction()
                .Track();
                break;
            case enumEvents.gameResult:
                new SpilTracking.BaseGameResult()
                .Track();
                break;
            case enumEvents.itemCrafted:
                new SpilTracking.BaseItemCrafted("itemId")
                .Track();
                break;
            case enumEvents.itemCreated:
                new SpilTracking.BaseItemCreated("itemId")
                .Track();
                break;
            case enumEvents.itemUpdated:
                new SpilTracking.BaseItemUpdated("content", "itemId")
                .Track();
                break;
            case enumEvents.deckUpdated:
                new SpilTracking.BaseDeckUpdated("content", "itemId")
                .Track();
                break;
            case enumEvents.matchComplete:
                new SpilTracking.BaseMatchComplete()
                .Track();
                break;
            case enumEvents.matchLost:
                new SpilTracking.BaseMatchLost()
                .Track();
                break;
            case enumEvents.matchTie:
                new SpilTracking.BaseMatchTie()
                .Track();
                break;
            case enumEvents.matchWon:
                new SpilTracking.BaseMatchWon()
                .Track();
                break;
            case enumEvents.pawnMoved:
                new SpilTracking.BasePawnMoved("name")
                .Track();
                break;
            case enumEvents.playerLeagueChanged:
                new SpilTracking.BasePlayerLeagueChanged()
                .Track();
                break;
            case enumEvents.timedAction:
                new SpilTracking.BaseTimedAction("timedAction")
                .Track();
                break;
            case enumEvents.transitionToMenu:
                new SpilTracking.BaseTransitionToMenu()
                .Track();
                break;
            case enumEvents.transitionToGame:
                new SpilTracking.BaseTransitionToGame("type")
                .Track();
                break;
        }
    }

    enum enumEvents
    {
        notificationSent,
        notificationReceived,
        notificationOpened,
        notificationDismissed,
        milestoneAchieved,
        levelStart,
        levelComplete,
        levelFailed,
        levelUp,
        equip,
        upgrade,
        levelCreate,
        levelDownload,
        levelRate,
        endlessModeStart,
        endlessModeEnd,
        playerDies,
        iapPurchased,
        iapRestored,
        iapFailed,
        tutorialComplete,
        tutorialSkipped,
        register,
        share,
        invite,
        levelAppeared,
        levelDiscarded,
        errorShown,
        timeElapLoad,
        timeoutDetected,
        objectStateChanged,
        uiElementClicked,
        sendGift,
        levelTimeOut,
        dialogueChosen,
        friendAdded,
        gameObjectInteraction,
        gameResult,
        itemCrafted,
        itemCreated,
        itemUpdated,
        deckUpdated,
        matchComplete,
        matchLost,
        matchTie,
        matchWon,
        pawnMoved,
        playerLeagueChanged,
        timedAction,
        transitionToMenu,
        transitionToGame
    }
}
