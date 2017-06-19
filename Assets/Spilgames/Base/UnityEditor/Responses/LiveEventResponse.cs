using System.Collections.Generic;
using SpilGames.Unity.Json;

#if UNITY_EDITOR


namespace SpilGames.Unity.Base.UnityEditor.Responses
{
    public class LiveEventResponse : ResponseEvent
    {
//        public static LiveEventOverview liveEventOverview = new LiveEventOverview();
//        
        public static void ProcessLiveEventResponse(ResponseEvent response){
            if(response.data != null){
                
            }
        }

//        public void advanceToNextStage() {
//            if (liveEventOverview.fromStartStage) {
//                if (liveEventOverview.currentStage != null) {
//                    openStageView(liveEventOverview.getCurrentStage());
//                    liveEventOverview.setFromStartStage(false);
//                } else {
//                    SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventError(ErrorCodes.LiveEventInvalidNextStage);
//                    if (WebViewActivity.getActivity() != null) {
//                        WebViewActivity.getActivity().finish();
//                    }
//                }
//            } else {
//                LiveEvent liveEvent = new LiveEvent();
//                liveEvent.setNextStage();
//
//                liveEvent.addCustomData("liveEventId", liveEventOverview.getLiveEventId());
//
//                SpilSdk.getInstance(context).trackEvent(liveEvent, null);
//            }
//        }
//
//        public void applyItems(JSONArray items) {
//            LiveEvent liveEvent = new LiveEvent();
//            liveEvent.setApplyItems();
//
//            liveEvent.addCustomData("liveEventId", liveEventOverview.getLiveEventId());
//
//            if (items != null) {
//                liveEvent.addCustomData("items", items);
//
//                try {
//                    for(int i = 0; i < items.length(); i++){
//                        int id = items.getJSONObject(i).getInt("id");
//                        int amount = items.getJSONObject(i).getInt("amount");
//                        SpilSdk.getInstance(context).subtractItemFromInventory(id, amount, PlayerDataUpdateReasons.LiveEvent, null, null, null);
//                    }
//                } catch (JSONException e) {
//                    e.printStackTrace();
//                }
//
//            } else {
//                liveEvent.addCustomData("items", "");
//            }
//
//            SpilSdk.getInstance(context).trackEvent(liveEvent, null);
//        }
//
//        public void processRequestLiveEvent(JSONObject data) {
//            if (data != null && data.length() > 0) {
//                try {
//                    if (data.has("config")) {
//                        liveEventOverview.setLiveEventConfig(data.getJSONObject("config"));
//                    }
//
//                    if (data.has("currentStage")) {
//                        liveEventOverview.setCurrentStage(data.getJSONObject("currentStage"));
//
//                        JSONArray rewards = liveEventOverview.getCurrentStage().getJSONArray("rewards");
//                        rewards = injectImageUrlToRewards(rewards);
//
//                        liveEventOverview.getCurrentStage().remove("rewards");
//                        liveEventOverview.getCurrentStage().put("rewards", rewards);
//
//                        liveEventOverview.setFromStartStage(true);
//                    }
//
//                    if (data.has("eventItems")) {
//                        liveEventOverview.setEventItems(data.getJSONArray("eventItems"));
//                    }
//
//                    if (data.has("liveEventId")) {
//                        liveEventOverview.setLiveEventId(data.getInt("liveEventId"));
//                    }
//
//                    if (data.has("startDate")) {
//                        liveEventOverview.setStartDate(data.getLong("startDate"));
//                    }
//
//                    if (data.has("endDate")) {
//                        liveEventOverview.setEndDate(data.getLong("startDate"));
//                    }
//
//                    if (data.has("stage")) {
//                        JSONObject startStage = data.getJSONObject("stage");
//
//                        JSONArray rewards = startStage.getJSONArray("rewards");
//                        rewards = injectImageUrlToRewards(rewards);
//
//                        startStage.remove("rewards");
//                        startStage.put("rewards", rewards);
//
//                        openStageView(startStage);
//                    }
//                } catch (JSONException e) {
//                    e.printStackTrace();
//                }
//            } else {
//                SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventError(ErrorCodes.LiveEventServerError);
//            }
//        }
//
//        public void processAdvanceToNextStage(JSONObject data) {
//            try {
//                boolean valid = data.getBoolean("valid");
//
//                if (!valid) {
//                    SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventError(ErrorCodes.LiveEventInvalidNextStage);
//                    if (WebViewActivity.getActivity() != null) {
//                        WebViewActivity.getActivity().finish();
//                    }
//                    return;
//                }
//
//                liveEventOverview.setCurrentStage(null);
//                if (data.has("nextStage")) {
//                    liveEventOverview.setCurrentStage(data.getJSONObject("nextStage"));
//
//                    JSONArray rewards = liveEventOverview.getCurrentStage().getJSONArray("rewards");
//                    rewards = injectImageUrlToRewards(rewards);
//
//                    liveEventOverview.getCurrentStage().remove("rewards");
//                    liveEventOverview.getCurrentStage().put("rewards", rewards);
//                } else if (data.has("noMoreStages")) {
//                    SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventCompleted();
//                    if (WebViewActivity.getActivity() != null) {
//                        WebViewActivity.getActivity().finish();
//                    }
//                    return;
//                }
//
//                if (liveEventOverview.getCurrentStage() != null) {
//                    openStageView(liveEventOverview.getCurrentStage());
//                } else {
//                    SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventError(ErrorCodes.LiveEventMissingNextStage);
//                }
//            } catch (JSONException e) {
//                e.printStackTrace();
//            }
//
//        }
//
//        public void processApplyItems(JSONObject data) {
//            try {
//                boolean valid = data.getBoolean("valid");
//
//                if (!valid) {
//                    SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventError(ErrorCodes.LiveEventInvalidNextStage);
//                    if (WebViewActivity.getActivity() != null) {
//                        WebViewActivity.getActivity().finish();
//                    }
//                    return;
//                }
//
//                boolean metRequirements = data.getBoolean("metRequirements");
//
//                if (!metRequirements) {
//                    SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventMetRequirements(false);
//                    return;
//                }
//                SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventMetRequirements(true);
//
//                JSONArray rewardsJSON = data.getJSONArray("reward");
//                rewardsJSON = injectImageUrlToRewards(rewardsJSON);
//
//                ArrayList<Reward> rewards = new ArrayList<>();
//
//                for (int i = 0; i < rewardsJSON.length(); i++) {
//                    Reward reward = new Reward();
//
//                    if (rewardsJSON.getJSONObject(i).get("id") instanceof String) {
//                        reward.setExternalId(rewardsJSON.getJSONObject(i).getString("id"));
//                    } else if (rewardsJSON.getJSONObject(i).get("id") instanceof Integer) {
//                        reward.setId(rewardsJSON.getJSONObject(i).getInt("id"));
//                    }
//
//                    reward.setType(rewardsJSON.getJSONObject(i).getString("type"));
//                    reward.setAmount(rewardsJSON.getJSONObject(i).getInt("amount"));
//                    if (rewardsJSON.getJSONObject(i).has("imageUrl")) {
//                        reward.setImageUrl(rewardsJSON.getJSONObject(i).getString("imageUrl"));
//                    }
//
//                    rewards.add(reward);
//                }
//
//                String reason = PlayerDataUpdateReasons.LiveEvent;
//                ArrayList<Reward> externalReward = null;
//
//                for (int i = 0; i < rewards.size(); i++) {
//                    if (rewards.get(i).getType().equals("CURRENCY")) {
//                        SpilSdk.getInstance(context).addCurrencyToWallet(rewards.get(i).getId(), rewards.get(i).getAmount(), reason, null, null, null);
//                    } else if (rewards.get(i).getType().equals("ITEM")) {
//                        SpilSdk.getInstance(context).addItemToInventory(rewards.get(i).getId(), rewards.get(i).getAmount(), reason, null, null, null);
//                    } else if (rewards.get(i).getType().equals("EXTERNAL")) {
//                        if (externalReward == null) {
//                            externalReward = new ArrayList<>();
//                        }
//                        externalReward.add(rewards.get(i));
//                    }
//                }
//
//                if (externalReward != null && !externalReward.isEmpty()) {
//                    sendRewardsToExternal(context, externalReward);
//                }
//
//                JSONObject nextStage = data.getJSONObject("nextStage");
//                nextStage.put("givenReward", rewardsJSON);
//
//                JSONArray nextStageRewards = nextStage.getJSONArray("rewards");
//                nextStageRewards = injectImageUrlToRewards(nextStageRewards);
//
//                nextStage.remove("rewards");
//                nextStage.put("rewards", nextStageRewards);
//
//                liveEventOverview.setCurrentStage(nextStage);
//                openStageView(nextStage);
//            } catch (JSONException e) {
//                e.printStackTrace();
//            }
//        }
//
//        private JSONArray injectImageUrlToRewards(JSONArray rewardsArray) {
//            SpilGameData spilGameData = SpilGameDataManager.getInstance(context).getGameData();
//            HashMap<Integer, Item> items = spilGameData.getItemsMap();
//
//            JSONArray rewardsWithImageUrl = new JSONArray();
//
//            try {
//                for (int i = 0; i < rewardsArray.length(); i++) {
//                    JSONObject item = rewardsArray.getJSONObject(i);
//
//                    String itemType = item.getString("type");
//                    if (itemType.equals("ITEM") || itemType.equals("CURRENCY")) {
//                        Item gameItem = items.get(item.getInt("id"));
//                        if (gameItem != null && gameItem.getImageUrl() != null) {
//                            item.put("imageUrl", gameItem.getImageUrl());
//                        }
//
//                    }
//
//                    rewardsWithImageUrl.put(item);
//                }
//            } catch (NullPointerException | JSONException e) {
//                e.printStackTrace();
//                return rewardsArray;
//            }
//
//            return rewardsWithImageUrl;
//        }
//
//        private void openStageView(JSONObject stage) {
//            try {
//                if (WebViewActivity.getActivity() == null) {
//                    Intent intent = new Intent(context.getApplicationContext(), WebViewActivity.class);
//                    intent.putExtra("webViewUrl", stage.getString("url"));
//                    intent.putExtra("eventName", "liveEvent");
//                    intent.putExtra("eventData", stage.toString());
//                    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
//                    context.getApplicationContext().startActivity(intent);
//                } else {
//                    WebViewActivity.getActivity().openNewPage(stage.getString("url"), "liveEvent", stage.toString());
//                }
//
//            } catch (JSONException e) {
//                e.printStackTrace();
//            }
//        }
//
//        private void sendRewardsToExternal(List<Reward> rewardList) {
//            string rewards = SpilSdk.getInstance(context).getGson().toJson(rewardList, new TypeToken<ArrayList<Reward>>() {}.getType());
//            SpilSdk.getInstance(context).getLiveEventCallbacks().liveEventReward(rewards);
//        }
//
//        public long getLiveEventStartDate() {
//            if (liveEventOverview != null) {
//                return liveEventOverview.startDate;
//            } else {
//                return 0;
//            }
//        }
//
//        public long getLiveEventEndDate() {
//            if (liveEventOverview != null) {
//                return liveEventOverview.endDate;
//            } else {
//                return 0;
//            }
//        }
//
//        public string getLiveEventConfig() {
//            if (liveEventOverview != null) {
//                return liveEventOverview.liveEventConfig.Print();
//            } else {
//                return null;
//            }
//        }
//        
//        public class LiveEventOverview{
//            public int liveEventId;
//            public JSONObject currentStage = null;
//            public JSONObject liveEventConfig = new JSONObject();
//            public JSONObject eventItems = new JSONObject(JSONObject.Type.ARRAY);
//            public long startDate;
//            public long endDate;
//            public bool fromStartStage;
//        }
//        
    }
}

#endif