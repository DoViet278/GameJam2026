using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> quests = new List<Quest>();

    public TextMeshProUGUI questText;

    private int currentQuestIndex = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ShowCurrentQuest();
    }

    void ShowCurrentQuest()
    {
        if (currentQuestIndex < quests.Count)
        {
            questText.text = quests[currentQuestIndex].questName;
        }
        else
        {
            questText.text = "Mission completed";
        }
    }

    public void CompleteCurrentQuest()
    {
        if (currentQuestIndex >= quests.Count) return;

        quests[currentQuestIndex].isCompleted = true;
        currentQuestIndex++;

        ShowCurrentQuest();
    }
}
