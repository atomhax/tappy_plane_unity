using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SpilGames.Unity.Base.Implementations.Tracking;
using SpilGames.Unity.Helpers.EventParams;

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
                .AddScore(0)
                .AddLocation("location")
                .AddIteration(0)
                .AddMilestoneDescription("milestoneDescription")
                .Track();
                break;
            case enumEvents.levelStart:
                new SpilTracking.BaseLevelStart("level")
                .AddAchievement("achievement")
                .AddActiveBooster(new List<string> { "activeBooster" })
                .AddCreatorId("creatorId")
                .AddCustomCreated(0)
                .AddDifficulty("difficulty")
                .AddIteration(0)
                .AddLevelId("levelId")
                .Track();
                break;
            case enumEvents.levelComplete:
                new SpilTracking.BaseLevelComplete("level")
                .AddAchievement("achievement")
                .AddAvgCombos(0)
                .AddCreatorId("creatorId")
                .AddCustomCreated(false)
                .AddDifficulty("difficulty")
                .AddIteration(0)
                .AddLevelId("levelId")
                .AddMoves(0)
                .AddMovesLeft(0)
                .AddObjectUsed(new List<LevelCompleteObjectUsed> { new LevelCompleteObjectUsed { objectCount = 0, objectId = "objectId", objectTimings = new[] { 0, 1, 2 }, objectType = "objectType" } })
                .AddRating("rating")
                .AddScore(0)
                .AddSpeed(0)
                .AddStars(0)
                .AddTimeLeft(0)
                .AddTurns(0)
                .Track();
                break;
            case enumEvents.levelFailed:
                new SpilTracking.BaseLevelFailed("level")
                .AddAchievement("achievement")
                .AddAvgCombos(0)
                .AddCreatorId("creatorId")
                .AddCustomCreated(false)
                .AddDifficulty("difficulty")
                .AddIteration(0)
                .AddLevelId("levelId")
                .AddMoves(0)
                .AddMovesLeft(0)
                .AddObjectUsed(new List<LevelCompleteObjectUsed> { new LevelCompleteObjectUsed { objectCount = 0, objectId = "objectId", objectTimings = new[] { 0, 1, 2 }, objectType = "objectType" } })
                .AddRating("rating")
                .AddScore(0)
                .AddSpeed(0)
                .AddStars(0)
                .AddTimeLeft(0)
                .AddTurns(0)
                .Track();
                break;
            case enumEvents.levelUp:
                new SpilTracking.BaseLevelUp("level", "objectId")
                .AddObjectUniqueId("uniqueId")
                .AddObjectUniqueIdType("uniqueIdType")
                .AddSkillId("skillId")
                .AddSourceId("sourceId")
                .AddSourceUniqueId("sourceUniqueId")
                .Track();
                break;
            case enumEvents.equip:
                new SpilTracking.BaseEquip("equippedItem")
                .AddEquippedTo("equippedTo")
                .AddUnequippedFrom("equippedFrom")
                .Track();
                break;
            case enumEvents.upgrade:
                new SpilTracking.BaseUpgrade("upgradeId", "level")
                .AddKey("key")
                .AddReason("reason")
                .AddIteration(0)
                .AddAchievement("achievement")
                .Track();
                break;
            case enumEvents.levelCreate:
                new SpilTracking.BaseLevelCreate("levelId", "level", "creatorId")
                .Track();
                break;
            case enumEvents.levelDownload:
                new SpilTracking.BaseLevelDownload("levelId", "creatorId")
                .AddRating(0)
                .Track();
                break;
            case enumEvents.levelRate:
                new SpilTracking.BaseLevelRate("levelId", "creatorId")
                .AddRating(0)
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
                .AddToken("token")
                .AddReason("reason")
                .AddLocation("location")
                .AddLocalPrice("0.0")
                .AddAmazonUserId("amazonUserId")
                .AddLocalCurrency("EUR")
                .Track();
                break;
            case enumEvents.iapRestored:
                new SpilTracking.BaseIAPRestored("skuId", "originalTransactionId", "originalPurchaseDate")
                .AddReason("reason")
                .Track();
                break;
            case enumEvents.iapFailed:
                new SpilTracking.BaseIAPFailed("skuId", "errorDescription")
                .AddReason("reason")
                .AddLocation("location")
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
                .AddReason("reason")
                .AddLocation("location")
                .Track();
                break;
            case enumEvents.invite:
                new SpilTracking.BaseInvite("platform")
                .AddLocation("location")
                .Track();
                break;
            case enumEvents.levelAppeared:
                new SpilTracking.BaseLevelAppeared("level")
                .AddDifficulty("difficulty")
                .Track();
                break;
            case enumEvents.levelDiscarded:
                new SpilTracking.BaseLevelDiscarded("level")
                .AddDifficulty("difficulty")
                .Track();
                break;
            case enumEvents.errorShown:
                new SpilTracking.BaseErrorShown("reason")
                .Track();
                break;
            case enumEvents.timeElapLoad:
                new SpilTracking.BaseTimeElapLoad(0, "pointInGame")
                .AddStartPoint(0)
                .Track();
                break;
            case enumEvents.timeoutDetected:
                new SpilTracking.BaseTimeoutDetected(0, "pointInGame")
                .Track();
                break;
            case enumEvents.objectStateChanged:
                new SpilTracking.BaseObjectStateChanged("changedObject", "status", "reason")
                .AddSituation("situation")
                .AddInvolvedParties("involvedParties")
                .AddAllChoiceResults("choiceResults")
                .AddOptionConditions("optionConditions")
                .AddChangedProperties("changedProperties")
                .AddAllSelectedChoices("allSelectedChoices")
                .Track();
                break;
            case enumEvents.uiElementClicked:
                new SpilTracking.BaseUIElementClicked("element")
                .AddType("type")
                .AddGrade(0)
                .AddLocation("location")
                .AddScreenName("screenName")
                .Track();
                break;
            case enumEvents.sendGift:
                new SpilTracking.BaseSendGift("platform")
                .AddLocation("location")
                .Track();
                break;
            case enumEvents.levelTimeOut:
                new SpilTracking.BaseLevelTimeOut()
                .Track();
                break;
            case enumEvents.dialogueChosen:
                new SpilTracking.BaseDialogueChosen("name", "choice", "choiceType", false, false, false, false, false)
                .AddTime(0)
                .AddIteration(0)
                .AddIterationReason(new List<string> { "iterationReason" })
                .Track();
                break;
            case enumEvents.friendAdded:
                new SpilTracking.BaseFriendAdded("friend")
                .AddPlatform("platform")
                .Track();
                break;
            case enumEvents.gameObjectInteraction:
                new SpilTracking.BaseGameObjectInteraction()
                .Track();
                break;
            case enumEvents.gameResult:
                new SpilTracking.BaseGameResult()
                .AddLabel("label")
                .AddItemId("itemId")
                .AddMatchId("matchId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.itemCrafted:
                new SpilTracking.BaseItemCrafted("itemId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.itemCreated:
                new SpilTracking.BaseItemCreated("itemId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.itemUpdated:
                new SpilTracking.BaseItemUpdated("content", "itemId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.deckUpdated:
                new SpilTracking.BaseDeckUpdated("content", "itemId")
                .AddLabel("label")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.matchComplete:
                new SpilTracking.BaseMatchComplete()
                .AddLabel("label")
                .AddItemId("itemId")
                .AddMatchId("matchId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.matchLost:
                new SpilTracking.BaseMatchLost()
                .AddLabel("label")
                .AddItemId("itemId")
                .AddMatchId("matchId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.matchTie:
                new SpilTracking.BaseMatchTie()
                .AddLabel("label")
                .AddItemId("itemId")
                .AddMatchId("matchId")
                .AddItemType("itemType")
                .Track();
                break;
            case enumEvents.matchWon:
                new SpilTracking.BaseMatchWon()
                .AddLabel("label")
                .AddItemId("itemId")
                .AddMatchId("matchId")
                .AddItemType(new List<string>{ "itemType" })
                .Track();
                break;
            case enumEvents.pawnMoved:
                new SpilTracking.BasePawnMoved("name")
                .AddKind("kind")
                .AddDelta("delta")
                .AddLabel("label")
                .AddEnergy("energy")
                .AddRarity("rarity")
                .AddReason("reason")
                .AddLocation("location")
                .Track();
                break;
            case enumEvents.playerLeagueChanged:
                new SpilTracking.BasePlayerLeagueChanged()
                .Track();
                break;
            case enumEvents.timedAction:
                new SpilTracking.BaseTimedAction("timedAction")
                .AddLabel("label")
                .AddTimedObject("timeObject")
                .AddTimeToFinish(0)
                .AddEffectMultiplier(0f)
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
