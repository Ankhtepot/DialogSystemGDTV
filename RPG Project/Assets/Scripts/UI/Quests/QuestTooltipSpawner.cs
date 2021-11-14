using Asset_Packs.GameDev.tv_Assets.Scripts.Utils.UI.Tooltips;
using Quests;
using UnityEngine;

namespace UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return true;
        }
        
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus questStatus = GetComponent<QuestItemUI>().QuestStatus;
            tooltip.GetComponent<QuestTooltipUI>().Setup(questStatus);
        }
    }
}
