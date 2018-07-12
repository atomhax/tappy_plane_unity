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
                SpilTracking.NotificationSent("uniqueNotificationId")
                    .Track();
                break;
            case enumEvents.notificationReceived:
                SpilTracking.NotificationReceived("uniqueNotificationId")
                    .Track();
                break;
            case enumEvents.notificationOpened:
                SpilTracking.NotificationOpened("uniqueNotificationId", false)
                    .Track();
                break;
            case enumEvents.notificationDismissed:
                SpilTracking.NotificationDismissed("uniqueNotificationId")
                    .Track();
                break;
            case enumEvents.milestoneAchieved:
                SpilTracking.MilestoneAchieved("name")
                    .AddScore(0)
                    .AddLocation("location")
                    .AddIteration(0)
                    .AddMilestoneDescription("milestoneDescription")
                    .Track();
                break;
            case enumEvents.levelStart:
                SpilTracking.LevelStart("level")
                    .AddAchievement("achievement")
                    .AddActiveBooster(new List<string> { "activeBooster" })
                    .AddCreatorId("creatorId")
                    .AddCustomCreated(true)
                    .AddDifficulty("difficulty")
                    .AddIteration(0)
                    .AddLevelId("levelId")
                    .Track();
                break;
            case enumEvents.levelComplete:
                SpilTracking.LevelComplete("level")
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
                SpilTracking.LevelFailed("level")
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
                SpilTracking.LevelUp("level", "objectId")
                    .AddObjectUniqueId("uniqueId")
                    .AddObjectUniqueIdType("uniqueIdType")
                    .AddSkillId("skillId")
                    .AddSourceId("sourceId")
                    .AddSourceUniqueId("sourceUniqueId")
                    .Track();
                break;
            case enumEvents.equip:
                SpilTracking.Equip("equippedItem")
                    .AddEquippedTo("equippedTo")
                    .AddUnequippedFrom("equippedFrom")
                    .Track();
                break;
            case enumEvents.upgrade:
                SpilTracking.Upgrade("upgradeId", "level")
                    .AddKey("key")
                    .AddReason("reason")
                    .AddIteration(0)
                    .AddAchievement("achievement")
                    .Track();
                break;
            case enumEvents.levelCreate:
                SpilTracking.LevelCreate("levelId", "level", "creatorId")
                    .Track();
                break;
            case enumEvents.levelDownload:
                SpilTracking.LevelDownload("levelId", "creatorId")
                    .AddRating(0)
                    .Track();
                break;
            case enumEvents.levelRate:
                SpilTracking.LevelRate("levelId", "creatorId")
                    .AddRating(0)
                    .Track();
                break;
            case enumEvents.endlessModeStart:
                SpilTracking.EndlessModeStart()
                    .Track();
                break;
            case enumEvents.endlessModeEnd:
                SpilTracking.EndlessModeEnd(0)
                    .Track();
                break;
            case enumEvents.playerDies:
                SpilTracking.PlayerDies("level")
                    .Track();
                break;
            case enumEvents.iapPurchased:
                SpilTracking.IAPPurchased("skuId", "transactionId")
                    .AddToken("token")
                    .AddReason("reason")
                    .AddLocation("location")
                    .AddLocalPrice("0.0")
                    .AddAmazonUserId("amazonUserId")
                    .AddLocalCurrency("EUR")
                    .Track();
                break;
            case enumEvents.iapRestored:
                SpilTracking.IAPRestored("skuId", "originalTransactionId", "originalPurchaseDate")
                    .AddReason("reason")
                    .Track();
                break;
            case enumEvents.iapFailed:
                SpilTracking.IAPFailed("skuId", "errorDescription")
                    .AddReason("reason")
                    .AddLocation("location")
                    .Track();
                break;
            case enumEvents.tutorialComplete:
                SpilTracking.TutorialComplete()
                    .Track();
                break;
            case enumEvents.tutorialSkipped:
                SpilTracking.TutorialSkipped()
                    .Track();
                break;
            case enumEvents.register:
                SpilTracking.Register("platform")
                    .Track();
                break;
            case enumEvents.share:
                SpilTracking.Share("platform")
                    .AddReason("reason")
                    .AddLocation("location")
                    .Track();
                break;
            case enumEvents.invite:
                SpilTracking.Invite("platform")
                    .AddLocation("location")
                    .Track();
                break;
            case enumEvents.levelAppeared:
                SpilTracking.LevelAppeared("level")
                    .AddDifficulty("difficulty")
                    .Track();
                break;
            case enumEvents.levelDiscarded:
                SpilTracking.LevelDiscarded("level")
                    .AddDifficulty("difficulty")
                    .Track();
                break;
            case enumEvents.errorShown:
                SpilTracking.ErrorShown("reason")
                .Track();
                break;
            case enumEvents.timeElapLoad:
                SpilTracking.TimeElapLoad(0, "pointInGame")
                    .AddStartPoint("Screen")
                    .Track();
                break;
            case enumEvents.timeoutDetected:
                SpilTracking.TimeoutDetected(0, "pointInGame")
                    .Track();
                break;
            case enumEvents.objectStateChanged:
                SpilTracking.ObjectStateChanged("changedObject", "status", "reason")
                    .AddSituation("situation")
                    .AddInvolvedParties("involvedParties")
                    .AddAllChoiceResults("choiceResults")
                    .AddOptionConditions("optionConditions")
                    .AddChangedProperties("changedProperties")
                    .AddAllSelectedChoices("allSelectedChoices")
                    .Track();
                break;
            case enumEvents.uiElementClicked:
                SpilTracking.UIElementClicked("element")
                    .AddType("type")
                    .AddGrade(0)
                    .AddLocation("location")
                    .AddScreenName("screenName")
                    .Track();
                break;
            case enumEvents.sendGift:
                SpilTracking.SendGift("platform")
                    .AddLocation("location")
                    .Track();
                break;
            case enumEvents.levelTimeOut:
                SpilTracking.LevelTimeOut()
                    .Track();
                break;
            case enumEvents.dialogueChosen:
                SpilTracking.DialogueChosen("name", "choice", "choiceType", false, false, false, false, false)
                    .AddTime(0)
                    .AddIteration(0)
                    .AddIterationReason(new List<string> { "iterationReason" })
                    .Track();
                break;
            case enumEvents.friendAdded:
                SpilTracking.FriendAdded("friend")
                    .AddPlatform("platform")
                    .Track();
                break;
            case enumEvents.gameObjectInteraction:
                SpilTracking.GameObjectInteraction()
                    .Track();
                break;
            case enumEvents.gameResult:
                SpilTracking.GameResult()
                    .AddLabel("label")
                    .AddItemId("itemId")
                    .AddMatchId("matchId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.itemCrafted:
                SpilTracking.ItemCrafted("itemId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.itemCreated:
                SpilTracking.ItemCreated("itemId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.itemUpdated:
                SpilTracking.ItemUpdated("content", "itemId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.deckUpdated:
                SpilTracking.DeckUpdated("content", "itemId")
                    .AddLabel("label")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.matchComplete:
                SpilTracking.MatchComplete()
                    .AddLabel("label")
                    .AddItemId("itemId")
                    .AddMatchId("matchId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.matchLost:
                SpilTracking.MatchLost()
                    .AddLabel("label")
                    .AddItemId("itemId")
                    .AddMatchId("matchId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.matchTie:
                SpilTracking.MatchTie()
                    .AddLabel("label")
                    .AddItemId("itemId")
                    .AddMatchId("matchId")
                    .AddItemType("itemType")
                    .Track();
                break;
            case enumEvents.matchWon:
                SpilTracking.MatchWon()
                    .AddLabel("label")
                    .AddItemId("itemId")
                    .AddMatchId("matchId")
                    .AddItemType(new List<string>{ "itemType" })
                    .Track();
                break;
            case enumEvents.pawnMoved:
                SpilTracking.PawnMoved("name")
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
                SpilTracking.PlayerLeagueChanged()
                    .Track();
                break;
            case enumEvents.timedAction:
                SpilTracking.TimedAction("timedAction")
                    .AddLabel("label")
                    .AddTimedObject("timeObject")
                    .AddTimeToFinish(0)
                    .AddEffectMultiplier(0f)
                    .Track();
                break;
            case enumEvents.transitionToMenu:
                SpilTracking.TransitionToMenu()
                    .Track();
                break;
            case enumEvents.transitionToGame:
                SpilTracking.TransitionToGame("type")
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
