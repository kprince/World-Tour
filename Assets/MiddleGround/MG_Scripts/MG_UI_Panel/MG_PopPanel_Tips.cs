using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiddleGround.UI
{
    public class MG_PopPanel_Tips : MG_UIBase
    {
        public Text text_content;
        public override IEnumerator OnEnter()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            text_content.text = MG_Manager.Instance.str_Tips;
            StartCoroutine(AutoClose());
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            yield return null;
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }

        IEnumerator AutoClose()
        {
            float time = MG_Manager.Instance.time_Tips;
            while (time>0)
            {
                yield return null;
                time -= Time.unscaledDeltaTime;
                text_content.text = MG_Manager.Instance.str_Tips;
            }
            MG_UIManager.Instance.ClosePopPanelAsync(MG_PopPanelType.Tips);
        }
    }
}
